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
        /// //进度
        /// </summary>
        float Progress { get; }

        /// <summary>
        /// 进度改变的时候的事件消息
        /// </summary>
       event System.Action<string, float> OnProgressChangedEvent;


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