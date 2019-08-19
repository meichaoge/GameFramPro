using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace GameFramePro.Localization
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    internal partial class LocalizationImage : LocalizationBase
    {
        private Image _TargetImage;

        public Image m_TargetImage
        {
            get
            {
                if (_TargetImage == null)
                    _TargetImage = transform.GetComponent<Image>();
                return _TargetImage;
            }
            set { _TargetImage = value; }
        }

        internal override LocalizationComponentUsage LocalizationUsage { get; set; } = LocalizationComponentUsage.Image;

        [SerializeField] internal List<ImageConfigure> m_ImageLanguageConfige = new List<ImageConfigure>();


        internal override bool ShowView(LocalizationLanguage languageType)
        {
            if (languageType == mShowLanguage)
            {
                return false;
            }
#if UNITY_EDITOR
            Debug.Log("刷新显示视图  LocalizationImage" + languageType);
#endif

            var config = m_ImageLanguageConfige.FirstOrDefault(x => x.m_Language == languageType);


            if (config == null)
            {
                Debug.LogError(string.Format("当前图片{0  }没有配置对应的语言{1}  ", gameObject.name, languageType));
                return false;
            }

            SetmageProperty(config);
            mShowLanguage = languageType;
            return true;
        }

        /// <summary>/// 切换设置Image 属性/// </summary>
        private void SetmageProperty(ImageConfigure config)
        {
#if UNITY_EDITOR
            if (config.m_SourceImage == null)
                Debug.LogError($"当前语言 {config.m_Language}  Name={gameObject.name}  上图片没有找到");
#endif

            if (config.mIsImageSpriteEnable)
                m_TargetImage.sprite = config.m_SourceImage;


            if (config.mIsImagePropertyEnable == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"当前语言 {config.m_Language}  Name={gameObject.name}  上关闭了属性修改的功能，无法切换图片属性");
#endif
                return; //不启用这些属性
            }

            m_TargetImage.type = config.m_ImageProperty.m_ImageType;
            m_TargetImage.color = config.m_ImageProperty.m_ImageColor;
            m_TargetImage.preserveAspect = config.m_ImageProperty.m_IsPreserveAspect;

            if (m_TargetImage.type != Image.Type.Filled)
                return;

            m_TargetImage.fillMethod = config.m_ImageProperty.m_FillMethod;
            switch (config.m_ImageProperty.m_FillMethod)
            {
                case Image.FillMethod.Radial180:
                    m_TargetImage.fillOrigin = (int) config.m_ImageProperty.m_fillOrigin180;
                    break;
                case Image.FillMethod.Radial360:
                    m_TargetImage.fillOrigin = (int) config.m_ImageProperty.m_fillOrigin360;
                    break;
                case Image.FillMethod.Radial90:
                    m_TargetImage.fillOrigin = (int) config.m_ImageProperty.m_fillOrigin90;
                    break;
                case Image.FillMethod.Horizontal:
                    m_TargetImage.fillOrigin = (int) config.m_ImageProperty.m_fillOriginHorizontal;
                    break;
                case Image.FillMethod.Vertical:
                    m_TargetImage.fillOrigin = (int) config.m_ImageProperty.m_fillOriginVertical;
                    break;
            }

            m_TargetImage.fillAmount = config.m_ImageProperty.m_FillAmount;
        }
    }


