using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{


    /// <summary>
    /// 使用 UnityWebRequest 的 下载任务
    /// </summary>
    public class UnityWebRequestDownloadTask <T>: ITaskProcess
    {
        public bool IsStartTask { get; private set; } = false;

        public string TaskUrl { get; private set; } = string.Empty;

        public bool IsDone { get; private set; } = false;
        public bool IsError { get; private set; } = false;
        /// <summary>
        /// 表示是否已经完成了任务的回调处理 避免异步处理时候的问题
        /// </summary>
        public bool IsCompleteInvoke { get; private set; } = false;

        public float Progress { get; private set; } = 0f;

        /// <summary>
        /// 参数传递过来的UnityWebRequest 对象 只在调用 UnityWebRequest.Send()或者 UnityWebRequest.SendWebRequest() 前有效,之后会被重新赋值
        /// </summary>
        protected UnityWebRequest UnityWebRequestTaskData { get; private set; } = null;  
        protected UnityWebRequestAsyncOperation mUnityWebRequestAsyncOperation { get; private set; } = null; //发送请求后的状态


        public UnityTaskPriorityEnum mTaskPriorityEnum = UnityTaskPriorityEnum.Normal;
     

        protected LinkedList<Action<T, string>> TaskCompleteCallBackLinkList = new LinkedList<Action<T, string>>(); //完成任务的事件链

        public event Action<string, float> OnProgressChangedEvent;

        protected Func<UnityWebRequest, T> GetCallbackDataHandler { get; private set; } //将UnityWebRequest 转换成需要的数据




        public UnityWebRequestDownloadTask()
        {
     
        }

        public UnityWebRequestDownloadTask(UnityWebRequest taskData, UnityTaskPriorityEnum priority, Action<T, string> completeCallback, Func<UnityWebRequest, T> getDataHandler)
        {
            InitialedUnityWebRequestDownloadTask(taskData, priority, completeCallback, getDataHandler);
        }

        public void InitialedUnityWebRequestDownloadTask(UnityWebRequest taskData, UnityTaskPriorityEnum priority, Action<T, string> completeCallback, Func<UnityWebRequest, T> getDataHandler)
        {
            TaskUrl = taskData.url;
            mTaskPriorityEnum = priority;
            UnityWebRequestTaskData = taskData;
            GetCallbackDataHandler = getDataHandler;
            TaskCompleteCallBackLinkList.AddLast(completeCallback);
        }


        public void AddCompleteCallback(Action<T, string> completeCallback)
        {
            AddCompleteCallback(completeCallback, UnityTaskPriorityEnum.None);
        }

        public void AddCompleteCallback(Action<T, string> completeCallback, UnityTaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.AddLast(completeCallback);
            if (priority != UnityTaskPriorityEnum.None)
                mTaskPriorityEnum = priority;
        }

        public void AddCompleteCallback(UnityWebRequestDownloadTask<T> task)
        {
            if (task == null) return;
            if (task.mTaskPriorityEnum!=UnityTaskPriorityEnum.None)
                mTaskPriorityEnum = task.mTaskPriorityEnum;

            if (task.TaskCompleteCallBackLinkList.Count != 0)
            {
                var target = task.TaskCompleteCallBackLinkList.First;
                while (target!=null)
                {
                    if (target.Value != null)
                        TaskCompleteCallBackLinkList.AddLast(target.Value);
                    target = target.Next;
                }
            }
        }


        public void RemoveCompleteCallback(Action<T, string> completeCallback)
        {
            RemoveCompleteCallback(completeCallback, UnityTaskPriorityEnum.None);
        }
        //移除回调 并修改优先级
        public void RemoveCompleteCallback(Action<T, string> completeCallback, UnityTaskPriorityEnum priority)
        {
            if (completeCallback != null)
                TaskCompleteCallBackLinkList.Remove(completeCallback);
            if (priority != UnityTaskPriorityEnum.None)
                mTaskPriorityEnum = priority;

            if (TaskCompleteCallBackLinkList.Count == 0)
            {
                Dispose(); //清理任务
                OnCompleted(true, false, 1f);
            }
        }


        public void OnCompleted(bool isDone, bool isError, float progress)
        {
            if (Progress != progress)
            {
                Progress = progress;
                if (OnProgressChangedEvent != null)
                    OnProgressChangedEvent.Invoke(TaskUrl, progress);
            }

            IsDone = isDone;
            IsError = isError;

            if (IsDone)
            {
                T callbackData = GetCallbackDataHandler(UnityWebRequestTaskData);
                var targetCallback = TaskCompleteCallBackLinkList.First;
                while (targetCallback != null)
                {
                    if (targetCallback.Value != null)
                        targetCallback.Value.Invoke(callbackData, TaskUrl);

                    targetCallback = targetCallback.Next;
                }//调用事件链
                TaskCompleteCallBackLinkList.Clear();
                IsCompleteInvoke = true;
            }
        }

        public void StartTask()
        {
            if (IsStartTask)
            {
                Debug.LogError(string.Format("StartTask Fail,this Task is Already Start!! url={0}", TaskUrl));
                return;
            }
            IsStartTask = true;
            if (UnityWebRequestTaskData != null)
            {
                if (mUnityWebRequestAsyncOperation != null)
                    Debug.LogError("StartTask Fail, the UnityWebRequest Already Start!!! "+TaskUrl);

                mUnityWebRequestAsyncOperation= UnityWebRequestTaskData.SendWebRequest();  //启动任务
                UnityWebRequestTaskData = mUnityWebRequestAsyncOperation.webRequest;
            }
        }

        public void Tick()
        {
            if (IsStartTask == false) return;
            if (mUnityWebRequestAsyncOperation == null)
            {
                OnCompleted(true, true, 1f);
                return;
            }
            //var webRequest = mUnityWebRequestAsyncOperation.webRequest;

            OnCompleted(UnityWebRequestTaskData.isDone, UnityWebRequestTaskData.isHttpError || UnityWebRequestTaskData.isNetworkError, UnityWebRequestTaskData.downloadProgress);
        }

        public void Dispose()
        {
            //if (IsStartTask )
            //{
            //   if(mUnityWebRequestAsyncOperation!=null&& mUnityWebRequestAsyncOperation.webRequest != null)
            //    {
            //        mUnityWebRequestAsyncOperation.webRequest.Abort();
            //        mUnityWebRequestAsyncOperation.webRequest.Dispose();
            //        mUnityWebRequestAsyncOperation = null;
            //    }
            //}
          

            if (UnityWebRequestTaskData != null)
            {
                UnityWebRequestTaskData.Abort();
                UnityWebRequestTaskData.Dispose();
                UnityWebRequestTaskData = null;
            }

            TaskCompleteCallBackLinkList.Clear();
        }



    }
}