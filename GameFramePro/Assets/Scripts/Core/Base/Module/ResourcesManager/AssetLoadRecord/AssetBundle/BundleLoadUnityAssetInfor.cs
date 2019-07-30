using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// AssetBundle 加载的AssetBundle
    /// </summary>
    public class BundleLoadUnityAssetInfor : BaseLoadUnityAssetInfor
    {
        private AssetBundle mLoadAssetBundleAsset = null;
        //从bundle 中加载的子资源
        private Dictionary<string, BundleLoadSubUnityAssetInfor> mAllLoadSubAssetInfor = new Dictionary<string, BundleLoadSubUnityAssetInfor>();


        public BundleLoadUnityAssetInfor(string assetUrl, AssetBundle bundle)
          : base(assetUrl, LoadedAssetTypeEnum.AssetBundle_UnKnown)
        {
            mLoadAssetBundleAsset = bundle;
        }

        public override bool IsLoadAssetEnable { get { return mLoadAssetBundleAsset != null; } }

        public bool IsEqual(BundleLoadUnityAssetInfor infor)
        {
            if (IsLoadAssetEnable == false) return false;
            if (infor.IsLoadAssetEnable == false) return false;
            return mLoadAssetBundleAsset.Equals(infor.mLoadAssetBundleAsset);
        }

        public override void RealseAsset()
        {
            if (IsLoadAssetEnable == false) return;
            UnLoadAsAssetBundleAsset(false);
        }


        //按照Resources 资源卸载自己
        public void UnLoadAsAssetBundleAsset(bool isUnloadAllLoadedObjects)
        {
            if (IsLoadAssetEnable == false)
                return;
            if (AssetType != LoadedAssetTypeEnum.AssetBundle_UnKnown)
            {
                Debug.LogError("卸载的资源 {0} 不是 AssetBundle 资源");
                return;
            }

            mLoadAssetBundleAsset.Unload(isUnloadAllLoadedObjects);
        }



        /// <summary>
        /// 加载AssetBundle 资源和对应的子资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public BundleLoadSubUnityAssetInfor LoadAssetBundleSubAsset(string assetName)
        {
            if (IsLoadAssetEnable == false) return null;
            if (AssetType != LoadedAssetTypeEnum.AssetBundle_UnKnown)
            {
                Debug.LogError("LoadAssetBundleAsset Fail,不是AssetBundle 资源 " + AssetUrl);
                return null;
            }
            BundleLoadSubUnityAssetInfor assetInfor = null;
            if (mAllLoadSubAssetInfor.TryGetValue(assetName, out assetInfor) && assetInfor.IsLoadAssetEnable)
                return assetInfor;


            UnityEngine.Object asset = mLoadAssetBundleAsset.LoadAsset<UnityEngine.Object>(assetName);
            assetInfor = new BundleLoadSubUnityAssetInfor(AssetUrl, assetName, asset);
            mAllLoadSubAssetInfor[assetName] = assetInfor;

            return assetInfor;
        }



        #region 接口实现
        public override string LoadTextAssetContent()
        {
            Debug.LogError("AssetBundle 资源不支持直接获取资源", AssetUrl);
            return string.Empty;
        }

        public override Sprite LoadSpriteFromSpriteRender()
        {
            Debug.LogError("AssetBundle 资源不支持直接获取资源", AssetUrl);
            return null;
        }
        public override AudioClip LoadAudioClip()
        {
            Debug.LogError("AssetBundle 资源不支持直接获取资源", AssetUrl);
            return null;
        }
        public override GameObject InstantiateInstance(Transform targetParent)
        {
            Debug.LogError("AssetBundle 资源不支持直接获取资源", AssetUrl);
            return null;
        }
        #endregion

    }
}