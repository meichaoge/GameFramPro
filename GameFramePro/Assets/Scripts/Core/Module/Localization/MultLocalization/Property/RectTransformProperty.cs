using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.Localization
{
    /// <summary>/// RectTransform 组件的属性定义/// </summary>
    [System.Serializable]
    internal class RectTransformProperty
    {
        public Vector2 m_OffsetMax = Vector2.zero;
        public Vector2 m_OffsetMin = Vector2.zero;
        public Vector2 m_AnchorMax = Vector2.zero;
        public Vector2 m_AnchorMin = Vector2.zero;
        public Vector2 m_Pivot = Vector2.zero;

        //**基础属性
        public Vector3 m_AnchorPosition3D = Vector3.zero;
        public Vector3 m_Angle = Vector3.zero;
        public Vector3 m_Scale = Vector3.zero;

        public RectTransformProperty()
        {
        }

        public RectTransformProperty(RectTransformProperty other)
        {
            m_AnchorPosition3D = other.m_AnchorPosition3D;
            m_OffsetMax = other.m_OffsetMax;
            m_OffsetMin = other.m_OffsetMin;


            m_AnchorMax = other.m_AnchorMax;
            m_AnchorMin = other.m_AnchorMin;
            m_Pivot = other.m_Pivot;
            m_Angle = other.m_Angle;
            m_Scale = other.m_Scale;
        }


        public void CloneFromRectTransform(RectTransform rtf)
        {
            m_AnchorPosition3D = rtf.anchoredPosition3D;
            m_OffsetMax = rtf.offsetMax;
            m_OffsetMin = rtf.offsetMin;

            m_AnchorMax = rtf.anchorMax;
            m_AnchorMin = rtf.anchorMin;
            m_Angle = rtf.localEulerAngles;
            m_Scale = rtf.localScale;
            m_Pivot = rtf.pivot;
        }

        public void CloneToRectTransform(RectTransform rtf)
        {
            rtf.anchoredPosition3D = m_AnchorPosition3D;
            rtf.offsetMax = m_OffsetMax;
            rtf.offsetMin = m_OffsetMin;

            rtf.anchorMax = m_AnchorMax;
            rtf.anchorMin = m_AnchorMin;
            rtf.localEulerAngles = m_Angle;
            rtf.localScale = m_Scale;
            rtf.pivot = m_Pivot;
        }
    } //

    /// <summary>/// RectTransform  属性配置/// </summary>
    [System.Serializable]
    internal class RectTransformConfigure
    {
#if UNITY_EDITOR
        [Header("语言名称")] public string languageName;
#endif
        [Header("是否启用配置，默认不启用，保存配置自动改为启用")] public bool IsEnableProperty = true;

        [HideInInspector] public LocalizationLanguage languageType;

        [Header("配置数据")] public RectTransformProperty property = new RectTransformProperty();

        [Header("标示是否需要修改 RectTransform 的 Anchor和Offset 等属性，默认为 true")]
        public bool mIsAnchorOrOffsetEnable = true;


        internal RectTransformConfigure(LocalizationLanguage language)
        {
            languageType = language;
#if UNITY_EDITOR
            languageName = languageType.ToString();
#endif
        }
    }
}
