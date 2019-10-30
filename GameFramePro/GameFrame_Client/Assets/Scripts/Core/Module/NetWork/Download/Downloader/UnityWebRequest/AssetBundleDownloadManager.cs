using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 下载AssetBundle 的下载器/// </summary>
    public class AssetBundleDownloadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static AssetBundleDownloadManager s_Instance = null;

        public static AssetBundleDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new AssetBundleDownloadManager();
                    DownloadManager.S_Instance.RegisterDownloadManager(s_Instance);
                }
                return s_Instance;
            }
        } // 


        #region  各种类型获取数据的接口

        /// <summary>/// 获取一个数据/// </summary>
        public virtual UnityWebRequestDownloadTask GetDataFromUrl(string taskUrl, uint crc, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return null;
            }
            //TODO从下载的缓存中读取数据

            //将新的下载任务添加到缓存的下载队列中
            UnityWebRequestDownloadTask newDownLoadTask = UnityWebRequestDownloadTask.GetUnityWebRequestDownloadTask(taskUrl,  callback, priorityEnum);
            newDownLoadTask.DownloadTaskCallbackData.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, crc);
            newDownLoadTask.ChangeDownloadState(TaskStateEum.Initialed);
            AllCacheDownLoadTasks.Add(newDownLoadTask);
            return newDownLoadTask;
        }


     

        #endregion
    }
}
