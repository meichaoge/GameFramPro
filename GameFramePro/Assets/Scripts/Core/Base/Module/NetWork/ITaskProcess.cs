using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>/// 任务的几种状态 注意Error/Cancel/Complete 必须在后面/// </summary>
    public enum TaskStateEum
    {
        None, //无效的初始状态
        Initialed = 1, //初始化 以及初始化后的等待中
        Running, //运行中
        Error = 10, //错误终止
        Cancel, //取消中断
        Complete, //完成了任务
    }


    /// <summary> /// 任务下载进度变化/// </summary>
    /// <param name="url">资源 url </param>/// <param name="process">下载进度</param>
    public delegate void OnTaskProcessChangeHandler(string url, float process);


    /// <summary>/// 所有耗时任务需要继承这个接口 提供当前任务的进度。类似于AsyncOperation 接口/// </summary>
    public interface ITaskProcess
    {
        /// <summary>/// 任务的状态/// </summary>
        TaskStateEum TaskState { get; }

        /// <summary>/// 进度(0--1f)/// </summary>
        float Progress { get; }

        /// <summary>/// 进度改变的时候的事件消息/// </summary>
        event OnTaskProcessChangeHandler OnProgressChangedEvent;

        /// <summary>/// 启动任务后的协程 对象/// </summary>
        CoroutineEx TaskInfor { get; }

        /// <summary>/// 完成任务时候/// </summary>
        void OnCompleted(bool isDone, bool isError, float progress);
    }
}