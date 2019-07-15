using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    public class StringDataDownloadManager : Single<StringDataDownloadManager>, IDownloadManager<string, UnityWebRequestDownloadTask<string>>
    {
        //所有的正在下载中的 UnityWebRequest 任务
        public Dictionary<string, UnityWebRequestDownloadTask<string>> AllDownloadingTasks { get; private set; }
        //按照优先级排序
        public LinkedList<UnityWebRequestDownloadTask<string>> AllWaitDownloadTasks { get; private set; }
        public NativeObjectPool<UnityWebRequestDownloadTask<string>> mUnityWebRequestTaskPoolManager = null;
        public int MaxDownloadTaskCount { get; private set; } = 10;


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedManager();
       //     DownloadManager.S_Instance.RegisterDownloadManager(this as IDownloadManager<object, ITaskProcess>);
        }
        public override void DisposeInstance()
        {
        //    DownloadManager.S_Instance.UnRegisterDownloadManager(this as IDownloadManager<object, ITaskProcess>);
            base.DisposeInstance();
        }
        public void InitialedManager()
        {
            AllDownloadingTasks = new Dictionary<string, UnityWebRequestDownloadTask<string>>();
            AllWaitDownloadTasks = new LinkedList<UnityWebRequestDownloadTask<string>>();
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask<string>>(MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
        }


        #region 对象池接口
        private void OnBeforGetWebRequestTaskItem(UnityWebRequestDownloadTask<string> task)
        {
        }

        private void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask<string> task)
        {
            if (task != null)
            {
                task.Dispose();
            }
        }
        #endregion



        /// <summary>
        /// 获取一个string 对象
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetStringDataFromUrl(string taskUrl, System.Action<string, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
        {
            if (callback == null)
            {
                Debug.LogError("GetAssetBundleFromUrl Fail, is Null " + (callback == null));
                return;
            }

            //TODO从下载的缓存中读取数据
            if (DownloadUtility.TryAddCallbackOnWorkingTask(taskUrl, callback, AllDownloadingTasks, priorityEnum))
                return;

            ClearCompletedTask(); //清理已经完成的任务
            //***创建一个请求
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            DownloadHandlerBuffer handler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = handler;

            if (AllDownloadingTasks.Count >= MaxDownloadTaskCount)
            {
                DownloadUtility.PustUnityWebRequestTaskForWaitingList(newTask, callback, AllWaitDownloadTasks);
            }//等待
            else
            {
                newTask.InitialedUnityWebRequestDownloadTask(webRequest, priorityEnum, callback, UnityWebRequestToAssetBundle);

                AllDownloadingTasks[taskUrl] = newTask;
                newTask.StartTask();
            }
        }
        /// <summary>
        /// 将结果转换成需要的数据
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        private string UnityWebRequestToAssetBundle(UnityWebRequest webRequest)
        {
            if (webRequest == null) return null;
            return (webRequest.downloadHandler as DownloadHandlerBuffer).text;
        }



        public void ClearCompletedTask()
        {
            var taskKeys = AllDownloadingTasks.Keys;
            UnityWebRequestDownloadTask<string> taskInfor = null;
            foreach (var taskUrl in taskKeys)
            {
                if (AllDownloadingTasks.TryGetValue(taskUrl, out taskInfor))
                {
                    if (taskInfor.IsCompleteInvoke)
                    {
                        AllDownloadingTasks.Remove(taskUrl); //完成了任务
                    }//完成了任务
                }
                else
                {
                    Debug.LogError("ClearCompletedTask Fail,Not Find Task " + taskUrl);
                }
            }
        }

        public void Tick()
        {
            ClearCompletedTask();
            foreach (var workingTask in AllDownloadingTasks)
            {
                workingTask.Value.Tick();
            }

            DownloadUtility.DoWaitingDownloadTask(this);
        }
    }
}