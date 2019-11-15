using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx.Reference
{
    /// <summary>
    /// 被引用的资源的状态
    /// </summary>
    [System.Serializable]
    public enum ReferenceAssetStateUsage
    {
        None, //初始状态
        BeingReferenceed, //被使用中
        NoReference, //无引用中 等待被删除
        Releasing, //释放资源中
    }

    [System.Serializable]
    /// <summary>
    /// 被组件引用的资源记录
    /// </summary>
    public sealed class ComponentReferenceAssetInfor
    {
        /// <summary>
        /// 引用资源的实例id
        /// </summary>
        public int mReferenceInstanceID
        {
            get { return mILoadAssetRecord == null ? -1 : mILoadAssetRecord.GetLoadAssetInstanceID(); }
        }


        /// <summary>
        /// 被引用的次数
        /// </summary>
        public int mReferenceCount { get; private set; } = 0;

        /// <summary>
        /// 记录上一次准备释放资源的时间
        /// </summary>
        public float mLastRecordReleaseTime { get; private set; } = 0;


        /// <summary>
        /// 表示是否是在回收队列中等待最后的回收
        /// </summary>
        public ReferenceAssetStateUsage mReferenceAssetStateUsage { get; private set; } = ReferenceAssetStateUsage.None;

        private HashSet<Component> mComponeReferences { get; set; } = new HashSet<Component>(); //所有引用这个资源的组件

        public ILoadAssetRecord mILoadAssetRecord { get; set; } //引用的资源记录

        public string ReferenceAssetUri
        {
            get { return mILoadAssetRecord == null ? string.Empty : mILoadAssetRecord.mAssetFullUri; }
        }


        #region 对象池

        private static NativeObjectPool<ComponentReferenceAssetInfor> s_BeferenceAssetPoolMgr;

        private static void OnBeforeGeBeferenceAsset(ComponentReferenceAssetInfor record)
        {
            record.mReferenceCount = 0;
            record.mLastRecordReleaseTime = Time.realtimeSinceStartup;
            record.mReferenceAssetStateUsage = ReferenceAssetStateUsage.None;
            record.mComponeReferences?.Clear();
            record.mILoadAssetRecord?.ReleaseLoadAssetRecord();
            record.mILoadAssetRecord = null;
        }

        //private static void OnBeforeRecycleBeferenceAsset(ComponentReferenceAssetInfor record)
        //{
        //   
        //}

        /// <summary>
        /// 获取 ComponentReferenceAssetInfor 实例对象
        /// </summary>
        /// <returns></returns>
        public static ComponentReferenceAssetInfor GetBeferenceAsset()
        {
            return s_BeferenceAssetPoolMgr.GetItemFromPool();
        }

        //  /// <summary>
        //  /// 获取 ComponentReferenceAssetInfor 实例对象
        //  /// </summary>
        //  /// <param name="fullUri"></param>
        //  /// <returns></returns>
        //  public static ComponentReferenceAssetInfor GetBeferenceAsset(ILoadAssetRecord loadAssetRecord, Component component)
        //  {
        //      var assetBundleAssetInfor = s_BeferenceAssetPoolMgr.GetItemFromPool();
        //      assetBundleAssetInfor.mILoadAssetRecord = loadAssetRecord;
        // //     assetBundleAssetInfor.mComponeReferences.Add(component);
        ////      assetBundleAssetInfor.mReferenceCount = 0;

        //      return assetBundleAssetInfor;
        //  }

        #endregion


        #region 构造函数

        static ComponentReferenceAssetInfor()
        {
            s_BeferenceAssetPoolMgr = new NativeObjectPool<ComponentReferenceAssetInfor>(50, OnBeforeGeBeferenceAsset, null);
        }

        public ComponentReferenceAssetInfor()
        {
        }

        #endregion

        #region 引用计数处理

        /// <summary>
        /// 记录组件 component 依赖资源 asset
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        public void AddReference(Component component, Object asset, ILoadAssetRecord loadAssetRecord)
        {
            if (mILoadAssetRecord == null)
                mILoadAssetRecord = loadAssetRecord;
            else
                Debug.LogError($"增加引用时候记录的值不一致{mILoadAssetRecord} ::{loadAssetRecord}");

            if (mComponeReferences.Contains(component))
                return;
            if (mReferenceCount < 0)
                mReferenceCount = 0;
            mComponeReferences.Add(component);
            ++mReferenceCount;
            switch (mReferenceAssetStateUsage)
            {
                case ReferenceAssetStateUsage.None:
                case ReferenceAssetStateUsage.NoReference:
                    mReferenceAssetStateUsage = ReferenceAssetStateUsage.BeingReferenceed;
                    break;
                case ReferenceAssetStateUsage.BeingReferenceed:
                    break;
                case ReferenceAssetStateUsage.Releasing:
                    OnTriggerReReference();
                    break;
                default:
                    Debug.LogError($"没有处理的状态{mReferenceAssetStateUsage}");
                    break;
            }
        }

        /// <summary>
        ///  去除组件 component 依赖资源 asset 的记录
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        public void ReduceReference(Component component, Object asset)
        {
            if (mComponeReferences.Contains(component) == false)
                return;
            mComponeReferences.Remove(component);
            if (mReferenceCount == 1)
            {
                mReferenceCount = 0;
                OnTriggerNoReference();
                return;
            }

            --mReferenceCount;
            if (mReferenceCount < 0)
                mReferenceCount = 0;
        }

        public void ReduceAllReference(Transform targetTrans)
        {
            int count = mComponeReferences.RemoveWhere(item => item != null ? item.transform == targetTrans : true);

            mReferenceCount -= count;
            if (mReferenceCount < 0)
                mReferenceCount = 0;
            if (mReferenceCount == 0)
            {
                OnTriggerNoReference();
                return;
            }
        }

        #endregion


        #region 状态更新

        /// <summary>
        /// 更新引用计数 去除空引用
        /// </summary>
        public void UpdateReferenceCount()
        {
            int count = mComponeReferences.RemoveWhere((component) => component == null || component.transform == null);
            mReferenceCount -= count;
            if (mReferenceCount < 0)
                mReferenceCount = 0;
            if (mReferenceCount == 0)
            {
                OnTriggerNoReference();
                return;
            }
        }

        /// <summary>
        /// 检测是否可以真正删除释放资源
        /// </summary>
        /// <returns></returns>
        public bool CheckIfCanRealRelease()
        {
            if (mILoadAssetRecord == null)
                return true;  //关联的资源被释放了

            if (mILoadAssetRecord.mMaxAliveAfterNoReference < 0)
                return false;
            if (mReferenceAssetStateUsage != ReferenceAssetStateUsage.Releasing)
                return false;

            bool isReleaseAble = Time.realtimeSinceStartup - mLastRecordReleaseTime >= mILoadAssetRecord.mMaxAliveAfterNoReference;

#if UNITY_EDITOR
            if (isReleaseAble)
                Debug.LogEditorInfor($"可以释放资源 {ReferenceAssetUri}，Now={Time.realtimeSinceStartup} --Record={mLastRecordReleaseTime} >=Max{mILoadAssetRecord.mMaxAliveAfterNoReference}");
#endif

            return isReleaseAble;
        }


        /// <summary>
        /// 当资源没有引用时候触发
        /// </summary>
        private void OnTriggerNoReference()
        {
            if (mReferenceCount != 0) return;
            if (mILoadAssetRecord != null && mILoadAssetRecord.mMaxAliveAfterNoReference < 0)
                mReferenceAssetStateUsage = ReferenceAssetStateUsage.BeingReferenceed; //不会自动删除的对象
            else
                mReferenceAssetStateUsage = ReferenceAssetStateUsage.NoReference;
        }

        /// <summary>
        /// 当资源被重新引用使用
        /// </summary>
        private void OnTriggerReReference()
        {
            if (mReferenceCount == 0) return;
            if (mReferenceAssetStateUsage == ReferenceAssetStateUsage.Releasing)
            {
                AssetDelayDeleteManager.RemoveDelayDeleteReferenceAsset(this);
                mLastRecordReleaseTime = Time.realtimeSinceStartup;
                mReferenceAssetStateUsage = ReferenceAssetStateUsage.BeingReferenceed;
            }
        }


        /// <summary>
        /// 通知正在被回收中
        /// </summary>
        public void NotifyBeingReleasing()
        {
            mReferenceAssetStateUsage = ReferenceAssetStateUsage.Releasing;
            mLastRecordReleaseTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// 通知需要删除自身引用的资源
        /// </summary>
        public void NotifyBeDelete()
        {
            if (mReferenceAssetStateUsage == ReferenceAssetStateUsage.None && mILoadAssetRecord == null)
            {
                //     Debug.LogError($"没有被真正引用的资源");
                s_BeferenceAssetPoolMgr.RecycleItemToPool(this);
                return;
            }


            mReferenceCount = 0;
            mComponeReferences.Clear();
            mReferenceAssetStateUsage = ReferenceAssetStateUsage.None;

#if UNITY_EDITOR
            if(mILoadAssetRecord!=null)
            Debug.Log($"资源{ReferenceAssetUri} 释放资源");
#endif

            mILoadAssetRecord?.ReleaseLoadAssetRecord();
            mILoadAssetRecord = null;

            s_BeferenceAssetPoolMgr.RecycleItemToPool(this);
        }

        #endregion
    }
}