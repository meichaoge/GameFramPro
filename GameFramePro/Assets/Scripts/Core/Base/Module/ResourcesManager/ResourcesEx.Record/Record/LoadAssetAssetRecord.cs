using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;


namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 通过AssetBundle  或者Resources 加载的真实资源 (非AssetBundle 资源)/// </summary>
    [System.Serializable]
    public class LoadAssetAssetRecord : LoadAssetBaseRecord
    {
        /// <summary>/// 加载到的资源信息/// </summary>
        public LoadBasicAssetInfor LoadBasicObjectAssetInfor { get; protected set; }

        /// <summary>/// 所有引用这个加载记录中的资源/// </summary>
        protected List<BaseBeReferenceInformation> mAllBeReferenceInformations { get; set; } = new List<BaseBeReferenceInformation>(10);

        public override bool IsReferenceEnable
        {
            get { return LoadBasicObjectAssetInfor != null ? LoadBasicObjectAssetInfor.IsLoadAssetEnable : false; }
        }

        #region 构造函数

        public LoadAssetAssetRecord()
        {
        }

        public LoadAssetAssetRecord(string assetUrl, LoadedAssetTypeEnum typeEnum, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            Initial(assetUrl, typeEnum, assetInformation, manager);
        }


        public void Initial(string assetUrl, LoadedAssetTypeEnum typeEnum, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            base.Initial(assetUrl, typeEnum, manager);
            LoadBasicObjectAssetInfor = assetInformation;
            ReferenceCount = 0;
        }

        #endregion


        #region 基类重写和实现

//        /// <summary>/// 判断参数值指定的两个资源是否相同/// </summary>
//        public virtual bool IsReferenceEqual(LoadAssetAssetRecord record)
//        {
//            if (record == null) return false;
//            if (record.LoadBasicObjectAssetInfor.IsLoadAssetEnable == false) return false;
//            if (LoadBasicObjectAssetInfor.IsLoadAssetEnable == false) return false;
//            if (record.LoadBasicObjectAssetInfor.LoadAssetType != LoadBasicObjectAssetInfor.LoadAssetType) return false;
//
//            return record.LoadBasicObjectAssetInfor.AssetUrl == LoadBasicObjectAssetInfor.AssetUrl;
//        }


        public override void NotifyReleaseRecord()
        {
            LoadBasicObjectAssetInfor.ReleaseAsset();
            base.NotifyReleaseRecord();
            LoadBasicObjectAssetInfor = null;
        }

        #endregion

        #region 引用记录的中资源

        /// <summary>/// 通知有其他对象引用这条记录中的某个资源/// </summary>
        public void AddBeReferenceInformation(BaseBeReferenceInformation beReferenceInformation)
        {
            if (beReferenceInformation == null || beReferenceInformation.IsReferenceAssetEnable == false)
            {
                Debug.LogError("AddBeReferenceInformation Fail,参数为null ");
                return;
            }

            AddReference();

            for (int dIndex = 0; dIndex < mAllBeReferenceInformations.Count; dIndex++)
            {
                if (mAllBeReferenceInformations[dIndex].IsReferenceEqual(beReferenceInformation))
                    return;
            }

            Debug.LogEditorInfor($"新增引用资源{beReferenceInformation.AssetName}");
            mAllBeReferenceInformations.Add(beReferenceInformation);
        }

        /// <summary>/// 通知有其他对象减少了引用这条记录中的某个资源/// </summary>
        public void ReduceBeReferenceInformation(BaseBeReferenceInformation beReferenceInformation)
        {
            if (beReferenceInformation == null || beReferenceInformation.IsReferenceAssetEnable == false)
            {
                Debug.LogError("ReduceBeReferenceInformation Fail,参数为null ");
                return;
            }

            ReduceReference(false);
            for (int dex = mAllBeReferenceInformations.Count - 1; dex >= 0; dex--)
            {
                if (mAllBeReferenceInformations[dex].IsReferenceEqual(beReferenceInformation))
                {
                    if (beReferenceInformation.ReferenceCount == 0)
                        mAllBeReferenceInformations.RemoveAt(dex);
                    return;
                }
            }
        }

        #endregion
    }
}
