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


        #region 加载缓存资源

        public Object LoadAssetFromCache(string assetpath)
        {
            return LoadAssetFromCache<Object>(assetpath);
        }

        /// <summary>
        ///  从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public T LoadAssetFromCache<T>(string assetpath) where T:UnityEngine.Object
        {
            ResourcesLoadAssetRecord infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (infor != null)
                    infor.AddReference();

                return infor.TargetAsset as T; //可能是null
            }
            return null;
        }

        #endregion

        #region 同步加载资源

        //Resources 同步加载资源
        public void ResourcesLoadAssetSync(string assetpath, System.Action<Object> loadCallback)
        {
            ResourcesLoadAssetSync<Object>(assetpath, loadCallback);
        }
        //Resources 同步加载资源
        public void ResourcesLoadAssetSync<T>(string assetpath, System.Action<T> loadCallback) where T : UnityEngine.Object
        {
            T asset = LoadAssetFromCache<T>(assetpath);
            if (asset != null)
            {
                RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }
            asset = Resources.Load<T>(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(asset);
        }

        public Object ResourcesLoadAssetSync(string assetpath)
        {
            return ResourcesLoadAssetSync<Object>(assetpath);
        }
        public T ResourcesLoadAssetSync<T>(string assetpath) where T : UnityEngine.Object
        {
            T asset = LoadAssetFromCache<T>(assetpath);
            if (asset != null)
            {
                RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
                return asset;
            }
            asset = Resources.Load<T>(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            RecordResourcesLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 
            return asset;
        }

        #endregion

        #region 异步加载资源

        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync(string assetpath, System.Action<Object> loadCallback, System.Action<string, float> procressCallback)
        {
            ResourcesLoadAssetAsync<Object>(assetpath, loadCallback, procressCallback);
        }

        //Resources 异步加载资源
        public void ResourcesLoadAssetAsync<T>(string assetpath, System.Action<T> loadCallback, System.Action<string, float> procressCallback) where T : UnityEngine.Object
        {
            T asset = LoadAssetFromCache<T>(assetpath);
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
                        loadCallback.Invoke(result.asset as T);
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
                        loadCallback.Invoke(result.asset as T);
                }, null);
            }//不考虑加载进度
        }
        #endregion

        #region 资源释放
        /// <summary>
        /// 较少某个加载资源的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns>如果成功则返回true</returns>
        public bool ReleaseReference<T>(T asset) where T : UnityEngine.Object
        {
            if (asset == null) return false;
            int instanceID = asset.GetInstanceID();

            string recordKey = string.Empty;
            if (mAllResoucesLoadAssetInstanceIds.TryGetValue(instanceID, out recordKey))
            {
                ResourcesLoadAssetRecord record = null;
                if(mAllLoadedAssetRecord.TryGetValue(recordKey,out record))
                {
                    record.ReduceReference();
                    return true;
                }
                Debug.LogError("数据记录不一致 " + recordKey);
                return false;
            }
            return false;
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
                    RecordAssetInstanceId(infor.InstanceID, assetpath);
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
            RecordAssetInstanceId(infor.InstanceID, assetpath);
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
