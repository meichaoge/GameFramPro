using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro;

#if UNITY_EDITOR
public class Debug_ControllAppSetting : MonoBehaviour
{
    [Header("Debug 日志输出的颜色")] public Color Debug_InforLevelColor = Color.magenta;
    public Color Debug_EditorInforLevelColor = Color.cyan;
    public Color Debug_EditorErroColor = Color.yellow;


    private void OnValidate()
    {
        Debug.S_LogColorDefine.mInforLevelColor = ColorExpand.ColotToHtm(Debug_InforLevelColor);
        Debug.S_LogColorDefine.mEditorInforLevelColor = ColorExpand.ColotToHtm(Debug_EditorInforLevelColor);
        Debug.S_LogColorDefine.mEditorErroColor = ColorExpand.ColotToHtm(Debug_EditorErroColor);
    }
}
#endif
