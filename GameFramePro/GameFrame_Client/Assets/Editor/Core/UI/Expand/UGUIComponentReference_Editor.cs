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
                AutoReMapTags();
            }

            if (GUILayout.Button("自动关联多语言key", GUILayout.Height(25)))
            {
                AutoConnectLocalizationKeyComponent();
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

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(componentItem, true);
                    mUGUIComponentReference.GetComponentByTypeDefine();
                    EditorGUI.EndChangeCheck();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;
        }



        /// <summary>
        /// 重新映射Tags
        /// </summary>
        private void AutoReMapTags()
        {
            Transform[] allChilds = mUGUIComponentReference.transform.GetComponentsInChildren<Transform>();
            foreach (var child in allChilds)
            {
                try
                {
                    if (string.IsNullOrEmpty(child.tag)) continue;
                }
                catch (Exception e)
                {
                    Debug.LogError($"异常节点{child.name} : {e}");
                }

                #region 重新映射Tags
                bool isNeedUpdate = false;
                string old = child.tag;
                if (child.tag == "UI.Text")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIText.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Image")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIImage.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.RawImage")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIRawImage.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Button")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIButton.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Toggle")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIToggle.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Slider")
                {
                    child.tag = UGUIComponentTypeEnum.UGUISlider.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Scrollbar")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIScrollbar.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Dropdown")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIDropDown.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.InputField")
                {
                    child.tag = UGUIComponentTypeEnum.UGUIInputField.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UI.Canvas")
                {
                    child.tag = UGUIComponentTypeEnum.UICanvas.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UnityEngine.GameObject")
                {
                    child.tag = UGUIComponentTypeEnum.GameObject.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UnityEngine.Transform")
                {
                    child.tag = UGUIComponentTypeEnum.Transform.ToString(); isNeedUpdate = true;
                }
                else if (child.tag == "UnityEngine.RectTransform")
                {
                    child.tag = UGUIComponentTypeEnum.RectTransform.ToString(); isNeedUpdate = true;
                }

                if (isNeedUpdate)
                {
                    Debug.Log($"更新节点{child.name} OldTag={old} 为新的{child.tag }");
                }
                #endregion

            }
            mUGUIComponentReference.RemoveAllUnDefineComponentTag();
        }


        private void AutoConnectLocalizationKeyComponent()
        {
            Text[] allChilds = mUGUIComponentReference.transform.GetComponentsInChildren<Text>();
            foreach (var child in allChilds)
            {
                if (string.IsNullOrEmpty(child.text)) continue;
                if (LocalizationManager.CheckIfMatchLocalizationKey(child.text) == false) continue;
                LocalizationKeyComponent localizationKey = child.transform.GetComponent<LocalizationKeyComponent>();

                if (localizationKey == null)
                {
                    localizationKey = child.transform.GetAddComponent<LocalizationKeyComponent>();
                    Debug.Log($"-->>节点 {child.name} 添加本地化多语言组件");

                    localizationKey.BindTargetText();
                    localizationKey.UpdateConnectLocalizationKey(child.text, false);
                }

                mUGUIComponentReference.AutoRecordLocalizationKeyComponent(localizationKey);
            }
        }
    }
}