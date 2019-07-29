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

        public override bool isReferenceEqual(BaseLoadAssetRecord record)
        {
            if (record == null) return false;


            switch (record.AssetLoadedType)
            {
                case LoadedAssetTypeEnum.None:
                    Debug.LogError("无法确定的类型 " + AssetLoadedType);
                    return false;
                case LoadedAssetTypeEnum.Resources_UnKnown:
                    if (record.AssetUrl == this.AssetUrl)
                        return true;
                    return false;
                case LoadedAssetTypeEnum.AssetBundle_UnKnown:
                    return false;
                default:
                    Debug.LogError("没预处理的类型" + record.AssetLoadedType);
                    return false;
            }
        }


    }
}