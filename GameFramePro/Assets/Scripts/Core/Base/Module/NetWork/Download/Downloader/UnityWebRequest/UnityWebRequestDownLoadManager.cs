using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 使用 UnityWebRequest 的所有下载器
    /// </summary> 
    public class UnityWebRequestDownLoadManager : BaseDownloadManager<UnityWebRequestDownloadTask, UnityWebRequest>
    {
        protected static NativeObjectPool<UnityWebRequestDownloadTask> mUnityWebRequestTaskPoolManager = null;
        protected static bool s_IsInitialed = false;






        static UnityWebRequestDownLoadManager()
        {
            if (s_IsInitialed == false)
            {
                s_IsInitialed = true;
                mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask>(20, null, OnBeforRecycleWebRequestTaskItem);
                return;
            }
            Debug.LogError("UnityWebRequestDownLoadManager Already Intial");
        }

        #region 对象池接口
        protected static void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask task)
        {
            if (task != null)
            {
                task.ClearDownloadTask();
            }
        }
        #endregion

        protected override UnityWebRequestDownloadTask GetDownloadTaskInstance(string taskUrl, Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum, params object[] otherParameter)
        {
            return null;
        }
    }
}