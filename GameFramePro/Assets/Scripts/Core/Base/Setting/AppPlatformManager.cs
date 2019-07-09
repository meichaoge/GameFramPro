using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 支持的平台
    /// </summary>
    public enum AppPlatformEnum
    {
        Android,
        IOS,
        Windows64
    }

    /// <summary>
    /// 管理平台相关
    /// </summary>
    public class AppPlatformManager : Single<AppPlatformManager>
    {


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
                default:
                    Debug.LogError(string.Format("GetBuildTargetFromAppPlatformEnum Fail,Not Support Platform={0}", platform));
                    return UnityEditor.BuildTarget.StandaloneWindows64;
            }
        }
#endif

        #endregion




    }
}