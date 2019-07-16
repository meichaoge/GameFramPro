using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    public class StringDataDownloadManager : BaseDownloadManager<string, UnityWebRequestDownloadTask<string>, UnityWebRequest>
    {
        protected static StringDataDownloadManager s_Instance = null;
        public static StringDataDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new StringDataDownloadManager();
                return s_Instance;
            }
        }// 
        StringDataDownloadManager()
        {
            InitialedManager();
        }


        public NativeObjectPool<UnityWebRequestDownloadTask<string>> mUnityWebRequestTaskPoolManager = null;

        public override void InitialedManager()
        {
            base.InitialedManager();
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask<string>>(MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
            //       DownloadManager.S_Instance.RegisterDownloadManager(this as IDownloadManager<object, ITaskProcess>);
        }

        protected override UnityWebRequestDownloadTask<string> GetDownloadTaskInstance(string taskUrl, Action<string, bool, string> callback, UnityTaskPriorityEnum priorityEnum)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, GetDataFromDownloadHandler);
            return newTask;
        }

        protected override string GetDataFromDownloadHandler(UnityWebRequest downloadHandler)
        {
            if (downloadHandler == null) return null;
            return (downloadHandler.downloadHandler as DownloadHandlerBuffer).text;
        }



    }
}