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
        public string Debug_AssetUrl = string.Empty;
        public int Debug_ReferenceCount = 1;
        public LoadedAssetTypeEnum Debug_AssetLoadedType = LoadedAssetTypeEnum.None;
        public UnityEngine.Object Debug_TargetAsset = null;
        public float Debug_RemainTimeToBeDelete = 0;
        public long Debug_MarkToDeleteTime = 0;
        public IAssetManager Debug_BelongAssetManager = null;
        public List<AssetBundleAssetDepdenceRecord> Debug_AllDepdenceAssetBundleRecord = new List<AssetBundleAssetDepdenceRecord>();
        public List<string> Debug_mAllBeReferenceAssetRecord = new List<string>();

        public void UpdateData() 
        {
            Debug_InstanceID = InstanceID;
            Debug_AssetUrl = AssetUrl;
            Debug_ReferenceCount = ReferenceCount;
            Debug_AssetLoadedType = AssetLoadedType;
            Debug_TargetAsset = TargetAsset;
            Debug_RemainTimeToBeDelete = RemainTimeToBeDelete;
            Debug_MarkToDeleteTime = MarkToDeleteTime;
            Debug_BelongAssetManager = BelongAssetManager;
            Debug_AllDepdenceAssetBundleRecord.Clear();
            Debug_AllDepdenceAssetBundleRecord.AddRange(mAllDepdenceAssetBundleRecord.Values);
            Debug_mAllBeReferenceAssetRecord.Clear();
            Debug_mAllBeReferenceAssetRecord.AddRange(mAllBeReferenceAssetRecord);
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

        protected Dictionary<int, AssetBundleAssetDepdenceRecord> mAllDepdenceAssetBundleRecord =new Dictionary<int, AssetBundleAssetDepdenceRecord>();//当前AssetBundle 依赖的的其他AssetBundle

            // 所有从这里加载的 AssetBundleSubAssetLoadRecord 资源记录 key=AssetBundleSubAssetLoadRecord  的 url
        protected HashSet<string> mAllBeReferenceAssetRecord = new HashSet<string>();

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

            AssetUrl = assetPath;
            InstanceID = asset.GetInstanceID();
            ReferenceCount = 0;
            AssetLoadedType = typeEnum;
            BelongAssetManager = manager;
            TargetAsset = asset;
            RemainTimeToBeDelete = BelongAssetManager.MaxAliveTimeAfterNoReference;
         
        }

        #endregion

        #region 管理对其他的AssetBundle 依赖

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

        #endregion

        #region 加载的资源对当前AssetBundle 依赖
        public void AddSubAssetReference(AssetBundleSubAssetLoadRecord record)
        {
            if (mAllBeReferenceAssetRecord.Contains(record.AssetUrl) == false)
            {
                mAllBeReferenceAssetRecord.Add(record.AssetUrl);
                ++ReferenceCount;
                Debug.LogInfor("AddSubAssetReference Success!! 资源{0} 开始引用AssetBundle {1}", record.AssetUrl, AssetUrl);
            }
        }

        public void ReduceSubAssetReference(AssetBundleSubAssetLoadRecord record)
        {
            if (record.ReferenceCount == 0 )
            {
                if (mAllBeReferenceAssetRecord.Contains(record.AssetUrl))
                {
                    mAllBeReferenceAssetRecord.Remove(record.AssetUrl);
                    --ReferenceCount;
                    Debug.LogInfor("ReduceSubAssetReference Success!! 资源{0} 不再引用AssetBundle {1}", record.AssetUrl, AssetUrl);
                }
            }

            if (ReferenceCount == 0)
                Debug.LogError("TODO  卸载AssetBundle " + AssetUrl);
        }

        #endregion


        public void AddReference()
        {
            Debug.LogError("AssetBundleAssetDepdenceRecord 不需要实现这个接口 AddReference");

            //if (TargetAsset == null)
            //{
            //    Debug.LogErrorFormat("资源{0} 已经被卸载了, 无法增加引用", AssetUrl);
            //    ReferenceCount = 0;
            //}
            //else
            //{
            //    ++ReferenceCount;
            //}
            //NotifyReferenceChange(this, BelongAssetManager);
        }

        public void ReduceReference(bool isforceDelete = false)
        {
            Debug.LogError("AssetBundleAssetDepdenceRecord 不需要实现这个接口 ReduceReference");
            //if (TargetAsset == null)
            //{
            //    Debug.LogErrorFormat("资源{0}  已经被卸载了, 无法减少引用", AssetUrl);
            //    return;
            //}
            //else
            //{
            //    if (isforceDelete)
            //        ReferenceCount = 0;
            //    else
            //        --ReferenceCount;
            //    if (ReferenceCount == 0)
            //        MarkToDeleteTime = DateTime.UtcNow.ToTimestamp_Millisecond();
            //}
            //NotifyReferenceChange(this, BelongAssetManager);
        }

        public bool TimeTick(float tickTime)
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
            BelongAssetManager.NotifyAssetRelease(this);
            AssetUrl = null;
            ReferenceCount = 0;
            AssetLoadedType = LoadedAssetTypeEnum.None;
            mAllDepdenceAssetBundleRecord.Clear();
            BelongAssetManager = null;
            TargetAsset = null;
        }

        public void NotifyReferenceChange()
        {
            BelongAssetManager.NotifyAssetReferenceChange(this);
        }

    }
}