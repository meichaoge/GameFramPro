using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 加载的AssetBundle 包资源
    /// </summary>
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class AssetBundleAssetDepdenceRecord : ILoadAssetRecord
    {
#if UNITY_EDITOR
        #region Show
        public int Debug_InstanceID = 0;
        public string Debug_AssetPath = string.Empty;
        public int Debug_ReferenceCount = 1;
        public LoadedAssetTypeEnum Debug_AssetLoadedType = LoadedAssetTypeEnum.None;
        public UnityEngine.Object Debug_TargetAsset = null;
        public float Debug_RemainTimeToBeDelete = 0;
        public long Debug_MarkToDeleteTime = 0;
        public IAssetManager Debug_BelongAssetManager = null;
        public List<AssetBundleAssetDepdenceRecord> Debug_AllDepdenceAssetBundleRecord = new List<AssetBundleAssetDepdenceRecord>();

        public void UpdateData() 
        {
            Debug_InstanceID = InstanceID;
            Debug_AssetPath = AssetPath;
            Debug_ReferenceCount = ReferenceCount;
            Debug_AssetLoadedType = AssetLoadedType;
            Debug_TargetAsset = TargetAsset;
            Debug_RemainTimeToBeDelete = RemainTimeToBeDelete;
            Debug_MarkToDeleteTime = MarkToDeleteTime;
            Debug_BelongAssetManager = BelongAssetManager;
            Debug_AllDepdenceAssetBundleRecord.Clear();
            Debug_AllDepdenceAssetBundleRecord.AddRange(mAllDepdenceAssetBundleRecord.Values);
        }

        #endregion
#endif



        public int InstanceID { get; protected set; } = 0;
        public string AssetPath { get; protected set; } = string.Empty;
        public int ReferenceCount { get; protected set; } = 1;
        public LoadedAssetTypeEnum AssetLoadedType { get; protected set; } = LoadedAssetTypeEnum.None;
        public UnityEngine.Object TargetAsset { get; protected set; } = null;
        public float RemainTimeToBeDelete { get; protected set; } = 0;
        public long MarkToDeleteTime { get; protected set; } = 0;
        public IAssetManager BelongAssetManager { get; protected set; } = null;

        protected Dictionary<int, AssetBundleAssetDepdenceRecord> mAllDepdenceAssetBundleRecord =new Dictionary<int, AssetBundleAssetDepdenceRecord>();//当前AssetBundle 依赖的的其他AssetBundle


        #region 构造函数& 设置
        public AssetBundleAssetDepdenceRecord()
        {

        }

        public AssetBundleAssetDepdenceRecord(string assetPath, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Initial(assetPath, typeEnum, asset, manager);
        }

        public void Initial(string assetPath, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Debug.Assert(manager != null);
            Debug.Assert(asset != null);

            AssetPath = assetPath;
            InstanceID = asset.GetInstanceID();
            ReferenceCount = 1;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            TargetAsset = asset;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
         
        }

        #endregion


        public void AddDepdence(AssetBundleAssetDepdenceRecord depdence)
        {
            if (depdence == null)
                return;

            if (mAllDepdenceAssetBundleRecord.ContainsKey(depdence.InstanceID))
            {
                Debug.LogError("AddDepdence Fail,Already Contain Key"+ depdence.InstanceID);
                return;
            }
            mAllDepdenceAssetBundleRecord[depdence.InstanceID] = depdence;
            depdence.AddReference();
        }


        public void ClearAllDepdence()
        {
            foreach (var depdence in mAllDepdenceAssetBundleRecord)
            {
                if (depdence.Value != null)
                    depdence.Value.ReduceReference();
            }
            mAllDepdenceAssetBundleRecord.Clear();
        }
        public void ReduceDepdence(AssetBundleAssetDepdenceRecord depdence)
        {
            if (mAllDepdenceAssetBundleRecord.ContainsKey(depdence.InstanceID))
            {
                depdence.ReduceReference();
                mAllDepdenceAssetBundleRecord.Remove(depdence.InstanceID);          
                return;
            }
            Debug.LogError("ReduceDepdence Fail,Not Contain Key" + depdence.InstanceID);
        }
        public void AddReference()
        {
            if (TargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0} 已经被卸载了, 无法增加引用", AssetPath);
                BelongAssetManager.MarkTargetAssetNull(this);
                return;
            }
            ++ReferenceCount;
        }

        public void ReduceReference(bool isforceDelete = false)
        {
            if (TargetAsset == null)
            {
                Debug.LogErrorFormat("资源{0}  已经被卸载了, 无法减少引用", AssetPath);
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
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            mAllDepdenceAssetBundleRecord.Clear();
            BelongAssetManager = null;
            TargetAsset = null;
        }

    }
}