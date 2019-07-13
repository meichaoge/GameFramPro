using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 所有耗时任务需要继承这个接口 提供当前任务的进度。类似于AsyncOperation 接口
    /// </summary>
    public interface ITaskProcess:IDisposable
    {
        bool IsStartTask { get; }

        /// <summary>
        /// 外部生成唯一标示这个任务的
        /// </summary>
        string TaskUrl { get; }

        /// <summary>
        /// //是否完成
        /// </summary>
        bool IsDone { get; }
        /// <summary>
        /// 是否有错误
        /// </summary>
        bool IsError { get; }
        /// <summary>
        /// 表示是否已经完成了任务的回调处理 避免异步处理时候的问题
        /// </summary>
        bool IsCompleteInvoke { get; }

        /// <summary>
        /// //进度
        /// </summary>
        float Progress { get; }



        /// <summary>
        /// 外部调用获取当前进度的
        /// </summary>
        void Tick();// 

        /// <summary>
        /// 完成任务时候
        /// </summary>
        void OnCompleted(bool isDone, bool isError, float progress);

        /// <summary>
        /// 任务开始  
        /// </summary>
        void StartTask();

      

    }
}