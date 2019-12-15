using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 从Resources 加载的资源
    /// </summary>
    [System.Serializable]
    public sealed class LoadResourcesAssetRecord : ILoadAssetRecord
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
            s_LoadResourcesAssetRecordPoolMgr = new NativeObjectPool<LoadResourcesAssetRecord>(50, null, null);
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

        //private static void OnBeforeRecycleLoadResourcesAssetRecord(LoadResourcesAssetRecord record)
        //{
        //    if (record == null) return;
        //    record.mResourcesAsset = null;
        //    record. mAssetFullUri = string.Empty;
        //}

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
            assetBundleAssetInfor. mReferenceAssetStateUsage = ReferenceAssetStateUsage.BeingReferenceed;
            assetBundleAssetInfor. mTimerHashCode = 0;
            return assetBundleAssetInfor;
        }

        ///// <summary>
        ///// 释放 LoadResourcesAssetRecord 对象
        ///// </summary>
        ///// <param name="resourcesAssetRecord"></param>
        //private static void ReleaseAssetBundleRecordInfor(LoadResourcesAssetRecord resourcesAssetRecord)
        //{
        //    s_LoadResourcesAssetRecordPoolMgr.RecycleItemToPool(resourcesAssetRecord);
        //}

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
            mAssetFullUri = string.Empty;
            s_LoadResourcesAssetRecordPoolMgr.RecycleItemToPool(this);
          //  ReleaseAssetBundleRecordInfor(this);
        }

        #endregion

        #region 资源标记卸载和真正卸载


        /// <summary>
        /// 表示是否是在回收队列中等待最后的回收
        /// </summary>
        public ReferenceAssetStateUsage mReferenceAssetStateUsage { get; protected set; } = ReferenceAssetStateUsage.None;

        private float mMarkNoReferenceTime { get; set; } = 0;
        private int mTimerHashCode { get; set; } = 0;

        public void MarkNoReferenceAssetRecord(bool isForceMark)
        {
            if (isForceMark==false&& mMaxAliveAfterNoReference <= 0)
                return; //不释放的资源
            if(isForceMark&& mMaxAliveAfterNoReference <= 0)
            {
                RemveUnReferenceAsset();
                return;
            }//强制移除

            mMarkNoReferenceTime = Time.realtimeSinceStartup;
            mReferenceAssetStateUsage = ReferenceAssetStateUsage.NoReference;
            mTimerHashCode = TimeTickUtility.S_Instance.RegisterCountDownTimer(mMaxAliveAfterNoReference, 0, RegisterDeleteAssetRecordCallback);
        }

        private void RegisterDeleteAssetRecordCallback(float time,int timerHashcode)
        {
            if (mTimerHashCode != timerHashcode)
                return;
            if(mReferenceAssetStateUsage== ReferenceAssetStateUsage.NoReference)
            {
                RemveUnReferenceAsset();
                return;
            }
        }

        //移除超时没有被引用的资源
        private void RemveUnReferenceAsset()
        {
            LocalResourcesManager.RemoveLocalResourcesAssetRecord(this);
            if (mTimerHashCode != 0)
                TimeTickUtility.S_Instance.UnRegisterTimer_Delay(mTimerHashCode);
        }

        public void MarkReferenceAssetRecord()
        {
            mReferenceAssetStateUsage = ReferenceAssetStateUsage.BeingReferenceed;
            if(mTimerHashCode!=0)
            TimeTickUtility.S_Instance.UnRegisterTimer_Delay(mTimerHashCode);
        }
        #endregion
    }
}