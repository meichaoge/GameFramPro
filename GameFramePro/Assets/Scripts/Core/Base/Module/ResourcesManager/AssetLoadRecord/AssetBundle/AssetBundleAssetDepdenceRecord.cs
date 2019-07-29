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
    public class AssetBundleAssetDepdenceRecord : BaseLoadAssetRecord
    {
#if UNITY_EDITOR

        public List<AssetBundleAssetDepdenceRecord> Debug_AllDepdenceAssetBundleRecord = new List<AssetBundleAssetDepdenceRecord>();
        public List<string> Debug_mAllBeReferenceAssetRecord = new List<string>();

        public override void UpdateData()
        {
            base.UpdateData();
            Debug_AllDepdenceAssetBundleRecord.Clear();
            Debug_AllDepdenceAssetBundleRecord.AddRange(mAllDepdenceAssetBundleRecord.Values);
            Debug_mAllBeReferenceAssetRecord.Clear();
            Debug_mAllBeReferenceAssetRecord.AddRange(mAllBeReferenceAssetRecord);
        }

#endif

        public BundleLoadUnityAssetInfor AssetBundleLoadBundleInfor { get { return LoadUnityObjectAssetInfor as BundleLoadUnityAssetInfor; } }
        protected Dictionary<int, AssetBundleAssetDepdenceRecord> mAllDepdenceAssetBundleRecord =new Dictionary<int, AssetBundleAssetDepdenceRecord>();//当前AssetBundle 依赖的的其他AssetBundle

            // 所有从这里加载的 AssetBundleSubAssetLoadRecord 资源记录 key=AssetBundleSubAssetLoadRecord  的 url
        protected HashSet<string> mAllBeReferenceAssetRecord = new HashSet<string>();

        #region 构造函数& 设置
        public AssetBundleAssetDepdenceRecord() { }

        //public AssetBundleAssetDepdenceRecord(string assetPath, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        //{
        //    Initial(assetPath, typeEnum, asset, manager);
        //}

        #endregion

        #region 管理对其他的AssetBundle 依赖

        public void AddDepdence(AssetBundleAssetDepdenceRecord depdence)
        {
            //if (depdence == null)
            //    return;

            //if (mAllDepdenceAssetBundleRecord.ContainsKey(depdence.InstanceID))
            //{
            //    Debug.LogError("AddDepdence Fail,Already Contain Key" + depdence.InstanceID);
            //    return;
            //}
            //mAllDepdenceAssetBundleRecord[depdence.InstanceID] = depdence;
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
            //if (mAllDepdenceAssetBundleRecord.ContainsKey(depdence.InstanceID))
            //{
            //    depdence.ReduceReference();
            //    mAllDepdenceAssetBundleRecord.Remove(depdence.InstanceID);          
            //    return;
            //}
            //Debug.LogError("ReduceDepdence Fail,Not Contain Key" + depdence.InstanceID);
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
            {
                ResourcesManager.UnLoadAssetBundle(this, true);
              //  Debug.LogError("TODO  卸载AssetBundle " + AssetUrl);
            }
        }

        #endregion


        public override void AddReference()
        {
            Debug.LogError("AssetBundleAssetDepdenceRecord 不需要实现这个接口 AddReference");
        }

        public override void ReduceReference(bool isforceDelete = false)
        {
            Debug.LogError("AssetBundleAssetDepdenceRecord 不需要实现这个接口 ReduceReference");
        }


        public override void NotifyReleaseRecord()
        {
            mAllDepdenceAssetBundleRecord.Clear();
            base.NotifyReleaseRecord();
        }

    }
}