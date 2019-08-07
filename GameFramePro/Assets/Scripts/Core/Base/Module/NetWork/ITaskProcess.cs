using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>/// 任务的几种状态/// </summary>
    public enum TaskStateEum
    {
        None, //无效的初始状态
        Initialed, //初始化 以及初始化后的等待中
        Running, //运行中
        Error, //错误终止
        Cancel, //取消中断
        Complete, //完成了任务
    }


    /// <summary>
    /// 所有耗时任务需要继承这个接口 提供当前任务的进度。类似于AsyncOperation 接口
    /// </summary>
    public interface ITaskProcess
    {
        /// <summary>/// 任务的状态/// </summary>
        TaskStateEum TaskState { get; }

        /// <summary>/// 进度(0--1f)/// </summary>
        float Progress { get; }

        /// <summary>/// 进度改变的时候的事件消息/// </summary>
        event System.Action<string, float> OnProgressChangedEvent;

        /// <summary>/// 外部调用获取当前进度的/// </summary>
        void Tick(); // 

        /// <summary>/// 完成任务时候/// </summary>
        void OnCompleted(bool isDone, bool isError, float progress);
    }
}
