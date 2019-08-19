using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 编辑器下key 定义
    /// </summary>
    public static class EditorPlayerPrefsKeyDefine
    {
        public static string S_LastSelectLocalizationExcelPath { get { return "Editor_LastSelectLocalizationExcelPath"; } } //记录上一次选择的Localization Excel
    }
}