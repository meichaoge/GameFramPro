using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 加载Resources 时候的记录
    /// </summary>
    public class ResourcesLoadAssetRecord : ILoadAssetRecord
    {
        public int InstanceID { get; protected set; } = 0;
        public string AssetPath { get; protected set; } = string.Empty;
        public int ReferenceCount { get; protected set; } = 1;
        public LoadedAssetTypeEnum AssetLoadedType { get; protected set; } = LoadedAssetTypeEnum.None;
        public UnityEngine.Object TargetAsset { get; protected set; } = null;
        public float RemainTimeToBeDelete { get; protected set; } = 0;
        public long MarkToDeleteTime { get; protected set; } = 0;
        public IAssetManager BelongAssetManager { get; protected set; } = null;

        public string AssetName { get; protected set; } //资源名称

        #region 构造函数& 设置
        public ResourcesLoadAssetRecord()
        {

        }

        public ResourcesLoadAssetRecord(string assetPath, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
          : this(assetPath, System.IO.Path.GetFileNameWithoutExtension(assetPath), typeEnum, asset, manager)
        {

        }

        public ResourcesLoadAssetRecord(string assetPath, string assetName, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Initial(assetPath, assetName, typeEnum, asset, manager);
        }

        public void Initial(string assetPath, string assetName, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Debug.Assert(manager != null);
            Debug.Assert(asset != null);

            AssetPath = assetPath;
            AssetName = assetName;
            InstanceID = asset.GetInstanceID();
            ReferenceCount = 1;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            TargetAsset = asset;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
        }

        #endregion



        public void AddReference()
        {
            if (TargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0} Name={1} 已经被卸载了, 无法增加引用", AssetPath, AssetName);
                BelongAssetManager.MarkTargetAssetNull(this);
                return;
            }
            ++ReferenceCount;
        }

        public void ReduceReference(bool isforceDelete = false)
        {
            if (TargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0} Name={1} 已经被卸载了, 无法减少引用", AssetPath, AssetName);
                BelongAssetManager.MarkTargetAssetNull(this);
                return;
            }

            if (isforceDelete)
                ReferenceCount = 0;
            else
                --ReferenceCount;
            if (ReferenceCount == 0)
            {
                MarkToDeleteTime = DateTime.UtcNow.ToTimestamp_Millisecond();
                BelongAssetManager.NotifyAssetNoReference(this);
            }
        }

        public bool TimeTick(float tickTime)
        {
            if (TargetAsset == null)
            {
                BelongAssetManager.MarkTargetAssetNull(this);
                return false;
            }

            RemainTimeToBeDelete = RemainTimeToBeDelete - tickTime;
            return RemainTimeToBeDelete > 0f;
        }


        public void NotifyNoReference()
        {
            if (TargetAsset == null) return;
            GameObject go = TargetAsset as GameObject;
            if (go != null)
            {
                go.SetActive(false);
            }
        }

        public bool NotifyReReference()
        {
            ReferenceCount = 1;
            return TargetAsset != null;
        }

        public void NotifyReleaseRecord()
        {
            AssetPath = null;
            AssetName = null;
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            BelongAssetManager = null;
            TargetAsset = null;
        }


    }
}