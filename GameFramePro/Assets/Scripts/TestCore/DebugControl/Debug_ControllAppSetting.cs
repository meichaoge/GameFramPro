using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

#if UNITY_EDITOR
public class Debug_ControllAppSetting : MonoBehaviour
{
    public bool IsLoadResourcesAssetPriorit = true;


    private void OnValidate()
    {
        AppSetting.S_IsLoadResourcesAssetPriority = IsLoadResourcesAssetPriorit;
    }

}
#endif