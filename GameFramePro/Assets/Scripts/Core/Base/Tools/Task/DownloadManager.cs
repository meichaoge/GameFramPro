using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using GameFramePro.NetWorkEx;

namespace GameFramePro
{
    /// <summary>
    /// 下载任务的优先级
    /// </summary>
    public enum UnityWebRequestTaskPriorityEnum
    {
        None = 0, //不可用的
        LowLevel = 1, //非必要的资源 比如头像等是这个优先级
        Normal = 10,  //大多数任务处于这个优先级
        HighLevel = 50,
        Immediately = 100,//只要条件允许立刻开始这个任务
    }

    /// <summary>
    /// 处理所有的与下载相关的事务 内部使用 UnityWebRequest
    /// </summary>
    public class DownloadManager : Single_Mono_AutoCreateNotDestroy<DownloadManager>
    {

        #region 下载相关设置
        private float mTickedOffsetTime = 0.1f;//每过去这么长时间主动查询下状态
        private float mLastRecodTime = 0f;

        public static int S_MaxDownloadTaskCount = 20; //最多同时存在的任务数量
        //所有的正在下载中的 UnityWebRequest 任务
        private Dictionary<string, UnityWebRequestDownloadTask> mAllDownloadingUnityWebRequestTasks = new Dictionary<string, UnityWebRequestDownloadTask>();
        private string[] mAllCompleteTaskForDelete = new string[S_MaxDownloadTaskCount]; //所有完成下载的任务 避免每次都去检测

        //按照优先级排序
        private LinkedList<UnityWebRequestDownloadTask> mAllWaiteDownloadUnityWebRequestTasks = new LinkedList<UnityWebRequestDownloadTask>();

        private NativeObjectPool<UnityWebRequestDownloadTask> mUnityWebRequestTaskPoolManager = null;

        #endregion

    

        #region Unity Mono Frame

        protected override void Awake()
        {
            base.Awake();
         
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask>(S_MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
        }

        private void Update()
        {
            if (mLastRecodTime == 0)
                mLastRecodTime = Time.realtimeSinceStartup;

            if(Time.realtimeSinceStartup- mLastRecodTime>= mTickedOffsetTime)
            {
                mLastRecodTime = Time.realtimeSinceStartup;
                if (mAllDownloadingUnityWebRequestTasks.Count > 0)
                {
                    foreach (var taskInfor in mAllDownloadingUnityWebRequestTasks.Values)
                    {
                        taskInfor.Tick();
                    }

                    CheckDownloadingTaskIsComplete();
                    DoClearCompleteTask();
                }

                DOWaitingUnityWebRequestDownloadTask();
            }
        }
        #endregion



        #region 对象池接口
        private void OnBeforGetWebRequestTaskItem(UnityWebRequestDownloadTask task)
        {
        }

        private void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask task)
        {
            if (task != null)
            {
                task.Dispose();
            }
        }
        #endregion


        #region 各种下载的接口
        private Action<T, string> TranslateAction<T>(Action<object, string> completeCallback)
        {
            return completeCallback as Action<T, string>;
        }

        private Action<object, string> TranslateAction<T>(Action<T, string> completeCallback)
        {
            return completeCallback as Action<object, string>;
        }


