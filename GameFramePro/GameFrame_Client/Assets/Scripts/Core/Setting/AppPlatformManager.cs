﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
# if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFramePro
{
    /// <summary>
    /// 支持的平台
    /// </summary>
    public enum AppPlatformEnum
    {
        Android,
        IOS,
        Windows64,
        Mac
    }

    /// <summary>
    /// 管理平台相关
    /// </summary>
    public static class AppPlatformManager
    {

        #region 平台相关数据

#if UNITY_ANDROID

        private static AndroidJavaObject _current;
        public static AndroidJavaObject Current
        {
            get
            {
                if (_current == null)
                {
                    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    _current = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                return _current;
            }
        }
#endif

        #endregion


        /// <summary>
        /// 获取各个平台对应的目录
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetPlatformFolderName(AppPlatformEnum platform)
        {
            switch (platform)
            {
                case AppPlatformEnum.Android:
                    return "Android";
                case AppPlatformEnum.IOS:
                    return "IOS";
                case AppPlatformEnum.Windows64:
                    return "Windows64";
                case AppPlatformEnum.Mac:
                    return "Mac";
                default:
                    Debug.LogError("没有定义的类型 " + platform);
                    return "UnKnow";
            }
        }



        #region 平台转换相关
# if UNITY_EDITOR
        /// <summary>
        /// 平台类型转换
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static UnityEditor.BuildTarget GetBuildTargetFromAppPlatformEnum(AppPlatformEnum platform)
        {
            switch (platform)
            {
                case AppPlatformEnum.Android:
                    return UnityEditor.BuildTarget.Android;
                case AppPlatformEnum.IOS:
                    return UnityEditor.BuildTarget.iOS;
                case AppPlatformEnum.Windows64:
                    return UnityEditor.BuildTarget.StandaloneWindows64;
                case AppPlatformEnum.Mac:
                    return UnityEditor.BuildTarget.StandaloneOSX;
                default:
                    Debug.LogError(string.Format("GetBuildTargetFromAppPlatformEnum Fail,Not Support Platform={0}", platform));
                    return UnityEditor.BuildTarget.StandaloneWindows64;
            }
        }

        public static AppPlatformEnum GetAppPlatformEnumFromBuildTarget(UnityEditor.BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return AppPlatformEnum.Mac;
                case UnityEditor.BuildTarget.StandaloneWindows:
                    return AppPlatformEnum.Windows64;
                case UnityEditor.BuildTarget.iOS:
                    return AppPlatformEnum.IOS;
                case UnityEditor.BuildTarget.Android:
                    return AppPlatformEnum.Android;
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return AppPlatformEnum.Windows64;
                default:
                    Debug.LogError(string.Format("GetBuildTargetFromAppPlatformEnum Fail,Not Support Platform={0}", buildTarget));
                    return AppPlatformEnum.Windows64;
            }
        }

        public static UnityEditor.BuildTargetGroup GetBuildTargetGroupFromAppPlatformEnum(AppPlatformEnum platform)
        {
            switch (platform)
            {
                case AppPlatformEnum.Android:
                    return UnityEditor.BuildTargetGroup.Android;
                case AppPlatformEnum.IOS:
                    return UnityEditor.BuildTargetGroup.iOS;
                case AppPlatformEnum.Windows64:
                    return UnityEditor.BuildTargetGroup.Standalone;
                case AppPlatformEnum.Mac:
                    return UnityEditor.BuildTargetGroup.Standalone;
                default:
                    Debug.LogError(string.Format("GetBuildTargetFromAppPlatformEnum Fail,Not Support Platform={0}", platform));
                    return UnityEditor.BuildTargetGroup.Standalone;
            }
        }


        /// <summary>
        /// 获取各个平台对应的目录
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetPlatformFolderName(UnityEditor.BuildTarget buildTarget)
        {
            return GetPlatformFolderName(GetAppPlatformEnumFromBuildTarget(buildTarget));
        }


        /// <summary>
        /// 获取各个平台对应的目录
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetCurBuildPlatformFolderName()
        {
            //EditorUserBuildSettings.activeBuildTarget
            return GetPlatformFolderName(GetAppPlatformEnumFromBuildTarget(EditorUserBuildSettings.activeBuildTarget));
        }
#endif

        #endregion

        /// <summary>
        /// 获取当前运行时的路径
        /// </summary>
        /// <returns></returns>
        public static string GetRuntimePlatformFolderName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    return GetPlatformFolderName(AppPlatformEnum.Android);
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return GetPlatformFolderName(AppPlatformEnum.Windows64);
                case RuntimePlatform.OSXEditor:
                    return GetPlatformFolderName(AppPlatformEnum.Mac);
                case RuntimePlatform.OSXPlayer:
                    return GetPlatformFolderName(AppPlatformEnum.Mac);
                case RuntimePlatform.IPhonePlayer:
                    return GetPlatformFolderName(AppPlatformEnum.IOS);
                default:
                    Debug.LogError("GetRuntimePlatformFolderName Fail,Not Support " + Application.platform);
                    break;
            }
            return GetPlatformFolderName(AppPlatformEnum.Windows64);
        }


    }
}