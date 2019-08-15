using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

#endif


namespace GameFramePro.Localization
{
    [System.Serializable]
    internal class ImageProperty
    {
        public bool m_IsPreserveAspect;
        public Color m_ImageColor = Color.white;
        public Image.Type m_ImageType = Image.Type.Simple;
        public Image.FillMethod m_FillMethod = Image.FillMethod.Radial360;
        public float m_FillAmount = 0;

        [Tooltip("---只在m_FillMethod== Image.FillMethod.Radial180 时候有效--")]
        public Image.Origin180 m_fillOrigin180 = Image.Origin180.Bottom;

        [Tooltip("---只在m_FillMethod== Image.FillMethod.Radial360 时候有效--")]
        public Image.Origin360 m_fillOrigin360 = Image.Origin360.Bottom;

        [Tooltip("---只在m_FillMethod== Image.FillMethod.Radial90 时候有效--")]
        public Image.Origin90 m_fillOrigin90 = Image.Origin90.BottomLeft;

        [Tooltip("---只在m_FillMethod== Image.FillMethod.Horizontal 时候有效--")]
        public Image.OriginHorizontal m_fillOriginHorizontal = Image.OriginHorizontal.Left;

        [Tooltip("---只在m_FillMethod== Image.FillMethod.Vertical 时候有效--")]
        public Image.OriginVertical m_fillOriginVertical = Image.OriginVertical.Bottom;


        internal ImageProperty()
        {
        }

        internal ImageProperty(ImageProperty other)
        {
            m_ImageColor = other.m_ImageColor;
            m_ImageType = other.m_ImageType;
            m_IsPreserveAspect = other.m_IsPreserveAspect;
            m_FillMethod = other.m_FillMethod;

            switch (other.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    m_fillOrigin180 = other.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    m_fillOrigin360 = other.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    m_fillOrigin90 = other.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    m_fillOriginHorizontal = other.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    m_fillOriginVertical = other.m_fillOriginVertical;
                    break;
            }

            m_FillAmount = other.m_FillAmount;
        }

        public void CloneFromImage(Image image)
        {
            this.m_ImageColor = image.color;
            this.m_IsPreserveAspect = image.preserveAspect;
            this.m_ImageType = image.type;

            this.m_FillMethod = image.fillMethod;
            this.m_FillAmount = image.fillAmount;

            switch (image.fillMethod)
            {
                case Image.FillMethod.Radial180:
                    this.m_fillOrigin180 = (Image.Origin180) image.fillOrigin;
                    break;
                case Image.FillMethod.Radial360:
                    this.m_fillOrigin360 = (Image.Origin360) image.fillOrigin;
                    break;
                case Image.FillMethod.Radial90:
                    this.m_fillOrigin90 = (Image.Origin90) image.fillOrigin;
                    break;
                case Image.FillMethod.Horizontal:
                    this.m_fillOriginHorizontal = (Image.OriginHorizontal) image.fillOrigin;
                    break;
                case Image.FillMethod.Vertical:
                    this.m_fillOriginVertical = (Image.OriginVertical) image.fillOrigin;
                    break;
            }
        }

        public void CloneToImage(Image image)
        {
            image.type = this.m_ImageType;
            image.color = this.m_ImageColor;
            image.preserveAspect = this.m_IsPreserveAspect;
            if (image.type != Image.Type.Filled) return;

            image.fillMethod = this.m_FillMethod;
            switch (this.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    image.fillOrigin = (int) this.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    image.fillOrigin = (int) this.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    image.fillOrigin = (int) this.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    image.fillOrigin = (int) this.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    image.fillOrigin = (int) this.m_fillOriginVertical;
                    break;
            }

            image.fillAmount = this.m_FillAmount;
        }
    } //图片可配置的属性

    [System.Serializable]
    internal class ImageConfigure
    {
#if UNITY_EDITOR
        [HideInInspector] public string m_LanguageName; //显示用的
#endif
        public LocalizationLanguage m_Language { get; private set; }

        [Tooltip("标示m_SourceImage属性的值是否会在运行时修改对应Image的Sprite属性,默认启动")]
        public bool mIsImageSpriteEnable = true;

