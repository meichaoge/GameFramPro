using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    ///  提供泛型加载器的各种实现
    /// </summary>
    /// <typeparam name="U">任务类型，继承自 BaseDownloadTask< W></typeparam>
    /// <typeparam name="W">真是的下载器类型(UnityWebRequest)</typeparam>
    public abstract class BaseDownloadManager<U, W> : IDownloadManager<U> where U : BaseDownloadTask<W>
    {
        public Dictionary<string, U> AllDownloadingTasks { get; protected set; }= new Dictionary<string, U>();

        public LinkedList<U> AllWaitDownloadTasks { get; protected set; } = new LinkedList<U>();
        public int MaxDownloadTaskCount { get; protected set; } = 20;




        public virtual void Tick()
        {
            if (AllDownloadingTasks != null && AllDownloadingTasks.Count > 0)
            {
                ClearCompletedTask();
                foreach (var workingTask in AllDownloadingTasks.Values)
                    workingTask.Tick();
                DownloadUtility<W>.DoWaitingDownloadTask< U>(this);
            }
        }


        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public virtual void GetDataFromUrl(string taskUrl, System.Action<W, bool, string> callback, UnityTaskPriorityEnum priorityEnum=UnityTaskPriorityEnum.Normal,params object[] otherParameter)
        {
            if (callback == null)
            {
                Debug.LogError("GetDataFromUrl Fail, is Null " + (callback == null));
                return;
            }

            //TODO从下载的缓存中读取数据
            if (DownloadUtility<W>.TryAddCallbackOnWorkingTask(taskUrl, callback, AllDownloadingTasks, priorityEnum))
                return;

            ClearCompletedTask();         //清理已经完成的任务

            if (AllDownloadingTasks.Count >= MaxDownloadTaskCount)
            {
                LinkedListNode<U> targetNode = null; //要插入的新节点的位置
                bool isAddFirstNode = false;// 是否需要插入到最前面
                bool isAddCallbackLinkedList = false;  //如果这个值为true  则只需要把回调插入targetNode 中
                if (AllWaitDownloadTasks == null || AllWaitDownloadTasks.Count == 0)
                {
                    isAddFirstNode = true;
                    targetNode = null;
                    isAddCallbackLinkedList = false;
                }
                else
                {
                    isAddFirstNode = false;
                    isAddCallbackLinkedList = DownloadUtility<W>.TryAddDownloadTaskAtWaitingList< U>(taskUrl, priorityEnum, AllWaitDownloadTasks, out targetNode);
                }


                if (isAddCallbackLinkedList)
                {
                    targetNode.Value.AddCompleteCallback(callback);
                }
                else
                {
                    U newDownLoadTask = GetDownloadTaskInstance(taskUrl, callback, priorityEnum, otherParameter);

                    if (isAddFirstNode)
                    {
                        AllWaitDownloadTasks.AddFirst(newDownLoadTask);
                    } //第一个等待任务  只在之前没有等待任务时候会走这个状态
                    else
                    {
                        if (targetNode == null)
                            AllWaitDownloadTasks.AddLast(newDownLoadTask);
                        else
                            AllWaitDownloadTasks.AddBefore(targetNode, newDownLoadTask);

                    }
                } //需要新建一个下载任务

            }//需要添加到等待任务链表中
            else
            {

                U newDownLoadTask = GetDownloadTaskInstance(taskUrl, callback, priorityEnum, otherParameter);
                AllDownloadingTasks[taskUrl] = newDownLoadTask;
                newDownLoadTask.StartDownloadTask();
            }//直接加入到正在执行的下载任务中 并启动任务
        }


        protected abstract U GetDownloadTaskInstance(string taskUrl, System.Action<W, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter);
        //{
        //    //var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
        //    //UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
        //    //webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
        //    //newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, UnityWebRequestToAssetBundle);
        //}

        /// <summary>
        ///  清理已经完成的任务
        /// </summary>
        public virtual void ClearCompletedTask()
        {
            if (AllDownloadingTasks == null || AllDownloadingTasks.Count == 0)
                return;

            HashSet<string> allTaskKeys = new HashSet<string>();
            foreach (var item in AllDownloadingTasks.Keys)
            {
                allTaskKeys.Add(item);
            }


            U taskInfor = null;
            foreach (var taskKey in allTaskKeys)
            {
                if (AllDownloadingTasks.TryGetValue(taskKey, out taskInfor))
                {
                    if (taskInfor.IsCompleteInvoke)
                    {
                        AllDownloadingTasks.Remove(taskKey); //完成了任务
                    }//完成了任务
                }
                else
                {
                    Debug.LogError("ClearCompletedTask Fail,Not Find Task " + taskKey);
                }
            }
        }






    }
}