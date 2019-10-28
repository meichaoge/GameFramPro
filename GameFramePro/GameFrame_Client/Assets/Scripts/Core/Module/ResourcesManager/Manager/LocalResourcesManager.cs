using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary> /// 专用于从 Resources 加载和管理资源  /// </summary>
    public sealed class LocalResourcesManager : Single<LocalResourcesManager>
    {
        //key  =assetFullUri 
        private static readonly Dictionary<string, LoadResourcesAssetRecord> mAllLoadResourcesAssetRecords = new Dictionary<string, LoadResourcesAssetRecord>(200); //Cache 所有加载的资源


        /// <summary>
        /// 从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        private LoadResourcesAssetRecord LoadResourcesAssetFromCache(string assetFullUri)
        {
            if (mAllLoadResourcesAssetRecords.TryGetValue(assetFullUri, out var infor))
                return infor;
            return null;
        }

        /// <summary>
        /// Resources 同步加载资源 (注意加载路径中不能带有扩展名否则加载失败)
        /// </summary>
        /// <param name="assetFullUri"></param>
        /// <returns></returns>
        public ILoadAssetRecord ResourcesLoadAssetSync(string assetFullUri)
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
        public void ResourcesLoadAssetAsync(string assetFullUri, System.Action<ILoadAssetRecord> loadCallback, System.Action<string, float> procressCallback)
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
        /// 移除所有没有被引用的 Resources 记录
        /// </summary>
        public void RemoveAllUnReferenceResourcesRecord(ref HashSet<int> resourcesAssetInstanceIDs)
        {
            if (resourcesAssetInstanceIDs == null)
                resourcesAssetInstanceIDs = new HashSet<int>();
            Dictionary<string, LoadResourcesAssetRecord> tempResourcesAssetRecords = new Dictionary<string, LoadResourcesAssetRecord>(mAllLoadResourcesAssetRecords);

            foreach (var resourcesAssetRecordInfor in tempResourcesAssetRecords.Values)
            {
                if (resourcesAssetRecordInfor == null) continue;

                int resourcesAssetInstanceID = resourcesAssetRecordInfor.GetLoadAssetInstanceID();
                if (resourcesAssetInstanceID != -1 && resourcesAssetInstanceIDs.Contains(resourcesAssetInstanceID) == false)
                    resourcesAssetInstanceIDs.Add(resourcesAssetInstanceID);

                mAllLoadResourcesAssetRecords.Remove(resourcesAssetRecordInfor.mAssetFullUri);
                LoadResourcesAssetRecord.ReleaseAssetBundleRecordInfor(resourcesAssetRecordInfor);
            }
        }


        /// <summary>/// / 记录加载的资源/// </summary>
        private void RecordResourcesLoadAsset(String assetUrl, LoadResourcesAssetRecord asset)
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