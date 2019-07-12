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
        private Dictionary<string, AssetBundleLoadAssetRecord> mAllLoadedAssetBundleRecord = new Dictionary<string, AssetBundleLoadAssetRecord>(50); //所有加载的AssetBundle 资源
        private NativeObjectPool<AssetBundleLoadAssetRecord> mAssetBundleLoadAssetRecordPoolMgr;


        private Dictionary<string, AssetBundleSubAssetLoadRecord> mAllLoadedAssetBundleSubAssetRecord = new Dictionary<string, AssetBundleSubAssetLoadRecord>(50); //所有加载的AssetBundle 包里面的资源
        private NativeObjectPool<AssetBundleSubAssetLoadRecord> mAssetBundleLoadSubAssetRecordPoolMgr;

        private HashSet<int> mAllAssetBundleLoadAssetInstanceIds = new HashSet<int>();
        #endregion


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedAssetBundleLoad();
        }

        #region IAssetManager 接口实现

        private void InitialedAssetBundleLoad()
        {
            mAssetBundleLoadAssetRecordPoolMgr = new NativeObjectPool<AssetBundleLoadAssetRecord>(50, OnBeforGetAssetBundleLoadAssetRecord, OnBeforRecycleAsssetBundleLoadAssetRecord);
            mAssetBundleLoadSubAssetRecordPoolMgr = new NativeObjectPool<AssetBundleSubAssetLoadRecord>(50, OnBeforGetAssetBundleLoadSubAssetRecord, OnBeforRecycleAsssetBundleLoadSubAssetRecord);

        }

        private void OnBeforGetAssetBundleLoadAssetRecord(AssetBundleLoadAssetRecord record)
        {

        }

        private void OnBeforRecycleAsssetBundleLoadAssetRecord(AssetBundleLoadAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        private void OnBeforGetAssetBundleLoadSubAssetRecord(AssetBundleSubAssetLoadRecord record)
        {

        }

        private void OnBeforRecycleAsssetBundleLoadSubAssetRecord(AssetBundleSubAssetLoadRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        public float MaxAliveTimeAfterNoReference { get { return 600; } } //最多存在10分钟

        public void NotifyAssetNoReference(ILoadAssetRecord infor)
        {
            if (infor == null) return;
            if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(infor.AssetPath))
            {
                mAllLoadedAssetBundleSubAssetRecord.Remove(infor.AssetPath);
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
            AssetBundleSubAssetLoadRecord recordInfor = null;
            string key = record.AssetPath;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(key, out recordInfor))
            {
                mAssetBundleLoadSubAssetRecordPoolMgr.RecycleItemToPool(recordInfor);
                mAllLoadedAssetBundleSubAssetRecord.Remove(key);
            }
        }
        #endregion

        private Object LoadAssetFromCache(string assetName)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if(mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetName,out record))
            {
                if (record.TargetAsset != null)
                {
                    record.AddReference();
                    return record.TargetAsset;
                }
            }
            return null;
        }


        #region 同步加载本地的 AssetBundle


        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAsserBundleAssetSync(string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            Object assetObject = LoadAssetFromCache(assetName);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }

            AssetBundle assetBundle = GetAssetBundleByPath(assetBundlePath);
            if (assetBundle != null)
            {
                Object asset = assetBundle.LoadAsset(assetName);
                RecordAssetBundleLoadSubAsset(assetBundlePath, assetName, asset, assetBundle);
                if (loadCallback != null)
                    loadCallback(asset);
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
            AssetBundleLoadAssetRecord record = null;
            if (mAllLoadedAssetBundleRecord.TryGetValue(assetBundlePath, out record))
            {
                if (record != null && record.TargetAsset != null)
                {
                    record.AddReference();
                    return record.TargetAsset as AssetBundle;
                }
                Debug.LogInfor(string.Format("GetAssetBundleByPath Fail，AssetBundle={0} 资源已经被卸载了", assetBundlePath));
                mAllLoadedAssetBundleRecord.Remove(assetBundlePath);
            }
            return null;
        }

        private void RecordAssetBundleLoadSubAsset(string assetBundlePath, string assetName, Object asset, AssetBundle parentAssetBundle)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if(mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetName,out record))
            {
                record.AddReference();
                return;
            }

            record = mAssetBundleLoadSubAssetRecordPoolMgr.GetItemFromPool();
            record.Initial(assetBundlePath, assetName,LoadedAssetTypeEnum.AssetBundle_UnKnown,asset,this, assetBundlePath);

            mAllLoadedAssetBundleSubAssetRecord[assetName] = record;
        }


        #endregion


    }
}