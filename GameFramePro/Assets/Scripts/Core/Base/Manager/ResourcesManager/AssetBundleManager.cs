using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// AssetBundle 资源管理器
    /// </summary>
    public class AssetBundleManager : Single<AssetBundleManager>
    {

        #region Date
        private Dictionary<string, AssetBundle> mAllLoadedAssetBundleAssets = new Dictionary<string, AssetBundle>(50); //所有加载的AssetBundle 资源
        #endregion

        //同步加载AssetBundle 方法
        public void LoadAsserBundleAssetSync(string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            AssetBundle assetBundle = GetAssetBundleByPath(assetBundlePath);
            if (assetBundle != null)
            {
                Object aset = assetBundle.LoadAsset(assetName);

                if (loadCallback != null)
                    loadCallback(aset);
            }
        }

        #region 辅助
        /// <summary>
        /// 根据路劲获取已经加载的 AssetBundle 可能得到空结果
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        private AssetBundle GetAssetBundleByPath(string assetBundlePath)
        {
            AssetBundle result = null;
            if(mAllLoadedAssetBundleAssets.TryGetValue(assetBundlePath,out result))
            {
                if (result != null)
                    return result;
                Debug.LogInfor(string.Format("GetAssetBundleByPath Fail，AssetBundle={0} 资源已经被卸载了", assetBundlePath));
                mAllLoadedAssetBundleAssets.Remove(assetBundlePath);
            }
            return result;
        }
        #endregion


    }
}