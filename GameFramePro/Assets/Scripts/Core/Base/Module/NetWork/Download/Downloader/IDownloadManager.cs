using GameFramePro.NetWorkEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    public interface IDownloadManager<U> where U : IDownloadTaskProcess
    {
        //所有的正在下载中的 UnityWebRequest 任务
        Dictionary<string, U> AllDownloadingTasks { get; }
        //按照优先级排序
        LinkedList<U> AllWaitDownloadTasks { get; }

        int MaxDownloadTaskCount { get; }


        void InitialedManager(); //初始化管理器

        void Tick(); //定时获取状态

    }
}