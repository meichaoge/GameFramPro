using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// AssetBundle 资源管理器
    /// </summary>
    public class AssetBundleManager : Single<AssetBundleManager>, IAssetManager
    {

        #region Date
        private Dictionary<string, AssetBundle> mAllLoadedAssetBundleAssets = new Dictionary<string, AssetBundle>(50); //所有加载的AssetBundle 资源
        private HashSet<int> mAllResoucesLoadAssetInstanceIds = new HashSet<int>();
        private NativeObjectPool<ResourcesLoadAssetRecord> mResourcesLoadAssetRecordPoolMgr;
        #endregion


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedAssetBundleLoad();
        }

        #region IAssetManager 接口实现

        private void InitialedAssetBundleLoad()
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
            //if (infor == null) return;
            //if (mAllLoadedAssetRecord.ContainsKey(infor.AssetPath))
            //{
            //    mAllLoadedAssetRecord.Remove(infor.AssetPath);
            //    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(infor);
            //}
            //else
            //{
            //    Debug.LogError("NotifyAssetNoReference Error !!" + infor.AssetPath);
            //}
        }

        public void MarkTargetAssetNull(ILoadAssetRecord record)
        {
            //if (record == null) return;
            //ResourcesLoadAssetRecord recordInfor = null;
            //string key = record.AssetPath;
            //if (mAllLoadedAssetRecord.TryGetValue(key, out recordInfor))
            //{
            //    mResourcesLoadAssetRecordPoolMgr.RecycleItemToPool(recordInfor);
            //    mAllLoadedAssetRecord.Remove(key);
            //}
        }
        #endregion


        #region 同步加载本地的 AssetBundle

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAsserBundleAssetSync(string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            AssetBundle assetBundle = GetAssetBundleByPath(assetBundlePath);
            if (assetBundle != null)
            {
                Object aset = assetBundle.LoadAsset(assetName);

                if (loadCallback != null)
                    loadCallback(aset);
                return;
            }
        }

        #endregion

        #region 辅助
        /// <summary>
        /// 根据路劲获取已经加载的 AssetBundle 可能得到空结果
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        private AssetBundle GetAssetBundleByPath(string assetBundlePath)
        {
            AssetBundle result = null;
            if(mAllLoadedAssetBundleAssets.TryGetValue(assetBundlePath,out result))
            {
                if (result != null)
                    return result;
                Debug.LogInfor(string.Format("GetAssetBundleByPath Fail，AssetBundle={0} 资源已经被卸载了", assetBundlePath));
                mAllLoadedAssetBundleAssets.Remove(assetBundlePath);
            }
            return result;
        }
        #endregion


    }
}