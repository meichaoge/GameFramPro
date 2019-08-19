using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    public static class ConstDefine
    {
#if UNITY_EDITOR
        public static readonly string S_EditorName = "Editor"; //Editor 目录名
#endif

        public static readonly string S_MetaExtension = ".meta"; //meta 扩展名
        public static readonly string S_TextAssetExtension = ".txt"; //文件文件扩展名
        public static readonly string S_PrefabExtension = ".prefab"; //文件文件扩展名
        public static readonly string S_AssetBundleManifestExtension = ".manifest"; //AssetBundleManifest文件文件扩展名
        public static readonly string S_AssetBundleExtension = ".unity3d"; //AssetBundle t文件文件扩展名

        public static readonly string S_LocalizationKeyFlag = "_@"; //本地化语言key 的前两个字符标示 ,也是查找本地化key的模式


        public static readonly string S_ResourcesName = "Resources"; //Resources 目录名
        public static readonly string S_AssetsName = "Assets"; //Assets 目录名


        private static readonly string S_ExportDirectoryName = "Export"; //项目导出需要提交到CDN的目录名
        public static readonly string S_LocalStoreDirectoryName = "Download"; //外部下载保存本地的顶层目录
        public static readonly string S_AssetBundleDirectoryName = "AssetBundle"; //所有关于AssetBundle 资源生成的顶层目录
        public static readonly string S_PreLoadTextureTopDirectoryName = "PreloadTexture"; //所有关于 图片 资源生成的顶层目录


        public static readonly string S_LocalizationDirectoryName = "Localization"; //本地化语言相对于Resources路径
        public static readonly string S_AssetBundleConfigFileName = "AssetBundleContainAssetInfor.csv"; //所有关于AssetBundle 资源配置文件名
        public static readonly string S_PreloadImgConfiFileName = "PreloadImageAssetConfig.json"; //预加载图片的名称配置


        public const string S_StringEmpty = ""; //空字符串


        /// <summary>/// Resources 绝对路径/// </summary>
        public static string S_ResourcesRealPath
        {
            get { return $"{Application.dataPath}/{S_ResourcesName}"; }
        }

        /// <summary>/// Application.dataPath 的父目录（主要是用在编辑器下组合  AssetDatabase.GetAssetPath()获取的路径）/// </summary>
        public static string S_ApplicationAssetParentRealPath
        {
            get { return Application.dataPath.GetFilePathParentDirectory(1); }
        }

        /// <summary>/// 导出资源的绝对路径/// </summary>
        public static string S_ExportRealPath
        {
            get { return Application.dataPath.GetFilePathParentDirectory(1).CombinePathEx(S_ExportDirectoryName); }
        }
    }
}
