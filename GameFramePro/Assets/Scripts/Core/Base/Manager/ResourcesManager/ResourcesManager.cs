using GameFramePro.ResourcesEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 负责整个项目的加载逻辑处理
    /// </summary>
    public partial class ResourcesManager : Single<ResourcesManager>, IAssetManager
    {

        #region Data
        //key  =assetpath 
        private Dictionary<string, BaseLoadedAssetInfor> mAllLoadedAssetRecord = new Dictionary<string, BaseLoadedAssetInfor>(200); //Cache 所有加载的资源
        #endregion

        #region IAssetManager 接口实现

        public int MaxAliveTimeAfterNoReference { get { return 600; } } //最多存在10分钟

        public void NotifyAssetNoReference(BaseLoadedAssetInfor infor)
        {
            throw new System.NotImplementedException();
        }

        public void NotifyAssetForceDelete(BaseLoadedAssetInfor infor)
        {
            if (infor == null) return;
            if (mAllLoadedAssetRecord.ContainsKey(infor.mAssetPath))
                mAllLoadedAssetRecord.Remove(infor.mAssetPath);
        }

        #endregion


        /// <summary>
        /// 从缓存中加载一个资源 可能返回null(标示没有加载过这个资源，或者这个资源已经被释放了)
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public Object LoadResourcesFromCache(string assetpath)
        {
            BaseLoadedAssetInfor infor = null;
            if(mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (infor != null)
                    infor.AddReference();

                if (infor.mTargetAsset != null)
                    return infor.mTargetAsset;
            }
            return null;
        }

        #region  Resources 加载资源接口(同步&异步)
        //Resources 同步加载资源
        public void LoadResourcesAssetSync(string assetpath, System.Action<Object> loadCallback)
        {
            Object asset = LoadResourcesFromCache(assetpath);  //优先加载缓存中的
            if (asset != null)
            {
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }

            asset = Resources.Load(assetpath);
            if (asset == null)
                Debug.LogError(string.Format("LoadResourcesAssetSync Fail,AssetPath={0}  not exit", assetpath));

            RecordLoadAsset(assetpath, asset); //当 asset==null 时候记录返回 

            if (loadCallback != null)
                loadCallback(asset);
        }

        //Resources 异步加载资源
        public void LoadResourcesAssetAsync(string assetpath, System.Action<Object> loadCallback, System.Action<string, float> procressCallback)
        {
            Object asset = LoadResourcesFromCache(assetpath);  //优先加载缓存中的
            if (asset != null)
            {
                if (procressCallback != null)
                    procressCallback(assetpath,1f);

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
            BaseLoadedAssetInfor infor = null;
            if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
            {
                if (object.ReferenceEquals(asset, infor.mTargetAsset))
                {
                    infor.AddReference(); //增加引用次数
                //    Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1}", assetpath, asset));
                    return;
                }
                Debug.LogError(string.Format("RecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
                return;
            }
            infor = new BaseLoadedAssetInfor(assetpath, LoadedAssetTypeEnum.Resources_UnKnown, asset,this);
            mAllLoadedAssetRecord[assetpath] = infor;
        }

        //public void UnRecordLoadAsset(string assetpath, Object asset)
        //{
        //    if (asset == null)
        //        return;
        //    BaseLoadedAssetInfor infor = null;
        //    if (mAllLoadedAssetRecord.TryGetValue(assetpath, out infor))
        //    {
        //        if (object.ReferenceEquals(asset, infor.mTargetAsset))
        //        {
        //            mAllLoadedAssetRecord.Remove(assetpath);
        //            return;
        //        }
        //        Debug.LogError(string.Format("UnRecordLoadAsset Fail,Already Exit Asset at path={0} of asset={1} Not Equal", assetpath, asset));
        //        return;
        //    }

        //    Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit Asset at path={0} of asset={1}", assetpath, asset));
        //}

        //public void UnRecordLoadAsset(string assetpath)
        //{
        //    if (mAllLoadedAssetRecord.ContainsKey(assetpath))
        //    {
        //        mAllLoadedAssetRecord.Remove(assetpath);
        //        return;
        //    }
        //    Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit Asset at path={0} ", assetpath));
        //}

        /// <summary>
        /// 减少对象的引用 如果 isforceDelete =true,则会强制删除这个对象
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="isforceDelete"></param>
        public void UnRecordLoadAsset(Object asset,bool isforceDelete)
        {
            if (asset == null)
                return;

            string assetpath = string.Empty;
            foreach (var item in mAllLoadedAssetRecord)
            {
                if (item.Value.mTargetAsset == asset)
                {
                    assetpath = item.Key;
                    break;
                }
            }

            if (string.IsNullOrEmpty(assetpath) == false)
            {
                var loadAssetInfor = mAllLoadedAssetRecord[assetpath];
                loadAssetInfor.ReduceReference(isforceDelete);
                return;
            }

            Debug.LogError(string.Format("UnRecordLoadAsset Fail,Not Exit asset={0}", asset));
        }


        #endregion

    }
}