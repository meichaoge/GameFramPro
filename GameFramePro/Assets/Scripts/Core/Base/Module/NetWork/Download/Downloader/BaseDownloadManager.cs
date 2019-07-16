using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    ///  提供泛型加载器的各种实现
    /// </summary>
    /// <typeparam name="T">需要加载的资源类型(AssetBundle,byte,)</typeparam>
    /// <typeparam name="U">任务类型，继承自 BaseDownloadTask<T, W></typeparam>
    /// <typeparam name="W">真是的下载器类型(UnityWebRequest)</typeparam>
    public abstract class BaseDownloadManager<T, U, W> :  IDownloadManager<U> where U : BaseDownloadTask<T, W>
    {
        //所有的正在下载中的 UnityWebRequest 任务
        public Dictionary<string, U> AllDownloadingTasks { get; private set; }
        //按照优先级排序
        public LinkedList<U> AllWaitDownloadTasks { get; private set; }
        public int MaxDownloadTaskCount { get; private set; } = 20;
      


        public virtual void InitialedManager()
        {
            AllDownloadingTasks = new Dictionary<string, U>();
            AllWaitDownloadTasks = new LinkedList<U>();
        }


        #region 对象池接口
        protected virtual void OnBeforGetWebRequestTaskItem(U task)
        {
        }

        protected virtual void OnBeforRecycleWebRequestTaskItem(U task)
        {
            if (task != null)
            {
                task.ClearDownloadTask();
            }
        }
        #endregion

        public virtual void Tick()
        {
            if (AllDownloadingTasks != null && AllDownloadingTasks.Count > 0)
            {
                DownloadUtility<W>.ClearCompletedTask<T, U>(this);
                foreach (var workingTask in AllDownloadingTasks.Values)
                    workingTask.Tick();
                DownloadUtility<W>.DoWaitingDownloadTask<T, U>(this);
            }
        }


        /// <summary>
        /// 获取一个数据
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public void GetDataFromUrl(string taskUrl, System.Action<T, bool, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
        {
            if (callback == null)
            {
                Debug.LogError("GetDataFromUrl Fail, is Null " + (callback == null));
                return;
            }

            //TODO从下载的缓存中读取数据
            if (DownloadUtility<W>.TryAddCallbackOnWorkingTask(taskUrl, callback, AllDownloadingTasks, priorityEnum))
                return;

            DownloadUtility<W>.ClearCompletedTask<T, U>(this);   //清理已经完成的任务

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
                    isAddCallbackLinkedList = DownloadUtility<W>.TryAddDownloadTaskAtWaitingList<T, U>(taskUrl, priorityEnum, AllWaitDownloadTasks, out targetNode);
                }


                if (isAddCallbackLinkedList)
                {
                    targetNode.Value.AddCompleteCallback(callback);
                }
                else
                {
                    //var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
                    //UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
                    //webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
                    //newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, UnityWebRequestToAssetBundle);

                    U newDownLoadTask = GetDownloadTaskInstance(taskUrl, callback, priorityEnum);

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
                //var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
                //UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
                //webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
                //newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, UnityWebRequestToAssetBundle);

                U newDownLoadTask = GetDownloadTaskInstance(taskUrl, callback, priorityEnum);
                AllDownloadingTasks[taskUrl] = newDownLoadTask;
                newDownLoadTask.StartDownloadTask();
            }//直接加入到正在执行的下载任务中 并启动任务
        }


        protected abstract U GetDownloadTaskInstance(string taskUrl, System.Action<T, bool, string> callback, UnityTaskPriorityEnum priorityEnum);
        //{
        //    //var newTask = mUnityWebRequestTaskPoolManager.GetItemFromPool();
        //    //UnityWebRequest webRequest = new UnityWebRequest(taskUrl);
        //    //webRequest.downloadHandler = new DownloadHandlerAssetBundle(taskUrl, uint.MaxValue);
        //    //newTask.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback, UnityWebRequestToAssetBundle);
        //}



        /// <summary>
        /// 将结果转换成需要的数据
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        protected abstract T GetDataFromDownloadHandler(W downloadHandler);
        //{
        //    if (realDownloader == null) return default(T);
        //    return (webRequest.downloadHandler as DownloadHandlerAssetBundle).assetBundle;
        //}





    }
}