﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载任务的优先级
    /// </summary>
    public enum UnityTaskPriorityEnum
    {
        None = 0, //不可用的
        LowLevel = 1, //非必要的资源 比如头像等是这个优先级
        Normal = 10,  //大多数任务处于这个优先级
        HighLevel = 50,
        Immediately = 100,//只要条件允许立刻开始这个任务
    }

    /// <summary>
    /// 下载任务的状态
    /// </summary>
    public enum DownloadStateEnum
    {
        None = 0, //无效的初始状态
        Initialed, //初始化
        Waiting,  //等待下载
        Running,
        Done, //完成
        Cancel,//取消
        Error, //错误
    }

    /// <summary>
    /// 下载任务的基类
    /// </summary>
    /// <typeparam name="W">真是的下载器类型(UnityWebRequest)</typeparam>
    public class BaseDownloadTask< W> : IDownloadTaskProcess
    {



        public bool IsDone { get; protected set; } = false;
        public bool IsError { get; protected set; } = false;
        public string TaskUrl { get; protected set; } = string.Empty;
        public float Progress { get; protected set; } = 0f;
        public bool IsCompleteInvoke { get; protected set; } = false;

        public UnityTaskPriorityEnum TaskPriorityEnum { get; protected set; } = UnityTaskPriorityEnum.Normal;  //下载任务的优先级
        public DownloadStateEnum DownloadTaskStateEnum { get; protected set; } = DownloadStateEnum.None; //当前的状态

        protected LinkedList<Action<W, bool, string>> TaskCompleteCallBackLinkList = new LinkedList<Action<W, bool, string>>(); //完成任务的事件链 bool 标示是否成功

        /// <summary>
        /// 回调回去的参数  刚开始又初始化的时候赋值传入，开始下载后更新这个对象
        /// </summary>
        protected W DownloadTaskCallbackData { get; set; } = default(W);



        public event Action<string, float> OnProgressChangedEvent; //下载进度的回调

        #region 构造函数和初始化

        public BaseDownloadTask() { }

        public BaseDownloadTask(string url, W taskData, UnityTaskPriorityEnum priority, Action<W, bool, string> completeCallback)
        {
            InitialedDownloadTask(url, taskData, priority, completeCallback);
        }

        public void InitialedDownloadTask(string url, W taskData, UnityTaskPriorityEnum priority, Action<W, bool, string> completeCallback)
        {
            TaskUrl = url;
            TaskPriorityEnum = priority;
            DownloadTaskCallbackData = taskData;
            TaskCompleteCallBackLinkList.AddLast(completeCallback);
            ChangeDownloadState(DownloadStateEnum.Initialed);
        }
        #endregion

        #region IDownloadTaskProcess 接口实现


        public virtual void OnCompleted(bool isDone, bool isError, float progress)
        {
            if (Progress != progress)
            {
                Progress = progress;
                if (OnProgressChangedEvent != null)
                    OnProgressChangedEvent.Invoke(TaskUrl, progress);
            }

            IsDone = isDone;
            IsError = isError;

            if (IsDone || IsError)
            {
                if (IsError)
                    ChangeDownloadState(DownloadStateEnum.Error);
                else
                    ChangeDownloadState(DownloadStateEnum.Done);

                var targetCallback = TaskCompleteCallBackLinkList.First;
                while (targetCallback != null)
                {
                    if (targetCallback.Value != null)
                        targetCallback.Value.Invoke(DownloadTaskCallbackData, !IsError && IsDone, TaskUrl);

                    targetCallback = targetCallback.Next;
                }//调用事件链
                TaskCompleteCallBackLinkList.Clear();
                IsCompleteInvoke = true;
            }
        }

        public virtual void StartDownloadTask() { }
      
        public virtual void CancelDownloadTask()
        {
            ChangeDownloadState(DownloadStateEnum.Cancel);
            ClearDownloadTask();
        }

        /// <summary>
        /// 任务的Tick 只处理自身的状态变化 和回调处理，不能删除自身或者调用StartDownloadTask
        /// </summary>
        public virtual void Tick() { }

        public virtual void ClearDownloadTask() 
        {
            ChangeDownloadState(DownloadStateEnum.None);
            TaskCompleteCallBackLinkList.Clear();
            OnProgressChangedEvent = null;
        }
        #endregion


        public virtual void ChangeDownloadState(DownloadStateEnum downloadState)
        {
            if (DownloadTaskStateEnum == downloadState)
                return;
            DownloadTaskStateEnum = downloadState;
        }


        #region  增加或者移除回调
        public void AddCompleteCallback(Action<W, bool, string> completeCallback)
        {
            AddCompleteCallback(completeCallback, UnityTaskPriorityEnum.None);
        }

        public void AddCompleteCallback(Action<W, bool, string> completeCallback, UnityTaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.AddLast(completeCallback);
            if (priority != UnityTaskPriorityEnum.None)
                TaskPriorityEnum = priority;
        }

        public void AddCompleteCallback(BaseDownloadTask<W> task)
        {
            if (task == null) return;
            if (task.TaskPriorityEnum != UnityTaskPriorityEnum.None)
                TaskPriorityEnum = task.TaskPriorityEnum;

            if (task.TaskCompleteCallBackLinkList.Count != 0)
            {
                var target = task.TaskCompleteCallBackLinkList.First;
                while (target != null)
                {
                    if (target.Value != null)
                        TaskCompleteCallBackLinkList.AddLast(target.Value);
                    target = target.Next;
                }
            }
        }


        public void RemoveCompleteCallback(Action<W, bool, string> completeCallback)
        {
            RemoveCompleteCallback(completeCallback, UnityTaskPriorityEnum.None);
        }
        //移除回调 并修改优先级
        public void RemoveCompleteCallback(Action<W, bool, string> completeCallback, UnityTaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.Remove(completeCallback);
            if (priority != UnityTaskPriorityEnum.None)
                TaskPriorityEnum = priority;

            if (TaskCompleteCallBackLinkList.Count == 0)
            {
                OnCompleted(true, false, 1f);
                CancelDownloadTask(); //取消下载任务
            }
        }
        #endregion




    }
}