        public Sprite m_SourceImage = null;


        [Tooltip("标示m_ImageProperty属性的值是否会在运行时修改对应Image的属性,默认不启动")]
        public bool mIsImagePropertyEnable = false;

        [Space(10)]
        public ImageProperty m_ImageProperty;

        public ImageConfigure(LocalizationLanguage language)
        {
            m_Language = language;
            m_ImageProperty = new ImageProperty();

#if UNITY_EDITOR
            m_LanguageName = m_Language.ToString();
#endif
        }

        public ImageConfigure(LocalizationLanguage language, ImageProperty imageProperty)
        {
            m_Language = language;
            m_ImageProperty = new ImageProperty(imageProperty);

#if UNITY_EDITOR
            m_LanguageName = m_Language.ToString();
#endif
        }
    } //每一种语言类型对应的配置属性


#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ImageProperty))]
    internal class ImagePropertyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty m_IsPreserveAspect = property.FindPropertyRelative("m_IsPreserveAspect");
            SerializedProperty m_ImageColor = property.FindPropertyRelative("m_ImageColor");
            SerializedProperty m_ImageType = property.FindPropertyRelative("m_ImageType");
            SerializedProperty m_FillMethod = property.FindPropertyRelative("m_FillMethod");
            SerializedProperty m_fillOriginHorizontal = property.FindPropertyRelative("m_fillOriginHorizontal");
            SerializedProperty m_fillOriginVertical = property.FindPropertyRelative("m_fillOriginVertical");
            SerializedProperty m_fillOrigin90 = property.FindPropertyRelative("m_fillOrigin90");
            SerializedProperty m_fillOrigin180 = property.FindPropertyRelative("m_fillOrigin180");
            SerializedProperty m_fillOrigin360 = property.FindPropertyRelative("m_fillOrigin360");
            SerializedProperty m_FillAmount = property.FindPropertyRelative("m_FillAmount");

            EditorGUI.BeginProperty(position, label, property); // 开始绘制属性

            Rect basePropertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);


            EditorGUI.PropertyField(basePropertyRect, m_IsPreserveAspect);
            EditorGUI.PropertyField(basePropertyRect.AddRectY(EditorGUIUtility.singleLineHeight), m_ImageColor);
            EditorGUI.PropertyField(basePropertyRect.AddRectY(2 * EditorGUIUtility.singleLineHeight), m_ImageType);

            var imageType = (Image.Type) (m_ImageType.intValue);
            if (imageType == Image.Type.Filled)
            {
                EditorGUI.PropertyField(basePropertyRect.AddRectY(3 * EditorGUIUtility.singleLineHeight), m_FillMethod);
                Image.FillMethod fillMethod = (Image.FillMethod) (m_FillMethod.intValue);

                switch (fillMethod)
                {
                    case Image.FillMethod.Horizontal:
                        EditorGUI.PropertyField(basePropertyRect.AddRectY(4 * EditorGUIUtility.singleLineHeight), m_fillOriginHorizontal);
                        break;
                    case Image.FillMethod.Vertical:
                        EditorGUI.PropertyField(basePropertyRect.AddRectY(4 * EditorGUIUtility.singleLineHeight), m_fillOriginVertical);
                        break;
                    case Image.FillMethod.Radial90:
                        EditorGUI.PropertyField(basePropertyRect.AddRectY(4 * EditorGUIUtility.singleLineHeight), m_fillOrigin90);
                        break;
                    case Image.FillMethod.Radial180:
                        EditorGUI.PropertyField(basePropertyRect.AddRectY(4 * EditorGUIUtility.singleLineHeight), m_fillOrigin180);
                        break;
                    case Image.FillMethod.Radial360:
                        EditorGUI.PropertyField(basePropertyRect.AddRectY(4 * EditorGUIUtility.singleLineHeight), m_fillOrigin360);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                m_FillAmount.floatValue = EditorGUI.Slider(basePropertyRect.AddRectY(5 * EditorGUIUtility.singleLineHeight), new GUIContent("m_FillAmount"), m_FillAmount.floatValue, 0, 1f);
            }

            EditorGUI.EndProperty(); // 完成绘制
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var imageType = (Image.Type) (property.FindPropertyRelative("m_ImageType").intValue);
            if (imageType == Image.Type.Filled)
                return base.GetPropertyHeight(property, label) + 5f * EditorGUIUtility.singleLineHeight;
            return base.GetPropertyHeight(property, label) + 2f * EditorGUIUtility.singleLineHeight;
        }
    }


