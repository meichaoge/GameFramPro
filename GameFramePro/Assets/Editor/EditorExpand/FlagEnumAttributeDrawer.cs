using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

[CustomPropertyDrawer(typeof(FlagEnumAttribute))]
public class FlagEnumAttributeDrawer : PropertyDrawer
{
    /// <summary>
    /// 绘制EnumFlagsAttribute 的表现效果
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // base.OnGUI(position, property, label);
        property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
        //    Debug.Log("CurSelect EnumFlag Value=" + property.intValue);
    }
}
