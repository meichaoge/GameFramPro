﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 从AssetBundle  中加载的资源的信息 
    /// </summary>
    [System.Serializable]
    internal sealed class LoadAssetBundleAssetRecord : ILoadAssetRecord
    {
        //无引用后默认存活60秒
        private const float sDefaultNoReferenceAliveTime = 60;
        public string mAssetFullUri { get; private set; } // 完整的资源路径

        public LoadAssetSourceUsage mLoadAssetSourceUsage { get; private set; } = LoadAssetSourceUsage.AssetBundleAsset;

        public bool IsRecordEnable
        {
            get { return mBundleAsset != null; }
        }

        public float mMaxAliveAfterNoReference { get; private set; } = sDefaultNoReferenceAliveTime;

        private string mBudleAssetRelativeUri { get; set; } // 相对于所属包的资源路径
        private UnityEngine.Object mBundleAsset { get; set; } //加载的资源

        private AssetBundleRecordInfor mDependenceAssetBundleRecord { get;  set; } //当前资源依赖的AssetBundle 记录

        #region 构造函数

        static LoadAssetBundleAssetRecord()
        {
            s_LoadAssetBundleAssetRecordPoolMgr = new NativeObjectPool<LoadAssetBundleAssetRecord>(50, null, OnBeforeRecycleLoadAssetBundleAssetRecord);
        }

        public LoadAssetBundleAssetRecord()
        {
        }

        #endregion

        #region 对象池

        private static NativeObjectPool<LoadAssetBundleAssetRecord> s_LoadAssetBundleAssetRecordPoolMgr;

        private static void OnBeforeRecycleLoadAssetBundleAssetRecord(LoadAssetBundleAssetRecord record)
        {
            if (record == null) return;

            record.mDependenceAssetBundleRecord?.NotifyRemoveBeReferenceByLoadAsset(record);
            record.mAssetFullUri = record.mBudleAssetRelativeUri = string.Empty;
            record.mLoadAssetSourceUsage = LoadAssetSourceUsage.None;
            record.mBundleAsset = null;
        }

 

        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static LoadAssetBundleAssetRecord GetLoadAssetBundleAssetRecord(string fullUri, AssetBundleRecordInfor referenceAssetBundle, string assetRelativeUri, Object asse, float noReferenceAliveTIme = sDefaultNoReferenceAliveTime)
        {
            var assetBundleAssetInfor = s_LoadAssetBundleAssetRecordPoolMgr.GetItemFromPool();
            assetBundleAssetInfor.mAssetFullUri = fullUri;
            assetBundleAssetInfor.mDependenceAssetBundleRecord = referenceAssetBundle;
            assetBundleAssetInfor.mBudleAssetRelativeUri = assetRelativeUri;
            assetBundleAssetInfor.mBundleAsset = asse;
            assetBundleAssetInfor.mMaxAliveAfterNoReference = noReferenceAliveTIme;

            referenceAssetBundle.NotifyAddBeReferenceByLoadAsset(assetBundleAssetInfor);
            return assetBundleAssetInfor;
        }

        /// <summary>
        /// 释放 AssetBundleRecordInfor 对象
        /// </summary>
        /// <param name="assetBundleRecordInfor"></param>
        public static void ReleaseAssetBundleRecordInfor(LoadAssetBundleAssetRecord assetBundleRecordInfor)
        {
            s_LoadAssetBundleAssetRecordPoolMgr.RecycleItemToPool(assetBundleRecordInfor);
        }

        #endregion


       

        #region ILoadAssetRecord 接口实现

        public Object GetLoadAsset()
        {
            return mBundleAsset;
        }

        public int GetLoadAssetInstanceID()
        {
            return mBundleAsset != null ? mBundleAsset.GetInstanceID() : -1;
        }

        public void ReleaseLoadAssetRecord()
        {
            s_LoadAssetBundleAssetRecordPoolMgr.RecycleItemToPool(this); //回收自身
        }

        #endregion
    }
}