using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// AssetBundle 加载的AssetBundle
    /// </summary>
    public class BundleLoadUnityAssetInfor: BaseLoadUnityAssetInfor
    {

        public override bool IsLoadAssetEnable { get { return mLoadAssetBundleAsset != null; } }

        private AssetBundle mLoadAssetBundleAsset = null;
        //从bundle 中加载的子资源
        private Dictionary<string, BundleLoadSubUnityAssetInfor> mAllLoadSubAssetInfor = new Dictionary<string, BundleLoadSubUnityAssetInfor>();


        public BundleLoadUnityAssetInfor(string assetUrl, AssetBundle bundle):base(assetUrl,LoadedAssetTypeEnum.AssetBundle_UnKnown)
        {
            AssetUrl = assetUrl;
            mLoadAssetBundleAsset = bundle;
        }




        //按照Resources 资源卸载自己
        public void UnLoadAsAssetBundleAsset(bool isUnloadAllLoadedObjects)
        {
            mLoadAssetBundleAsset.Unload(isUnloadAllLoadedObjects);
        }


        /// <summary>
        /// 加载AssetBundle 资源和对应的子资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BundleLoadSubUnityAssetInfor LoadAssetBundleSubAsset(string assetName)
        {
            BundleLoadSubUnityAssetInfor assetInfor = null;
            if (mAllLoadSubAssetInfor.TryGetValue(assetName, out assetInfor) && assetInfor.IsLoadAssetEnable)
                return assetInfor;


            UnityEngine.Object asset = mLoadAssetBundleAsset.LoadAsset<UnityEngine.Object>(assetName);
            assetInfor = new BundleLoadSubUnityAssetInfor(AssetUrl, assetName, asset);
            mAllLoadSubAssetInfor[assetName] = assetInfor;

            return assetInfor;
        }

        public override void RealseAsset()
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