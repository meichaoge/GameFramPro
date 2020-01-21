using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using GameFramePro.NetWorkEx;
using GameFramePro.CacheEx;

namespace GameFramePro
{
    /// <summary>
    /// 实时下载图片显示
    /// </summary>
    public static class RealtimeImageDownloadHelper
    {
        private class DownloadTextureFailRecord
        {
            public string mTetureUri { get; set; }
            public int mDownloadFailTimes { get; set; } //下载失败次数
            public int mLastDownloadFailTimeSeconds { get; set; } //上一次下载失败的时间秒
        }




        //所有下载资源回调
        private static Dictionary<string, List<System.Action<bool, string, bool, object>>> mAllRegisterCallbackInfors = new Dictionary<string, List<Action<bool, string, bool, object>>>();
        //下载失败的记录 避免一直尝试下载
        private static Dictionary<string, DownloadTextureFailRecord> s_AllDownloadFailRecords = new Dictionary<string, DownloadTextureFailRecord>();
        private static NativeObjectPool<DownloadTextureFailRecord> s_DownloadTextureFailRecordPool = new NativeObjectPool<DownloadTextureFailRecord>(20, null, BeforeRecycleDownloadFailRecord);

        /// <summary>
        /// 下载或者取缓存的图片
        /// </summary>
        /// <param name="textureUri">图片相对路径 也用于保存资源的相对路径</param>
        /// <param name="topCDNUri">顶层的CDN 路径</param>
        /// <param name="downLoadCallback"></param>
        /// <param name="textureFormat"></param>
        /// <param name="isSprite"></param>
        /// <param name="isForceDownload">是否强制下载</param>
        /// <param name="isAutoCache"></param>
        public static void DownloadTexture(string textureUri, string topCDNUri, System.Action<bool, string, bool, object> downLoadCallback, TextureFormat textureFormat = TextureFormat.RGBA32, bool isSprite = true, bool isForceDownload = false, bool isAutoCache = true)
        {
            RegisterImageDownloadCallback(textureUri, downLoadCallback);
            byte[] localOrCacheData = AppTextureCacheManager.S_Instance.GetDataFromCache(textureUri);

            if (localOrCacheData == null || localOrCacheData.Length == 0 || isForceDownload)
            {
                bool isNeedDownload = true;
                if(s_AllDownloadFailRecords.TryGetValue(textureUri,out var downloadTextureFailRecord)&& downloadTextureFailRecord!=null)
                {
                    if (downloadTextureFailRecord.mDownloadFailTimes <= 2)
                    {
                        if ((System.DateTime.Now.Second - downloadTextureFailRecord.mLastDownloadFailTimeSeconds) < 5)
                            isNeedDownload = false;
                    }
                    else if (downloadTextureFailRecord.mDownloadFailTimes <= 4)
                    {
                        if ((System.DateTime.Now.Second - downloadTextureFailRecord.mLastDownloadFailTimeSeconds) < 30)
                            isNeedDownload = false;
                    }
                    else if (downloadTextureFailRecord.mDownloadFailTimes >= 5)
                    {
                        isNeedDownload = false;
                    }
                }

                if (isNeedDownload == false)
                {
                    Debug.Log($"判断上一次下载失败 且还在失效时间内 {textureUri}");
                    TriggerDownloadImageCallback(textureUri, null, isSprite);
                    return;
                }

                string realUri = topCDNUri.ConcatPathEx(textureUri);
#if UNITY_EDITOR
                Debug.Log($"下载图片资源{textureUri}==>{topCDNUri}");
#endif
                DownloadManager.GetTextureDataFromUrl(realUri, (webRequest, isSuccess, url) =>
                {
                    OnDownloadTextureCallback(textureUri, isSuccess, webRequest);
                }, TaskPriorityEnum.Immediately);
                return;
            }//需要下载
            else
            {
                OnGetCacheTextureCallback(textureUri, localOrCacheData, textureFormat, isSprite);
            }
        }
        //注册回调
        public static void RegisterImageDownloadCallback(string textureUri, System.Action<bool, string, bool, object> downLoadCallback)
        {
            if (downLoadCallback == null) return;

            if (mAllRegisterCallbackInfors.TryGetValue(textureUri, out var callbackInfors))
            {
                if (callbackInfors.Contains(downLoadCallback) == false)
                    callbackInfors.Add(downLoadCallback);
            }
            else
            {
                List<System.Action<bool, string, bool, object>> callbackLists = new List<Action<bool, string, bool, object>>();
                callbackLists.Add(downLoadCallback);
                mAllRegisterCallbackInfors[textureUri] = callbackLists;
            }
        }

