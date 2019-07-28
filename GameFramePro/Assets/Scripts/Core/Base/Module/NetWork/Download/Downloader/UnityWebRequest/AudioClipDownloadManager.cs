﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 声音资源下载器
    /// </summary>
    public class AudioClipDownloadManager : UnityWebRequestDownLoadManager
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

        #region  各种类型获取数据的接口
        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public virtual void GetDataFromUrl(string taskUrl, AudioType type, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
        {
            if (string.IsNullOrEmpty(taskUrl))
            {
                Debug.LogError("TryAddToDownloadTaskLink Fail,Parameter is null of taskUrl");
                return;
            }
            //TODO从下载的缓存中读取数据

            #region 将新的下载任务添加到缓存的下载链表中

            UnityWebRequestDownloadTask newDownLoadTask = null;
            LinkedListNode<UnityWebRequestDownloadTask> targetNode = AllCacheDownloadTaskLinkedList.First;
            while (targetNode != null)
            {
                if (targetNode.Value.TaskPriorityEnum > priorityEnum)
                {
                    targetNode = targetNode.Next;
                    continue;
                } //优先级不同
                var targetNodeValue = targetNode.Value;
                if (targetNodeValue.TaskPriorityEnum == priorityEnum)
                {
                    if (targetNodeValue.TaskUrl == taskUrl)
                    {
                        if (targetNodeValue.IsCompleteInvoke == false)
                        {
                            targetNodeValue.AddCompleteCallback(callback);
                            return;
                        }
                        else
                            Debug.LogError("当前下载任务已经完成，无法添加回调 " + targetNodeValue.TaskUrl);
                    }
                }

                if (targetNodeValue.TaskPriorityEnum < priorityEnum)
                {
                    newDownLoadTask = GetDownloadTaskInstance(taskUrl,type, callback, priorityEnum);
                    newDownLoadTask.ChangeDownloadState(DownloadStateEnum.Waiting);
                    AllCacheDownloadTaskLinkedList.AddBefore(targetNode, newDownLoadTask); //添加新的任务到后面
                    return;
                }

                targetNode = targetNode.Next;
            }
            newDownLoadTask = GetDownloadTaskInstance(taskUrl,type, callback, priorityEnum);
            newDownLoadTask.ChangeDownloadState(DownloadStateEnum.Waiting);
            AllCacheDownloadTaskLinkedList.AddLast(newDownLoadTask); //添加新的任务到最后面
            #endregion
        }


        protected UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, AudioType type, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum)
        {
            var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
            webRequest.timeout = mTimeOut;
            webRequest.downloadHandler = new DownloadHandlerAudioClip(taskUrl, AudioType.MPEG);
            newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return newTask;
        }

        #endregion




    }
}