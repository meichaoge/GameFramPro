using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 从Resources 加载的资源
    /// </summary>
    [System.Serializable]
    public class LoadResourcesAssetRecord : ILoadAssetRecord
    {
        //无引用后默认存活60秒
        private const float sDefaultNoReferenceAliveTime = 60;

        public string mAssetFullUri { get; set; } // 完整的资源路径

        public LoadAssetSourceUsage mLoadAssetSourceUsage { get; protected set; } = LoadAssetSourceUsage.ResourcesAsset;

        public bool IsRecordEnable
        {
            get { return mResourcesAsset != null; }
        }

        public float mMaxAliveAfterNoReference { get; private set; } = sDefaultNoReferenceAliveTime;

        protected UnityEngine.Object mResourcesAsset { get; set; } //加载的资源


        #region 构造函数

        static LoadResourcesAssetRecord()
        {
            s_LoadResourcesAssetRecordPoolMgr = new NativeObjectPool<LoadResourcesAssetRecord>(50, null, OnBeforeRecycleLoadResourcesAssetRecord);
        }

        public LoadResourcesAssetRecord()
        {
        }

        #endregion


        #region 对象池

        private static NativeObjectPool<LoadResourcesAssetRecord> s_LoadResourcesAssetRecordPoolMgr;

        //private static void OnBeforeGeLoadResourcesAssetRecord(LoadResourcesAssetRecord record)
        //{
        //}

        private static void OnBeforeRecycleLoadResourcesAssetRecord(LoadResourcesAssetRecord record)
        {
            if (record == null) return;
            record.mResourcesAsset = null;
        }

        /// <summary>
        /// 获取 LoadResourcesAssetRecord 实例对象
        /// </summary>
        /// <returns></returns>
        public static LoadResourcesAssetRecord GetLoadResourcesAssetRecord()
        {
            return s_LoadResourcesAssetRecordPoolMgr.GetItemFromPool();
        }

        /// <summary>
        /// 获取 LoadResourcesAssetRecord 实例对象
        /// </summary>
        /// <param name="fullUri"></param>
        /// <returns></returns>
        public static LoadResourcesAssetRecord GetLoadResourcesAssetRecord(string fullUri, UnityEngine.Object asse, float noReferenceAliveTIme = sDefaultNoReferenceAliveTime)
        {
            var assetBundleAssetInfor = s_LoadResourcesAssetRecordPoolMgr.GetItemFromPool();
            assetBundleAssetInfor.mAssetFullUri = fullUri;
            assetBundleAssetInfor.mResourcesAsset = asse;
            assetBundleAssetInfor.mMaxAliveAfterNoReference = noReferenceAliveTIme;

            return assetBundleAssetInfor;
        }

        /// <summary>
        /// 释放 LoadResourcesAssetRecord 对象
        /// </summary>
        /// <param name="resourcesAssetRecord"></param>
        public static void ReleaseAssetBundleRecordInfor(LoadResourcesAssetRecord resourcesAssetRecord)
        {
            s_LoadResourcesAssetRecordPoolMgr.RecycleItemToPool(resourcesAssetRecord);
        }

        #endregion



        #region ILoadAssetRecord 接口实现

        public Object GetLoadAsset()
        {
            return mResourcesAsset;
        }

        public int GetLoadAssetInstanceID()
        {
            return mResourcesAsset != null ? mResourcesAsset.GetInstanceID() : -1;
        }

        public void ReleaseLoadAssetRecord()
        {
            mResourcesAsset = null;
            ReleaseAssetBundleRecordInfor(this);
        }

        #endregion
    }
}