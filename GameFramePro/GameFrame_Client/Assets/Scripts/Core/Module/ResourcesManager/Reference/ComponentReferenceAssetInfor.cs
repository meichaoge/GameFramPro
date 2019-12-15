using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx.Reference
{
    [System.Serializable]
    /// <summary>
    /// 被组件引用的资源记录
    /// </summary>
    internal sealed class ComponentReferenceAssetInfor
    {
        /// <summary>
        /// 被引用的次数
        /// </summary>
        internal int mReferenceCount { get; private set; } = 0;

        private HashSet<Component> mComponeReferences { get; set; } = new HashSet<Component>(); //所有引用这个资源的组件

        internal ILoadAssetRecord mILoadAssetRecord { get; set; } //引用的资源记录

        /// <summary>
        /// 引用资源的实例id
        /// </summary>
        internal int mReferenceInstanceID { get { return mILoadAssetRecord == null ? -1 : mILoadAssetRecord.GetLoadAssetInstanceID(); } }

        internal string ReferenceAssetUri { get { return mILoadAssetRecord == null ? string.Empty : mILoadAssetRecord.mAssetFullUri; } }

        #region 构造函数

        static ComponentReferenceAssetInfor()
        {
            s_BeferenceAssetPoolMgr = new NativeObjectPool<ComponentReferenceAssetInfor>(50, null, OnBeforeRecycleBeferenceAsset);
        }

        public ComponentReferenceAssetInfor()
        {
        }

        #endregion

        #region 对象池

        private static NativeObjectPool<ComponentReferenceAssetInfor> s_BeferenceAssetPoolMgr;


        private static void OnBeforeRecycleBeferenceAsset(ComponentReferenceAssetInfor record)
        {
            record.mReferenceCount = 0;
            record.mComponeReferences?.Clear();
            record.mILoadAssetRecord?.ReleaseLoadAssetRecord();
            record.mILoadAssetRecord = null;
        }

        /// <summary>
        /// 获取 ComponentReferenceAssetInfor 实例对象
        /// </summary>
        /// <returns></returns>
        internal static ComponentReferenceAssetInfor GetComponentReferenceAssetInforInstance()
        {
            return s_BeferenceAssetPoolMgr.GetItemFromPool();
        }

        #endregion


        #region 引用计数处理

        /// <summary>
        /// 记录组件 component 依赖资源 asset
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        internal void AddReference(Component component, Object asset, ILoadAssetRecord loadAssetRecord)
        {
            if (mILoadAssetRecord == null)
                mILoadAssetRecord = loadAssetRecord;
            else
            {
                if (mILoadAssetRecord != loadAssetRecord)
                    Debug.LogError($"增加引用时候记录的值不一致{mILoadAssetRecord} ::{loadAssetRecord}");
                return;
            }

            if (mComponeReferences.Contains(component))
                return;
            if (mReferenceCount < 0)
                mReferenceCount = 0;
            mComponeReferences.Add(component);
            ++mReferenceCount;
            mILoadAssetRecord.MarkReferenceAssetRecord();
        }

        /// <summary>
        ///  去除组件 component 依赖资源 asset 的记录
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        internal void ReduceReference(Component component, Object asset)
        {
            if (mComponeReferences.Contains(component) == false)
                return;
            mComponeReferences.Remove(component);
            if (mReferenceCount <= 1)
            {
                mReferenceCount = 0;
                OnTriggerNoReference();
                return;
            }
            --mReferenceCount;
        }

        internal void ReduceAllReference(Transform targetTrans)
        {
            if (mComponeReferences == null || mComponeReferences.Count == 0)
                return;

            int count = mComponeReferences.RemoveWhere(item => item != null ? item.transform == targetTrans : true);
            mReferenceCount -= count;
            if (mReferenceCount <= 0)
            {
                mReferenceCount = 0;
                OnTriggerNoReference();
                return;
            }
        }

        #endregion


        #region 状态更新

        /// <summary>
        /// 更新引用计数 去除空引用
        /// </summary>
        internal void UpdateReferenceCount()
        {
            if (mComponeReferences == null || mComponeReferences.Count == 0)
                return;

            int count = mComponeReferences.RemoveWhere((component) => component == null || component.transform == null);
            mReferenceCount -= count;
            if (mReferenceCount <= 0)
            {
                mReferenceCount = 0;
                OnTriggerNoReference();
                return;
            }
        }

        /// <summary>
        /// 当资源没有引用时候触发
        /// </summary>
        private void OnTriggerNoReference()
        {
            if (mReferenceCount != 0) return;
            if (mILoadAssetRecord != null && mILoadAssetRecord.mMaxAliveAfterNoReference < 0)
            {
                mILoadAssetRecord.MarkReferenceAssetRecord();
            }
            else
            {
                ReferenceAssetManager.RemoveNoReferenceAssetRecord(this);
                mILoadAssetRecord.MarkNoReferenceAssetRecord(false);
                s_BeferenceAssetPoolMgr.RecycleItemToPool(this);
            }
        }

        #endregion
    }
}