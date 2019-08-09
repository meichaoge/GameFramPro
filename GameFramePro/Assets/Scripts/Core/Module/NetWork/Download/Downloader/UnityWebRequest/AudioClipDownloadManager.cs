﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 声音资源下载器/// </summary>
    public class AudioClipDownloadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static AudioClipDownloadManager s_Instance = null;

        public static AudioClipDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new AudioClipDownloadManager();
                return s_Instance;
            }
        } // 

        #region  各种类型获取数据的接口

        /// <summary>/// 获取一个数据/// </summary>
        public virtual UnityWebRequestDownloadTask GetDataFromUrl(string taskUrl, AudioType type, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return null;
            }
            //TODO从下载的缓存中读取数据

            //将新的下载任务添加到缓存的下载队列中
            UnityWebRequestDownloadTask newDownLoadTask = GetDownloadTaskInstance(taskUrl, type, callback, priorityEnum);
            newDownLoadTask.ChangeDownloadState(TaskStateEum.Initialed);
            AllCacheDownLoadTasks.Add(newDownLoadTask);
            return newDownLoadTask;
        }


        protected UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, AudioType type, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum)
        {
            var newTask = UnityWebRequestTaskUtility.S_Instance.GetUnityWebRequestDownloadTaskFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl)
            {
                timeout = UnityWebRequestDownloadTask.S_Timeout,
                downloadHandler = new DownloadHandlerAudioClip(taskUrl, AudioType.MPEG),
            };
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }

        #endregion
    }
}
