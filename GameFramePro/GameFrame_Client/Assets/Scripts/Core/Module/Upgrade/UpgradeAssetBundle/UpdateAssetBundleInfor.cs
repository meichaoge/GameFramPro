using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.Upgrade
{


    [System.Serializable]
    internal class UpdateAssetBundleInfor
    {
        public string mNeedUpdateAssetBundleUri = string.Empty;
        public string mAssetAbsFullUri = string.Empty; //本地资源的绝对路径 资源删除时候用
        public long mAssetByteSize = 0;
        public string mMd5Code = string.Empty;


        public UpdateAssetBundleInfor()
        {
        }

        public UpdateAssetBundleInfor(string assetBundleUri, long byteSize, string md5, string fileAbsUri)
        {
            mNeedUpdateAssetBundleUri = assetBundleUri;
            mAssetByteSize = byteSize;
            mMd5Code = md5;
            mAssetAbsFullUri = fileAbsUri;
        }

        public UpdateAssetBundleInfor(string assetBundleFileName,AssetBundleInfor bundleInfor,  string fileAbsUri)
        {
            mNeedUpdateAssetBundleUri = assetBundleFileName;

            mAssetByteSize = bundleInfor.GetABundleDetail(assetBundleFileName).Size;
            mMd5Code = bundleInfor.GetABundleDetail(assetBundleFileName).MD5;
            mAssetAbsFullUri = fileAbsUri;
        }
    }
}