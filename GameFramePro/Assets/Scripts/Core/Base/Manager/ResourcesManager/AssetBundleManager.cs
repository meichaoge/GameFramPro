using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    //

    /// <summary>
    /// AssetBundle 资源管理器
    /// </summary>
    public class AssetBundleManager : Single<AssetBundleManager>
    {

        #region Date
        private Dictionary<string, AssetBundle> mAllLoadedAssetBundleAssets = new Dictionary<string, AssetBundle>(); //所有加载的AssetBundle 资源
        #endregion

        //同步加载AssetBundle 方法
        public void LoadAsserBundleAssetSync(string assetBundlePath, string assetName, System.Action<UnityEngine.Object> loadCallback)
        {

        }


    }
}