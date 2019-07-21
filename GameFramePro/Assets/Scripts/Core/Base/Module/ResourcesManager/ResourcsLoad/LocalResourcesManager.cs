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
        private Dictionary<int, string> mAllResoucesLoadAssetInstanceIds = new Dictionary<int, string>();
        private NativeObjectPool<ResourcesLoadAssetRecord> mResourcesLoadAssetRecordPoolMgr;
#if UNITY_EDITOR
        public Dictionary<string, ResourcesLoadAssetRecord> Debug_mAllLoadedAssetRecord { get { return mAllLoadedAssetRecord; } }
        public Dictionary<int, string> Debug_mAllResoucesLoadAssetInstanceIds { get { return mAllResoucesLoadAssetInstanceIds; } }
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

    

        public float MaxAliveTimeAfterNoReference { get { return 60; } } //最多存在1分钟

        public void NotifyAssetRelease(ILoadAssetRecord record)
        {
            if (record == null) return;
            if (record is ResourcesLoadAssetRecord)
            {
                if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                    mAllLoadedAssetRecord.Remove(record.AssetUrl);
                else
                    Debug.LogError("NotifyAssetRelease Error  不存在这个资源 {0}的记录" , record.AssetUrl);

                ResourcesManager.UnLoadResourceAsset(record);
                return;
            }
            Debug.LogError("NotifyAssetRelease Fail,无法处理的类型 " + record.GetType());
        }
        public void NotifyAssetReferenceChange(ILoadAssetRecord record)
        {
            if (record == null) return;
            if (record is ResourcesLoadAssetRecord)
            {
                #region 处理Resources 资源加载和释放
                if (record.ReferenceCount == 0)
                {
                    if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl))
                        AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(record);
                    else
                        Debug.LogError("NotifyAssetReferenceChange Error !!" + record.AssetUrl);
                } //资源没有引用时候 释放资源
                else
                {
                    if (mAllLoadedAssetRecord.ContainsKey(record.AssetUrl) == false)
                        mAllLoadedAssetRecord[record.AssetUrl] = record as ResourcesLoadAssetRecord;
                }

                #endregion
                return;
            }
            Debug.LogError("NotifyAssetReferenceChange Fail,无法处理的类型 " + record.GetType());
        }

        #endregion


        #region 加载缓存资源



        /// <summary>
        ///  从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public ResourcesLoadAssetRecord LoadAssetFromCache(string assetpath)
        {
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                //if (infor != null)
                //    infor.AddReference();
                return infor;
            }
            return null;
        }

        #endregion

        #region 同步加载资源

        //Resources 同步加载资源
        public void LoadAssetSync(string assetpath, System.Action<ResourcesLoadAssetRecord> loadCallback)
        {
            ResourcesLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.TargetAsset != null)
            {
                RecordResourcesLoadAsset(assetpath, record.TargetAsset); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(record);
                return;
            }
            Object asset = Resources.Load<UnityEngine.Object>(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            record = RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(record);
        }


        public ResourcesLoadAssetRecord LoadAssetSync(string assetpath)
        {
            ResourcesLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.TargetAsset != null)
            {
                record = RecordResourcesLoadAsset(assetpath, record.TargetAsset); //当 asset==null 时候记录返回 
                return record;
            }
            UnityEngine.Object asset = Resources.Load<UnityEngine.Object>(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            record = RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
            return record;
        }

        #endregion

        #region 异步加载资源


        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<ResourcesLoadAssetRecord> loadCallback, System.Action<string, float> procressCallback) 
        {
            ResourcesLoadAssetRecord assetRecord = LoadAssetFromCache(assetpath);
            if (assetRecord != null&& assetRecord.TargetAsset!=null)
            {
                assetRecord= RecordResourcesLoadAsset(assetpath, assetRecord.TargetAsset); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }


            ResourceRequest request = Resources.LoadAsync(assetpath);
            if (procressCallback != null)
            {
                AsyncManager.S_Instance.StartAsyncOperation(request as AsyncOperation, (async) =>
                {
                    ResourceRequest result = async as ResourceRequest;
                    assetRecord= RecordResourcesLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(assetRecord);
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
                    assetRecord= RecordResourcesLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(assetRecord);
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
        public ResourcesLoadAssetRecord RecordResourcesLoadAsset(string assetpath, Object asset)
        {
            if (asset == null)
                return null;
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (object.ReferenceEquals(asset, infor.TargetAsset))
                {
                    RecordAssetInstanceId(infor.InstanceID, assetpath);
                 //   infor.AddReference(); //增加引用次数
                                          //    Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1}", assetpath, asset));
                    return infor;
                }
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
                return infor;
            }
            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetpath) as ResourcesLoadAssetRecord;
            if (infor == null)
                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();

            infor.Initial(assetpath,  LoadedAssetTypeEnum.Resources_UnKnown, asset, this);
            RecordAssetInstanceId(infor.InstanceID, assetpath);
            mAllLoadedAssetRecord[assetpath] = infor;
            return infor;
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

        private void RecordAssetInstanceId(int instanceID, string assetPath)
        {
            if (mAllResoucesLoadAssetInstanceIds.ContainsKey(instanceID))
                return;
            mAllResoucesLoadAssetInstanceIds.Add(instanceID, assetPath);
        }

        private void UnRecordAssetInstanceId(int instanceID)
        {
            if (mAllResoucesLoadAssetInstanceIds.ContainsKey(instanceID))
                mAllResoucesLoadAssetInstanceIds.Remove(instanceID);
        }

        //判断是否是通过Resources 加载的
        public bool IsAssetLoadedByResource(int instanceID)
        {
            return mAllResoucesLoadAssetInstanceIds.ContainsKey(instanceID);
        }

    
        #endregion


    }
}
