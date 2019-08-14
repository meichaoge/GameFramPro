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
        public static int S_Timeout { get; protected set; } = 15; //超时时间15秒+

        //  protected UnityWebRequestAsyncOperation mUnityWebRequestAsyncOperation { get; set; } = null; //发送请求后的状态


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
