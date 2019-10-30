using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 使用 UnityWebRequest 的 下载任务/// </summary>
    public class UnityWebRequestDownloadTask : BaseDownloadTask<UnityWebRequest>
    {

        static UnityWebRequestDownloadTask()
        {
            mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask>(20, null, OnBeforRecycleWebRequestTaskItem);
        }

        #region 对象池
        protected static NativeObjectPool<UnityWebRequestDownloadTask> mUnityWebRequestTaskPoolManager = null;

        protected static void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask task)
        {
            task?.ClearDownloadTask();
        }


        /// <summary>/// 从缓存中获取对象/// </summary>
        public static UnityWebRequestDownloadTask GetUnityWebRequestDownloadTask(string taskUrl,Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum, int timeOut=15)
        {
            UnityWebRequestDownloadTask task = mUnityWebRequestTaskPoolManager.GetItemFromPool();

            UnityWebRequest webRequest = new UnityWebRequest(taskUrl)
            {
                timeout = timeOut,
            };
            task.InitialedDownloadTask(taskUrl, webRequest, priorityEnum, callback);
            return task;
        }
     



        /// <summary>/// 回收对象/// </summary>
        public static void RecycleUnityWebRequestDownloadTask(UnityWebRequestDownloadTask task)
        {
            if (task == null)
            {
                Debug.LogError("RecycleUnityWebRequestDownloadTask Fail, Parameter is null");
                return;
            }

            mUnityWebRequestTaskPoolManager.RecycleItemToPool(task);
        }

        #endregion



        #region IDownloadTaskProcess 接口实现

        public override void StartDownloadTask()
        {
            if (TaskState != TaskStateEum.Initialed)
            {
                Debug.LogError($"下载任务状态异常 不是初始化完成的状态 {TaskState}  TaskUrl={TaskUrl}");
                return;
            }

            ChangeDownloadState(TaskStateEum.Running);

            if (DownloadTaskCallbackData != null)
            {
                var mUnityWebRequestAsyncOperation = DownloadTaskCallbackData.SendWebRequest(); //启动任务
                TaskSuperCoroutinenfor = AsyncManager.StartAsyncOperation(mUnityWebRequestAsyncOperation, OnCompleteDownloadTask, OnProcessChange);
            }
            else
            {
                ChangeDownloadState(TaskStateEum.Error);
                OnCompleted(false, true, 0);
            }
        }

        public override void CancelDownloadTask()
        {
            if (TaskState != TaskStateEum.Running)
            {
                Debug.LogError($"下载任务状态异常 不是运行 状态 {TaskState}  TaskUrl={TaskUrl}");
                return;
            }


            if (TaskSuperCoroutinenfor != null)
                TaskSuperCoroutinenfor.StopCoroutine();
            base.CancelDownloadTask();
        }

        public override void ClearDownloadTask()
        {
            if (DownloadTaskCallbackData != null)
            {
                DownloadTaskCallbackData.Abort();
                DownloadTaskCallbackData.Dispose();
                DownloadTaskCallbackData = null;
            }


            base.ClearDownloadTask();
        }

        #endregion

        #region 任务的状态

        /// <summary>/// 完成下载任务的回调/// </summary>
        protected override void OnCompleteDownloadTask()
        {
#if UNITY_EDITOR
            Debug.LogEditorInfor($"完成下载任务 url={DownloadTaskCallbackData.url}");
#endif

            OnCompleted(DownloadTaskCallbackData.isDone, DownloadTaskCallbackData.isNetworkError || DownloadTaskCallbackData.isNetworkError, DownloadTaskCallbackData.downloadProgress);
        }

        #endregion
    }
}
