using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 编辑器下每个AssetBundle 包资源信息
    /// </summary>
    [System.Serializable]
    public class EditorAssetBundleInfor
    {
        public string mAssetBundlePackageName = string.Empty; //每个生成的 AssetBundle 的BundleName
        public string mMD5Code;  //编辑器下有效 用于在两个版本之间比对资源
        public long mPackageSize;  //包大小 单位byte
        public List<EditorAssetBundleAssetInfor> mAllContainAssetInfor = new List<EditorAssetBundleAssetInfor>();//包含单个资源的信息
        public List<EditorAssetBundleInfor> mDepdenceAssetBundle = new List<EditorAssetBundleInfor>();//依赖的其他AssetBundle


        public override string ToString()
        {
            return string.Format("mAssetBundlePackageName= {0:30}    mMD5Code={1:50}    mPackageSize={2} ContainAsset={3}  Depdence={4}", mAssetBundlePackageName, mMD5Code, mPackageSize,
                mAllContainAssetInfor.Count, mDepdenceAssetBundle.Count);
        }

    }
}