using System.Collections;
using System.Collections.Generic;
using GameFramePro.Localization;
using UnityEngine;
# if UNITY_EDITOR
using UnityEditor;
using GameFramePro.EditorEx;

#endif


namespace GameFramePro
{
    /// <summary>
    /// 用于配置一些常用的设置
    /// </summary>
    public class ApplicationConfigure : ScriptableObject
    {

        [Space(20)]

        #region 资源加载+安全+追踪 配置

        [Tooltip("标示是否在优先加载Resources下资源, =false 时候优先加载外部AssetBundle资源")]
        public bool mIsLoadResourcesAssetPriority = true;


        [Tooltip("标示是否追踪资源的创建的开关, 以便需要的时候输出当前使用的资源，分析性能")]
        public bool mIsTraceRecourceCreate = true;


        [Tooltip("标示是否追踪异步&协程任务")] public bool mIsTrackAsyncTask = true;


        [Tooltip("标示是否在创建单例对象时候检测,如果不检测则可能存在多个单例脚本同时存在")]
        public bool mIsSingletonCreateSaftCheck = true;

        #endregion


        [Space(20)]

        #region 本地化配置

        [Tooltip(" Localization 本地化语言表的配置格式")]
        public ExportFormatEnum mLocalizationExportFormatType = ExportFormatEnum.Csv;

        #endregion

        [Space(20)]

        #region 屏幕适配 + 点击效果

        [Tooltip("当前目标分辨率")]
        public Vector2 mReferenceResolution = new Vector2(640, 1134);

        [Tooltip("标示是否启用 屏幕点击特效 (这里是在入口处判断，比ScreenClickManager 中的接口优先级和限制更高)")]
        public bool mIsClickEffectEnable = true;

        #endregion


    }
    
    
    public class ApplicationConfigure_Editor
    {
        [MenuItem("工具和扩展/项目配置/生成应用配置资源")]
        private static void CreateApplicationConfigure()
        {
            ScriptableObjectUtility.CreateUnityAsset<ApplicationConfigure>($"Assets/Resources/{string.Format(ConstDefine.S_ApplicationConfigureAssetPath,ConstDefine.S_ApplicationConfigureName)}.asset");
        }
    }
    
}