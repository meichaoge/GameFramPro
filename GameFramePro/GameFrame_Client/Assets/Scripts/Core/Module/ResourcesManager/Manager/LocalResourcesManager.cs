using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 专用于从 Resources 加载和管理资源 
    /// </summary>
    public static class LocalResourcesManager
    {
        private static float s_LastFullCheckTime { get; set; } = 0;  //上一次全资源检测的时间 
        private static float s_FullCheckTimeInterval { get { return 300; } } //全资源空引用检测时间间隔

        //key  =assetFullUri 
        private static readonly Dictionary<string, LoadResourcesAssetRecord> mAllLoadResourcesAssetRecords = new Dictionary<string, LoadResourcesAssetRecord>(200); //Cache 所有加载的资源


        /// <summary>
        /// 从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        private static LoadResourcesAssetRecord LoadResourcesAssetFromCache(string assetFullUri)
        {
            if (mAllLoadResourcesAssetRecords.TryGetValue(assetFullUri, out var infor))
            {
                if (infor == null || infor.IsRecordEnable == false)
                {
                    mAllLoadResourcesAssetRecords.Remove(assetFullUri);
                    infor?.ReleaseLoadAssetRecord();
                    return null;
                }
                return infor;
            }
            return null;
        }

        /// <summary>
        /// Resources 同步加载资源 (注意加载路径中不能带有扩展名否则加载失败)
        /// </summary>
        /// <param name="assetFullUri"></param>
        /// <returns></returns>
        public static ILoadAssetRecord ResourcesLoadAssetSync(string assetFullUri)
        {
            LoadResourcesAssetRecord resourcesAssetRecord = LoadResourcesAssetFromCache(assetFullUri);
            if (resourcesAssetRecord != null && resourcesAssetRecord.IsRecordEnable)
                return resourcesAssetRecord;

            Object asset = Resources.Load<UnityEngine.Object>(assetFullUri.GetPathWithOutExtension());
            if (asset == null)
            {
#if UNITY_EDITOR
                Debug.LogInfor($"ResourcesLoadAssetSync Fail,assetFullUri={assetFullUri}  not exit");
#endif
                return null;
            }

            resourcesAssetRecord = LoadResourcesAssetRecord.GetLoadResourcesAssetRecord(assetFullUri, asset);
            RecordResourcesLoadAsset(assetFullUri, resourcesAssetRecord);
            return resourcesAssetRecord;
        }


        /// <summary>
        /// Resources 异步加载资源 (注意加载路径中不能带有扩展名否则加载失败)
        /// </summary>
        /// <param name="assetFullUri"></param>
        /// <param name="loadCallback"></param>
        /// <param name="procressCallback"></param>
        public static void ResourcesLoadAssetAsync(string assetFullUri, System.Action<ILoadAssetRecord> loadCallback, System.Action<string, float> procressCallback)
        {
            LoadResourcesAssetRecord resourcesAssetRecord = LoadResourcesAssetFromCache(assetFullUri);
            if (resourcesAssetRecord != null && resourcesAssetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(resourcesAssetRecord);
                return;
            }

            ResourceRequest request = Resources.LoadAsync(assetFullUri.GetPathWithOutExtension());
            AsyncManager.StartAsyncOperation(request, () =>
            {
                resourcesAssetRecord = LoadResourcesAssetRecord.GetLoadResourcesAssetRecord(assetFullUri, request.asset);
                RecordResourcesLoadAsset(assetFullUri, resourcesAssetRecord);
                loadCallback?.Invoke(resourcesAssetRecord);
            }, (procress) => { procressCallback?.Invoke(assetFullUri, procress); });
        }

        /// <summary>
        ///  移除参数指定的实例ID 对应的 Resources 记录
        /// </summary>
        /// <param name="resourcesAssetInstanceIDs"></param>
        public static void RemoveAllUnReferenceResourcesRecord(HashSet<int> resourcesAssetInstanceIDs)
        {
            if (s_LastFullCheckTime == 0)
                s_LastFullCheckTime = Time.realtimeSinceStartup;

            if (resourcesAssetInstanceIDs != null && resourcesAssetInstanceIDs.Count > 0 || (Time.realtimeSinceStartup - s_LastFullCheckTime) >= s_FullCheckTimeInterval)
            {
                s_LastFullCheckTime = Time.realtimeSinceStartup;
                Dictionary<string, LoadResourcesAssetRecord> tempResourcesAssetRecords = new Dictionary<string, LoadResourcesAssetRecord>(mAllLoadResourcesAssetRecords);
                foreach (var resourcesAssetRecordInfor in tempResourcesAssetRecords)
                {
                    if (resourcesAssetRecordInfor.Value == null)
                    {
                        mAllLoadResourcesAssetRecords.Remove(resourcesAssetRecordInfor.Key);
                        continue;
                    }

                    if (resourcesAssetRecordInfor.Value.IsRecordEnable == false)
                    {
                        mAllLoadResourcesAssetRecords.Remove(resourcesAssetRecordInfor.Value.mAssetFullUri);
                        LoadResourcesAssetRecord.ReleaseAssetBundleRecordInfor(resourcesAssetRecordInfor.Value);
                        continue;
                    }

                    if (resourcesAssetInstanceIDs == null || resourcesAssetInstanceIDs.Count == 0)
                        continue;

                    int resourcesAssetInstanceID = resourcesAssetRecordInfor.Value.GetLoadAssetInstanceID();
                    if (resourcesAssetInstanceIDs.Contains(resourcesAssetInstanceID))
                    {
#if UNITY_EDITOR
                        Debug.Log($"释放Resources 资源： {resourcesAssetRecordInfor.Value.mAssetFullUri}");
#endif
                        mAllLoadResourcesAssetRecords.Remove(resourcesAssetRecordInfor.Value.mAssetFullUri);
                        LoadResourcesAssetRecord.ReleaseAssetBundleRecordInfor(resourcesAssetRecordInfor.Value);
                    }

                }
            }
        }



        /// <summary>/// / 记录加载的资源/// </summary>
        private static void RecordResourcesLoadAsset(string assetUrl, LoadResourcesAssetRecord asset)
        {
            if (asset == null)
                return;
            if (mAllLoadResourcesAssetRecords.TryGetValue(assetUrl, out var infor))
            {
                if (infor == asset)
                    return;
                Debug.LogError($"RecordResourcesLoadAsset Fail,Already Exit Asset at path={assetUrl} Not Equal");
                mAllLoadResourcesAssetRecords[assetUrl] = asset;
                return;
            }

            mAllLoadResourcesAssetRecords[assetUrl] = asset;
        }
    }
}