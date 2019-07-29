﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

public class Debug_ShowAllReference : MonoBehaviour
{

    [System.Serializable]
    public class AssetReferenceInfor
    {
        public GameObject mGameObject;
        public List<ComponentAssetReferenceInfor> mAllComponents = new List<ComponentAssetReferenceInfor>();
    }
    [System.Serializable]
    public class ComponentAssetReferenceInfor
    {
        public Component mComponent;
        public List<BaseAssetReference2> mAllReference = new List<BaseAssetReference2>();
    }


    public List<AssetReferenceInfor> mAllGameObjectReference = new List<AssetReferenceInfor>();

    private void Update()
    {
        mAllGameObjectReference.Clear();
        foreach (var assetReference in AssetReferenceManager.AllObjectComponentReferenceRecord)
        {
            AssetReferenceInfor assetReferenceInfor = new AssetReferenceInfor();
            assetReferenceInfor.mGameObject = assetReference.Key as GameObject;

            foreach (var item in assetReference.Value)
            {
                ComponentAssetReferenceInfor componentInfor = new ComponentAssetReferenceInfor();
                componentInfor.mComponent = item.Key;

                foreach (var baseReference in item.Value)
                {
                    baseReference.UpdateView();
                    componentInfor.mAllReference.Add(baseReference);
                }

                assetReferenceInfor.mAllComponents.Add(componentInfor);
            }

            mAllGameObjectReference.Add(assetReferenceInfor);
        }
    }

}
