using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;


namespace GameFramePro
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


#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class BaseLoadAssetRecord
    {
#if UNITY_EDITOR

        #region Show

        public string Debug_AssetUrl = string.Empty;
        public int Debug_ReferenceCount = 0;
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
        
        public int ReferenceCount { get; protected set; } = 0;
        public LoadedAssetTypeEnum AssetLoadedType { get; protected set; } = LoadedAssetTypeEnum.None;

        /// <summary>/// 加载到的资源信息/// </summary>
        public BaseLoadUnityAssetInfor LoadUnityObjectAssetInfor { get; protected set; } 
        /// <summary>/// 标示当前记录是否有效/// </summary>
        public virtual bool IsReferenceEnable
        {
            get
            {
                return LoadUnityObjectAssetInfor != null ? LoadUnityObjectAssetInfor.IsLoadAssetEnable : false;
            }
        }
        /// <summary>/// 判断参数值指定的两个资源是否相同/// </summary>
        public virtual bool isReferenceEqual(BaseLoadAssetRecord record)
        {
            if (record == null) return false;
            if (record.LoadUnityObjectAssetInfor.IsLoadAssetEnable == false) return false;
            if (LoadUnityObjectAssetInfor.IsLoadAssetEnable == false) return false;
            if (record.LoadUnityObjectAssetInfor.LoadAssetType != LoadUnityObjectAssetInfor.LoadAssetType) return false;

            return record.LoadUnityObjectAssetInfor.AssetUrl == LoadUnityObjectAssetInfor.AssetUrl;
        }

        protected int InstanceID
        {
            get { return IsReferenceEnable ? LoadUnityObjectAssetInfor.InstanceID : 0; }
        }


        public float RemainTimeToBeDelete { get; protected set; } = 0;
        public long MarkToDeleteTime { get; protected set; } = 0;
        public IAssetManager BelongAssetManager { get; protected set; } = null;


        #region 构造函数& 设置

        public virtual void Initial(string assetPath, LoadedAssetTypeEnum typeEnum, BaseLoadUnityAssetInfor assetInfor, IAssetManager manager)
        {
            Debug.Assert(manager != null);

            AssetUrl = assetPath;
            ReferenceCount = 0;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            LoadUnityObjectAssetInfor = assetInfor;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
        }

        #endregion


     


        public virtual void AddReference()
        {
            if (LoadUnityObjectAssetInfor.IsLoadAssetEnable == false)
            {
                Debug.LogError("资源{0}  已经被卸载了, 无法增加引用", AssetUrl);
                ReferenceCount = 0;
            }
            else
                ++ReferenceCount;

            NotifyReferenceChange();
        }

        public virtual void ReduceReference(bool isforceDelete = false)
        {
            if (LoadUnityObjectAssetInfor.IsLoadAssetEnable == false)
            {
                Debug.LogError("资源{0}  已经被卸载了, 无法减少引用", AssetUrl);
                ReferenceCount = 0;
            }
            else
            {
                if (isforceDelete)
                    ReferenceCount = 0;
                else
                    --ReferenceCount;
                if (ReferenceCount == 0)
                    MarkToDeleteTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            }

            NotifyReferenceChange();
        }

        public virtual bool TimeTick(float tickTime)
        {
            if (LoadUnityObjectAssetInfor.IsLoadAssetEnable == false)
            {
                ReferenceCount = 0;
                NotifyReferenceChange();
                return false;
            }

            RemainTimeToBeDelete = RemainTimeToBeDelete - tickTime;
            return RemainTimeToBeDelete > 0f;
        }

        public virtual bool NotifyReReference()
        {
            ReferenceCount = 1;
            return LoadUnityObjectAssetInfor.IsLoadAssetEnable;
        }

        public virtual void NotifyReleaseRecord()
        {
            BelongAssetManager.NotifyAssetRelease(this);
            AssetUrl = null;
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            BelongAssetManager = null;
            LoadUnityObjectAssetInfor.ReleaseAsset();
        }

        protected virtual void NotifyReferenceChange()
        {
            BelongAssetManager.NotifyAssetReferenceChange(this);
        }
    }
}
