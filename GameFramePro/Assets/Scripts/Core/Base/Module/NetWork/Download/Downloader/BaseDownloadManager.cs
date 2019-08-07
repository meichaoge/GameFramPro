using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>///  提供泛型加载器的各种实现/// </summary>
    /// <typeparam name="U">任务类型，继承自 BaseDownloadTask< W></typeparam>
    /// <typeparam name="W">真实的下载器类型(比如 UnityWebRequest)<</typeparam>
    public abstract class BaseDownloadManager<U, W> : IUpdateCountTick where U : BaseDownloadTask<W>
    {
        /// <summary>///  所有的下载任务，按照优先级排序，包含已经开始的和等待的任务/// </summary>
        public LinkedList<U> AllDownloadTaskLinkedList { get; protected set; } = new LinkedList<U>();

        /// <summary>///  所有的任务请求会先进入这里然后在 Tick 时候加入 AllDownloadTaskLinkedList/// </summary>
        public LinkedList<U> AllCacheDownloadTaskLinkedList { get; protected set; } = new LinkedList<U>();

        public List<U> AllCacheDownLoadTasks = new List<U>(20); //所有下一帧需要处理的回调

        /// <summary>/// 最大的同时下载任务量/// </summary>
        public int MaxDownloadTaskCount { get; protected set; } = 20;

        /// <summary>/// 下载中的任务总数/// </summary>
        public int CurDownloadingTaskCount { get; protected set; } = 0;

        /// <summary>/// 等待中的任务总数/// </summary>
        public int CurWaitingTaskCount { get; protected set; } = 0;


        #region IUpdateTick 接口

        public uint TickPerUpdateCount { get; protected set; } = 15;

        public void UpdateTick(float currentTime)
        {
            TickAllTasks();
            RemoveAllCompleteTasks();
            DealCacheTask();
            StartWaitingTaskTask();
        }

        #endregion

        #region 刷新当前类型的下载任务的状态

        /// <summary>/// 更新所有的下载任务状态/// </summary>
        protected virtual void TickAllTasks()
        {
            curUpdateCount = 0;
            if (AllDownloadTaskLinkedList != null && AllDownloadTaskLinkedList.Count != 0)
            {
                foreach (var targetNode in AllDownloadTaskLinkedList)
                {
                    if (targetNode == null) continue;
                    targetNode.Tick();
                    if (targetNode.TaskState == TaskStateEum.Running)
                        ++curUpdateCount;
                }
            }
        }

        /// <summary>/// 删除已经完成的下载任务/// </summary>
        protected virtual void RemoveAllCompleteTasks()
        {
            if (AllDownloadTaskLinkedList == null || AllDownloadTaskLinkedList.Count == 0)
                return;
            var targetTaskNode = AllDownloadTaskLinkedList.First;
            while (targetTaskNode != null)
            {
                if (targetTaskNode.Value.IsCompleteInvoke)
                {
                    var next = targetTaskNode.Next;
                    AllDownloadTaskLinkedList.Remove(targetTaskNode);
                    targetTaskNode = next;
                    continue;
                }

                targetTaskNode = targetTaskNode.Next;
            }
        }

        /// <summary>/// 将缓存中的任务加入现有的任务链表中，同时剔除已经完成的任务和开始新的下载任务/// </summary>
        protected virtual void DealCacheTask()
        {
            if (AllCacheDownLoadTasks == null || AllCacheDownLoadTasks.Count == 0)
                return;
            foreach (var cacheTask in AllCacheDownLoadTasks)
            {
                if (cacheTask == null) continue;
                if (cacheTask.TaskState != TaskStateEum.Initialed)
                {
                    Debug.LogError($"缓存队列中的任务{cacheTask.TaskUrl}状态{cacheTask.TaskState} 异常");
                    continue;
                }

                #region 按照任务的优先级插入到下载队列中

                var targetNode = AllDownloadTaskLinkedList.First;
                while (targetNode != null)
                {
                    if (targetNode.Value.TaskPriorityEnum > cacheTask.TaskPriorityEnum)
                    {
                        targetNode = targetNode.Next;
                        continue;
                    } //优先级不同

                    var targetNodeValue = targetNode.Value;
                    if (targetNodeValue.TaskPriorityEnum == cacheTask.TaskPriorityEnum)
                    {
                        if (targetNodeValue.TaskUrl == cacheTask.TaskUrl)
                        {
                            if (targetNodeValue.IsCompleteInvoke == false)
                            {
                                targetNodeValue.AddCompleteCallback(cacheTask);
                                return;
                            }
                            else
                            {
                                Debug.LogError("当前下载任务已经完成，无法添加回调 " + targetNodeValue.TaskUrl);
                            }
                        }
                    }

                    if (targetNodeValue.TaskPriorityEnum < cacheTask.TaskPriorityEnum)
                    {
                        cacheTask.ChangeDownloadState(TaskStateEum.Initialed);
                        AllDownloadTaskLinkedList.AddBefore(targetNode, cacheTask); //添加新的任务到后面
                        ++CurWaitingTaskCount;
                        return;
                    }

                    targetNode = targetNode.Next;
                }

                cacheTask.ChangeDownloadState(TaskStateEum.Initialed);
                AllDownloadTaskLinkedList.AddLast(cacheTask); //添加新的任务到最后面
                ++CurWaitingTaskCount;

                #endregion
            }

            AllCacheDownLoadTasks.Clear();
        }


        protected virtual void StartWaitingTaskTask()
        {
            if (CurWaitingTaskCount == 0) return;

            if (AllDownloadTaskLinkedList != null && AllDownloadTaskLinkedList.Count != 0)
            {
                foreach (var targetNode in AllDownloadTaskLinkedList)
                {
                    if (targetNode == null) continue;
                    if (CurDownloadingTaskCount >= MaxDownloadTaskCount)
                        return;
                    if (targetNode.TaskState == TaskStateEum.Initialed)
                    {
                        targetNode.StartDownloadTask();
                        --CurWaitingTaskCount;
                        ++CurDownloadingTaskCount;
                    }
                }
            }
        }


//        /// <summary>///  /// </summary>
//        protected virtual void RemoveAndStartTasks()
//        {
//
//            if (AllDownloadTaskLinkedList != null && AllDownloadTaskLinkedList.Count != 0)
//            {
//                LinkedListNode<U> targetNode = AllDownloadTaskLinkedList.First;
//                while (targetNode != null)
//                {
//                    if (targetNode.Value.IsCompleteInvoke)
//                    {
//                        var nextNode = targetNode.Next;
//                        AllDownloadTaskLinkedList.Remove(targetNode);
//                        targetNode = nextNode;
//                        --CurDownloadingTaskCount;
//                        continue;
//                    } //完成了下载任务 需要删除旧任务
//
//                    if (CurDownloadingTaskCount < MaxDownloadTaskCount)
//                    {
//                        if (targetNode.Value.TaskState == TaskStateEum.Initialed)
//                        {
//                            ++CurDownloadingTaskCount;
//                            targetNode.Value.StartDownloadTask();
//                            targetNode = targetNode.Next;
//                            continue;
//                        }
//                    } //启动新的下载任务
//
//                    targetNode = targetNode.Next;
//                }
//            }
//        }

        #endregion

        #region 任务状态变更

        #endregion
    }
}
