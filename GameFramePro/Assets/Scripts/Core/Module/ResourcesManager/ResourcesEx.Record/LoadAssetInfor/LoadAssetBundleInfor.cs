using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>///  加载的AssetBundle资源信息 /// </summary>
    public sealed class LoadAssetBundleInfor
    {
        /// <summary>/// 唯一标示一个资源的，通常赋值为加载这个资源时候使用的相对路径(相对于Resouces 目录)/// </summary>
        public string AssetUrl { get; protected set; }

        protected AssetBundle AssetBundleInformation { get; set; }

        public bool IsReferenceEnable
        {
            get { return AssetBundleInformation != null; }
        }

        //从bundle 中加载的子资源
        private Dictionary<string, LoadBasicAssetInfor> mAllLoadSubAssetInfor = new Dictionary<string, LoadBasicAssetInfor>();


        public LoadAssetBundleInfor(string assetUrl, AssetBundle bundle)
        {
            AssetUrl = assetUrl;
            AssetBundleInformation = bundle;
        }


        //按照Resources 资源卸载自己
        public void UnLoadAsAssetBundleAsset(bool isUnloadAllLoadedObjects)
        {
            if (IsReferenceEnable == false) return;
            AssetBundleInformation.Unload(isUnloadAllLoadedObjects);
        }


        /// <summary>/// 加载AssetBundle 资源和对应的子资源/// </summary>
        public LoadBasicAssetInfor LoadAssetBundleSubAsset(string assetName)
        {
            LoadBasicAssetInfor assetInfor = null;
            if (mAllLoadSubAssetInfor.TryGetValue(assetName, out assetInfor) && assetInfor.IsLoadAssetEnable)
                return assetInfor;


            UnityEngine.Object asset = AssetBundleInformation.LoadAsset<UnityEngine.Object>(assetName);
            assetInfor = new LoadBasicAssetInfor(AssetUrl, asset, LoadedAssetTypeEnum.AssetBundle_UnKnown);
            mAllLoadSubAssetInfor[assetName] = assetInfor;

            return assetInfor;
        }
    }
}
