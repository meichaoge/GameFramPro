using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace GameFramePro.Localization
{
    /// <summary>/// Text组件本地化脚本/// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    internal partial class LocalizationText : LocalizationBase
    {
        private Text mTargetText;

        public Text TargetText
        {
            get
            {
                if (mTargetText == null)
                    mTargetText = transform.GetComponent<Text>();
                return mTargetText;
            }
            set { mTargetText = value; }
        }

        internal override LocalizationComponentUsage LocalizationUsage { get; set; } = LocalizationComponentUsage.Text;

        //各种语言下的配置
        [SerializeField] internal List<TextConfigure> mTextConfigures = new List<TextConfigure>();

        internal override bool ShowView(LocalizationLanguage languageType)
        {
            if (languageType == mShowLanguage)
            {
                return false;
            }
#if UNITY_EDITOR
            Debug.Log("刷新显示视图  LocalizationText" + languageType);
#endif
            var config = mTextConfigures.FirstOrDefault(x => x.m_Language == languageType);
            if (config == null)
            {
                Debug.LogError($"不存在指定语言{languageType} 的 TextProperty ");
                return false;
            }

            SetTextProperty(config);
            mShowLanguage = languageType;
            return true;
        }


        /// <summary>/// 切换设置 Text 属性/// </summary>
        private void SetTextProperty(TextConfigure config)
        {
            if (config.mIsConfigureEnable == false)
                return; //不启用这些属性

            if (config.mIsChangeFontProperty)
            {
                TargetText.font = config.m_TextProperty.mFont;
                TargetText.material = config.m_TextProperty.MTextMaterial;
            }

            if (config.mIsChangeWrapModeOrAnchor)
            {
                TargetText.alignment = config.m_TextProperty.mTextAnchor;
                TargetText.horizontalOverflow = config.m_TextProperty.mHorizontalWrapMode;
                TargetText.verticalOverflow = config.m_TextProperty.mVerticalWrapMode;
            }


            TargetText.fontSize = config.m_TextProperty.mFontSize;
            TargetText.color = config.m_TextProperty.mTextColor;
        }
    }


#if UNITY_EDITOR
    internal partial class LocalizationText
    {
        #region 菜单

        [MenuItem("CONTEXT/LocalizationText/启用 mIsChangeFontProperty 属性")]
        private static void EnableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            var current = cmd.context as LocalizationText;
            if (current == null)
                return;

            foreach (var item in current.mTextConfigures)
                item.mIsChangeFontProperty = true;

            Debug.Log("启用 mIsChangeFontProperty 属性完成");
        }

        [MenuItem("CONTEXT/LocalizationText/禁用 mIsChangeFontProperty 属性")]
        private static void DisableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            var current = cmd.context as LocalizationText;
            if (current == null)
                return;

            foreach (var item in current.mTextConfigures)
                item.mIsChangeFontProperty = false;

            Debug.Log("禁用 mIsChangeFontProperty 属性完成");
        }


        [MenuItem("CONTEXT/LocalizationText/启用 mIsChangeWrapModeOrAnchor 属性")]
        private static void EnableImageDetailProperty(UnityEditor.MenuCommand cmd)
        {
            var current = cmd.context as LocalizationText;
            if (current == null)
                return;

            foreach (var item in current.mTextConfigures)
                item.mIsChangeWrapModeOrAnchor = true;

            Debug.Log("启用 mIsChangeWrapModeOrAnchor 属性完成");
        }

        [MenuItem("CONTEXT/LocalizationText/禁用 mIsChangeWrapModeOrAnchor 属性")]
        private static void DisableImageDetailProperty(UnityEditor.MenuCommand cmd)
        {
            var current = cmd.context as LocalizationText;
            if (current == null)
                return;

            foreach (var item in current.mTextConfigures)
                item.mIsChangeWrapModeOrAnchor = false;

            Debug.Log("禁用 mIsChangeWrapModeOrAnchor 属性完成");
        }

        #endregion


        protected override void Reset()
        {
            mTextConfigures.Clear();
            foreach (var obj in System.Enum.GetValues(typeof(LocalizationLanguage)))
            {
                var languageType = (LocalizationLanguage) obj;

                TextConfigure config = new TextConfigure(languageType);
                config.m_TextProperty.CloneFromIText(TargetText);
                mTextConfigures.Add(config);
            }

            base.Reset();
        }

        internal override bool SaveView(LocalizationLanguage Language)
        {
            var config = mTextConfigures.FirstOrDefault(x => x.m_Language == Language);
            if (config == null)
                return false;
            //if (!config.IsEnableProperty)
            //    config.IsEnableProperty = true;

            config.m_TextProperty.CloneFromIText(TargetText);
            return true;
        }

        internal override bool NewViewConfig(LocalizationLanguage languageType)
        {
            var config = mTextConfigures.FirstOrDefault(x => x.m_Language == languageType);
            if (config != null)
            {
                Debug.LogError($"已经存在当前语言下的配置{languageType}");
                return false;
            }

            config = new TextConfigure(languageType);
            config.m_TextProperty.CloneFromIText(TargetText);
            mTextConfigures.Add(config);
            return true;
        }
    }
#endif
}
