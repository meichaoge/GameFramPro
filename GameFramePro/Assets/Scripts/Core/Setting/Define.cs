using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    #region UI 相关的名称和路径定义

    public static class NameDefine
    {
        #region Common
        public static string UIProgressChangePageName => "UIProgressChangePage"; //进度条弹窗

        

        #endregion
        
        
        #region Login

        public static string UILoginChangePageName => "UILoginChangePage";
        public static string UILoginTipPopWindowName => "UILoginTipPopWindow";
        public static string UITipInforWidgetName => "UITipInforWidget";

        #endregion

        #region Home

        public static string UIHomeChangePageName => "UIHomeChangePage";

        #endregion
    }

    public static class PathDefine
    {
        #region Common
        public static string UIProgressChangePagePath=> "Prefabs/UI/Common/UIProgressChangePage"; //进度条弹窗

        

        #endregion
        
        #region Login

        public static string UILoginChangePagePath => "Prefabs/UI/Login/UILoginChangePage";
        public static string UILoginTipPopWindowPath => "Prefabs/UI/Login/UILoginTipPopWindow";
        public static string UITipInforWidgetPath => "Prefabs/UI/Login/UITipInforWidget";

        #endregion

        #region Home

        public static string UIHomeChangePagePath => "Prefabs/UI/Home/UIHomeChangePage";

        #endregion
    }

    #endregion

    /// <summary>/// 定义使用的特效相关属性/// </summary>
    public static class EffectDefine
    {
        public static string S_ScreenClickEffectPath => "UIEffects/ScreenClickEffect"; //屏幕点击特效路径

    }


    /// <summary>/// 定义项目用到的  本地持久化 key/// </summary>
    public static class PlayerPrefsKeyDefine
    {
        #region Localization 本地化

        public static string LocalizationLanguage_Key => "LastLocalizationLanguage"; //上一次选择的本地化语言

        #endregion
    }
}
