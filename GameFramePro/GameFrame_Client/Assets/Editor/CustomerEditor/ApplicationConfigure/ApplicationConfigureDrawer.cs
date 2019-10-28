using System.Collections;
using System.Collections.Generic;
using DG.DOTweenEditor.Core;
using UnityEditor;
using UnityEngine;


namespace GameFramePro.EditorEx
{
    [CustomEditor(typeof(ApplicationConfigure))]
    public class ApplicationConfigureDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            return;
//            SerializedObject serializedObject = new SerializedObject(target);
//            ApplicationConfigure applicationConfigure = target as ApplicationConfigure;
//
//            //  Debug.Log($"{so==null} ::  {so.FindProperty("mIsLoadResourcesAssetPriority").boolValue}");
//
//            EditorGUI.BeginChangeCheck();
//
//            bool IsChecked = EditorGUILayout.Toggle(serializedObject.FindProperty("mIsLoadResourcesAssetPriority").name, applicationConfigure.mIsLoadResourcesAssetPriority);
//            if (EditorGUI.EndChangeCheck())
//            {
//                applicationConfigure.mIsLoadResourcesAssetPriority = IsChecked;
//                Debug.LogEditorInfor($"修改了属性{applicationConfigure.mIsLoadResourcesAssetPriority}");
//            }
        }
    }
}