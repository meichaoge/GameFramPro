using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.Upgrade
{
    /// <summary>/// 标示更新Asset Bundle 资源时候的状态/// </summary>
    internal enum AssetBundleAssetUpdateTagEnum
    {
        AddAsset, //新增资源
        RemoveLocalAsset, //本地资源已经无效 需要删除
        UpdateAsset, //本地资源需要更新
    }


    [System.Serializable]
    internal class UpdateAssetBundleInfor
    {
        public AssetBundleAssetUpdateTagEnum mAssetBundleAssetUpdateTagEnum;
        public string mNeedUpdateAssetName = string.Empty;
        public long mAssetByteSize = 0;
        public uint mAssetCRC = 0;


        public UpdateAssetBundleInfor()
        {
        }

        public UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum tagEnum, string assetName, long byteSize, uint crc)
        {
            mAssetBundleAssetUpdateTagEnum = tagEnum;
            mNeedUpdateAssetName = assetName;
            mAssetByteSize = byteSize;
            mAssetCRC = crc;
        }

        public UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum tagEnum, AssetBundleInfor bundleInfor)
        {
            mAssetBundleAssetUpdateTagEnum = tagEnum;
            mNeedUpdateAssetName = bundleInfor.mBundleName;
            mAssetByteSize = bundleInfor.mBundleSize;
            mAssetCRC = bundleInfor.mBundleCRC;
        }
    }
}
