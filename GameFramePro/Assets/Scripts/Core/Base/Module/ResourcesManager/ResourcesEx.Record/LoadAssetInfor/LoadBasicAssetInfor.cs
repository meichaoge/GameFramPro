using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 对加载的资源的封装 对外提供接口，隐藏加载的对象实体/// </summary>
#if UNITY_EDITOR
    [System.Serializable]
#endif
    public sealed class LoadBasicAssetInfor
    {
        /// <summary>/// 唯一标示一个资源的，通常赋值为加载这个资源时候使用的相对路径(相对于Resources 目录)/// </summary>
        public string AssetUrl { get; protected set; }

        public int InstanceID
        {
            get { return mLoadUnityAsset ? mLoadUnityAsset.GetInstanceID() : 0; }
        }

        public LoadedAssetTypeEnum LoadAssetType { get; protected set; } = LoadedAssetTypeEnum.None;
        private readonly UnityEngine.Object mLoadUnityAsset = null; //实际上加载的资源对象

        public bool IsLoadAssetEnable
        {
            get { return mLoadUnityAsset != null; }
        }

        #region 构造函数

        public LoadBasicAssetInfor(string assetUrl, UnityEngine.Object asset, LoadedAssetTypeEnum assetType)
        {
            AssetUrl = assetUrl;
            mLoadUnityAsset = asset;
            LoadAssetType = assetType;
        }

        #endregion


        #region 其他接口

        public bool IsEqual(LoadBasicAssetInfor infor)
        {
            if (IsLoadAssetEnable == false) return false;
            if (infor.IsLoadAssetEnable == false) return false;
            return mLoadUnityAsset.Equals(infor.mLoadUnityAsset);
        }

        public void ReleaseAsset()
        {
            if (IsLoadAssetEnable == false) return;
            Resources.UnloadAsset(mLoadUnityAsset);
        }

        #endregion


        #region 接口实现

        public string LoadTextAssetContent()
        {
            if (IsLoadAssetEnable == false) return string.Empty;
            if (mLoadUnityAsset is TextAsset)
                return (mLoadUnityAsset as TextAsset).text;

            Debug.LogError("当前资源{0} 不是TextAsset 资源", AssetUrl);
            return string.Empty;
        }

        public Sprite LoadSpriteFromSpriteRender()
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


        public AudioClip LoadAudioClip()
        {
            if (IsLoadAssetEnable == false) return null;
            return mLoadUnityAsset as AudioClip;
        }

        public GameObject InstantiateInstance(Transform targetParent)
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
