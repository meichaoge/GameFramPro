using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

#if UNITY_EDITOR
/// <summary>
/// 编辑器下显示所有追踪的资源
/// </summary>
public class Debug_ShowAllTraceResources :MonoBehaviour
{
    public List<ResourcesLoadAssetRecord> mAllResourcesLoadAssets = new List<ResourcesLoadAssetRecord>();
    public List<int> mAllResourcesLoadAssetInstances = new List<int>();
    [Header("AssetBundle")]
    public List<AssetBundleAssetDepdenceRecord> mAllLoadAssetBundleCache = new List<AssetBundleAssetDepdenceRecord>();
    public List<AssetBundleSubAssetLoadRecord> mAllLoadedAssetBundleSubAssetRecord = new List<AssetBundleSubAssetLoadRecord>();




    private void Update()
    {
        #region Resources

        mAllResourcesLoadAssets.Clear();
        foreach (var item in LocalResourcesManager.S_Instance.Debug_mAllLoadedAssetRecord.Values)
        {
            item.UpdateData();
            mAllResourcesLoadAssets.Add(item);
        }
       
        mAllResourcesLoadAssetInstances.Clear();
        mAllResourcesLoadAssetInstances.AddRange(LocalResourcesManager.S_Instance.Debug_mAllResoucesLoadAssetInstanceIds);
        #endregion

        #region  AssetBundle
        mAllLoadAssetBundleCache.Clear();
        foreach (var item in AssetBundleManager.S_Instance.Debug_mAllLoadAssetBundleCache.Values)
        {
            item.UpdateData();
            mAllLoadAssetBundleCache.Add(item);
        }
        mAllLoadedAssetBundleSubAssetRecord.Clear();
        foreach (var item in AssetBundleManager.S_Instance.Debug_mAllLoadedAssetBundleSubAssetRecord.Values)
        {
            item.UpdateData();
            mAllLoadedAssetBundleSubAssetRecord.Add(item);
        }
        #endregion


    }


}
#endif