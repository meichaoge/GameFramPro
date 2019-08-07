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
        protected UnityWebRequestAsyncOperation mUnityWebRequestAsyncOperation { get; set; } = null; //发送请求后的状态


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
                if (mUnityWebRequestAsyncOperation != null)
                    Debug.LogError("StartDownloadTask Fail, the UnityWebRequest Already Start!!! " + TaskUrl);

                mUnityWebRequestAsyncOperation = DownloadTaskCallbackData.SendWebRequest(); //启动任务
                DownloadTaskCallbackData = mUnityWebRequestAsyncOperation.webRequest;
            }
            else
            {
                ChangeDownloadState(TaskStateEum.Error);
            }
        }

        public override void Tick()
        {
            if (TaskState != TaskStateEum.Running) return;
            if (mUnityWebRequestAsyncOperation == null)
            {
                OnCompleted(true, true, 1f);
                return;
            }

            OnCompleted(DownloadTaskCallbackData.isDone, DownloadTaskCallbackData.isHttpError || DownloadTaskCallbackData.isNetworkError, DownloadTaskCallbackData.downloadProgress);
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
    }
}
