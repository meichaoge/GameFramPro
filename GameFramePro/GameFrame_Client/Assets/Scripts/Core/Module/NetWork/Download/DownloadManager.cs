using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramePro.NetWorkEx;
using System.Reflection;
using UnityEngine.Networking;

namespace GameFramePro
{
    /// <summary>/// 对外提供下载接口 对内隐藏实现/// </summary>
    public class DownloadManager : Single<DownloadManager>, IUpdateCountTick
    {
        private HashSet<IUpdateTick> mAllDownloadeManagers = new HashSet<IUpdateTick>(); //所有的下载器


        #region IUpdateCountTick 接口

        protected int curUpdateCount = 0; //当前的帧基数
        public uint TickPerUpdateCount { get; protected set; } = 5;

        protected bool CheckIfNeedUpdateTick()
        {
            ++curUpdateCount;
            if (curUpdateCount == 1)
                return true; //确保第一次被调用

            if (curUpdateCount < TickPerUpdateCount)
                return false;

            curUpdateCount = 0;
            return true;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick() == false) return;

            if (mAllDownloadeManagers.Count > 0)
            {
                foreach (var item in mAllDownloadeManagers)
                    item.UpdateTick(currentTime);
            }
        }

        #endregion

//          protected override void InitialSingleton()
//          {
//              base.InitialSingleton();
//         //     AppEntryManager.S_Instance.RegisterUpdateTick(this);
//
//          }
//          public override void DisposeInstance()
//          {
//        //      AppEntryManager.S_Instance.UnRegisterUpdateTick(this);
//              base.DisposeInstance();
//          }


        #region 通用的下载接口 对外隐藏实现 (这里不能是静态的 否则没法注册)

        public void GetAssetBundleFromUrl(string taskUrl, uint crc, System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 60)
        {
            // AssetBundleDownloadManager.S_Instance.GetDataFromUrl(taskUrl,crc, callback, priorityEnum,timeOut);
        }

        public UnityWebRequestDownloadTask GetByteDataFromUrl(string taskUrl, long rangeFrom, long rangEnd,  System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal,int timeOut=60)
        {
//#if UNITY_EDITOR
//            Debug.LogEditorInfor($"taskUrl={taskUrl}");
//#endif

            return ByteDataDownloadManager.S_Instance.GetDataFromUrl(taskUrl, rangeFrom, rangEnd, callback, priorityEnum, timeOut);
        }


        /// <summary>/// 下载图片/// </summary>
        public UnityWebRequestDownloadTask GetTextureDataFromUrl(string taskUrl, System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 60)
        {
            return TextureDownloadManager.S_Instance.GetDataFromUrl(taskUrl, callback, priorityEnum, timeOut);
        }


        public static SuperCoroutine  GetContentLength(string taskUrl, System.Action<bool, long> completeCallback)
        {
            return AsyncManager.StartCoroutineEx(UnityWebRequestGetHead(taskUrl, completeCallback));
        }

        private static IEnumerator UnityWebRequestGetHead(string taskUrl, System.Action<bool, long> completeCallback)
        {
            UnityWebRequest headRequest = UnityWebRequest.Head(taskUrl);
            yield return headRequest.SendWebRequest();
            if (headRequest == null || headRequest.isHttpError || headRequest.isNetworkError || headRequest.isDone == false)
            {
                Debug.LogError($"下载失败{headRequest?.error}  {headRequest?.url}");
                completeCallback?.Invoke(false, 0);
                yield break;
            }
            var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));


            Debug.LogInfor($" 获取指定Url 头部长度成功{totalLength} ");
            completeCallback?.Invoke(true, totalLength);

        }

        #endregion


        #region 辅助

        /// <summary>/// 注册 需要更新的下载管理器计时滴答/// </summary>
        public void RegisterDownloadManager(IUpdateTick loader)
        {
            if (mAllDownloadeManagers.Contains(loader))
            {
                Debug.LogError("RegisterDownloadManager Fail,重复的下载器 " + loader);
                return;
            }

            Debug.LogEditorInfor($"RegisterDownloadManager Success !! {loader.GetType()} ");
            mAllDownloadeManagers.Add(loader);
        }

        /// <summary>/// 取消注册 需要更新的下载管理器计时滴答/// </summary>
        public void UnRegisterDownloadManager(IUpdateTick loader)
        {
            if (mAllDownloadeManagers.Contains(loader))
            {
                mAllDownloadeManagers.Remove(loader);
                Debug.LogEditorInfor($"UnRegisterDownloadManager Success !! {loader.GetType()} ");
                return;
            }

            Debug.LogError("UnRegisterDownloadManager Fail,不存在的下载器 " + loader);
        }

        #endregion
    }
}
