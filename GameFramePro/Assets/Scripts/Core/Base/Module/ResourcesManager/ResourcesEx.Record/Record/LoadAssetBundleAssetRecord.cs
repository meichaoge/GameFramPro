using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 加载的AssetBundle 资源/// </summary>
    public sealed class LoadAssetBundleAssetRecord : LoadAssetBaseRecord
    {
        public LoadAssetBundleInfor LoadAssetBundleInformation { get; protected set; }

        public override bool IsReferenceEnable
        {
            get { return LoadAssetBundleInformation != null; }
        }

        protected readonly Dictionary<int, LoadAssetBundleAssetRecord> mAllDependenceAssetBundleRecord = new Dictionary<int, LoadAssetBundleAssetRecord>(); //当前AssetBundle 依赖的的其他AssetBundle

        // 所有从这里加载的 AssetBundleSubAssetLoadRecord 资源记录 key=AssetBundleSubAssetLoadRecord  的 url
        protected HashSet<string> mAllBeReferenceAssetRecord = new HashSet<string>();

        #region 构造函数& 设置

        public LoadAssetBundleAssetRecord()
        {
        }

        public LoadAssetBundleAssetRecord(string assetUrl, LoadAssetBundleInfor assetBundle, IAssetManager manager)
        {
            Initial(assetUrl, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundle, manager);
        }

        public void Initial(string assetUrl, LoadedAssetTypeEnum typeEnum, LoadAssetBundleInfor assetBundle, IAssetManager manager)
        {
            base.Initial(assetUrl, typeEnum, manager);
            LoadAssetBundleInformation = assetBundle;
        }

        #endregion

        #region 管理对其他的AssetBundle 依赖

        public void AddDependence(LoadAssetBundleAssetRecord dependence)
        {
            if (dependence == null)
                return;

            if (mAllDependenceAssetBundleRecord.ContainsKey(dependence.InstanceID))
            {
                Debug.LogError("AddDependence Fail,Already Contain Key" + dependence.InstanceID);
                return;
            }

            mAllDependenceAssetBundleRecord[dependence.InstanceID] = dependence;
        }

        public void ClearAllDependence()
        {
            foreach (var dependence in mAllDependenceAssetBundleRecord)
            {
                dependence.Value?.ReduceReference();
            }

            mAllDependenceAssetBundleRecord.Clear();
        }

        public void ReduceDependence(LoadAssetBundleAssetRecord dependence)
        {
            if (mAllDependenceAssetBundleRecord.ContainsKey(dependence.InstanceID))
            {
                dependence.ReduceReference();
                mAllDependenceAssetBundleRecord.Remove(dependence.InstanceID);
                return;
            }

            Debug.LogError("ReduceDependence Fail,Not Contain Key" + dependence.InstanceID);
        }

        #endregion

        #region 加载的资源对当前AssetBundle 依赖

        public void AddSubAssetReference(LoadAssetBundleSubAssetRecord record)
        {
            if (mAllBeReferenceAssetRecord.Contains(record.AssetUrl) == false)
            {
                mAllBeReferenceAssetRecord.Add(record.AssetUrl);
                ++ReferenceCount;
                Debug.LogInfor("AddSubAssetReference Success!! 资源{0} 开始引用AssetBundle {1}", record.AssetUrl, AssetUrl);
            }
        }

        public void ReduceSubAssetReference(LoadAssetBundleSubAssetRecord record)
        {
            if (record.ReferenceCount == 0)
            {
                if (mAllBeReferenceAssetRecord.Contains(record.AssetUrl))
                {
                    mAllBeReferenceAssetRecord.Remove(record.AssetUrl);
                    --ReferenceCount;
                    Debug.LogInfor("ReduceSubAssetReference Success!! 资源{0} 不再引用AssetBundle {1}", record.AssetUrl, AssetUrl);
                }
            }

            if (ReferenceCount == 0)
            {
                ResourcesUtility.UnLoadAssetBundle(this, true);
                //  Debug.LogError("TODO  卸载AssetBundle " + AssetUrl);
            }
        }

        #endregion

        #region 基类重写

        public override void NotifyReleaseRecord()
        {
            base.NotifyReleaseRecord();
            LoadAssetBundleInformation.UnLoadAsAssetBundleAsset(true);
        }

        #endregion
    }
}
