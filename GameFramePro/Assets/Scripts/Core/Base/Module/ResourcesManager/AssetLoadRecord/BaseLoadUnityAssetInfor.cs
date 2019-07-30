using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 对加载的资源的封装 对外提供接口，隐藏加载的对象实体
    /// </summary>
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public abstract class BaseLoadUnityAssetInfor
    {
        /// <summary>
        /// 唯一标示一个资源的，通常赋值为加载这个资源时候使用的相对路径(相对于Resouces 目录)
        /// </summary>
        public string AssetUrl { get; protected set; }
        public LoadedAssetTypeEnum AssetType { get; protected set; } = LoadedAssetTypeEnum.None;
        public abstract bool IsLoadAssetEnable { get; }


        protected BaseLoadUnityAssetInfor(string assetUrl, LoadedAssetTypeEnum assetType)
        {
            AssetUrl = assetUrl;
            AssetType = assetType;
        }


        public abstract void RealseAsset();


        //**接口
        public abstract string LoadTextAssetContent();
        public abstract Sprite LoadSpriteFromSpriteRender();

        //创建一个实例
        public abstract GameObject InstantiateInstance(Transform targetParent);



    }
}