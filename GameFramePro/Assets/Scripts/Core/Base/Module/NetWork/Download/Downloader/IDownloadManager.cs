using GameFramePro.NetWorkEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    public interface IDownloadManager<U> where U : IDownloadTaskProcess
    {
        /// <summary>
        /// 所有的正在下载中的 UnityWebRequest 任务
        /// </summary>
        Dictionary<string, U> AllDownloadingTasks { get; }
        /// <summary>
        /// 排队中的任务， 按照优先级排序
        /// </summary>
        LinkedList<U> AllWaitDownloadTasks { get; }

        int MaxDownloadTaskCount { get; }

        void Tick(); //定时获取状态

        /// <summary>
        /// 清理已经完成的任务
        /// </summary>
        void ClearCompletedTask();

    }
}