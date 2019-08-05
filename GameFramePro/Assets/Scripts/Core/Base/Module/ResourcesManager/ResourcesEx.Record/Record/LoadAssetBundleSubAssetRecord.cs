using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 通过AssetBundle 加载得到里面的资源的记录
    /// </summary>
    [System.Serializable]
    public sealed class LoadAssetBundleSubAssetRecord : LoadAssetAssetRecord
    {
#if UNITY_EDITOR
        public string Debug_AssetBelongBundleName;

        public override void UpdateData()
        {
            base.UpdateData();
            Debug_AssetBelongBundleName = AssetBelongBundleName;
        }

#endif

        public string AssetBelongBundleName { get; protected set; } //资源所属的AssetBundle 名称

        #region  构造函数

        public LoadAssetBundleSubAssetRecord()
        {
        }

        public LoadAssetBundleSubAssetRecord(string assetUrl, string assetBundleName, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            Initial(assetUrl, assetBundleName, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetInformation, manager);
        }

        public void Initial(string assetUrl, string assetBundleName, LoadedAssetTypeEnum typeEnum, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            base.Initial(assetUrl, typeEnum, assetInformation, manager);
            AssetBelongBundleName = assetBundleName;
        }

        #endregion

        #region 基类实现

        public bool isReferenceEqual(LoadAssetAssetRecord record)
        {
            if (record == null) return false;
            switch (record.AssetLoadedType)
            {
                case LoadedAssetTypeEnum.None:
                    Debug.LogError("无法确定的类型 " + AssetLoadedType);
                    return false;
                case LoadedAssetTypeEnum.Resources_UnKnown:
                    return false;
                case LoadedAssetTypeEnum.AssetBundle_UnKnown:
                    if (record.AssetUrl == this.AssetUrl && (record as LoadAssetBundleSubAssetRecord).AssetBelongBundleName == this.AssetBelongBundleName)
                        return true;
                    return false;
                default:
                    Debug.LogError("没预处理的类型" + record.AssetLoadedType);
                    return false;
            }
        }


        /// <summary>
        /// 标示是否引用相同的资源
        /// </summary>
        /// <param name="record1"></param>
        /// <returns></returns>
        public bool IsReferenceSameAsset(LoadAssetBundleSubAssetRecord record1)
        {
            if (record1 != null)
            {
                if (record1.AssetUrl == this.AssetUrl && record1.AssetBelongBundleName == this.AssetBelongBundleName)
                    return true;
            }

            return false;
        }

        #endregion
    }
}
