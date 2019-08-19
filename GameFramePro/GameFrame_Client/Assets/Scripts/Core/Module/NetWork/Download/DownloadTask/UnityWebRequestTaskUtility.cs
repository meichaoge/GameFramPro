using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 负责缓存 UnityWebRequestDownloadTask 对象/// </summary>
    public class UnityWebRequestTaskUtility : Single<UnityWebRequestTaskUtility>
    {
        protected static NativeObjectPool<UnityWebRequestDownloadTask> mUnityWebRequestTaskPoolManager = null;
        protected static bool s_IsInitialed = false;


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            if (s_IsInitialed == false)
            {
                s_IsInitialed = true;
                mUnityWebRequestTaskPoolManager = new NativeObjectPool<UnityWebRequestDownloadTask>(20, null, OnBeforRecycleWebRequestTaskItem);
                return;
            }

            Debug.LogError("UnityWebRequestTaskUtility Already Initial");
        }


        protected static void OnBeforRecycleWebRequestTaskItem(UnityWebRequestDownloadTask task)
        {
            task?.ClearDownloadTask();
        }


        /// <summary>/// 从缓存中获取对象/// </summary>
        public UnityWebRequestDownloadTask GetUnityWebRequestDownloadTaskFromPool()
        {
            UnityWebRequestDownloadTask task = mUnityWebRequestTaskPoolManager.GetItemFromPool();
            return task;
        }

        /// <summary>/// 回收对象/// </summary>
        public void RecycleUnityWebRequestDownloadTask(UnityWebRequestDownloadTask task)
        {
            if (task == null)
            {
                Debug.LogError("RecycleUnityWebRequestDownloadTask Fail, Parameter is null");
                return;
            }

            mUnityWebRequestTaskPoolManager.RecycleItemToPool(task);
        }
    }
}
