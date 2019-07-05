using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 负责整个项目的加载逻辑处理
    /// </summary>
    public partial class ResourcesManager : Single<ResourcesManager>
    {

        #region Data
        private Dictionary<string, Object> mAllLoadedAssetRecord = new Dictionary<string, Object>(); //Cache 所有加载的资源
        #endregion

        /// <summary>
        /// 从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public Object LoadResourcesFromCache(string assetpath)
        {
            Object asset = null;
            mAllLoadedAssetRecord.TryGetValue(assetpath, out asset);
            return asset;
        }

        #region  Resources 加载资源接口(同步&异步)
        //Resources 同步加载资源
        public void LoadResourcesAssetSync(string assetpath, System.Action<Object> loadCallback)
        {
            var asset = Resources.Load(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAsset Fail,AssetPath={0}  not exit", assetpath));

            RecordLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(asset);
        }

        //Resources 异步加载资源
        public void LoadResourcesAssetAsync(string assetpath, System.Action<Object> loadCallback, System.Action<string, float> procressCallback)
        {
            ResourceRequest request = Resources.LoadAsync(assetpath);

            if (procressCallback != null)
            {
                AsyncManager.S_Instance.StartAsyncOperation(request as AsyncOperation, (async) =>
                {
                    ResourceRequest result = async as ResourceRequest;
                    RecordLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
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
                    RecordLoadAsset(assetpath, result.asset); //当 asset==null 时候记录返回 
                    if (loadCallback != null)
                        loadCallback.Invoke(result.asset);
                }, null);
            }//不考虑加载进度


        }
        #endregion

        #region AssetBundle 加载资源接口 (同步&异步)
        //AssetBundle 同步加载
        public void LoadAssetBundleAssetSync(string assetpath, System.Action<Object> loadCallback)
        {

        }

        //AssetBundle 异步加载
        public void LoadAssetBundleAssetAsync(string assetpath, System.Action<Object> loadCallback)
        {

        }
        #endregion


        #region 资源处理
        /// <summary>
        /// 记录加载的资源
        /// </summary>
        /// <param name="assetpath"></param>
        /// <param name="asset"></param>
        public void RecordLoadAsset(string assetpath, Object asset)
        {
            if (asset == null)
                return;
            Object record = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out record))
            {
                if (object.ReferenceEquals(asset, record))
                {
                    Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1}", assetpath, asset));
                    return;
                }
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
                return;
            }
            mAllLoadedAssetRecord[assetpath] = asset;
        }

        public void UnRecordLoadAsset(string assetpath, Object asset)
        {
            if (asset == null)
                return;
            Object record = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out record))
            {
                if (object.ReferenceEquals(asset, record))
                {
                    mAllLoadedAssetRecord.Remove(assetpath);
                    return;
                }
                Debug.LogError(string.Format("UnRecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
                return;
            }

            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit Asset at path={0} of asset={1}", assetpath, asset));
        }

        public void UnRecordLoadAsset(string assetpath)
        {
            if (mAllLoadedAssetRecord.ContainsKey(assetpath))
            {
                mAllLoadedAssetRecord.Remove(assetpath);
                return;
            }
            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit Asset at path={0} ", assetpath));
        }

        public void UnRecordLoadAsset(Object asset)
        {
            if (asset == null)
                return;

            string assetpath = string.Empty;
            foreach (var item in mAllLoadedAssetRecord)
            {
                if (item.Value == asset)
                {
                    assetpath = item.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(assetpath) == false)
            {
                mAllLoadedAssetRecord.Remove(assetpath);
                return;
            }

            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", asset));
        }
        #endregion

    }
}