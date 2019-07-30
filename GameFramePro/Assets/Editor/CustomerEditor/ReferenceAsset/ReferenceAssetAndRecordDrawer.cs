using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    [CustomPropertyDrawer(typeof(PropertyDrawer))]
    public class ReferenceAssetAndRecord:PropertyDrawer
    {




        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //  base.OnGUI(position, property, label);

            SerializedProperty ReferenceAsset = property.FindPropertyRelative("ReferenceAsset");
            SerializedProperty ReferenceAsset2 = property.FindPropertyRelative("Debug_ResourcesLoadAssetRecord_Current");
            SerializedProperty ReferenceAsset3 = property.FindPropertyRelative("Debug_AssetBundleSubAssetLoadRecord_Current");

        }
    }
}