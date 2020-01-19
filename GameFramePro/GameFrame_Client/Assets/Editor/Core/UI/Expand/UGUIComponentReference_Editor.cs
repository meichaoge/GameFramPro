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

    [CustomEditor(typeof(UGUIComponentReference))]
    public class UGUIComponentReference_Editor : Editor
    {
        private SerializedProperty mSerilizeUGUIComponents;
        private SerializedProperty mAllLocalizationKeyComponents;

        private UGUIComponentReference mUGUIComponentReference;

        private void OnEnable()
        {
            mSerilizeUGUIComponents = serializedObject.FindProperty("mSerilizeUGUIComponents");
            mAllLocalizationKeyComponents = serializedObject.FindProperty("mAllLocalizationKeyComponents");
            mUGUIComponentReference = target as UGUIComponentReference;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical();
            GUILayout.Space(5);


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("根据ComponentTypeEnum创建Tag", GUILayout.Height(25)))
                mUGUIComponentReference.AddAllUGUIComponentTypeTag();

            if (GUILayout.Button("移除没有预定义的Tags", GUILayout.Height(25)))
                mUGUIComponentReference.RemoveAllUnDefineComponentTag();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("自动重新映射UITags", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoReMapTags();
                Undo.RegisterFullObjectHierarchyUndo(mUGUIComponentReference.gameObject, "AutoReMapTag");

            }

            if (GUILayout.Button("自动关联多语言key", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoConnectLocalizationKeyComponent();
                mUGUIComponentReference.AutoRecordLocalizationKeyComponent();
                Undo.RegisterFullObjectHierarchyUndo(mUGUIComponentReference.gameObject, "AutoSerlizeLocalizationKey");

            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("自动序列化标记Tag 的组件&设置序列化组件对象的Tag", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoRecordReferenceWithTag();
                mUGUIComponentReference.AutoSetSerializaComponentTag();
                Undo.RegisterFullObjectHierarchyUndo(mUGUIComponentReference.gameObject, "SerializeTgs");
            }


            if (GUILayout.Button("检测是否有重名的对象", GUILayout.Height(25)))
                mUGUIComponentReference.CheckIfContainSameNameComponent();

            if (GUILayout.Button("获取当前对象的UI定义", GUILayout.Height(25)))
                UGUIComponentReference.ShowComponentReferenceByConfig(mUGUIComponentReference.gameObject);

            GUILayout.Space(5);
            GUILayout.EndVertical();


            DrawSerilizeUGUIComponent();
            DrawAllLocalizationKeyComponents();

            serializedObject.ApplyModifiedProperties();
            //       base.OnInspectorGUI();
        }


        private void DrawSerilizeUGUIComponent()
        {
            if (EditorGUILayout.PropertyField(mSerilizeUGUIComponents))
            {
                EditorGUI.indentLevel++;
                // 设置元素个数
                mSerilizeUGUIComponents.arraySize = EditorGUILayout.IntField("Size", mSerilizeUGUIComponents.arraySize);

                for (int dex = 0; dex < mSerilizeUGUIComponents.arraySize; dex++)
                {
                    var componentItem = mSerilizeUGUIComponents.GetArrayElementAtIndex(dex);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("+"), GUILayout.Width(25)))
                    {
                        mSerilizeUGUIComponents.InsertArrayElementAtIndex(dex + 1);
                        var newComponentItem = mSerilizeUGUIComponents.GetArrayElementAtIndex(dex + 1);
                        newComponentItem.FindPropertyRelative("mReferenceComponent").objectReferenceValue = null;
                        newComponentItem.FindPropertyRelative("mComponentType").intValue = 0;
                        break;
                    }

                    if (GUILayout.Button(new GUIContent("-"), GUILayout.Width(25)))
                    {
                        mSerilizeUGUIComponents.DeleteArrayElementAtIndex(dex);
                        break;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(componentItem, true);
                    mUGUIComponentReference.GetComponentByTypeDefine();
                    EditorGUI.EndChangeCheck();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;
        }


        private void DrawAllLocalizationKeyComponents()
        {
            if (EditorGUILayout.PropertyField(mAllLocalizationKeyComponents))
            {
                EditorGUI.indentLevel++;
                // 设置元素个数
                mAllLocalizationKeyComponents.arraySize = EditorGUILayout.IntField("Size", mAllLocalizationKeyComponents.arraySize);

                for (int dex = 0; dex < mAllLocalizationKeyComponents.arraySize; dex++)
                {
                    var componentItem = mAllLocalizationKeyComponents.GetArrayElementAtIndex(dex);
                    EditorGUILayout.BeginHorizontal();
                    //if (GUILayout.Button(new GUIContent("+"), GUILayout.Width(25)))
                    //{
                    //    mAllLocalizationKeyComponents.InsertArrayElementAtIndex(dex + 1);
                    //    var newComponentItem = mAllLocalizationKeyComponents.GetArrayElementAtIndex(dex + 1);
                    //    newComponentItem.FindPropertyRelative("mReferenceComponent").objectReferenceValue = null;
                    //    newComponentItem.FindPropertyRelative("mComponentType").intValue = 0;
                    //    break;
                    //}

                    //if (GUILayout.Button(new GUIContent("-"), GUILayout.Width(25)))
                    //{
                    //    mAllLocalizationKeyComponents.DeleteArrayElementAtIndex(dex);
                    //    break;
                    //}

                    //EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(componentItem, true);
                    //mUGUIComponentReference.GetComponentByTypeDefine();
                    //EditorGUI.EndChangeCheck();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;
        }
 
    }
}