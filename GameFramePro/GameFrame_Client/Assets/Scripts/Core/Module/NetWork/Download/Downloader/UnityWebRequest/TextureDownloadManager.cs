using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.CacheEx;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 下载图片/// </summary>
    public class TextureDownloadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static TextureDownloadManager s_Instance = null;

        public static TextureDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new TextureDownloadManager();
                    DownloadManager.RegisterDownloadManager(s_Instance);
                }

                return s_Instance;
            }
        } // 

        #region  各种类型获取数据的接口

        /// <summary>/// 获取一个数据/// </summary>
        public virtual UnityWebRequestDownloadTask GetDataFromUrl(string taskUrl, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 60)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return null;
            }
            //TODO从下载的缓存中读取数据

            //将新的下载任务添加到缓存的下载队列中
            UnityWebRequestDownloadTask newDownLoadTask = UnityWebRequestDownloadTask.GetUnityWebRequestDownloadTask(taskUrl, callback, priorityEnum, timeOut);
            newDownLoadTask.DownloadTaskCallbackData.downloadHandler = new DownloadHandlerTexture();
            newDownLoadTask.ChangeDownloadState(TaskStateEum.Initialed);
            AllCacheDownLoadTasks.Add(newDownLoadTask);
            return newDownLoadTask;
        }



        #endregion
    }
}
