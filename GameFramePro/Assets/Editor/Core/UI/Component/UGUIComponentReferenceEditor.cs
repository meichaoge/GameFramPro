using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using GameFramePro.UI;


namespace GameFramePro.EditorEx
{
    [CustomEditor(typeof(UGUIComponentReference))]
    public class UGUIComponentReferenceEditor : Editor
    {
        private SerializedProperty mSerilizeUGUIComponents;
        private UGUIComponentReference mUGUIComponentReference;

        private void OnEnable()
        {
            mSerilizeUGUIComponents = serializedObject.FindProperty("mSerilizeUGUIComponents");
            mUGUIComponentReference = target as UGUIComponentReference;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("关联指定类型的组件",GUILayout.Height(30)))
                mUGUIComponentReference.GetComponentByTypeDefine();

            if (GUILayout.Button("检测是否有重名的对象", GUILayout.Height(30)))
                mUGUIComponentReference.CheckIfContainSameNameObject();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //if (EditorGUILayout.PropertyField(mSerilizeUGUIComponents))
            //{
            //    EditorGUI.indentLevel++;
            //    // 设置元素个数
            //    mSerilizeUGUIComponents.arraySize = EditorGUILayout.IntField("Size", mSerilizeUGUIComponents.arraySize);

            //    for (int dex = 0; dex < mSerilizeUGUIComponents.arraySize; dex++)
            //    {
            //        var componentItem = mSerilizeUGUIComponents.GetArrayElementAtIndex(dex);
            //        EditorGUILayout.PropertyField(componentItem, true);
            //    }
            //}
            //EditorGUI.indentLevel--;
            //serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}