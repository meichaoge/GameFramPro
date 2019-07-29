using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// AssetBundle 包中加载的子资源
    /// </summary>
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public class AssetBundleSubAssetLoadRecord : BaseLoadAssetRecord
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

        #region 构造函数& 设置
        public AssetBundleSubAssetLoadRecord() { }


        public AssetBundleSubAssetLoadRecord(string assetUrl, string assetBundleName, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            Initial(assetUrl, assetBundleName, typeEnum, asset, manager);
        }

        public void Initial(string assetUrl, string assetBundleName, LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        {
            base.Initial(assetUrl, typeEnum, asset, manager);
            AssetBelongBundleName = assetBundleName;
        }

        #endregion


        /// <summary>
        /// 标示是否引用相同的资源
        /// </summary>
        /// <param name="record1"></param>
        /// <returns></returns>
        public bool IsReferenceSameAsset(AssetBundleSubAssetLoadRecord record1)
        {
            if (record1 != null)
            {
                if (record1.AssetUrl == this.AssetUrl && record1.AssetBelongBundleName == this.AssetBelongBundleName)
                    return true;
            }

            return false;
        }




    }
}