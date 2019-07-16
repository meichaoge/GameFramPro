using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.NetWorkEx
{
    public interface IDownloadTaskProcess: ITaskProcess
    {
        /// <summary>
        /// 外部生成唯一标示这个任务的
        /// </summary>
        string TaskUrl { get; }
        /// <summary>
        ///  表示是否已经完成了任务的回调处理 避免异步处理时候的问题
        /// </summary>
        bool IsCompleteInvoke { get;}
        /// <summary>
        /// 下载优先级
        /// </summary>
        UnityTaskPriorityEnum TaskPriorityEnum { get; }

        /// <summary>
        /// 开始下载任务
        /// </summary>
        void StartDownloadTask();

        /// <summary>
        /// 强制取消下载任务.除非明确知道是取消这个任务，否则不建议使用.
        /// 可以使用 BaseDownloadTask 中 RemoveCompleteCallback 来移除某一个下载回调
        /// </summary>
        void CancelDownloadTask();

        void ClearDownloadTask(); //清理资源

    }
}