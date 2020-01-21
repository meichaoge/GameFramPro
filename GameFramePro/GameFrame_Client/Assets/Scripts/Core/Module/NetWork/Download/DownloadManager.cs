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
    public static class DownloadManager
    {
        private static HashSet<IUpdateTick> mAllDownloadeManagers { get; set; } = new HashSet<IUpdateTick>(); //所有的下载器
        /// <summary>
        /// 标识是否正在计时中
        /// </summary>
        public static bool IsTicking { get; private set; } = false;


        private static void UpdateAllDownloadTick()
        {
            if (mAllDownloadeManagers.Count > 0)
            {
                foreach (var item in mAllDownloadeManagers)
                    item.UpdateTick(Time.time);
            }
        }


        #region 通用的下载接口 对外隐藏实现 (这里不能是静态的 否则没法注册)

        public static void GetAssetBundleFromUrl(string taskUrl, uint crc, System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 60)
        {
            // AssetBundleDownloadManager.S_Instance.GetDataFromUrl(taskUrl,crc, callback, priorityEnum,timeOut);
        }

        public static UnityWebRequestDownloadTask GetByteDataFromUrl(string taskUrl, long rangeFrom, long rangEnd,  System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal,int timeOut=60)
        {
//#if UNITY_EDITOR
//            Debug.LogEditorInfor($"taskUrl={taskUrl}");
//#endif

            return ByteDataDownloadManager.S_Instance.GetDataFromUrl(taskUrl, rangeFrom, rangEnd, callback, priorityEnum, timeOut);
        }


        /// <summary>/// 下载图片/// </summary>
        public static UnityWebRequestDownloadTask GetTextureDataFromUrl(string taskUrl, System.Action<UnityWebRequest, bool, string> callback, TaskPriorityEnum priorityEnum = TaskPriorityEnum.Normal, int timeOut = 60)
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



        /// <summary>/// 注册 需要更新的下载管理器计时滴答/// </summary>
        public static void RegisterDownloadManager(IUpdateTick loader)
        {
            if (mAllDownloadeManagers.Contains(loader))
            {
                Debug.LogError("RegisterDownloadManager Fail,重复的下载器 " + loader);
                return;
            }

            Debug.LogEditorInfor($"RegisterDownloadManager Success !! {loader.GetType()} ");
            mAllDownloadeManagers.Add(loader);
            ControllDownloadManagerUpdateTick();
        }

        /// <summary>/// 取消注册 需要更新的下载管理器计时滴答/// </summary>
        public static void UnRegisterDownloadManager(IUpdateTick loader)
        {
            if (mAllDownloadeManagers.Contains(loader))
            {
                mAllDownloadeManagers.Remove(loader);
                ControllDownloadManagerUpdateTick();
                Debug.LogEditorInfor($"UnRegisterDownloadManager Success !! {loader.GetType()} ");
                return;
            }

            Debug.LogError("UnRegisterDownloadManager Fail,不存在的下载器 " + loader);
        }

        private static void ControllDownloadManagerUpdateTick()
        {
            bool isNeedTick = mAllDownloadeManagers.Count > 0;
            if (isNeedTick == IsTicking)
                return;

            if (isNeedTick)
            {
                IsTicking = true;
                AppModuleTickManager.AddUpdateCallback(UpdateAllDownloadTick);
            }
            else
            {
                IsTicking = false;
                AppModuleTickManager.RemoveUpdateCallback(UpdateAllDownloadTick);
            }

        }

    }
}
