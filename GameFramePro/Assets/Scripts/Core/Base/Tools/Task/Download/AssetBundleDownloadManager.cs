using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载AssetBundle 的下载器
    /// </summary>
    public class AssetBundleDownloadManager : IDownloadManager<AssetBundle, UnityWebRequestDownloadTask<AssetBundle>>
    {
        //所有的正在下载中的 UnityWebRequest 任务
        public Dictionary<string, UnityWebRequestDownloadTask<AssetBundle>> AllDownloadingTasks { get; private set; }
        //按照优先级排序
        public LinkedList<UnityWebRequestDownloadTask<AssetBundle>> AllWaitDownloadTasks { get; private set; }
        public NativeObjectPool<UnityWebRequestDownloadTask<AssetBundle>> mUnityWebRequestTaskPoolManager = null;
        public int MaxDownloadTaskCount { get; private set; } = 20;

        public void InitialedManager()
        {
            AllDownloadingTasks = new Dictionary<string, UnityWebRequestDownloadTask<AssetBundle>>();
            AllWaitDownloadTasks = new LinkedList<UnityWebRequestDownloadTask<AssetBundle>>();
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask<AssetBundle>>(MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
        }


        #region 对象池接口
        private void OnBeforGetWebRequestTaskItem(UnityWebRequestDownloadTask<AssetBundle> task)
        {
        }

        private void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask<AssetBundle> task)
        {
            if (task != null)
            {
                task.Dispose();
            }
        }
        #endregion



        /// <summary>
        /// 获取一个AssetBundle 对象
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetAssetBundleFromUrl(string taskUrl, System.Action<AssetBundle, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
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
            var  newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
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
        private AssetBundle UnityWebRequestToAssetBundle(UnityWebRequest webRequest)
        {
            if (webRequest == null) return null;
            return (webRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        }



        public void ClearCompletedTask()
        {
            var taskKeys = AllDownloadingTasks.Keys;
            UnityWebRequestDownloadTask<AssetBundle> taskInfor = null;
            foreach (var taskUrl in taskKeys)
            {
                if(AllDownloadingTasks.TryGetValue(taskUrl,out taskInfor))
                {
                    if(taskInfor.IsCompleteInvoke)
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
            DownloadUtility.DoWaitingDownloadTask(this);
        }


    }
}