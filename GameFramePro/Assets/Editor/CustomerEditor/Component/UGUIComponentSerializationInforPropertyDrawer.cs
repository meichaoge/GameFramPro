using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using GameFramePro.UI;

namespace GameFramePro.EditorEx
{
    [CustomPropertyDrawer(typeof(UGUIComponentReference.UGUIComponentSerializationInfor))]
    public class UGUIComponentSerializationInforPropertyDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property); // 开始绘制属性
                                                                // 获取属性前值 label, 就是显示在此属性前面的那个名称 label
                                                                //   position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float EditorLableWidth = EditorGUIUtility.labelWidth; //需要先保存这个值 然后还原

            float lableWidth = 50;
            var labelRect = new Rect(position.x, position.y, lableWidth, position.height); //显示前面的Lable

            float contentInforWidth = position.width - lableWidth; //内容区间的宽度

            var mReferenceComponentRect = new Rect(position.x + lableWidth, position.y, contentInforWidth * 0.6f, position.height);
            var mComponentTypeRect = new Rect(mReferenceComponentRect.x + mReferenceComponentRect.width, position.y, contentInforWidth * 0.4f, position.height);

            EditorGUI.LabelField(labelRect, label);
            EditorGUIUtility.labelWidth = contentInforWidth * 0.2f; //控制每个属性文本区间大小
            EditorGUI.PropertyField(mReferenceComponentRect, property.FindPropertyRelative("mReferenceComponent"), new GUIContent("Component"));
            EditorGUI.PropertyField(mComponentTypeRect, property.FindPropertyRelative("mComponentType"), new GUIContent("ComponentType"));



            EditorGUIUtility.labelWidth = EditorLableWidth;
            EditorGUI.EndProperty();          // 完成绘制
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }





    }
}