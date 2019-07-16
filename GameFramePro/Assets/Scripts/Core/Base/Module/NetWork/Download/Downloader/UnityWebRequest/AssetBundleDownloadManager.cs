using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载AssetBundle 的下载器
    /// </summary>
    public class AssetBundleDownloadManager : UnityWebRequestDownLoadManager
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
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }


        
    }
}