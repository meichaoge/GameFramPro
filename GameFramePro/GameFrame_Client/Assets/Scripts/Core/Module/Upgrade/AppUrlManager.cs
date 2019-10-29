using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace GameFramePro
{
    /// <summary>/// 管理各个模块CDN 的URL/// </summary>
    internal static class AppUrlManager
    {
        public static string s_TopCDNUrl { get; set; } = string.Format("http://superxu3d.tcmapi.cn/goalon/TestApplication/{0}/{1}", ApplicationManager.mApplicatonPropertySettings.mAppResourcesVersion,
            AppPlatformManager.GetRuntimePlatformFolderName());

        //private static string s_TopCDNUrl { get; set; } = string.Format("https://supergoal-content-bucket.oss-cn-hongkong.aliyuncs.com/goalon/TestApplication/{0}/{1}", ApplicationManager.mApplicatonPropertySettings.mAppResourcesVersion,
        //AppPlatformManager.GetRuntimePlatformFolderName());
        //    $"https://superxu3d.tcmapi.cn/goalon/TestApplication/{ApplicationManager.mApplicatonPropertySettings.mAppResourcesVersion}/{AppPlatformManager.GetRuntimePlatformFolderName()}";



        private static string s_AssetBundleCDNTopUrl = string.Empty;
        /// <summary>/// AssetBundle CDN顶层目录url/// </summary>
        public static string S_AssetBundleCDNTopUrl
        {
            get
            {
                if (string.IsNullOrEmpty(s_AssetBundleCDNTopUrl))
                    s_AssetBundleCDNTopUrl = s_TopCDNUrl.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                return s_AssetBundleCDNTopUrl;
            }
        }


        private static string s_TextureCDNTopUrl = string.Empty;

        /// <summary>/// Texture CDN顶层目录url/// </summary>
        public static string S_TextureCDNTopUrl
        {
            get
            {
                if (string.IsNullOrEmpty(s_TextureCDNTopUrl))
                    s_TextureCDNTopUrl = s_TopCDNUrl.CombinePathEx(ConstDefine.S_PreLoadTextureTopDirectoryName);
                return s_TextureCDNTopUrl;
            }
        }



    }
}
