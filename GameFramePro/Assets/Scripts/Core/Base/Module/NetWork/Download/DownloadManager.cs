﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramePro.NetWorkEx;
using System.Reflection;
using UnityEngine.Networking;

namespace GameFramePro
{
    /// <summary>
    /// 对外提供下载接口 对内隐藏实现
    /// </summary>
    public class DownloadManager : Single<DownloadManager>, IUpdateTick
    {
        //private HashSet<IDownloadManager<object, ITaskProcess>> mAllDownloadeManagers = new HashSet<IDownloadManager<object, ITaskProcess>>(); //所有的下载器


        #region IUpdateTick 接口
        protected int curUpdateCount = 0; //当前的帧基数
        public int TickPerUpdateCount { get; protected set; } = 5;

        public void UpdateTick(float currentTime)
        {
            ++curUpdateCount;
            if (curUpdateCount < TickPerUpdateCount)
                return;
            curUpdateCount = 0;

            //     AssetBundleDownloadManager.S_Instance.Tick();
            ByteDataDownloadManager.S_Instance.UpdateTick();

            //if (mAllDownloadeManagers.Count > 0)
            //{
            //    foreach (var item in mAllDownloadeManagers)
            //        item.Tick();
            //}
        }
        #endregion

        //  protected override void InitialSingleton()
        //  {
        //      base.InitialSingleton();
        // //     AppManager.S_Instance.RegisterUpdateTick(this);

        //  }
        //  public override void DisposeInstance()
        //  {
        ////      AppManager.S_Instance.UnRegisterUpdateTick(this);
        //      base.DisposeInstance();
        //  }



        #region 通用的下载接口 对外隐藏实现 (这里不能是静态的 否则没法注册)
        public void GetAssetBundleFromUrl(string taskUrl, uint crc, System.Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
        {
            // AssetBundleDownloadManager.S_Instance.GetDataFromUrl(taskUrl,crc, callback, priorityEnum);
        }

        public void GetByteDataFromUrl(string taskUrl, System.Action<UnityWebRequest, bool, string> callback, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal)
        {
            ByteDataDownloadManager.S_Instance.GetDataFromUrl(taskUrl, callback, priorityEnum);
        }
        #endregion


        #region 辅助
        //public void RegisterDownloadManager(IDownloadManager<, > loader)
        //{
        //    if (mAllDownloadeManagers.Contains(loader))
        //    {
        //        Debug.LogError("RegisterDownloadManager Fail,重复的下载器 " + loader);
        //        return;
        //    }
        //    Debug.LogEditorInfor("RegisterDownloadManager Success " + loader.GetType());
        //    mAllDownloadeManagers.Add(loader);
        //}

        //public void UnRegisterDownloadManager(IDownloadManager<object, ITaskProcess> loader)
        //{
        //    if (mAllDownloadeManagers.Contains(loader))
        //    {
        //        mAllDownloadeManagers.Remove(loader);
        //        Debug.LogEditorInfor("UnRegisterDownloadManager Success " + loader.GetType());
        //        return;
        //    }

        //    Debug.LogError("UnRegisterDownloadManager Fail,不存在的下载器 " + loader);
        //    return;
        //}


        #endregion


    }
}
