using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary> /// 专用于从 Resources 加载和管理资源  /// </summary>
    public sealed class LocalResourcesManager : Single<LocalResourcesManager>, IAssetManager
    {
        #region Data

        //key  =assetpath 
        private readonly Dictionary<string, LoadAssetResourcesAssetRecord> mAllLoadedAssetRecord = new Dictionary<string, LoadAssetResourcesAssetRecord>(200); //Cache 所有加载的资源
        private NativeObjectPool<LoadAssetResourcesAssetRecord> mResourcesLoadAssetRecordPoolMgr;
#if UNITY_EDITOR
        public Dictionary<string, LoadAssetResourcesAssetRecord> Debug_mAllLoadedAssetRecord
        {
            get { return mAllLoadedAssetRecord; }
        }
#endif

        #endregion

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedResourcesLoad();
        }


        #region 对象池接口

        private void InitialedResourcesLoad()
        {
            mResourcesLoadAssetRecordPoolMgr = new NativeObjectPool<LoadAssetResourcesAssetRecord>(50, OnBeforeGetResourcesLoadAssetRecord, OnBeforeRecycleResourcesLoadAssetRecord);
        }

        private void OnBeforeGetResourcesLoadAssetRecord(LoadAssetResourcesAssetRecord record)
        {
        }

        private void OnBeforeRecycleResourcesLoadAssetRecord(LoadAssetResourcesAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }

        #endregion

        #region IAssetManager 接口实现

        public float MaxAliveTimeAfterNoReference
        {
            get { return 60; }
        } //最多存在1分钟

        public void NotifyAssetRelease(LoadAssetBaseRecord record)
        {
            if (record == null) return;

            Debug.LogInfor("有资源记录被移除 " + record.AssetUrl);

            if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                mAllLoadedAssetRecord.Remove(record.AssetUrl);
            else
                Debug.LogError("NotifyAssetRelease Error  不存在这个资源 {0}的记录", record.AssetUrl);
        }

        public void NotifyAssetReferenceChange(LoadAssetBaseRecord record, bool isAddReference)
        {
            if (record == null) return;

            //处理Resources 资源加载和释放

            if (isAddReference)
            {
                if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl) == false)
                    mAllLoadedAssetRecord[record.AssetUrl] = record as LoadAssetResourcesAssetRecord;
            }
            else
            {
                if (record.ReferenceCount == 0)
                {
                    if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                        AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(record);
                    else
                        Debug.LogError("NotifyAssetReferenceChange Error !!" + record.AssetUrl);
                } //资源没有引用时候 释放资源
            }
        }

        #endregion


        #region 加载缓存资源

        /// <summary>///  从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)/// </summary>
        public LoadAssetResourcesAssetRecord LoadAssetFromCache(string assetpath)
        {
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out var infor))
                return infor;
            return null;
        }

        #endregion

        #region 同步& 异步 加载资源 (注意加载路径中不能带有扩展名否则加载失败)

        //Resources 同步加载资源
        public void LoadAssetSync(string assetpath, System.Action<LoadAssetResourcesAssetRecord> loadCallback)
        {
            LoadAssetResourcesAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadBasicObjectAssetInfor.IsLoadAssetEnable)
            {
                RecordResourcesLoadAsset(record.LoadBasicObjectAssetInfor); //当 asset==null 时候记录返回 
                loadCallback?.Invoke(record);
                return;
            }

            Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            var assetInfor = new LoadBasicAssetInfor(assetpath, asset, LoadedAssetTypeEnum.Resources_UnKnown);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 
            loadCallback?.Invoke(record);
        }

        public LoadAssetResourcesAssetRecord LoadAssetSync(string assetpath)
        {
            LoadAssetResourcesAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadBasicObjectAssetInfor.IsLoadAssetEnable)
            {
                record = RecordResourcesLoadAsset(record.LoadBasicObjectAssetInfor); //当 asset==null 时候记录返回 
                return record;
            }

            UnityEngine.Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
            {
#if UNITY_EDITOR
                Debug.LogInfor(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));
#endif
                return null;
            }

            var assetInfor = new LoadBasicAssetInfor(assetpath, asset, LoadedAssetTypeEnum.Resources_UnKnown);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 
            return record;
        }


        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<LoadAssetResourcesAssetRecord> loadCallback, System.Action<string, float> procressCallback)
        {
            LoadAssetResourcesAssetRecord assetRecord = LoadAssetFromCache(assetpath);
            if (assetRecord != null && assetRecord.LoadBasicObjectAssetInfor.IsLoadAssetEnable)
            {
                assetRecord = RecordResourcesLoadAsset(assetRecord.LoadBasicObjectAssetInfor); //当 asset==null 时候记录返回 
                loadCallback?.Invoke(assetRecord);
                return;
            }


            ResourceRequest request = Resources.LoadAsync(assetpath.GetPathWithOutExtension());
            if (procressCallback != null)
            {
                AsyncManager.StartAsyncOperation(request, () =>
                {
                    assetRecord = RecordResourcesLoadAsset(new LoadBasicAssetInfor(assetpath, request.asset, LoadedAssetTypeEnum.Resources_UnKnown)); //当 asset==null 时候记录返回 
                    loadCallback?.Invoke(assetRecord);
                }, (procress) =>
                {
                    if (procressCallback != null)
                        procressCallback(assetpath, procress);
                });
            } //返回加载进度
            else
            {
                AsyncManager.StartAsyncOperation(request, completeCallback: () =>
                {
                    assetRecord = RecordResourcesLoadAsset(new LoadBasicAssetInfor(assetpath, request.asset, LoadedAssetTypeEnum.Resources_UnKnown)); //当 asset==null 时候记录返回 
                    loadCallback?.Invoke(assetRecord);
                }, null);
            } //不考虑加载进度
        }

        #endregion

        #region 辅助记录

        /// <summary>/// / 记录加载的资源/// </summary>
        public LoadAssetResourcesAssetRecord RecordResourcesLoadAsset(LoadBasicAssetInfor loadAssetInfor)
        {
            if (loadAssetInfor == null)
                return null;
            if (mAllLoadedAssetRecord.TryGetValue(loadAssetInfor.AssetUrl, out var infor))
            {
                if (infor.LoadBasicObjectAssetInfor.IsEqual(loadAssetInfor))
                    return infor;
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} Not Equal", loadAssetInfor.AssetUrl));
                return infor;
            }

            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(loadAssetInfor.AssetUrl) as LoadAssetResourcesAssetRecord;
            if (infor == null)
                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();

            infor.Initial(loadAssetInfor.AssetUrl, LoadedAssetTypeEnum.Resources_UnKnown, loadAssetInfor, this);
            mAllLoadedAssetRecord[loadAssetInfor.AssetUrl] = infor;
            return infor;
        }

//         /// <summary>/// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象/// </summary>
//         public void UnRecordResourcesLoadAsset(LoadBasicAssetInfor loadAssetInfor, bool isforceDelete)
//         {
//             if (loadAssetInfor == null || loadAssetInfor.IsLoadAssetEnable == false)
//                 return;
// 
//             string assetpath = string.Empty;
//             foreach (var item in mAllLoadedAssetRecord)
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
//                 var resourcesAssetRecord = mAllLoadedAssetRecord[assetpath];
//                 resourcesAssetRecord.ReduceReference(isforceDelete);
//                 return;
//             }
// 
//             Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", loadAssetInfor.AssetUrl));
//         }

        #endregion
    }
}
