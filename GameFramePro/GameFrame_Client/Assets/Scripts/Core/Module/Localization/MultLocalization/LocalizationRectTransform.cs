using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace GameFramePro.Localization
{
    /// <summary>/// RectTransform本地化组件/// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    internal partial class LocalizationRectTransform : LocalizationBase
    {
        private RectTransform mTargetRectTransform = null;

        public RectTransform TargetRectTransform
        {
            get
            {
                if (mTargetRectTransform == null)
                    mTargetRectTransform = transform as RectTransform;
                return mTargetRectTransform;
            }
        }

        internal override LocalizationComponentUsage LocalizationUsage { get; set; } = LocalizationComponentUsage.RectTransform;


        [Header("多语言配置数据")] public List<RectTransformConfigure> mTotalLanguageConfigs = new List<RectTransformConfigure>();


        /// <summary>/// 初始化视图/// </summary>
        internal override bool ShowView(LocalizationLanguage languageType)
        {
            if (languageType == mShowLanguage)
            {
                return false;
            }
#if UNITY_EDITOR
            Debug.Log("刷新显示视图  LocalizationRectTransform" + languageType);
#endif
            RectTransformConfigure config = mTotalLanguageConfigs.FirstOrDefault(x => x.languageType == languageType);
            if (config == null)
                return false;

            SetRectTransformProperty(config);
            mShowLanguage = languageType;
            return true;
        }

        /// <summary>/// 切换设置 RectTransform 属性/// </summary>
        protected void SetRectTransformProperty(RectTransformConfigure configure)
        {
            if (configure.IsEnableProperty == false)
            {
#if UNITY_EDITOR

                Debug.Log($"无法设置设置指定组件{gameObject.name} 上RectTransform 的属性，当前语言{configure.languageType} 已经被禁言了 ");
#endif
                return;
            }


            if (configure.mIsAnchorOrOffsetEnable)
            {
                TargetRectTransform.anchorMax = configure.property.m_AnchorMax;
                TargetRectTransform.anchorMin = configure.property.m_AnchorMin;
                TargetRectTransform.pivot = configure.property.m_Pivot;

                TargetRectTransform.offsetMax = configure.property.m_OffsetMax;
                TargetRectTransform.offsetMin = configure.property.m_OffsetMin;
            }

            TargetRectTransform.localScale = configure.property.m_Scale;
            TargetRectTransform.eulerAngles = configure.property.m_Angle;
            TargetRectTransform.anchoredPosition = configure.property.m_AnchorPosition3D;
        }
    }


#if UNITY_EDITOR
    internal partial class LocalizationRectTransform
    {
        #region 菜单

        [MenuItem("CONTEXT/LocalizationRectTransform/启用 IsEnableProperty 属性")]
        private static void EnableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationRectTransform current = cmd.context as LocalizationRectTransform;
            if (current == null)
                return;

            foreach (var item in current.mTotalLanguageConfigs)
            {
                item.IsEnableProperty = true;
            }

            Debug.Log("启用 IsEnableProperty 属性完成");
        }

        [MenuItem("CONTEXT/LocalizationRectTransform/禁用 IsEnableProperty 属性")]
        private static void DisableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationRectTransform current = cmd.context as LocalizationRectTransform;
            if (current == null)
                return;

            foreach (var item in current.mTotalLanguageConfigs)
            {
                item.IsEnableProperty = false;
            }

            Debug.Log("禁用 IsEnableProperty 属性完成");
        }

        #endregion


        protected override void Reset()
        {
            mTotalLanguageConfigs.Clear();
            foreach (var obj in System.Enum.GetValues(typeof(LocalizationLanguage)))
            {
                var languageType = (LocalizationLanguage) obj;
                var config = new RectTransformConfigure(languageType);
                config.property.CloneFromRectTransform(TargetRectTransform);
                mTotalLanguageConfigs.Add(config);
            }

            base.Reset();
        }

        /// <summary>/// 保存视图/// </summary>
        internal override bool SaveView(LocalizationLanguage languageType)
        {
            RectTransformConfigure config = mTotalLanguageConfigs.FirstOrDefault(x => x.languageType == languageType);
            if (config == null)
                return false;
            config.property.CloneFromRectTransform(TargetRectTransform);
            return true;
        }

        /// <summary>/// 新建视图/// </summary>
        internal override bool NewViewConfig(LocalizationLanguage languageType)
        {
            RectTransformConfigure config = mTotalLanguageConfigs.FirstOrDefault(x => x.languageType == languageType);
            if (config != null)
            {
                Debug.LogError($"已经存在当前语言下的配置{languageType}");
                return false;
            }

            config = new RectTransformConfigure(languageType);
            config.property.CloneFromRectTransform(TargetRectTransform);
            mTotalLanguageConfigs.Add(config);
            return true;
        }
    }

#endif
}