        public static void UnRegisterImageDownloadCallback(string textureUri, System.Action<bool, string, bool, object> downLoadCallback)
        {
            if (downLoadCallback == null) return;

            if (mAllRegisterCallbackInfors.TryGetValue(textureUri, out var callbackInfors))
            {
                if (callbackInfors.Contains(downLoadCallback))
                    callbackInfors.Remove(downLoadCallback);
            }
        }

        //获取缓存资源回调
        private static void OnGetCacheTextureCallback(string textureUri, byte[] cacheDate, TextureFormat textureFormat = TextureFormat.RGBA32, bool isSprite = true, bool isAutoCache = true)
        {
            if (mAllRegisterCallbackInfors.TryGetValue(textureUri, out var callbackInfors))
            {
                object result = null;
                Texture2D texture = new Texture2D(100, 100, textureFormat, false, true);
                texture.LoadImage(cacheDate);

                result = texture;
                if (isSprite)
                    result = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                TriggerDownloadImageCallback(textureUri, result, isSprite);
            }
        }
        //下载资源回调
        private static void OnDownloadTextureCallback(string textureUri, bool isSuccess, UnityWebRequest webRequest, TextureFormat textureFormat = TextureFormat.RGBA32, bool isSprite = true, bool isAutoCache = true)
        {
            var handlerTexture = webRequest.downloadHandler as DownloadHandlerTexture;
            if (mAllRegisterCallbackInfors.TryGetValue(textureUri, out var callbackInfors))
            {
                if (isSuccess == false)
                {
                    AddDownloadFailRecord(textureUri);
                    TriggerDownloadImageCallback(textureUri, null, isSprite);
                    return;
                }

                RemoveDownloadFailRecord(textureUri);
                object result = handlerTexture.texture;
                if (isSprite)
                    result = Sprite.Create(handlerTexture.texture, new Rect(0, 0, handlerTexture.texture.width, handlerTexture.texture.height), Vector2.one * 0.5f);
                TriggerDownloadImageCallback(textureUri, result, isSprite);
            }

            if (isAutoCache)
            {
                AppTextureCacheManager.S_Instance.TryCache(handlerTexture.data, textureUri, DateTime_Ex.DaySecondCount);
            }
        }

        //触发事件
        private static void TriggerDownloadImageCallback(string textureUri, object result, bool isSprite)
        {
            if (mAllRegisterCallbackInfors.TryGetValue(textureUri, out var callbackInfors))
            {
                for (int dex = callbackInfors.Count - 1; dex >= 0; dex--)
                {
                    var downLoadCallback = callbackInfors[dex];
                    downLoadCallback?.Invoke(result != null, textureUri, isSprite, result);
                }
                mAllRegisterCallbackInfors.Remove(textureUri);
            }
        }

        //移除下载失败记录
        private static void RemoveDownloadFailRecord(string textureUri)
        {
            if(s_AllDownloadFailRecords.TryGetValue(textureUri,out var downloadFailRecord))
            {
                s_AllDownloadFailRecords.Remove(textureUri);
                s_DownloadTextureFailRecordPool.RecycleItemToPool(downloadFailRecord);
            }
        }


        private static void AddDownloadFailRecord(string textureUri)
        {
            if (s_AllDownloadFailRecords.TryGetValue(textureUri, out var downloadFailRecord))
            {
                downloadFailRecord.mDownloadFailTimes += 1;
                downloadFailRecord.mLastDownloadFailTimeSeconds = System.DateTime.Now.Second;
            }
            else
            {
                downloadFailRecord = s_DownloadTextureFailRecordPool.GetItemFromPool();
                downloadFailRecord.mTetureUri = textureUri;
                downloadFailRecord.mDownloadFailTimes = 1;
                downloadFailRecord.mLastDownloadFailTimeSeconds = System.DateTime.Now.Second;

                s_AllDownloadFailRecords[textureUri] = downloadFailRecord; 
            }
        }

        private static void BeforeRecycleDownloadFailRecord(DownloadTextureFailRecord downloadTextureFailRecord)
        {
            if (downloadTextureFailRecord == null) return;
            downloadTextureFailRecord.mTetureUri = string.Empty;
            downloadTextureFailRecord.mDownloadFailTimes = 0;
            downloadTextureFailRecord.mLastDownloadFailTimeSeconds = 0;
        }

    }
}