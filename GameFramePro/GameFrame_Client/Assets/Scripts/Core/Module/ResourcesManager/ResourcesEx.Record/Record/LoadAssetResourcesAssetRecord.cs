using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    ///  Resources 加载的资源
    /// </summary>
    [System.Serializable]
    public sealed class LoadAssetResourcesAssetRecord : LoadAssetAssetRecord
    {
        #region 构造函数

        public LoadAssetResourcesAssetRecord()
        {
        }

        public LoadAssetResourcesAssetRecord(string assetUrl, LoadBasicAssetInfor assetInformation, IAssetManager manager)
        {
            base.Initial(assetUrl, LoadedAssetTypeEnum.Resources_UnKnown, manager);
        }

        #endregion
    }
}
