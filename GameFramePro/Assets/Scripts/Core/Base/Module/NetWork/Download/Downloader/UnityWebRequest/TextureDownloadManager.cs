using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载图片
    /// </summary>
    public class TextureDownloadManager : UnityWebRequestDownLoadManager
    {
        protected static TextureDownloadManager s_Instance = null;
        public static TextureDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new TextureDownloadManager();
                return s_Instance;
            }
        }// 



        protected override UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerTexture();
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }
    }
}