using GameFramePro.NetWorkEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载任务的优先级
    /// </summary>
    public enum UnityTaskPriorityEnum
    {
        None = 0, //不可用的
        LowLevel = 1, //非必要的资源 比如头像等是这个优先级
        Normal = 10,  //大多数任务处于这个优先级
        HighLevel = 50,
        Immediately = 100,//只要条件允许立刻开始这个任务
    }

    public interface IDownloadManager <T,U> where U: ITaskProcess
    {
        //所有的正在下载中的 UnityWebRequest 任务
        Dictionary<string, U> AllDownloadingTasks { get; }
        //按照优先级排序
        LinkedList<U> AllWaitDownloadTasks { get; }

        int MaxDownloadTaskCount { get; }


        void InitialedManager(); //初始化管理器


        void ClearCompletedTask(); //清理已经完成的任务
        void Tick(); //定时获取状态

    }
}