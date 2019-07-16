using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    public class ByteDataDownloadManager : BaseDownloadManager<byte[], UnityWebRequestDownloadTask<byte[]>, UnityWebRequest>
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

        ByteDataDownloadManager()
        {
            InitialedManager();
        }



        public NativeObjectPool<UnityWebRequestDownloadTask<byte[]>> mUnityWebRequestTaskPoolManager = null;

        

        public override void InitialedManager()
        {
            base.InitialedManager();
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask<byte[]>>(MaxDownloadTaskCount, OnBeforGetWebRequestTaskItem, OnBeforRecycleWebRequestTaskItem);
            //       DownloadManager.S_Instance.RegisterDownloadManager(this as IDownloadManager<object, ITaskProcess>);
        }

        protected override UnityWebRequestDownloadTask<byte[]> GetDownloadTaskInstance(string taskUrl, Action<byte[], bool, string> callback, UnityTaskPriorityEnum priorityEnum)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, GetDataFromDownloadHandler);
            return newTask;
        }

        protected override byte[] GetDataFromDownloadHandler(UnityWebRequest downloadHandler)
        {
            if (downloadHandler == null) return null;
            return (downloadHandler.downloadHandler as DownloadHandlerBuffer).data;
        }



     
    }
}