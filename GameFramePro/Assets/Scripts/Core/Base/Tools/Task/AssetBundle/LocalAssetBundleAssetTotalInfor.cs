using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{

    /// <summary>
    /// 单个AssetBundle 信息
    /// </summary>
    [System.Serializable]
    public class LocalAssetBundleInfor
    {
        public string mBundleName; //对应 EditorAssetBundleInfor.mAssetBundlePackageName
        public long mBundleSize;// 对应 EditorAssetBundleInfor.mPackageSize
        public int mBundleAssetsCount;// 包含的资源数量
        public HashSet<string> mContainAssetPathInfor = new HashSet<string>(); //包含的资源路径
    }

    /// <summary>
    /// 记录了从服务器获取的所有的AssetBundle 对于的信息
    /// </summary>
    [System.Serializable]
    public class LocalAssetBundleAssetTotalInfor
    {
        public string mVersion = "1.0.0.1"; //对应 EditorTotalAssetBundleInfor. mVersion 资源版本
        public long mConfigBuildTime = 0L; //对应 EditorTotalAssetBundleInfor.mConfigBuildTime  创建这个配置文件的时间(utc);
        public long mTotalSize; //对应 EditorTotalAssetBundleInfor.mTotalSize  总大小

        public Dictionary<string, LocalAssetBundleInfor> mTotalAssetBundleInfor = new Dictionary<string, LocalAssetBundleInfor>(); //总的AssetBundle 信息

    }
}