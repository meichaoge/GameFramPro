using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    
    /// <summary>/// 以 DownloadHandlerBuffer 为下载器的下载接口/// </summary>
    public class ByteDataDownloadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static ByteDataDownloadManager s_Instance = null;

        public static ByteDataDownloadManager S_Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new ByteDataDownloadManager();
                    DownloadManager.S_Instance.RegisterDownloadManager(s_Instance);
                }
                return s_Instance;
            }
        } // 



        /// <summary>
        /// 获取一个Byte buffer 
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="rangeFrom">如果与rangEnd 同时为0 则忽略这两个参数，否则作为头部信息的一部分</param>
        /// <param name="rangEnd"></param>
        /// <param name="callback"></param>
        /// <param name="priorityEnum"></param>
        /// <returns></returns>
        public UnityWebRequestDownloadTask GetDataFromUrl(string taskUrl, long rangeFrom, long rangEnd, Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 6)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return null;
            }
       
            //TODO从下载的缓存中读取数据
            //将新的下载任务添加到缓存的下载队列中
            UnityWebRequestDownloadTask newDownLoadTask = UnityWebRequestDownloadTask.GetUnityWebRequestDownloadTask(taskUrl,  callback, priorityEnum, timeOut);
            newDownLoadTask.DownloadTaskCallbackData.downloadHandler = new DownloadHandlerBuffer();
            newDownLoadTask.mRangeFrom = rangeFrom;
            newDownLoadTask.mRangeTo = rangEnd;

            if (rangeFrom != 0 || rangEnd != 0)
            {
                newDownLoadTask.DownloadTaskCallbackData.SetRequestHeader("Range", $"bytes={rangeFrom}-{rangEnd}");
            }

            newDownLoadTask.ChangeDownloadState(TaskStateEum.Initialed);
            AllCacheDownLoadTasks.Add(newDownLoadTask);
            return newDownLoadTask;
        }

       






    }
}
