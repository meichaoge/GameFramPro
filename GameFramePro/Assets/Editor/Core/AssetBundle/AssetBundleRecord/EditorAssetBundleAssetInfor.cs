using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 编辑器下每个AssetBundle 包中的单个资源信息
    /// </summary>
    [System.Serializable]
    public class EditorAssetBundleAssetInfor 
    {
        public string mAssetRelativePath;  //资源路径
        public string mMD5Code;  //编辑器下有效 用于在两个版本之间比对资源
        public long mFileAssetSize; //实际文件资源大小

        public override string ToString()
        {
            return string.Format("mAssetRelativePath= {0:50} \t mMD5Code={1} \t mFileAssetSize={2}", mAssetRelativePath, mMD5Code, mFileAssetSize);
        }


    }
}