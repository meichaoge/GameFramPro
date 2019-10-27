using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 从AssetBundle  中加载的资源的信息 
    /// </summary>
    [System.Serializable]
    public class LoadAssetBundleAssetRecord : ILoadAssetRecord
    {

        //无引用后默认存活60秒
        private const float sDefaultNoReferenceAliveTime = 60;
        public string mAssetFullUri { get; set; } // 完整的资源路径

        public LoadAssetSourceUsage mLoadAssetSourceUsage { get; protected set; } = LoadAssetSourceUsage.AssetBundleAsset;
        public bool IsRecordEnable { get { return mBundleAsset != null; } }

        public float mMaxAliveAfterNoReference { get; private set; } = sDefaultNoReferenceAliveTime;

        protected string mPackageAssetBundleUri { get; set; } //所属的包名
        protected string mBudleAssetRelativeUri { get; set; } // 相对于所属包的资源路径
        protected UnityEngine.Object mBundleAsset { get; set; } //加载的资源


        #region 对象池


        private static NativeObjectPool<LoadAssetBundleAssetRecord> s_LoadAssetBundleAssetRecordPoolMgr;
        private static void OnBeforeGeLoadAssetBundleAssetRecord(LoadAssetBundleAssetRecord record)
        {
            if (record == null) return;
            record.mAssetFullUri = record.mPackageAssetBundleUri = record.mBudleAssetRelativeUri = string.Empty;
            record.mLoadAssetSourceUsage = LoadAssetSourceUsage.None;
            record.mBundleAsset = null;
        }

        private static void OnBeforeRecycleLoadAssetBundleAssetRecord(LoadAssetBundleAssetRecord record)
        {
            if (record == null) return;
            record. mLoadAssetSourceUsage = LoadAssetSourceUsage.None;
            record. mBundleAsset = null;
        }

        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <returns></returns>
        public static LoadAssetBundleAssetRecord GetLoadAssetBundleAssetRecord()
        {
            return s_LoadAssetBundleAssetRecordPoolMgr.GetItemFromPool();
        }

        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static LoadAssetBundleAssetRecord GetLoadAssetBundleAssetRecord(string fullUri, string assetBundleUri, string assetRelativeUri, UnityEngine.Object asse, float noReferenceAliveTIme = sDefaultNoReferenceAliveTime)
        {
            var assetBundleAssetInfor = s_LoadAssetBundleAssetRecordPoolMgr.GetItemFromPool();
            assetBundleAssetInfor.mAssetFullUri = fullUri;
            assetBundleAssetInfor.mPackageAssetBundleUri = assetBundleUri;
            assetBundleAssetInfor.mBudleAssetRelativeUri = assetRelativeUri;
            assetBundleAssetInfor.mBundleAsset = asse;
            assetBundleAssetInfor.mMaxAliveAfterNoReference = noReferenceAliveTIme;

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


        #region 构造函数

        static LoadAssetBundleAssetRecord()
        {
            s_LoadAssetBundleAssetRecordPoolMgr = new NativeObjectPool<LoadAssetBundleAssetRecord>(50, OnBeforeGeLoadAssetBundleAssetRecord, OnBeforeRecycleLoadAssetBundleAssetRecord);
        }

        public LoadAssetBundleAssetRecord()
        {
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
            AssetBundleManager.S_Instance.RemoveLoadAssetBundleAssetRecord(this);
        }
        #endregion

    }
}