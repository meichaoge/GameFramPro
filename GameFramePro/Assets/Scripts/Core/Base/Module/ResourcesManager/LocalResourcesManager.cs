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
                                                                                                                                            //     private Dictionary<int, string> mAllResoucesLoadAssetInstanceIds = new Dictionary<int, string>();
        private NativeObjectPool<ResourcesLoadAssetRecord> mResourcesLoadAssetRecordPoolMgr;
#if UNITY_EDITOR
        public Dictionary<string, ResourcesLoadAssetRecord> Debug_mAllLoadedAssetRecord { get { return mAllLoadedAssetRecord; } }
        //        public Dictionary<int, string> Debug_mAllResoucesLoadAssetInstanceIds { get { return mAllResoucesLoadAssetInstanceIds; } }
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
                    Debug.LogError("NotifyAssetRelease Error  不存在这个资源 {0}的记录", record.AssetUrl);

                ResourcesManager.UnLoadResourceAsset(record as ResourcesLoadAssetRecord);
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
                        AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(record as ResourcesLoadAssetRecord);
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

        #region 同步& 异步 加载资源 (注意加载路径中不能带有扩展名否则加载失败)

        //Resources 同步加载资源
        public void LoadAssetSync(string assetpath, System.Action<ResourcesLoadAssetRecord> loadCallback)
        {
            ResourcesLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadResourceAssetInfor.IsLoadAssetEnable)
            {
                RecordResourcesLoadAsset(record.LoadResourceAssetInfor); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(record);
                return;
            }
            Object asset = Resources.Load<UnityEngine.Object>(assetpath.GetPathWithOutExtension());
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            ResourceLoadUnityAssetInfor assetInfor = new ResourceLoadUnityAssetInfor(assetpath, asset);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(record);
        }
        public ResourcesLoadAssetRecord LoadAssetSync(string assetpath)
        {
            ResourcesLoadAssetRecord record = LoadAssetFromCache(assetpath);
            if (record != null && record.LoadUnityObjectAssetInfor.IsLoadAssetEnable)
            {
                record = RecordResourcesLoadAsset(record.LoadResourceAssetInfor); //当 asset==null 时候记录返回 
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

            ResourceLoadUnityAssetInfor assetInfor = new ResourceLoadUnityAssetInfor(assetpath, asset);
            record = RecordResourcesLoadAsset(assetInfor); //当 asset==null 时候记录返回 
            return record;
        }


        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<ResourcesLoadAssetRecord> loadCallback, System.Action<string, float> procressCallback)
        {
            ResourcesLoadAssetRecord assetRecord = LoadAssetFromCache(assetpath);
            if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor.IsLoadAssetEnable)
            {
                assetRecord = RecordResourcesLoadAsset(assetRecord.LoadResourceAssetInfor); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }


            ResourceRequest request = Resources.LoadAsync(assetpath.GetPathWithOutExtension());
            if (procressCallback != null)
            {
                AsyncManager.S_Instance.StartAsyncOperation(request as AsyncOperation, (async) =>
                {
                    assetRecord = RecordResourcesLoadAsset(new ResourceLoadUnityAssetInfor(assetpath, (async as ResourceRequest).asset)); //当 asset==null 时候记录返回 
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
                    assetRecord = RecordResourcesLoadAsset(new ResourceLoadUnityAssetInfor(assetpath, (async as ResourceRequest).asset)); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(assetRecord);
                }, null);
            }//不考虑加载进度
        }
        #endregion

        #region 辅助记录

        /// <summary>
        /// / 记录加载的资源
        /// </summary>
        /// <param name="loadAssetInfor"></param>
        /// <returns></returns>
        public ResourcesLoadAssetRecord RecordResourcesLoadAsset(ResourceLoadUnityAssetInfor loadAssetInfor)
        {
            if (loadAssetInfor == null)
                return null;
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(loadAssetInfor.AssetUrl, out infor))
            {
                if (infor.LoadResourceAssetInfor.IsEqual(loadAssetInfor))
                {
                    return infor;
                }
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} Not Equal", loadAssetInfor.AssetUrl));
                return infor;
            }
            infor = AssetDelayDeleteManager.TryGetILoadAssetRecord(loadAssetInfor.AssetUrl) as ResourcesLoadAssetRecord;
            if (infor == null)
                infor = mResourcesLoadAssetRecordPoolMgr.GetItemFromPool();

            infor.Initial(loadAssetInfor.AssetUrl, LoadedAssetTypeEnum.Resources_UnKnown, loadAssetInfor, this);
            mAllLoadedAssetRecord[loadAssetInfor.AssetUrl] = infor;
            return infor;
        }

        /// <summary>
        /// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isforceDelete"></param>
        public void UnRecordResourcesLoadAsset(ResourceLoadUnityAssetInfor loadAssetInfor, bool isforceDelete)
        {
            if (loadAssetInfor == null || loadAssetInfor.IsLoadAssetEnable == false)
                return;

            string assetpath = string.Empty;
            foreach (var item in mAllLoadedAssetRecord)
            {
                if (item.Value.LoadResourceAssetInfor.IsEqual(loadAssetInfor))
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
