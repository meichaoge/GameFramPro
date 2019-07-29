using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 加载Resources 时候的记录
    /// </summary>
    #if UNITY_EDITOR
    [System.Serializable]
#endif
    public class ResourcesLoadAssetRecord : BaseLoadAssetRecord
    {

        public ResourceLoadUnityAssetInfor LoadResourceAssetInfor { get { return LoadUnityObjectAssetInfor as ResourceLoadUnityAssetInfor; } }



        #region 构造函数& 设置
        public ResourcesLoadAssetRecord()
        {

        }

  
        //public ResourcesLoadAssetRecord(string assetPath,  LoadedAssetTypeEnum typeEnum, UnityEngine.Object asset, IAssetManager manager)
        //{
        //    Initial(assetPath, typeEnum, asset, manager);
        //}


        #endregion



    }
}