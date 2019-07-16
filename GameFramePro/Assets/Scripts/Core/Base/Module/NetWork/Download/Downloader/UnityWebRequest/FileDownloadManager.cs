using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载并保存到本地的文件
    /// </summary>
    public class FileDownloadManager : UnityWebRequestDownLoadManager
    {
        protected static AssetBundleDownloadManager s_Instance = null;
        public static AssetBundleDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new AssetBundleDownloadManager();
                return s_Instance;
            }
        }// 





        protected override UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            if(otherParameter==null|| otherParameter.Length == 0)
            {
                Debug.LogError("GetDownloadTaskInstance Fail,FileDownloadManager Need saveFilePath Parameter");
            }
            webRequest.downloadHandler = new DownloadHandlerFile(otherParameter[0].ToString());

            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }
    }
}