using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    public  static partial class ConstDefine
    {
#if UNITY_EDITOR
        public static readonly string S_EditorName = "Editor"; //Editor 目录名
#endif

        public static readonly string S_MetaExtension = ".meta"; //meta 扩展名
        public static readonly string S_TextAssetExtension = ".txt"; //文件文件扩展名
        public static readonly string S_PrefabExtension = ".prefab"; //文件文件扩展名
        public static readonly string S_AssetBundleManifestExtension = ".manifest"; //AssetBundleManifest文件文件扩展名
        public static readonly string S_AssetBundleExtension = ".unity3d"; //AssetBundle t文件文件扩展名
        public static readonly char S_AssetBundleAssetNameSeparatorChar = 'E'; //AssetBundle 资源中前面长度与后面Md5 分割符号

        public static readonly string S_LocalizationKeyFlag = "_@"; //本地化语言key 的前两个字符标示 ,也是查找本地化key的模式


        public static readonly string S_ResourcesName = "Resources"; //Resources 目录名
        public static readonly string S_AssetsName = "Assets"; //Assets 目录名


        private static readonly string S_ExportDirectoryName = "Export"; //项目导出需要提交到CDN的目录名
        public static readonly string S_LocalStoreDirectoryName = "Download"; //外部下载保存本地的顶层目录
        public static readonly string S_LocalStoreDirectoryTempName = "TempDownload"; //外部下载保存本地的顶层目录(缓存目录)


        public static readonly string S_AssetBundleDirectoryName = "AssetBundle"; //所有关于AssetBundle 资源生成的顶层目录
        public static readonly string S_PreLoadTextureTopDirectoryName = "PreloadTexture"; //所有关于 图片 资源生成的顶层目录


        public static readonly string S_LocalizationDirectoryName = "Localization"; //本地化语言相对于Resources路径
        public static readonly string S_AssetBundleConfigFileName = "AssetBundleContainAssetInfor.txt"; //所有关于AssetBundle 资源配置文件名
        public static readonly string S_PreloadImgConfiFileName = "PreloadImageAssetConfig.json"; //预加载图片的名称配置


        //.asset 资源配置 都是在Resources 目录下
        public static string S_ApplicationConfigureAssetPath { get; set; } = "ConfigureAsset/{0}"; //相对于Resources 路径
        public static readonly string S_ApplicationConfigureName = "ApplicationConfigureSettings"; //项目配置 .asset 资源文件名
        public static readonly string S_ApplicatonPropertyName = "ApplicatonPropertySettings"; //项目配置 .asset 资源文件名


        public const string S_StringEmpty = ""; //空字符串

    }

    public static partial class ConstDefine
    {
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

        /// <summary>
        /// 分享图片缓存目录
        /// </summary>
        public static string S_ShareImageTopDirectory { get { return Application.persistentDataPath.CombinePathEx("share"); } }


        private static string s_LoadDownloadAssetStoryDirectory = string.Empty;

        /// <summary>
        /// 本地下载资源的保存顶级目录(区分平台)
        /// </summary>
        public static string S_LoadDownloadAssetStoryDirectoryUri
        {
            get
            {
                if (string.IsNullOrEmpty(s_LoadDownloadAssetStoryDirectory))
                    s_LoadDownloadAssetStoryDirectory = Application.persistentDataPath.CombinePathEx(S_LocalStoreDirectoryName);

                return s_LoadDownloadAssetStoryDirectory;
            }
        }



        private static string s_LocalAssetBundleTopDirectoryPath = string.Empty;

        /// <summary>
        /// 本地AssetBundle 顶层存储的路径
        /// </summary>
        public static string S_LocalAssetBundleTopDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_LocalAssetBundleTopDirectoryPath))
                    s_LocalAssetBundleTopDirectoryPath = S_LoadDownloadAssetStoryDirectoryUri.CombinePathEx(S_AssetBundleDirectoryName);

                return s_LocalAssetBundleTopDirectoryPath;
            }
        }
        
        
    }
}