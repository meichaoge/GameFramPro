using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.Localization
{
    /// <summary>/// 对本地化模块屏蔽外部支持的语言 ，这里是本地化使用的语言/// </summary>
    ///每种新增的语言都需要制定值 避免增加语言时候覆盖其他的配置
    internal enum LocalizationLanguage
    {
        Unknow = 0, // 未知
        Chinese = 1, //简体中文
        English = 3, //英语
    }


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
            Language language = LocalizationManager.CurLanguage;
            return TransformLanguage(language);
        }
    }
}
