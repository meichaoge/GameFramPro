using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    public static class NameDefine
    {
        #region Login

        public  static string UILoginChangePageName { get { return "UILoginChangePage"; } }
        public static string UILoginTipPopWindowName { get { return "UILoginTipPopWindow"; } }
        #endregion


        #region Home

        public static string UIHomeChangePageName { get { return "UIHomeChangePage"; } }
        #endregion


    }

    public static class PathDefine
    {
        #region Login

        public static string UILoginChangePagePath { get { return "Prefabs/UI/Login/UILoginChangePage"; } }
        public static string UILoginTipPopWindowPath { get { return "Prefabs/UI/Login/UILoginTipPopWindow"; } }

        #endregion

        #region Home
        public static string UIHomeChangePagePath { get { return "Prefabs/UI/Home/UIHomeChangePage"; } }

        #endregion

    }




    /// <summary>
    /// 定义项目用到的  本地持久化 key
    /// </summary>
    public static class PlayerPrefsKeyDefine
    {
        #region Localization 本地化
        //上一次选择的本地化语言
        public static string LocalizationLanguage_Key{ get { return "LastLocalizationLanguage"; } }
        #endregion

    }


}