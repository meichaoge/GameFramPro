using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 配置应用的设置
    /// </summary>
    public static class AppSetting 
    {

        [Header("标示是否在优先加载Resources下资源, =false 时候优先加载外部AssetBundle资源")]
        private static bool s_IsLoadResourcesAssetPriority = true;
        public static bool S_IsLoadResourcesAssetPriority
        {
            get { return s_IsLoadResourcesAssetPriority; }
            set
            {
                if (value != s_IsLoadResourcesAssetPriority)
                {
                    s_IsLoadResourcesAssetPriority = value;
                    Debug.LogEditorInfor(string.Format("加载资源的方式优先级发生改变为 {0}", s_IsLoadResourcesAssetPriority));
                }
            }
        }

        [Header("标示是否追踪资源的创建的开关, 以便需要的时候输出当前使用的资源，分析性能")]
        private static bool s_IsTraceRecourceCreate = true;
        public static bool S_IsTraceResourceCreate
        {
            get { return s_IsTraceRecourceCreate; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持开启追踪资源,请在运行前设置");
                    return;
                }
                if (value != s_IsTraceRecourceCreate)
                {
                    s_IsTraceRecourceCreate = value;
                    Debug.LogEditorInfor(string.Format("追踪资源状态改变为 {0}", s_IsTraceRecourceCreate));
                }
            }
        }


        [Header("标示是否追踪异步&协程任务")]
        private static bool s_IsTrackAsyncTask = true;
        public static bool S_IsTrackAsyncTask
        {
            get { return s_IsTrackAsyncTask; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持追踪异步&协程任务,请在运行前设置");
                    return;
                }
                if (value != s_IsTrackAsyncTask)
                {
                    s_IsTrackAsyncTask = value;
                    Debug.LogEditorInfor(string.Format("对象创建状态改变为 {0}", s_IsTrackAsyncTask));
                }
            }
        }

        [Header("标示是否在创建单例对象时候检测,如果不检测则可能存在多个单例脚本同时存在")]
        private static bool s_IsSingletonCreateSaftCheck = true;
        public static bool S_IsSingletonCreateSaftCheck
        {
            get { return s_IsSingletonCreateSaftCheck; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持安全的对象穿件检测,请在运行前设置");
                    return;
                }
                if (value != s_IsSingletonCreateSaftCheck)
                {
                    s_IsSingletonCreateSaftCheck = value;
                    Debug.LogEditorInfor(string.Format("对象创建状态改变为 {0}", s_IsSingletonCreateSaftCheck));
                }
            }
        }



    }
}