using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    
    /// <summary>/// 以 DownloadHandlerBuffer 为下载器的下载接口/// </summary>
    public class ByteDataDownloadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static ByteDataDownloadManager s_Instance = null;

        public static ByteDataDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ByteDataDownloadManager();
                    DownloadManager.S_Instance.RegisterDownloadManager(s_Instance);
                }
                return s_Instance;
            }
        } // 



        /// <summary>/// 获取一个数据/// </summary>
        public UnityWebRequestDownloadTask GetDataFromUrl(string taskUrl, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return null;
            }
            //TODO从下载的缓存中读取数据


            //将新的下载任务添加到缓存的下载队列中
            UnityWebRequestDownloadTask newDownLoadTask = UnityWebRequestDownloadTask.GetUnityWebRequestDownloadTask(taskUrl,  callback, priorityEnum);
            newDownLoadTask.DownloadTaskCallbackData.downloadHandler = new DownloadHandlerBuffer();

            newDownLoadTask.ChangeDownloadState(TaskStateEum.Initialed);
            AllCacheDownLoadTasks.Add(newDownLoadTask);
            return newDownLoadTask;
        }

    




    }
}
