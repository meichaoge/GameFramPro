//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using GameFramePro;
//using GameFramePro.ResourcesEx;
//
//#if UNITY_EDITOR
///// <summary>
///// 编辑器下显示所有追踪的资源
///// </summary>
//public class Debug_ShowAllTraceResources : MonoBehaviour
//{
//    public List<LoadAssetResourcesAssetRecord> mAllResourcesLoadAssets = new List<LoadAssetResourcesAssetRecord>();
//
//    [Header("AssetBundle")] public List<LoadAssetBundleAssetRecord> mAllLoadAssetBundleCache = new List<LoadAssetBundleAssetRecord>();
//
//    public List<LoadAssetBundleSubAssetRecord> mAllLoadedAssetBundleSubAssetRecord = new List<LoadAssetBundleSubAssetRecord>();
//
//
//    private void Update()
//    {
//        #region Resources
//
//        mAllResourcesLoadAssets.Clear();
//        foreach (var item in LocalResourcesManager.S_Instance.Debug_mAllLoadedAssetRecord.Values)
//        {
//            item.UpdateData();
//            mAllResourcesLoadAssets.Add(item);
//        }
//
//        mAllResourcesLoadAssets.Sort((a, b) => a.AssetUrl.CompareTo(b.AssetUrl));
//
//        #endregion
//
//        #region  AssetBundle
//
//        mAllLoadAssetBundleCache.Clear();
//        foreach (var item in AssetBundleManager.S_Instance.Debug_mAllLoadAssetBundleCache.Values)
//        {
//            item.UpdateData();
//            mAllLoadAssetBundleCache.Add(item);
//        }
//
//        mAllLoadAssetBundleCache.Sort((a, b) => a.AssetUrl.CompareTo(b.AssetUrl));
//
//
//        mAllLoadedAssetBundleSubAssetRecord.Clear();
//        foreach (var item in AssetBundleManager.S_Instance.Debug_mAllLoadedAssetBundleSubAssetRecord.Values)
//        {
//            item.UpdateData();
//            mAllLoadedAssetBundleSubAssetRecord.Add(item);
//        }
//
//        mAllLoadedAssetBundleSubAssetRecord.Sort((a, b) => a.AssetUrl.CompareTo(b.AssetUrl));
//
//        #endregion
//    }
//}
//#endif
