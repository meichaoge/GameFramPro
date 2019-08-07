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

    [Header("Debug 日志输出的颜色")] public Color Debug_InforLevelColor = Color.magenta;
    public Color Debug_EditorInforLevelColor = Color.cyan;
    public Color Debug_EditorErroColor = Color.yellow;


    private void OnValidate()
    {
        AppSetting.S_IsLoadResourcesAssetPriority = IsLoadResourcesAssetPriorit;
        AppSetting.S_IsClickEffectEnable = IsClickEffectEnable;
        Debug.S_LogColorDefine.mInforLevelColor = ColorExpand.ColotToHtm(Debug_InforLevelColor);
        Debug.S_LogColorDefine.mEditorInforLevelColor = ColorExpand.ColotToHtm(Debug_EditorInforLevelColor);
        Debug.S_LogColorDefine.mEditorErroColor = ColorExpand.ColotToHtm(Debug_EditorErroColor);
    }
}
#endif
