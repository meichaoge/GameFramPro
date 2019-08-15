using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.Localization
{
    internal static class LocalizationHelper
    {
        /// <summary>/// 外部支持的语言转成本地支持的语言/// </summary>
        public static LocalizationLanguage TransformLanguage(Language language)
        {
            switch (language)
            {
                case Language.zh_CN:
                case Language.zh_HK:
                    return LocalizationLanguage.Chinese;
                case Language.en_US:
                case Language.en_GB:
                    return LocalizationLanguage.English;
                default:
                    Debug.LogError($"TransformLanguage Fail,没有定义的语言{language}");
                    return LocalizationLanguage.Unknow;
            }
        }


        /// <summary>/// 获取当前的多语言/// </summary>
        public static LocalizationLanguage GetCurLocalizationLanguage()
        {
            Language language = LocalizationManager.S_Instance.CurLanguage;
            return TransformLanguage(language);
        }
    }
}
