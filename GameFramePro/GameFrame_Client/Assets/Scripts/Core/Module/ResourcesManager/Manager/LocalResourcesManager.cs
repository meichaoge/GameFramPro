using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary> /// 专用于从 Resources 加载和管理资源  /// </summary>
    public sealed class LocalResourcesManager : Single<LocalResourcesManager>//, IAssetManager
    {

        //key  =assetpath 
        private static readonly Dictionary<string, LoadResourcesAssetRecord> mAllLoadResourcesAssetRecords = new Dictionary<string, LoadResourcesAssetRecord>(200); //Cache 所有加载的资源



        #region IAssetManager 接口实现

        public float MaxAliveTimeAfterNoReference
        {
            get { return 60; }
        } //最多存在1分钟

        //        public void NotifyAssetRelease(LoadAssetBaseRecord record)
        //        {
        //            if (record == null) return;

        //            Debug.LogInfor("有资源记录被移除 " + record.AssetUrl);

        //            if (mAllLoadResourcesAssetRecords.ContainsKey(record.AssetUrl))
        //                mAllLoadResourcesAssetRecords.Remove(record.AssetUrl);
        //            else
        //                Debug.LogError("NotifyAssetRelease Error  不存在这个资源 {0}的记录", record.AssetUrl);
        //        }

        //        public void NotifyAssetReferenceChange(LoadAssetBaseRecord record, bool isAddReference)
        //        {
        //            if (record == null) return;

        //            //处理Resources 资源加载和释放

        ////            if (isAddReference)
        ////            {
        ////                if (mAllLoadResourcesAssetRecords.ContainsKey(record.AssetUrl) == false)
        ////                    mAllLoadResourcesAssetRecords[record.AssetUrl] = record as LoadAssetResourcesAssetRecord;
        ////            }
        ////            else
        ////            {
        ////                if (record.ReferenceCount == 0)
        ////                {
        ////                    if (mAllLoadResourcesAssetRecords.ContainsKey(record.AssetUrl))
        ////                        AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(record);
        ////                    else
        ////                        Debug.LogError("NotifyAssetReferenceChange Error !!" + record.AssetUrl);
        ////                } //资源没有引用时候 释放资源
        ////            }
        //        }

        #endregion


        #region 同步& 异步 加载资源 (注意加载路径中不能带有扩展名否则加载失败)
        /// <summary>///  从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)/// </summary>
        private LoadResourcesAssetRecord LoadResourcesAssetFromCache(string assetpath)
        {
            if (mAllLoadResourcesAssetRecords.TryGetValue(assetpath, out var infor))
                return infor;
            return null;
        }

        //Resources 同步加载资源
        public void ResourcesLoadAssetSync(string assetpath, System.Action<ILoadAssetRecord> loadCallback)
        {
            LoadResourcesAssetRecord record = LoadResourcesAssetFromCache(assetpath);
            if (record != null)
            {
                loadCallback?.Invoke(record);
                return;
            }

            Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
            {
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));
                loadCallback?.Invoke(null);
                return;
            }
            record = LoadResourcesAssetRecord.GetLoadResourcesAssetRecord(assetpath,asset);
            RecordResourcesLoadAsset(assetpath, record);
            loadCallback?.Invoke(record);
        }

        public ILoadAssetRecord ResourcesLoadAssetSync(string assetpath)
        {
            LoadResourcesAssetRecord record = LoadResourcesAssetFromCache(assetpath);
            if (record != null)
                return record;

            Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
            {
#if UNITY_EDITOR
                Debug.LogInfor($"LoadResourcesAssetSync Fail,AssetPath={assetpath}  not exit");
#endif
                return null;
            }
            record = LoadResourcesAssetRecord.GetLoadResourcesAssetRecord(assetpath, asset);
            RecordResourcesLoadAsset(assetpath, record);
            return record;
        }


        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<ILoadAssetRecord> loadCallback, System.Action<string, float> procressCallback)
        {
            LoadResourcesAssetRecord resourcesAssetRecord = LoadResourcesAssetFromCache(assetpath);
            if (resourcesAssetRecord != null)
            {
                loadCallback?.Invoke(resourcesAssetRecord);
                return;
            }
            ResourceRequest request = Resources.LoadAsync(assetpath.GetPathWithOutExtension());
            AsyncManager.StartAsyncOperation(request, () =>
            {
                resourcesAssetRecord = LoadResourcesAssetRecord.GetLoadResourcesAssetRecord(assetpath, request.asset);
                RecordResourcesLoadAsset(assetpath, resourcesAssetRecord);
                loadCallback?.Invoke(resourcesAssetRecord);
            }, (procress) => { procressCallback?.Invoke(assetpath, procress); });
        }

        #endregion


        #region 辅助记录

        /// <summary>/// / 记录加载的资源/// </summary>
        public void RecordResourcesLoadAsset(String assetUrl, LoadResourcesAssetRecord asset)
        {
            if (asset == null)
                return;
            if (mAllLoadResourcesAssetRecords.TryGetValue(assetUrl, out var infor))
            {
                if (infor == asset)
                    return;
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} Not Equal", assetUrl));
                mAllLoadResourcesAssetRecords[assetUrl] = asset;
                return;
            }

//            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(loadAssetInfor.AssetUrl) as LoadAssetResourcesAssetRecord;
//            if (infor == null)
//                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();
//
//            infor.Initial(loadAssetInfor.AssetUrl, LoadedAssetTypeEnum.Resources_UnKnown, loadAssetInfor, this);
            mAllLoadResourcesAssetRecords[assetUrl] = asset;
        }

//         /// <summary>/// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象/// </summary>
//         public void UnRecordResourcesLoadAsset(LoadBasicAssetInfor loadAssetInfor, bool isforceDelete)
//         {
//             if (loadAssetInfor == null || loadAssetInfor.IsLoadAssetEnable == false)
//                 return;
// 
//             string assetpath = string.Empty;
//             foreach (var item in mAllLoadResourcesAssetRecords)
//             {
//                 if (item.Value.LoadBasicObjectAssetInfor.IsEqual(loadAssetInfor))
//                 {
//                     assetpath = item.Key;
//                     break;
//                 }
//             }
// 
//             if (string.IsNullOrEmpty(assetpath) == false)
//             {
//                 var resourcesAssetRecord = mAllLoadResourcesAssetRecords[assetpath];
//                 resourcesAssetRecord.ReduceReference(isforceDelete);
//                 return;
//             }
// 
//             Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", loadAssetInfor.AssetUrl));
//         }

        #endregion
    }
}