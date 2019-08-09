using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 下载任务的优先级/// </summary>
    public enum TaskPriorityEnum
    {
        None = 0, //不可用的
        LowLevel = 1, //非必要的资源 比如头像等是这个优先级
        Normal = 10, //大多数任务处于这个优先级
        HighLevel = 50,
        Immediately = 100, //只要条件允许立刻开始这个任务
    }


    /// <summary>/// 下载任务的基类/// </summary>
    /// <typeparam name="W">真实的下载器类型(比如 UnityWebRequest)</typeparam>
    public abstract class BaseDownloadTask<W> : ITaskProcess
    {
        /// <summary>/// 任务的状态/// </summary>
        public TaskStateEum TaskState { get; protected set; } = TaskStateEum.None;

        public string TaskUrl { get; protected set; } = string.Empty;
        public float Progress { get; protected set; } = 0f;
        public CoroutineEx TaskCoroutineExInfor { get; protected set; } = null;
        public TaskPriorityEnum TaskPriorityEnum { get; protected set; } = TaskPriorityEnum.Normal; //下载任务的优先级

        protected readonly LinkedList<Action<W, bool, string>> TaskCompleteCallBackLinkList = new LinkedList<Action<W, bool, string>>(); //完成任务的事件链 bool 标示是否成功

        /// <summary>/// 回调回去的参数  刚开始又初始化的时候赋值传入，开始下载后更新这个对象/// </summary>
        public W DownloadTaskCallbackData { get; protected set; } = default(W);

        /// <summary>/// 下载任务变化事件 参数 Url 和 进度/// </summary>
        public event OnTaskProcessChangeHandler OnProgressChangedEvent; //下载进度的回调

        #region 构造函数和初始化

        public void InitialedDownloadTask(string url, W taskData, TaskPriorityEnum priority, Action<W, bool, string> completeCallback)
        {
            TaskUrl = url;
            TaskPriorityEnum = priority;
            DownloadTaskCallbackData = taskData;
            TaskCompleteCallBackLinkList.AddLast(completeCallback);
            ChangeDownloadState(TaskStateEum.Initialed);
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


            if (isDone || isError)
            {
                if (isError)
                    ChangeDownloadState(TaskStateEum.Error);
                else
                    ChangeDownloadState(TaskStateEum.Complete);

                var targetCallback = TaskCompleteCallBackLinkList.First;
                while (targetCallback != null)
                {
                    if (targetCallback.Value != null)
                        targetCallback.Value.Invoke(DownloadTaskCallbackData, !isError && isDone, TaskUrl);

                    targetCallback = targetCallback.Next;
                } //调用事件链

                TaskCompleteCallBackLinkList.Clear();
            }
        }

        /// <summary>/// 需要在实现类中启动协程检测任务 /// </summary>
        public abstract void StartDownloadTask();

        public virtual void CancelDownloadTask()
        {
            ChangeDownloadState(TaskStateEum.Cancel);
            ClearDownloadTask();
        }

        public virtual void ClearDownloadTask()
        {
            ChangeDownloadState(TaskStateEum.None);
            TaskCompleteCallBackLinkList.Clear();
            OnProgressChangedEvent = null;
        }

        #endregion


        public virtual void ChangeDownloadState(TaskStateEum taskState)
        {
            if (TaskState == taskState)
                return;
            TaskState = taskState;
        }

        #region 任务的状态

        /// <summary>/// 完成下载任务的回调/// </summary>
        protected abstract void OnCompleteDownloadTask();


        /// <summary>/// 下载的进度变化/// </summary>
        protected virtual void OnProcessChange(float process)
        {
            if (OnProgressChangedEvent != null)
                OnProgressChangedEvent(TaskUrl, process);
        }

        #endregion

        #region  增加或者移除回调

        public void AddCompleteCallback(Action<W, bool, string> completeCallback)
        {
            AddCompleteCallback(completeCallback, TaskPriorityEnum.None);
        }

        public void AddCompleteCallback(Action<W, bool, string> completeCallback, TaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.AddLast(completeCallback);
            if (priority != TaskPriorityEnum.None)
                TaskPriorityEnum = priority;
        }

        public void AddCompleteCallback(BaseDownloadTask<W> task)
        {
            if (task == null) return;
            if (task.TaskPriorityEnum != TaskPriorityEnum.None)
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
            RemoveCompleteCallback(completeCallback, TaskPriorityEnum.None);
        }

        //移除回调 并修改优先级
        public void RemoveCompleteCallback(Action<W, bool, string> completeCallback, TaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.Remove(completeCallback);
            if (priority != TaskPriorityEnum.None)
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
