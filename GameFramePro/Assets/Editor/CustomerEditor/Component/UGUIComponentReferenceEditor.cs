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

            #region 功能 枚举创建Tag
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("根据 UGUIComponentTypeEnum 枚举创建Tag", GUILayout.Height(25)))
                mUGUIComponentReference.AddAllUGUIComponentTypeTag();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            #endregion

            #region 功能
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("自动序列化标记Tag 的组件", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoRecordReferenceWithTag();
            }

            if (GUILayout.Button("关联指定类型的组件", GUILayout.Height(25)))
                mUGUIComponentReference.GetComponentByTypeDefine();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            #endregion

            #region 功能
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();


            if (GUILayout.Button("检测是否有重名的对象", GUILayout.Height(25)))
                mUGUIComponentReference.CheckIfContainSameNameObject();

            if (GUILayout.Button("检测关联组件的类型是否正确", GUILayout.Height(25)))
                mUGUIComponentReference.CheckSerializationComponentType();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            #endregion



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