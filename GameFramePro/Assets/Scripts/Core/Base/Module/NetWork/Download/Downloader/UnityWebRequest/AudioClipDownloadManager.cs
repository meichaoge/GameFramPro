using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 声音资源下载器
    /// </summary>
    public class AudioClipDownloadManager: UnityWebRequestDownLoadManager
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
        }// 



        protected override UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerAudioClip(taskUrl, AudioType.MPEG);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }
    }
}