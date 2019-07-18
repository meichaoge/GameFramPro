using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    ///  提供泛型加载器的各种实现
    /// </summary>
    /// <typeparam name="U">任务类型，继承自 BaseDownloadTask< W></typeparam>
    /// <typeparam name="W">真是的下载器类型(UnityWebRequest)</typeparam>
    public abstract class BaseDownloadManager<U, W> : IDownloadManager<U> where U : BaseDownloadTask<W>
    {

        public LinkedList<U> AllDownloadTaskLinkedList { get; protected set; } = new LinkedList<U>();
        public LinkedList<U> AllCacheDownloadTaskLinkedList { get; protected set; } = new LinkedList<U>();

        public int MaxDownloadTaskCount { get; protected set; } = 20;
        public int CurDownloadingTaskCount { get; protected set; } = 0;
        public int TickPerUpdateCount { get; protected set; } = 10;
        protected int CurUpdateCount = 0;

        /// <summary>
        /// 管理器的Tick  负责遍历所有的任务，剔除已经完成的任务和开始新的下载任务
        /// </summary>
        public virtual void UpdateTick()
        {
            ++CurUpdateCount;

            if (AllDownloadTaskLinkedList != null && AllDownloadTaskLinkedList.Count != 0)
            {
                LinkedListNode<U> targetNode = AllDownloadTaskLinkedList.First;
                while (targetNode != null)
                {
                    if (targetNode.Value.IsCompleteInvoke)
                    {
                        var nextNode = targetNode.Next;
                        AllDownloadTaskLinkedList.Remove(targetNode);
                        targetNode = nextNode;
                        --CurDownloadingTaskCount;
                        continue;
                    }//完成了下载任务 需要删除旧任务

                    if (CurDownloadingTaskCount < MaxDownloadTaskCount)
                    {
                        if (targetNode.Value.DownloadTaskStateEnum == DownloadStateEnum.Waiting)
                        {
                            ++CurDownloadingTaskCount;
                            targetNode.Value.StartDownloadTask();
                            targetNode = targetNode.Next;
                            continue;
                        }
                    } //启动新的下载任务

                    targetNode = targetNode.Next;
                }
            }

            if (AllCacheDownloadTaskLinkedList.Count > 0)
                DownloadUtility<W>.TryDealWithCacheDownloadTask(this);

            if (CurUpdateCount == TickPerUpdateCount)
            {
                CurUpdateCount = 0;
                if (AllDownloadTaskLinkedList != null && AllDownloadTaskLinkedList.Count != 0)
                {
                    LinkedListNode<U> targetNode = AllDownloadTaskLinkedList.First;
                    while (targetNode != null)
                    {
                        targetNode.Value.Tick();
                        targetNode = targetNode.Next;
                    }
                }
            }

        }






    }
}