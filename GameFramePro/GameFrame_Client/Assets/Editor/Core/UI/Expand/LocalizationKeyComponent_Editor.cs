using GameFramePro.Localization;
using GameFramePro.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro.EditorEx
{
    [CustomEditor(typeof(LocalizationKeyComponent))]
    public class LocalizationKeyComponent_Editor : Editor
    {
        private LocalizationKeyComponent mLocalizationKeyComponent;

        private void OnEnable()
        {
            mLocalizationKeyComponent = target as LocalizationKeyComponent;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("更新多语言Key 内容", GUILayout.Height(30)))
            {
                mLocalizationKeyComponent.UpdateConnectLocalizationKey(false);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

    }
}