        /// <summary>
        /// 获取一个AssetBundle 对象
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetAssetBundleFromUrl(string taskUrl, System.Action<AssetBundle, string> callback, UnityWebRequestTaskPriorityEnum priorityEnum = UnityWebRequestTaskPriorityEnum.Normal)
        {
            System.Action<object, string> completeCallback = TranslateAction(callback);
            if (completeCallback == null)
            {
                Debug.LogError("GetAssetBundleFromUrl Fail, is Null " + (callback == null));
                return;
            }


            //TODO从下载的缓存中读取数据
            if (TryAddCallbackOnWorkingTask(taskUrl, completeCallback, priorityEnum))
                return;

            CheckDownloadingTaskIsComplete(); //清理已经完成的任务
            //***创建一个请求
            UnityWebRequestDownloadTask newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
            webRequest.downloadHandler = handler;

            if (mAllDownloadingUnityWebRequestTasks.Count >= S_MaxDownloadTaskCount)
            {
                PustUnityWebRequestTaskForWaitingList(newTask, completeCallback);
            }//等待
            else
            {
                newTask.InitialedUnityWebRequestDownloadTask(webRequest, priorityEnum, completeCallback, UnityWebRequestToAssetBundle);

                mAllDownloadingUnityWebRequestTasks[taskUrl] = newTask;
                newTask.StartTask();
            }
        }
        /// <summary>
        /// 将结果转换成需要的数据
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        private object UnityWebRequestToAssetBundle(UnityWebRequest webRequest)
        {
            if (webRequest == null) return null;
            return (webRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        }


        /// <summary>
        /// 获取一个 String 对象
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetStringDataFromUrl(string taskUrl, System.Action<string, string> callback, UnityWebRequestTaskPriorityEnum priorityEnum = UnityWebRequestTaskPriorityEnum.Normal)
        {
            System.Action<object, string> completeCallback = TranslateAction(callback);
            if (completeCallback == null)
            {
                Debug.LogError("GetStringDataFromUrl Fail, is Null " + (callback == null));
                return;
            }

            //TODO从下载的缓存中读取数据
            if (TryAddCallbackOnWorkingTask(taskUrl, completeCallback, priorityEnum))
                return;

            CheckDownloadingTaskIsComplete(); //清理已经完成的任务
            //***创建一个请求
            UnityWebRequestDownloadTask newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = handler;

            if (mAllDownloadingUnityWebRequestTasks.Count >= S_MaxDownloadTaskCount)
            {
                PustUnityWebRequestTaskForWaitingList(newTask, completeCallback);
            }//等待
            else
            {
                newTask.InitialedUnityWebRequestDownloadTask(webRequest, priorityEnum, completeCallback, UnityWebRequestToString);

                mAllDownloadingUnityWebRequestTasks[taskUrl] = newTask;
                newTask.StartTask();
            }
        }
        private object UnityWebRequestToString(UnityWebRequest webRequest)
        {
            if (webRequest == null) return null;
            return (webRequest.downloadHandler as DownloadHandlerBuffer).text;
        }



        /// <summary>
        /// 获取一个 byte[]  数组对象
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetByteDataFromUrl(string taskUrl, System.Action<byte[], string> callback, UnityWebRequestTaskPriorityEnum priorityEnum = UnityWebRequestTaskPriorityEnum.Normal)
        {
            System.Action<object, string> completeCallback = TranslateAction(callback);
            if (completeCallback == null)
            {
                Debug.LogError("GetByteDataFromUrl Fail, is Null " + (callback == null));
                return;
            }

            //TODO从下载的缓存中读取数据
            if (TryAddCallbackOnWorkingTask(taskUrl, completeCallback, priorityEnum))
                return;

            CheckDownloadingTaskIsComplete(); //清理已经完成的任务
            //***创建一个请求
            UnityWebRequestDownloadTask newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = handler;

            if (mAllDownloadingUnityWebRequestTasks.Count >= S_MaxDownloadTaskCount)
            {
                PustUnityWebRequestTaskForWaitingList(newTask, completeCallback);
            }//等待
            else
            {
                newTask.InitialedUnityWebRequestDownloadTask(webRequest, priorityEnum, completeCallback, UnityWebRequestToByteData);

                mAllDownloadingUnityWebRequestTasks[taskUrl] = newTask;
                newTask.StartTask();
            }
        }
        private object UnityWebRequestToByteData(UnityWebRequest webRequest)
        {
            if (webRequest == null) return null;
            return (webRequest.downloadHandler as DownloadHandlerBuffer).data;
        }




        #endregion



        #region 辅助
        /// <summary>
        /// 检测等待的任务
        /// </summary>
        private void DOWaitingUnityWebRequestDownloadTask()
        {
            if (mAllDownloadingUnityWebRequestTasks.Count >= S_MaxDownloadTaskCount) return;
            if (mAllWaiteDownloadUnityWebRequestTasks.Count == 0) return;

            var waitingTask = mAllWaiteDownloadUnityWebRequestTasks.First;
            while (waitingTask!=null)
            {
                if (TryAddCallbackOnWorkingTask(waitingTask.Value)==false)
                {
#if UNITY_EDITOR
                    if (mAllDownloadingUnityWebRequestTasks.ContainsKey(waitingTask.Value.TaskUrl))
                        Debug.LogEditorError("DOWaitingUnityWebRequestDownloadTask Fail,Already Exit Task " + waitingTask.Value.TaskUrl);
#endif

                    mAllDownloadingUnityWebRequestTasks[waitingTask.Value.TaskUrl] = waitingTask.Value;
                    waitingTask.Value.StartTask();
                }

                if (mAllDownloadingUnityWebRequestTasks.Count >= S_MaxDownloadTaskCount)
                    return;
                waitingTask = waitingTask.Next;
            }
        }


        private void DoClearCompleteTask()
        {
            for (int dex = 0; dex < mAllCompleteTaskForDelete.Length; dex++)
            {
                var taskUrl = mAllCompleteTaskForDelete[dex];
                if (string.IsNullOrEmpty(taskUrl))
                    continue;

                mAllDownloadingUnityWebRequestTasks.Remove(taskUrl);
                mAllCompleteTaskForDelete[dex] = string.Empty;
            }
        }

        private void CheckDownloadingTaskIsComplete()
        {
            int dex = 0;
            DoClearCompleteTask();
            foreach (var workingTask in mAllDownloadingUnityWebRequestTasks)
            {
                if (workingTask.Value == null)
                {
                    mAllCompleteTaskForDelete[dex] = workingTask.Key;
                    ++dex;
                    continue;
                }

                if (workingTask.Value.IsCompleteInvoke)
                {
                    mAllCompleteTaskForDelete[dex] = workingTask.Key;
                    ++dex;
                    continue;
                }
            }
            if (dex > 0)
                DoClearCompleteTask();
        }

        /// <summary>
        /// 当插入任务或者定时检测时候发现任务完成了
        /// </summary>
        /// <param name="task"></param>
        private void NotifyOnUnityWebRequestTaskCompleted(UnityWebRequestDownloadTask task)
        {

        }


        /// <summary>
        /// 如果有正在下载的任务且没有完成回调 ，则直接把这个任务插入回调中
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        /// <returns>返回值标示是否成功的将当前需要下载的任务插入正在下载的任务回调中</returns>
        private bool TryAddCallbackOnWorkingTask(string taskUrl, System.Action<object, string> completeCallback, UnityWebRequestTaskPriorityEnum priorityEnum = UnityWebRequestTaskPriorityEnum.Normal)
        {
            foreach (var workingTask in mAllDownloadingUnityWebRequestTasks.Values)
            {
                if (workingTask.TaskUrl == taskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(completeCallback, priorityEnum);
                        return true;
                    } //插入正在下载的任务回调中
                }
            }

            return false;
        }

