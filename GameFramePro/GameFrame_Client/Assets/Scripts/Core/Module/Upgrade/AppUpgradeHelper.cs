using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.Localization;

namespace GameFramePro.Upgrade
{
    /// <summary>/// 对更新模块屏蔽外部支持的语言 ，这里需要更新的资源支持的语言/// </summary>
    ///每种新增的语言都需要制定值 避免增加语言时候覆盖其他的配置
    public enum UpgradeLanguage
    {
        //  Unknow = 0, // 未知
        Chinese = 1, //简体中文
        English = 3, //英语
    }


    internal static class AppUpgradeHelper
    {
        /// <summary>/// 外部支持的语言转成本地支持的语言/// </summary>
        public static UpgradeLanguage TransformLanguage(Language language)
        {
            switch (language)
            {
                case Language.zh_CN:
                case Language.zh_HK:
                    return UpgradeLanguage.Chinese;
                case Language.en_US:
                case Language.en_GB:
                    return UpgradeLanguage.English;
                default:
                    Debug.LogError($"TransformLanguage Fail,没有定义的语言{language}");
                    return UpgradeLanguage.Chinese;
            }
        }


        /// <summary>/// 获取当前的多语言/// </summary>
        public static UpgradeLanguage GetCurUpgradeLanguage()
        {
            var language = LocalizationManager.CurLanguage;
            return TransformLanguage(language);
        }
    }
}
