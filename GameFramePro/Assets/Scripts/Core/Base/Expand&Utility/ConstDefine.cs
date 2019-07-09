using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    public static class ConstDefine
    {
        private static string s_ResourcesRealPath = string.Empty;
        /// <summary>
        /// Resources 绝对路径
        /// </summary>
        public static string S_ResourcesRealPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_ResourcesRealPath))
                    s_ResourcesRealPath = string.Format("{0}/Resources", Application.dataPath);
                return s_ResourcesRealPath;
            }
        }

        public static string S_MetaExtension = ".meta"; //meta 扩展名
    }
}
