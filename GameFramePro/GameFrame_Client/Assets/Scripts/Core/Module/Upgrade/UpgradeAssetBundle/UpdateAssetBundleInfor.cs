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
        public long mLocalAssetLength = 0;  //本地资源的大小
        public long mExpectAsssetLength = 0; //CDN 上资源大小

        public string mMd5Code = string.Empty;


        public UpdateAssetBundleInfor()
        {
        }

        public UpdateAssetBundleInfor(string assetBundleUri, long localByteSize, long serverByteSize, string md5, string fileAbsUri)
        {
            mNeedUpdateAssetBundleUri = assetBundleUri;
            mLocalAssetLength = localByteSize;
            mExpectAsssetLength = serverByteSize;

            mMd5Code = md5;
            mAssetAbsFullUri = fileAbsUri;
        }

        public UpdateAssetBundleInfor(string assetBundleFileName, long localByteSize, AssetBundleInfor bundleInfor,  string fileAbsUri)
        {
            mNeedUpdateAssetBundleUri = assetBundleFileName;
            mLocalAssetLength = localByteSize;

            mExpectAsssetLength = bundleInfor.GetABundleDetail(assetBundleFileName).Size;
            mMd5Code = bundleInfor.GetABundleDetail(assetBundleFileName).MD5;
            mAssetAbsFullUri = fileAbsUri;
        }
    }
}