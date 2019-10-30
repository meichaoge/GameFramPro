using GameFramePro.Localization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{


    /// <summary>
    /// 应用程序动态配置的项 可以通过lua 修改
    /// </summary>
    public class ApplicationDynamicConfigure
    {
     
    }



    /// <summary>
    /// 应用控制管理器
    /// </summary>
    public  class ApplicationManager : Single<ApplicationManager>
    {
        public static string S_TopCDNUrl { get; set; } = string.Format("http://superxu3d.tcmapi.cn/goalon/TestApplication/{0}/{1}", S_Instance.mAppResourcesVersion,  AppPlatformManager.GetRuntimePlatformFolderName());

        //private static string S_TopCDNUrl { get; set; } = string.Format("https://supergoal-content-bucket.oss-cn-hongkong.aliyuncs.com/goalon/TestApplication/{0}/{1}", ApplicationManager.S_Instance.mAppResourcesVersion,
        //AppPlatformManager.GetRuntimePlatformFolderName());
        //    $"https://superxu3d.tcmapi.cn/goalon/TestApplication/{ApplicationManager.mApplicatonPropertySettings.mAppResourcesVersion}/{AppPlatformManager.GetRuntimePlatformFolderName()}";

        public static ApplicationDynamicConfigure mApplicationDynamicConfigure { get; set; } = new ApplicationDynamicConfigure();


        /// <summary>
        /// 应用的版本号  (只跟随安装包更新)
        /// </summary>
        public string mAppVersion { get;  set; } = "1.0.0";
        /// <summary>
        /// 应用的资源版本号 (只跟随安装包更新)
        /// </summary>
        public string mAppResourcesVersion { get;  set; } = "1.0.0"; //s 代表资源

        //******* 资源加载+安全+追踪 配置 *********************
        /// <summary>
        /// 标示是否追踪异步&协程任务
        /// </summary>
        public static bool mIsTrackAsyncTask = true;
        /// <summary>
        /// 标示是否在创建单例对象时候检测,如果不检测则可能存在多个单例脚本同时存在
        /// </summary>
        public static bool mIsSingletonCreateSaftCheck = true;

        //************ 本地化配置 **************
        /// <summary>
        /// Localization 本地化语言表的配置格式
        /// </summary>
        public ExportFormatEnum mLocalizationExportFormatType = ExportFormatEnum.Csv;


        //*********** 屏幕适配 + 点击效果 ***************
        /// <summary>
        /// 当前目标分辨率
        /// </summary>
        public Vector2 mReferenceResolution = new Vector2(640, 1134);

    }
}