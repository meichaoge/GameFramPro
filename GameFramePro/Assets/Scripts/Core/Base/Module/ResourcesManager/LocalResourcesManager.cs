using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary> /// 专用于从 Resources 加载和管理资源  /// </summary>
    public class LocalResourcesManager : Single<LocalResourcesManager>, IAssetManager
    {
        #region Data

        //key  =assetpath 
        private readonly Dictionary<string, BaseLoadAssetRecord> mAllLoadedAssetRecord = new Dictionary<string, BaseLoadAssetRecord>(200); //Cache 所有加载的资源
        private NativeObjectPool<BaseLoadAssetRecord> mResourcesLoadAssetRecordPoolMgr;
#if UNITY_EDITOR
        public Dictionary<string, BaseLoadAssetRecord> Debug_mAllLoadedAssetRecord
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

        #region IAssetManager 接口实现

        private void InitialedResourcesLoad()
        {
            mResourcesLoadAssetRecordPoolMgr = new NativeObjectPool<BaseLoadAssetRecord>(50, OnBeforeGetResourcesLoadAssetRecord, OnBeforeRecycleResourcesLoadAssetRecord);
        }

        private void OnBeforeGetResourcesLoadAssetRecord(BaseLoadAssetRecord record)
        {
        }

        private void OnBeforeRecycleResourcesLoadAssetRecord(BaseLoadAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        public float MaxAliveTimeAfterNoReference
        {
            get { return 60; }
        } //最多存在1分钟

        public void NotifyAssetRelease(BaseLoadAssetRecord record)
        {
            if (record == null) return;

            if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                mAllLoadedAssetRecord.Remove(record.AssetUrl);
            else
                Debug.LogError("NotifyAssetRelease Error  不存在这个资源 {0}的记录", record.AssetUrl);

            if (record.IsReferenceEnable)
                record.LoadUnityObjectAssetInfor.ReleaseAsset();
        }

        public void NotifyAssetReferenceChange(BaseLoadAssetRecord record)
        {
            if (record == null) return;

            #region 处理Resources 资源加载和释放

            if (record.ReferenceCount == 0)
            {
                if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(record );
                else
                    Debug.LogError("NotifyAssetReferenceChange Error !!" + record.AssetUrl);
            } //资源没有引用时候 释放资源
            else
            {
                if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl) == false)
                    mAllLoadedAssetRecord[record.AssetUrl] = record ;
            }

            #endregion
        }

        #endregion


        #region 加载缓存资源

        /// <summary>///  从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)/// </summary>
        public BaseLoadAssetRecord LoadAssetFromCache(string assetpath)
        {
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out var infor))
                return infor;
            return null;
        }

        #endregion

        #region 同步& 异步 加载资源 (注意加载路径中不能带有扩展名否则加载失败)

        //Resources 同步加载资源
        public void LoadAssetSync(string assetpath, System.Action<BaseLoadAssetRecord> loadCallback)
        {
            BaseLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadUnityObjectAssetInfor.IsLoadAssetEnable)
            {
                RecordResourcesLoadAsset(record.LoadUnityObjectAssetInfor); //当 asset==null 时候记录返回 
                loadCallback?.Invoke(record);
                return;
            }

            Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            var assetInfor = new BaseLoadUnityAssetInfor(assetpath, asset, LoadedAssetTypeEnum.Resources_UnKnown);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 
            loadCallback?.Invoke(record);
        }

        public BaseLoadAssetRecord LoadAssetSync(string assetpath)
        {
            BaseLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadUnityObjectAssetInfor.IsLoadAssetEnable)
            {
                record = RecordResourcesLoadAsset(record.LoadUnityObjectAssetInfor); //当 asset==null 时候记录返回 
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

            var assetInfor = new BaseLoadUnityAssetInfor(assetpath, asset, LoadedAssetTypeEnum.Resources_UnKnown);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 
            return record;
        }


        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<BaseLoadAssetRecord> loadCallback, System.Action<string, float> procressCallback)
        {
            BaseLoadAssetRecord assetRecord = LoadAssetFromCache(assetpath);
            if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor.IsLoadAssetEnable)
            {
                assetRecord = RecordResourcesLoadAsset(assetRecord.LoadUnityObjectAssetInfor); //当 asset==null 时候记录返回 
                loadCallback?.Invoke(assetRecord);
                return;
            }


            ResourceRequest request = Resources.LoadAsync(assetpath.GetPathWithOutExtension());
            if (procressCallback != null)
            {
                AsyncManager.S_Instance.StartAsyncOperation(request, (async) =>
                {
                    assetRecord = RecordResourcesLoadAsset(new BaseLoadUnityAssetInfor(assetpath, (async as ResourceRequest).asset, LoadedAssetTypeEnum.Resources_UnKnown)); //当 asset==null 时候记录返回 
                    loadCallback?.Invoke(assetRecord);
                }, (procress) =>
                {
                    if (procressCallback != null)
                        procressCallback(assetpath, procress);
                });
            } //返回加载进度
            else
            {
                AsyncManager.S_Instance.StartAsyncOperation(request, completeCallback: (async) =>
                {
                    assetRecord = RecordResourcesLoadAsset(new BaseLoadUnityAssetInfor(assetpath, (async as ResourceRequest).asset, LoadedAssetTypeEnum.Resources_UnKnown)); //当 asset==null 时候记录返回 
                    loadCallback?.Invoke(assetRecord);
                }, null);
            } //不考虑加载进度
        }

        #endregion

        #region 辅助记录

        /// <summary>/// / 记录加载的资源/// </summary>
        public BaseLoadAssetRecord RecordResourcesLoadAsset(BaseLoadUnityAssetInfor loadAssetInfor)
        {
            if (loadAssetInfor == null)
                return null;
            if (mAllLoadedAssetRecord.TryGetValue(loadAssetInfor.AssetUrl, out var infor))
            {
                if (infor.LoadUnityObjectAssetInfor.IsEqual(loadAssetInfor))
                    return infor;
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} Not Equal", loadAssetInfor.AssetUrl));
                return infor;
            }

            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(loadAssetInfor.AssetUrl);
            if (infor == null)
                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();

            infor.Initial(loadAssetInfor.AssetUrl, LoadedAssetTypeEnum.Resources_UnKnown, loadAssetInfor, this);
            mAllLoadedAssetRecord[loadAssetInfor.AssetUrl] = infor;
            return infor;
        }

        /// <summary>/// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象/// </summary>
        public void UnRecordResourcesLoadAsset(BaseLoadUnityAssetInfor loadAssetInfor, bool isforceDelete)
        {
            if (loadAssetInfor == null || loadAssetInfor.IsLoadAssetEnable == false)
                return;

            string assetpath = string.Empty;
            foreach (var item in mAllLoadedAssetRecord)
            {
                if (item.Value.LoadUnityObjectAssetInfor.IsEqual(loadAssetInfor))
                {
                    assetpath = item.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(assetpath) == false)
            {
                var resourcesAssetRecord = mAllLoadedAssetRecord[assetpath];
                resourcesAssetRecord.ReduceReference(isforceDelete);
                return;
            }

            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", loadAssetInfor.AssetUrl));
        }

        #endregion
    }
}
