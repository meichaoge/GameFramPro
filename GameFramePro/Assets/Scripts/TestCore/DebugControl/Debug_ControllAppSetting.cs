using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

#if UNITY_EDITOR
public class Debug_ControllAppSetting : MonoBehaviour
{
    [Header("是否优先加载Resources 资源")] public bool IsLoadResourcesAssetPriorit = true;
    [Header("是否启用屏幕点击特效")] public bool IsClickEffectEnable = true;

    public Debug.LogColorDefine ColorDefine = Debug.S_LogColorDefine;

    private void OnValidate()
    {
        AppSetting.S_IsLoadResourcesAssetPriority = IsLoadResourcesAssetPriorit;
        AppSetting.S_IsClickEffectEnable = IsClickEffectEnable;
        Debug.S_LogColorDefine = ColorDefine;
    }
}
#endif
