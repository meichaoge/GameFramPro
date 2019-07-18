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
    public class LocalResourcesManager : Single<LocalResourcesManager>, IAssetManager
    {
        #region Data
        //key  =assetpath 
        private Dictionary<string, ResourcesLoadAssetRecord> mAllLoadedAssetRecord = new Dictionary<string, ResourcesLoadAssetRecord>(200); //Cache 所有加载的资源
        private HashSet<int> mAllResoucesLoadAssetInstanceIds = new HashSet<int>();
        private NativeObjectPool<ResourcesLoadAssetRecord> mResourcesLoadAssetRecordPoolMgr;
#if UNITY_EDITOR
        public Dictionary<string, ResourcesLoadAssetRecord> Debug_mAllLoadedAssetRecord { get { return mAllLoadedAssetRecord; } }
        public HashSet<int> Debug_mAllResoucesLoadAssetInstanceIds { get { return mAllResoucesLoadAssetInstanceIds; } }
#endif
        #endregion


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedRsourcesLoad();
        }

        #region IAssetManager 接口实现

        private void InitialedRsourcesLoad()
        {
            mResourcesLoadAssetRecordPoolMgr = new NativeObjectPool<ResourcesLoadAssetRecord>(50, OnBeforGetResourcesLoadAssetRecord, OnBeforRecycleResourcesLoadAssetRecord);
        }

        private void OnBeforGetResourcesLoadAssetRecord(ResourcesLoadAssetRecord record)
        {

        }

        private void OnBeforRecycleResourcesLoadAssetRecord(ResourcesLoadAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        public float MaxAliveTimeAfterNoReference { get { return 600; } } //最多存在10分钟

        public void NotifyAssetNoReference(ILoadAssetRecord infor)
        {
            if (infor == null) return;
            if (mAllLoadedAssetRecord.ContainsKey(infor.AssetPath))
            {
                mAllLoadedAssetRecord.Remove(infor.AssetPath);
                AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(infor);
            }
            else
            {
                Debug.LogError("NotifyAssetNoReference Error !!" + infor.AssetPath);
            }
        }

        public void MarkTargetAssetNull(ILoadAssetRecord record)
        {
            if (record == null) return;
            ResourcesLoadAssetRecord recordInfor = null;
            string key = record.AssetPath;
            if (mAllLoadedAssetRecord.TryGetValue(key, out recordInfor))
            {
                mResourcesLoadAssetRecordPoolMgr.RecycleItemToPool(recordInfor);
                mAllLoadedAssetRecord.Remove(key);
            }
        }
        #endregion



        /// <summary>
        /// 从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private Object ResourcesLoadAssetFromCache(string assetpath)
        {
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (infor != null)
                    infor.AddReference();

                return infor.TargetAsset; //可能是null
            }
            return null;
        }



        #region 同步异步加载资源
        //Resources 同步加载资源
        public void ResourcesLoadAssetSync(string assetpath, System.Action<Object> loadCallback)
        {
            Object asset = ResourcesLoadAssetFromCache(assetpath);
            if (asset != null)
            {
                RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }
            asset = Resources.Load(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(asset);
        }

        public Object ResourcesLoadAssetSync(string assetpath)
        {
            Object asset = ResourcesLoadAssetFromCache(assetpath);
            if (asset != null)
            {
                RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
                return asset;
            }
            asset = Resources.Load(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
            return asset;
        }
        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<Object> loadCallback, System.Action<string, float> procressCallback)
        {
            Object asset = ResourcesLoadAssetFromCache(assetpath);
            if (asset != null)
            {
                RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }


            ResourceRequest request = Resources.LoadAsync(assetpath);

            if (procressCallback != null)
            {
                AsyncManager.S_Instance.StartAsyncOperation(request as AsyncOperation, (async) =>
                {
                    ResourceRequest result = async as ResourceRequest;
                    RecordResourcesLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(result.asset);
                }, (procress) =>
                {
                    if (procressCallback != null)
                        procressCallback(assetpath, procress);
                });
            }//返回加载进度
            else
            {
                AsyncManager.S_Instance.StartAsyncOperation(request as AsyncOperation, (async) =>
                {
                    ResourceRequest result = async as ResourceRequest;
                    RecordResourcesLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(result.asset);
                }, null);
            }//不考虑加载进度
        }

        #endregion

        #region 辅助记录

        /// <summary>
        /// 记录加载的资源
        /// </summary>
        /// <param name="assetpath"></param>
        /// <param name="asset"></param>
        public void RecordResourcesLoadAsset(string assetpath, Object asset)
        {
            if (asset == null)
                return;
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (object.ReferenceEquals(asset, infor.TargetAsset))
                {
                    RecordAssetInstanceId(infor.InstanceID);
                    infor.AddReference(); //增加引用次数
                                          //    Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1}", assetpath, asset));
                    return;
                }
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
                return;
            }
            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetpath) as ResourcesLoadAssetRecord;
            if (infor == null)
                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();

            infor.Initial(assetpath, asset.name, LoadedAssetTypeEnum.Resources_UnKnown, asset, this);
            RecordAssetInstanceId(infor.InstanceID);
            mAllLoadedAssetRecord[assetpath] = infor;
        }

        

        /// <summary>
        /// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isforceDelete"></param>
        public void UnRecordResourcesLoadAsset(Object asset, bool isforceDelete)
        {
            if (asset == null)
                return;

            string assetpath = string.Empty;
            foreach (var item in mAllLoadedAssetRecord)
            {
                if (item.Value.TargetAsset == asset)
                {
                    assetpath = item.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(assetpath) == false)
            {
                var loadAssetInfor = mAllLoadedAssetRecord[assetpath];
                UnRecordAssetInstanceId(loadAssetInfor.InstanceID);
                loadAssetInfor.ReduceReference(isforceDelete);
                return;
            }

            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", asset));
        }



        private  void RecordAssetInstanceId(int instanceID)
        {
            if (mAllResoucesLoadAssetInstanceIds.Contains(instanceID))
                return;
            mAllResoucesLoadAssetInstanceIds.Add(instanceID);
        }


        private void UnRecordAssetInstanceId(int instanceID)
        {
            if (mAllResoucesLoadAssetInstanceIds.Contains(instanceID))
            mAllResoucesLoadAssetInstanceIds.Remove(instanceID);
        }

        //判断是否是通过Resources 加载的
        public bool IsAssetLoadedByResource(int instanceID)
        {
            return mAllResoucesLoadAssetInstanceIds.Contains(instanceID);
        }
        #endregion


    }
}
