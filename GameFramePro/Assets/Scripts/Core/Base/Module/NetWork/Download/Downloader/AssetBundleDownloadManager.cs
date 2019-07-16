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
    public class AssetBundleDownloadManager : BaseDownloadManager<AssetBundle, UnityWebRequestDownloadTask<AssetBundle>, UnityWebRequest>
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
        AssetBundleDownloadManager()
        {
            InitialedManager();
        }


        public NativeObjectPool<UnityWebRequestDownloadTask<AssetBundle>> mUnityWebRequestTaskPoolManager = null;

        public override void InitialedManager()
        {
            base.InitialedManager();
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask<AssetBundle>>(MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
     //       DownloadManager.S_Instance.RegisterDownloadManager(this as IDownloadManager<object, ITaskProcess>);
        }

        protected override UnityWebRequestDownloadTask<AssetBundle> GetDownloadTaskInstance(string taskUrl, Action<AssetBundle, bool, string> callback, UnityTaskPriorityEnum priorityEnum)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, GetDataFromDownloadHandler);
            return newTask;
        }

        protected override AssetBundle GetDataFromDownloadHandler(UnityWebRequest downloadHandler)
        {
            if (downloadHandler == null) return null;
            return (downloadHandler.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        }

        
    }
}