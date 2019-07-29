using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class BaseLoadAssetRecord : ILoadAssetRecord
    {
#if UNITY_EDITOR
        #region Show
        public string Debug_AssetUrl = string.Empty;
        public int Debug_InstanceID = 0;
        public int Debug_ReferenceCount = 0;
        public LoadedAssetTypeEnum Debug_AssetLoadedType = LoadedAssetTypeEnum.None;
        public UnityEngine.Object Debug_TargetAsset = null;
        public float Debug_RemainTimeToBeDelete = 0;
        public long Debug_MarkToDeleteTime = 0;
        public IAssetManager Debug_BelongAssetManager = null;


        public virtual void UpdateData()
        {
            Debug_InstanceID = InstanceID;
            Debug_AssetUrl = AssetUrl;
            Debug_ReferenceCount = ReferenceCount;
            Debug_AssetLoadedType = AssetLoadedType;
            Debug_TargetAsset = TargetAsset;
            Debug_RemainTimeToBeDelete = RemainTimeToBeDelete;
            Debug_MarkToDeleteTime = MarkToDeleteTime;
            Debug_BelongAssetManager = BelongAssetManager;
        }

        #endregion
#endif

        public int InstanceID { get; protected set; } = 0;
        public string AssetUrl { get; protected set; } = string.Empty;
        public int ReferenceCount { get; protected set; } = 0;
        public LoadedAssetTypeEnum AssetLoadedType { get; protected set; } = LoadedAssetTypeEnum.None;
        public UnityEngine.Object TargetAsset { get; protected set; } = null;
        public float RemainTimeToBeDelete { get; protected set; } = 0;
        public long MarkToDeleteTime { get; protected set; } = 0;
        public IAssetManager BelongAssetManager { get; protected set; } = null;


        #region 构造函数& 设置

        public virtual void Initial(string assetPath, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Debug.Assert(manager != null);
            Debug.Assert(asset != null);

            AssetUrl = assetPath;
            InstanceID = asset.GetInstanceID();
            ReferenceCount = 0;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            TargetAsset = asset;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
        }

        #endregion



        public virtual void AddReference()
        {
            if (TargetAsset == null)
            {
                Debug.LogError("资源{0}  已经被卸载了, 无法增加引用", AssetUrl);
                ReferenceCount = 0;
            }
            else
            {
                ++ReferenceCount;
            }
            NotifyReferenceChange();
        }

        public virtual void ReduceReference(bool isforceDelete = false)
        {
            if (TargetAsset == null)
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
            if (TargetAsset == null)
            {
                ReferenceCount = 0;
                NotifyReferenceChange();
                return false;
            }

            RemainTimeToBeDelete = RemainTimeToBeDelete - tickTime;
            return RemainTimeToBeDelete > 0f;
        }


        public virtual void NotifyNoReference()
        {
            if (TargetAsset == null) return;
            GameObject go = TargetAsset as GameObject;
            if (go != null)
            {
                go.SetActive(false);
            }
        }

        public virtual bool NotifyReReference()
        {
            ReferenceCount = 1;
            return TargetAsset != null;
        }

        public virtual void NotifyReleaseRecord()
        {
            BelongAssetManager.NotifyAssetRelease(this);
            AssetUrl = null;
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            BelongAssetManager = null;
            TargetAsset = null;
        }

        public virtual void NotifyReferenceChange()
        {
            BelongAssetManager.NotifyAssetReferenceChange(this);
        }

    }
}