﻿using System.Collections;
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



    private void Update()
    {
        mAllResourcesLoadAssets.Clear();
        foreach (var item in LocalResourcesManager.S_Instance.Debug_mAllLoadedAssetRecord.Values)
        {
            item.UpdateData();
            mAllResourcesLoadAssets.Add(item);
        }
       
        mAllResourcesLoadAssetInstances.Clear();
        mAllResourcesLoadAssetInstances.AddRange(LocalResourcesManager.S_Instance.Debug_mAllResoucesLoadAssetInstanceIds);

    }


}
#endif