using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

#if UNITY_EDITOR
public class Debug_ControllAppSetting : MonoBehaviour
{
    public bool IsLoadResourcesAssetPriorit = true;
    public Debug.LogColorDefine ColorDefine = Debug.S_LogColorDefine;

    private void OnValidate()
    {
        AppSetting.S_IsLoadResourcesAssetPriority = IsLoadResourcesAssetPriorit;
        Debug.S_LogColorDefine = ColorDefine;
    }

}
#endif