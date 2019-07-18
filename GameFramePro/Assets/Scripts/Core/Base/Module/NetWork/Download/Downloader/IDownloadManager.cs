using GameFramePro.NetWorkEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    public interface IDownloadManager<U> where U : IDownloadTaskProcess
    {

        /// <summary>
        ///所有的下载任务，按照优先级排序，包含已经开始的和等待的任务
        /// </summary>
        LinkedList<U> AllDownloadTaskLinkedList { get; }
        /// <summary>
        /// 所有的任务请求会先进入这里然后在 Tick 时候加入 AllDownloadTaskLinkedList
        /// </summary>
        LinkedList<U> AllCacheDownloadTaskLinkedList { get; }


        int MaxDownloadTaskCount { get; }
        /// <summary>
        /// 下载中的任务总数
        /// </summary>
        int CurDownloadingTaskCount { get; }

        /// <summary>
        /// 定义每这个数量的Mono  Update 更新一次
        /// </summary>
        int TickPerUpdateCount { get; }

        void UpdateTick(); //定时获取状态


    }
}