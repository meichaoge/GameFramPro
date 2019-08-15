using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace GameFramePro.Localization
{
    [System.Serializable]
    internal class TextProperty
    {
        public Font mFont;
        public int mFontSize;
        public TextAnchor mTextAnchor = TextAnchor.MiddleCenter;
        public HorizontalWrapMode mHorizontalWrapMode = HorizontalWrapMode.Overflow;
        public VerticalWrapMode mVerticalWrapMode = VerticalWrapMode.Overflow;
        public Color mTextColor = Color.black;
        public Material MTextMaterial;


        public TextProperty()
        {
        }

        public TextProperty(TextProperty targetText)
        {
            mFont = targetText.mFont;
            mFontSize = targetText.mFontSize;
            mTextAnchor = targetText.mTextAnchor;
            mHorizontalWrapMode = targetText.mHorizontalWrapMode;
            mVerticalWrapMode = targetText.mVerticalWrapMode;
            mTextColor = targetText.mTextColor;
            MTextMaterial = targetText.MTextMaterial;
        }

        public void CloneFromIText(Text targetText)
        {
            mFont = targetText.font;
            mFontSize = targetText.fontSize;
            mTextAnchor = targetText.alignment;
            mHorizontalWrapMode = targetText.horizontalOverflow;
            mVerticalWrapMode = targetText.verticalOverflow;
            mTextColor = targetText.color;
            MTextMaterial = targetText.material;
        }
    }

    /// <summary>/// 配置的Text 属性/// </summary>
    [System.Serializable]
    internal class TextConfigure
    {
#if UNITY_EDITOR
        public string m_LanguageName; //显示用的
#endif
        internal LocalizationLanguage m_Language;

        [FormerlySerializedAs("mIsTextConfigureEnable")] [Header("标示 mSourceText 属性本地化配置是否启用,默认启动")]
        public bool mIsConfigureEnable = true;

        public TextProperty m_TextProperty;


        [Header("标示 mSourceText 属性的值是否会在运行时修改字体属性,默认不启动")]
        public bool mIsChangeFontProperty = false;


        [Header("标示 mSourceText 属性的值是否会在运行时修改移除模式和对齐模式,默认启动")]
        public bool mIsChangeWrapModeOrAnchor = true;


        internal TextConfigure()
        {
        }

        internal TextConfigure(LocalizationLanguage language)
        {
            m_Language = language;
            m_TextProperty = new TextProperty();

#if UNITY_EDITOR
            m_LanguageName = m_Language.ToString();
#endif
        }

        internal TextConfigure(LocalizationLanguage language, TextProperty textProperty)
        {
            m_Language = language;
            m_TextProperty = new TextProperty(textProperty);

#if UNITY_EDITOR
            m_LanguageName = m_Language.ToString();
#endif
        }
    }


}
