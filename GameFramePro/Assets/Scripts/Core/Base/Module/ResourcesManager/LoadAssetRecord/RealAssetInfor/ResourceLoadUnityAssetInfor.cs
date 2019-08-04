using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// Reosuces 加载的Unity 资源
    /// </summary>
    public class ResourceLoadUnityAssetInfor : BaseLoadUnityAssetInfor
    {

        private UnityEngine.Object mLoadUnityAsset = null;



        public ResourceLoadUnityAssetInfor(string assetUrl, UnityEngine.Object asset) :base(assetUrl, LoadedAssetTypeEnum.Resources_UnKnown)
        {
            mLoadUnityAsset = asset;
        }

        public override bool IsLoadAssetEnable { get { return mLoadUnityAsset != null; } }

        public  bool IsEqual(ResourceLoadUnityAssetInfor infor)
        {
            if (IsLoadAssetEnable == false) return false;
            if (infor.IsLoadAssetEnable == false) return false;
            return mLoadUnityAsset.Equals(infor.mLoadUnityAsset);
        }

        public override void RealseAsset()
        {
            if (IsLoadAssetEnable == false) return;
            UnLoadAsResourcesAsset();
        }



        //按照Resources 资源卸载自己
        public void UnLoadAsResourcesAsset()
        {
            if (IsLoadAssetEnable == false)
                return;
            //if (AssetType != LoadedAssetTypeEnum.Resources_UnKnown)
            //{
            //    Debug.LogError("卸载的资源 {0} 不是Resouces 资源");
            //    return;
            //}
            Resources.UnloadAsset(mLoadUnityAsset);
        }


        #region 接口实现
        public override string LoadTextAssetContent()
        {
            if (IsLoadAssetEnable == false) return string.Empty;
            if (mLoadUnityAsset is TextAsset)
                return (mLoadUnityAsset as TextAsset).text;

            Debug.LogError("当前资源{0} 不是TextAsset 资源", AssetUrl);
            return string.Empty;
        }
        public override Sprite LoadSpriteFromSpriteRender()
        {
            if (IsLoadAssetEnable == false) return null;
            GameObject go = mLoadUnityAsset as GameObject;
            if (go == null)
            {
                Debug.LogError("当前资源{0} 不是GameObject 资源");
                return null;
            }
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                Debug.LogError("当前资源{0} 不是 没有组件 SpriteRenderer ");
                return null;
            }
            return renderer.sprite;
        }
        public override AudioClip LoadAudioClip()
        {
            if (IsLoadAssetEnable == false) return null;
            return mLoadUnityAsset as AudioClip;
        }

        public override GameObject InstantiateInstance(Transform targetParent)
        {
            if (IsLoadAssetEnable == false) return null;
            GameObject prefab = mLoadUnityAsset as GameObject;
            if (prefab == null)
            {
                Debug.LogError("当前资源{0} 不是GameObject 资源");
                return null;
            }
            GameObject go = ResourcesManager.Instantiate<GameObject>(prefab, targetParent, false);
            go.name = prefab.name;
            return go;
        }
        #endregion





    }
}