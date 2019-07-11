using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 指定AssetBundle中每个Asset信息(为了确保兼容这里字段最好只增不减)
    /// </summary>
    [System.Serializable]
    public class AssetBundleAssetInfor 
    {
        public string mAssetRelativePath; //加载时候识别的路径

    }
}