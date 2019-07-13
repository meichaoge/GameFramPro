using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 编辑器下所有 AssetBundle 资源信息
    /// </summary>
    [System.Serializable]
    public class EditorTotalAssetBundleInfor 
    {
        public string mAssetRelativePath; //加载时候识别的路径
        public string mVersion = "1.0.0.1"; //资源版本
        public long mConfigBuildTime = 0; //创建这个配置文件的时间(utc);
        public long mTotalSize; //总大小
        public Dictionary<string, EditorAssetBundleInfor> mTotalAssetBundleInfor = new Dictionary<string, EditorAssetBundleInfor>(); //包含的所有资源信息
    }
}