        private bool TryAddCallbackOnWorkingTask(UnityWebRequestDownloadTask task)
        {
            foreach (var workingTask in mAllDownloadingUnityWebRequestTasks.Values)
            {
                if (workingTask.TaskUrl == task.TaskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(task);
                        return true;
                    } //插入正在下载的任务回调中
                }
            }
            return false;
        }



        /// <summary>
        /// 将当前任务插入到等待链表中
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        private void PustUnityWebRequestTaskForWaitingList(UnityWebRequestDownloadTask task, System.Action<object, string> completeCallback)
        {
            LinkedListNode<UnityWebRequestDownloadTask> node = new LinkedListNode<UnityWebRequestDownloadTask>(task);

            if (mAllWaiteDownloadUnityWebRequestTasks.Count == 0)
            {
                mAllWaiteDownloadUnityWebRequestTasks.AddFirst(task);
                return;
            }

            var target = mAllWaiteDownloadUnityWebRequestTasks.First;
            while (target != null)
            {
                if (target.Value.mTaskPriorityEnum > task.mTaskPriorityEnum)
                {
                    target = target.Next;
                    continue;
                }

                if (target.Value.mTaskPriorityEnum == task.mTaskPriorityEnum)
                {
                    if (target.Value.TaskUrl != task.TaskUrl)
                    {
                        target = target.Next;
                        continue;
                    }

                    task.Dispose();  //公用一个下载器即可  次数可以想办法优化避免这里释放 TODO

                    target.Value.AddCompleteCallback(completeCallback); //插入到另一个等待任务回调链中
                    return;
                } //只能在这个优先级相等的地方
                else
                {
                    mAllWaiteDownloadUnityWebRequestTasks.AddBefore(target, task);
                    return;
                }
            }

            mAllWaiteDownloadUnityWebRequestTasks.AddLast(task);
        }
        #endregion


    }
}