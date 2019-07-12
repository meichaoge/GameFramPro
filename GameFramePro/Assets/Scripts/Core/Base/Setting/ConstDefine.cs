using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    public static class ConstDefine
    {
        public static readonly string S_MetaExtension = ".meta"; //meta 扩展名
        public static readonly string S_TextAssetExtension = ".txt"; //文件文件扩展名
        public static readonly string S_PrefabExtension = ".prefab"; //文件文件扩展名


        /// <summary>
        /// Resources 目录名
        /// </summary>
        public static readonly string S_ResourcesName = "Resources";
        public static readonly string S_AssetsName = "Assets";

        public static readonly string S_ExportDirectoryName = "Export"; //项目导出需要提交到CDN的目录名





        private static string s_ResourcesRealPath = string.Empty;
        /// <summary>
        /// Resources 绝对路径
        /// </summary>
        public static string S_ResourcesRealPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_ResourcesRealPath))
                {
                    s_ResourcesRealPath = string.Format(@"{0}/{1}", Application.dataPath, S_ResourcesName);
#if UNITY_EDITOR
                    Debug.LogEditorInfor("s_ResourcesRealPath=" + s_ResourcesRealPath);
#endif
                }
                return s_ResourcesRealPath;
            }
        }


        private static string s_ApplicationAssetParentRealPath = string.Empty;
        /// <summary>
        /// Application.dataPath 的父目录（主要是用在编辑器下组合  AssetDatabase.GetAssetPath()获取的路径）
        /// </summary>
        public static string S_ApplicationAssetParentRealPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_ApplicationAssetParentRealPath))
                {
                    s_ApplicationAssetParentRealPath = Application.dataPath.GetFilePathParentDirectory(1);
#if UNITY_EDITOR
                    Debug.LogEditorInfor("Application.dataPath=" + Application.dataPath+ "\t s_ApplicationAssetParentRealPath"+ s_ApplicationAssetParentRealPath);
#endif
                }
                return s_ApplicationAssetParentRealPath;
            }
        }

    }
}
