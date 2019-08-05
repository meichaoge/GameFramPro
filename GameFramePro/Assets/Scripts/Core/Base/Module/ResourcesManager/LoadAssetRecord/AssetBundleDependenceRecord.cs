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
    public class AssetBundleDependenceRecord : BaseLoadAssetRecord
    {
#if UNITY_EDITOR

        #region Show

        public List<AssetBundleDependenceRecord> Debug_AllDepdenceAssetBundleRecord = new List<AssetBundleDependenceRecord>();
        public List<string> Debug_mAllBeReferenceAssetRecord = new List<string>();

        public override void UpdateData()
        {
            base.UpdateData();
            Debug_AllDepdenceAssetBundleRecord.Clear();
            Debug_AllDepdenceAssetBundleRecord.AddRange(mAllDependenceAssetBundleRecord.Values);
            Debug_mAllBeReferenceAssetRecord.Clear();
            Debug_mAllBeReferenceAssetRecord.AddRange(mAllBeReferenceAssetRecord);
        }

        #endregion

#endif

        public BundleLoadUnityAssetInfor AssetBundleLoadBundleInfor
        {
            get { return LoadUnityObjectAssetInfor as BundleLoadUnityAssetInfor; }
        }

        protected readonly Dictionary<int, AssetBundleDependenceRecord> mAllDependenceAssetBundleRecord = new Dictionary<int, AssetBundleDependenceRecord>(); //当前AssetBundle 依赖的的其他AssetBundle

        // 所有从这里加载的 AssetBundleSubAssetLoadRecord 资源记录 key=AssetBundleSubAssetLoadRecord  的 url
        protected HashSet<string> mAllBeReferenceAssetRecord = new HashSet<string>();

        #region 构造函数& 设置

        public AssetBundleDependenceRecord()
        {
        }

        #endregion

        #region 管理对其他的AssetBundle 依赖

        public void AddDependence(AssetBundleDependenceRecord dependence)
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

        public void ReduceDependence(AssetBundleDependenceRecord dependence)
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
                ResourcesManager.UnLoadAssetBundle(this, true);
                //  Debug.LogError("TODO  卸载AssetBundle " + AssetUrl);
            }
        }

        #endregion


        public override void AddReference()
        {
            Debug.LogError("AssetBundleDependenceRecord 不需要实现这个接口 AddReference");
        }

        public override void ReduceReference(bool isforceDelete = false)
        {
            Debug.LogError("AssetBundleDependenceRecord 不需要实现这个接口 ReduceReference");
        }


        public override void NotifyReleaseRecord()
        {
            mAllDependenceAssetBundleRecord.Clear();
            base.NotifyReleaseRecord();
        }
    }
}
