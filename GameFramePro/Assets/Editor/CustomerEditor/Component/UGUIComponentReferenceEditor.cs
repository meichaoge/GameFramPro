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

            GUILayout.BeginVertical();
            GUILayout.Space(5);

            if (GUILayout.Button("根据ComponentTypeEnum创建Tag", GUILayout.Height(25)))
                mUGUIComponentReference.AddAllUGUIComponentTypeTag();

            if (GUILayout.Button("自动序列化标记Tag 的组件&设置序列化组件对象的Tag", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoRecordReferenceWithTag();
                mUGUIComponentReference.AutoSetSerializaComponentTag();

            }

            if (GUILayout.Button("检测是否有重名的对象", GUILayout.Height(25)))
                mUGUIComponentReference.CheckIfContainSameNameComponent();

            if (GUILayout.Button("获取当前对象的UI定义", GUILayout.Height(25)))
                UGUIComponentReference.ShowComponentReferenceByConfig(mUGUIComponentReference.gameObject);

            GUILayout.Space(5);
            GUILayout.EndVertical();


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
            serializedObject.ApplyModifiedProperties();
            //       base.OnInspectorGUI();
        }
    }
}