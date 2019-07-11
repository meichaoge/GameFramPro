using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 提供编辑器下常用的一些有效的功能
    /// </summary>
    public static class EditorHelperUtility
    {
#if UNITY_EDITOR

        /// <summary>
        /// 获取texture的原始文件尺寸
        /// </summary>
        public static void GetOriginalSize(TextureImporter importer, out int width, out int height)
        {
            object[] args = new object[2] { 0, 0 };
            MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            mi.Invoke(importer, args);
            width = (int)args[0];
            height = (int)args[1];
        }

   
#endif
    }
}