#if UNITY_EDITOR
    internal partial class LocalizationImage
    {
        #region 菜单

        [MenuItem("CONTEXT/LocalizationImage/一键启用 mIsImageSpriteEnable 属性")]
        private static void EnableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationImage current = cmd.context as LocalizationImage;
            if (current == null)
                return;

            foreach (var item in current.m_ImageLanguageConfige)
                item.mIsImageSpriteEnable = true;

            Debug.Log("一键启用 mIsImageSpriteEnable 属性完成");
        }

        [MenuItem("CONTEXT/LocalizationImage/一键禁用 mIsImageSpriteEnable 属性")]
        private static void DisableImageSpriteProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationImage current = cmd.context as LocalizationImage;
            if (current == null)
                return;

            foreach (var item in current.m_ImageLanguageConfige)
                item.mIsImageSpriteEnable = false;

            Debug.Log("一键禁用 mIsImageSpriteEnable 属性完成");
        }


        [MenuItem("CONTEXT/LocalizationImage/一键启用 mIsImagePropertyEnable 属性")]
        private static void EnableImageDetailProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationImage current = cmd.context as LocalizationImage;
            if (current == null)
                return;

            foreach (var item in current.m_ImageLanguageConfige)
                item.mIsImagePropertyEnable = true;

            Debug.Log("一键启用 mIsImagePropertyEnable 属性完成");
        }

        [MenuItem("CONTEXT/LocalizationImage/一键禁用 mIsImagePropertyEnable 属性")]
        private static void DisableImageDetailProperty(UnityEditor.MenuCommand cmd)
        {
            LocalizationImage current = cmd.context as LocalizationImage;
            if (current == null)
                return;

            foreach (var item in current.m_ImageLanguageConfige)
                item.mIsImagePropertyEnable = false;

            Debug.Log("一键禁用 mIsImagePropertyEnable 属性完成");
        }

        #endregion

        protected override void Reset()
        {
            m_ImageLanguageConfige.Clear();
            foreach (var language in System.Enum.GetValues(typeof(LocalizationLanguage)))
            {
                var languageType = (LocalizationLanguage) System.Enum.Parse(typeof(LocalizationLanguage), language.ToString());
                ImageConfigure config = new ImageConfigure(languageType);
                config.m_SourceImage = m_TargetImage.sprite;

                if (m_ImageLanguageConfige.Count == 0)
                {
                    GetmageProperty(ref config.m_ImageProperty);
                    m_ImageLanguageConfige.Add(config);
                    continue;
                } //第一个配置为当前的配置

                if (IsExitLanguageConfigure(languageType, m_ImageLanguageConfige))
                    continue; //重复的配置则过滤


                GetmageProperty(ref config.m_ImageProperty);
                m_ImageLanguageConfige.Add(config);
            }

            base.Reset();
        }


        internal override bool NewViewConfig(LocalizationLanguage languageType)
        {
            var config = m_ImageLanguageConfige.FirstOrDefault(x => x.m_Language == languageType);
            if (config != null)
            {
                Debug.LogError($"已经存在当前语言下的配置{languageType}");
                return false;
            }

            config = new ImageConfigure(languageType);
            config.m_ImageProperty.CloneFromImage(m_TargetImage);
            m_ImageLanguageConfige.Add(config);
            Debug.Log("保存当前语言配置成功 ...");
            return true;
        }

        internal override bool SaveView(LocalizationLanguage languageType)
        {
            var config = new ImageConfigure(CurEnditorLanguageType);
            config.m_SourceImage = m_TargetImage.sprite;
            GetmageProperty(ref config.m_ImageProperty);

            foreach (var targetConfig in m_ImageLanguageConfige)
            {
                if (targetConfig.m_Language == config.m_Language)
                {
                    targetConfig.m_ImageProperty = config.m_ImageProperty;
                    targetConfig.m_SourceImage = config.m_SourceImage;
                    Debug.Log("保存当前语言配置成功 ...");
                    return true;
                }
            }

            Debug.LogError("保存配置失败 ，不存在这个语言的配置" + languageType);
            return false;
        }


        /// <summary>/// 初始化图片的基本属性/// </summary>
        private void GetmageProperty(ref ImageProperty imageProperty)
        {
            if (m_TargetImage == null)
                return;

            if (imageProperty == null)
                imageProperty = new ImageProperty();

            imageProperty.m_ImageColor = m_TargetImage.color;
            imageProperty.m_IsPreserveAspect = m_TargetImage.preserveAspect;
            imageProperty.m_ImageType = m_TargetImage.type;

            imageProperty.m_FillMethod = m_TargetImage.fillMethod;
            imageProperty.m_FillAmount = m_TargetImage.fillAmount;

            switch (m_TargetImage.fillMethod)
            {
                case Image.FillMethod.Radial180:
                    imageProperty.m_fillOrigin180 = (Image.Origin180) m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Radial360:
                    imageProperty.m_fillOrigin360 = (Image.Origin360) m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Radial90:
                    imageProperty.m_fillOrigin90 = (Image.Origin90) m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Horizontal:
                    imageProperty.m_fillOriginHorizontal = (Image.OriginHorizontal) m_TargetImage.fillOrigin;
                    break;
                case Image.FillMethod.Vertical:
                    imageProperty.m_fillOriginVertical = (Image.OriginVertical) m_TargetImage.fillOrigin;
                    break;
            }
        }


        //检测当前语言是否已经配置过
        private bool IsExitLanguageConfigure(LocalizationLanguage language, List<ImageConfigure> dataSource)
        {
            for (int dex = 0; dex < dataSource.Count; ++dex)
            {
                if (dataSource[dex].m_Language == language)
                    return true;
            }

            return false;
        }
    }
    
    
    
    
    
    
#endif
}
