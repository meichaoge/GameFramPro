using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    public class ByteDataDownloadManager : UnityWebRequestDownLoadManager
    {
        protected static ByteDataDownloadManager s_Instance = null;
        public static ByteDataDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new ByteDataDownloadManager();
                return s_Instance;
            }
        }// 


        protected override UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }



     
    }
}