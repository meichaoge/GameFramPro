using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 资源加载时候的类型状态(Resources&LocalStore&AssetBundle) 后面可能对不同的类型进行处理/// </summary>
    public enum LoadedAssetTypeEnum
    {
        None = 0, //未知的类型

        // ReSharper disable once InconsistentNaming
        Resources_UnKnown = 1, //记录时候无法判断的类型

        ////***本地存储的 可能是运行时记录的资源&网络下载资源&缓存资源
        //LocalStore_UnKnown = 100, //记录时候无法判断的类型

        // ReSharper disable once InconsistentNaming
        AssetBundle_UnKnown = 200, //记录时候无法判断的类型
    }


    /// <summary>/// 资源加载时候的类型状态(Resources&LocalStore&AssetBundle) 后面可能对不同的类型进行处理/// </summary>
    [System.Serializable]
    public class LoadAssetBaseRecord : IReference
    {
#if UNITY_EDITOR

        #region Show

        public string Debug_AssetUrl = string.Empty;
        public int Debug_ReferenceCount;
        public LoadedAssetTypeEnum Debug_AssetLoadedType = LoadedAssetTypeEnum.None;
        public float Debug_RemainTimeToBeDelete = 0;
        public long Debug_MarkToDeleteTime = 0;
        public IAssetManager Debug_BelongAssetManager = null;

        public virtual void UpdateData()
        {
            Debug_AssetUrl = AssetUrl;
            Debug_ReferenceCount = ReferenceCount;
            Debug_AssetLoadedType = AssetLoadedType;
            Debug_RemainTimeToBeDelete = RemainTimeToBeDelete;
            Debug_MarkToDeleteTime = MarkToDeleteTime;
            Debug_BelongAssetManager = BelongAssetManager;
        }

        #endregion

#endif

        public string AssetUrl { get; protected set; } = string.Empty;

        public LoadedAssetTypeEnum AssetLoadedType { get; protected set; } = LoadedAssetTypeEnum.None;


        /// <summary>/// 标示当前记录是否有效/// </summary>
        public virtual bool IsReferenceEnable
        {
            get { return true; }
        }


        protected virtual int InstanceID
        {
            get { return 0; }
        }


        public float RemainTimeToBeDelete { get; protected set; } = 0;
        public long MarkToDeleteTime { get; protected set; } = 0;
        public IAssetManager BelongAssetManager { get; protected set; } = null;


        #region 构造函数& 设置

        protected virtual void Initial(string assetUrl, LoadedAssetTypeEnum typeEnum, IAssetManager manager)
        {
            Debug.Assert(manager != null);

            AssetUrl = assetUrl;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
        }

        #endregion


        #region IReference 接口

        public int ReferenceCount { get; protected set; } = 0;

        public virtual void AddReference()
        {
            if (IsReferenceEnable == false)
            {
                Debug.LogError("资源{0}  已经被卸载了, 无法增加引用", AssetUrl);
                ReferenceCount = 0;
                NotifyReferenceChange(false);
            }
            else
            {
                ++ReferenceCount;
                NotifyReferenceChange(true);
            }
        }

        public virtual void ReduceReference()
        {
            if (IsReferenceEnable == false)
            {
                Debug.LogError("资源{0}  已经被卸载了, 无法减少引用", AssetUrl);
                ReferenceCount = 0;
            }
            else
            {
                --ReferenceCount;
                if (ReferenceCount == 0)
                    MarkToDeleteTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            }

            NotifyReferenceChange(false);
        }

        #endregion


        public virtual bool TimeTick(float tickTime)
        {
            if (IsReferenceEnable == false)
            {
                ReferenceCount = 0;
                NotifyReferenceChange(false);
                return false;
            }

            if (ReferenceCount <= 0)
            {
                RemainTimeToBeDelete = RemainTimeToBeDelete - tickTime;
                return RemainTimeToBeDelete > 0f;
            }
            else
            {
                RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
                return true;
            }
        }


        public virtual void NotifyReleaseRecord()
        {
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            BelongAssetManager.NotifyAssetRelease(this);
            BelongAssetManager = null;
        }

        protected virtual void NotifyReferenceChange(bool isAddReference)
        {
            BelongAssetManager.NotifyAssetReferenceChange(this, isAddReference);
        }
    }
}
