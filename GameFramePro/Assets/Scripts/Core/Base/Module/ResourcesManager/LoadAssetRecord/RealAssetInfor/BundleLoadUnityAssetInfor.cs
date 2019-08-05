using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// AssetBundle 加载的AssetBundle
    /// </summary>
    public class BundleLoadUnityAssetInfor : BaseLoadUnityAssetInfor
    {
        //从bundle 中加载的子资源
        private Dictionary<string, BaseLoadUnityAssetInfor> mAllLoadSubAssetInfor = new Dictionary<string, BaseLoadUnityAssetInfor>();


        public BundleLoadUnityAssetInfor(string assetUrl, AssetBundle bundle) : base(assetUrl, bundle, LoadedAssetTypeEnum.AssetBundle_UnKnown)
        {
        }


        //按照Resources 资源卸载自己
        public void UnLoadAsAssetBundleAsset(bool isUnloadAllLoadedObjects)
        {
            if (IsLoadAssetEnable == false) return;
            (mLoadUnityAsset as AssetBundle).Unload(isUnloadAllLoadedObjects);
        }


        /// <summary>
        /// 加载AssetBundle 资源和对应的子资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BaseLoadUnityAssetInfor LoadAssetBundleSubAsset(string assetName)
        {
            BaseLoadUnityAssetInfor assetInfor = null;
            if (mAllLoadSubAssetInfor.TryGetValue(assetName, out assetInfor) && assetInfor.IsLoadAssetEnable)
                return assetInfor;


            UnityEngine.Object asset = (mLoadUnityAsset as AssetBundle).LoadAsset<UnityEngine.Object>(assetName);
            assetInfor = new BaseLoadUnityAssetInfor(AssetUrl, asset, LoadedAssetTypeEnum.AssetBundle_UnKnown);
            mAllLoadSubAssetInfor[assetName] = assetInfor;

            return assetInfor;
        }

        public override void ReleaseAsset()
        {
            throw new NotImplementedException();
        }

        public override string LoadTextAssetContent()
        {
            throw new NotImplementedException();
        }

        public override Sprite LoadSpriteFromSpriteRender()
        {
            throw new NotImplementedException();
        }

        public override AudioClip LoadAudioClip()
        {
            throw new NotImplementedException();
        }

        public override GameObject InstantiateInstance(Transform targetParent)
        {
            throw new NotImplementedException();
        }
    }
}