//    [CustomPropertyDrawer(typeof(ImageConfigure))]
//    internal class ImageConfigurePropertyDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            SerializedProperty mIsImageSpriteEnable = property.FindPropertyRelative("mIsImageSpriteEnable");
//            SerializedProperty m_SourceImage = property.FindPropertyRelative("m_SourceImage");
//            SerializedProperty mIsImagePropertyEnable = property.FindPropertyRelative("mIsImagePropertyEnable");
//            SerializedProperty m_ImageProperty = property.FindPropertyRelative("m_ImageProperty");
//
//            Rect basePropertyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//
//            EditorGUI.BeginProperty(position, label, property); // 开始绘制属性
//            EditorGUI.PropertyField(basePropertyRect.AddRectOffset(new Vector2(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight)), m_SourceImage);
//            
//            
//            mIsImageSpriteEnable.boolValue = EditorGUI.Toggle(basePropertyRect, new GUIContent("标示m_SourceImage属性的值是否会在运行时修改对应Image的Sprite属性,默认启动"), mIsImageSpriteEnable.boolValue);
//            if (mIsImageSpriteEnable.boolValue)
//                EditorGUI.PropertyField(basePropertyRect.AddRectOffset(new Vector2(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight)), m_SourceImage);
//
//            if (mIsImageSpriteEnable.boolValue)
//            {
//                mIsImagePropertyEnable.boolValue = EditorGUI.Toggle(basePropertyRect.AddRectY(2 * EditorGUIUtility.singleLineHeight), new GUIContent("标示m_ImageProperty属性的值是否会在运行时修改对应Image的属性,默认不启动"), mIsImagePropertyEnable.boolValue);
//                {
//                    EditorGUI.PropertyField(basePropertyRect.AddRectOffset(new Vector2(EditorGUIUtility.singleLineHeight, 3 * EditorGUIUtility.singleLineHeight)), m_ImageProperty);
//                }
//            }
//            else
//            {
//                mIsImagePropertyEnable.boolValue = EditorGUI.Toggle(basePropertyRect.AddRectY(EditorGUIUtility.singleLineHeight), new GUIContent("标示m_ImageProperty属性的值是否会在运行时修改对应Image的属性,默认不启动"), mIsImagePropertyEnable.boolValue);
//                {
//                    EditorGUI.PropertyField(basePropertyRect.AddRectOffset(new Vector2(EditorGUIUtility.singleLineHeight, 2 * EditorGUIUtility.singleLineHeight)), m_ImageProperty);
//                }
//            }
//            
//            EditorGUI.EndProperty();
//            // base.OnGUI(position, property, label);
//        }
//
//        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//        {
//            SerializedProperty mIsImageSpriteEnable = property.FindPropertyRelative("mIsImageSpriteEnable");
//            SerializedProperty mIsImagePropertyEnable = property.FindPropertyRelative("mIsImagePropertyEnable");
//            SerializedProperty m_ImageProperty = property.FindPropertyRelative("m_ImageProperty");
//
//            float offset = 0;
//            if (mIsImageSpriteEnable.boolValue)
//                offset += 2* EditorGUIUtility.singleLineHeight;
//            else
//                offset +=  EditorGUIUtility.singleLineHeight;
//            
//            if (mIsImagePropertyEnable.boolValue)
//                offset += 2* EditorGUIUtility.singleLineHeight;
//            else
//                offset +=  EditorGUIUtility.singleLineHeight;
//            
//            return base.GetPropertyHeight(property, label)+offset*3;
//        }
//    }

#endif
}
