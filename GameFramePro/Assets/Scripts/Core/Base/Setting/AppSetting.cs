using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 配置应用的设置
    /// </summary>
    public class AppSetting : Single_Mono_AutoCreateNotDestroy<AppSetting>
    {

        [Header("标示是否在优先加载Resources下资源, =false 时候优先加载外部AssetBundle资源")]
        [SerializeField]
        private bool m_IsLoadResourcesAssetPriority = true;
        public bool IsLoadResourcesAssetPriority
        {
            get { return m_IsLoadResourcesAssetPriority; }
            set
            {
                if (value != m_IsLoadResourcesAssetPriority)
                {
                    m_IsLoadResourcesAssetPriority = value;
                    Debug.LogEditorInfor(string.Format("加载资源的方式优先级发生改变为 {0}", m_IsLoadResourcesAssetPriority));
                }
            }
        }

        [Header("标示是否追踪资源的创建的开关, 以便需要的时候输出当前使用的资源，分析性能")]
        [SerializeField]
        private bool m_IsTraceRecourceCreate = true;
        public bool IsTraceResourceCreate
        {
            get { return m_IsTraceRecourceCreate; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持开启追踪资源,请在运行前设置");
                    return;
                }
                if (value != m_IsTraceRecourceCreate)
                {
                    m_IsTraceRecourceCreate = value;
                    Debug.LogEditorInfor(string.Format("追踪资源状态改变为 {0}", m_IsTraceRecourceCreate));
                }
            }
        }


        [Header("标示是否追踪异步&协程任务")]
        [SerializeField]
        private bool m_IsTrackAsyncTask = true;
        public bool IsTrackAsyncTask
        {
            get { return m_IsTrackAsyncTask; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持追踪异步&协程任务,请在运行前设置");
                    return;
                }
                if (value != m_IsTrackAsyncTask)
                {
                    m_IsTrackAsyncTask = value;
                    Debug.LogEditorInfor(string.Format("对象创建状态改变为 {0}", m_IsTrackAsyncTask));
                }
            }
        }

        [Header("标示是否在创建单例对象时候检测,如果不检测则可能存在多个单例脚本同时存在")]
        [SerializeField]
        private bool m_IsSingletonCreateSaftCheck = true;
        public bool IsSingletonCreateSaftCheck
        {
            get { return m_IsSingletonCreateSaftCheck; }
            set
            {
                if (Application.isPlaying)
                {
                    Debug.LogError("运行时不支持安全的对象穿件检测,请在运行前设置");
                    return;
                }
                if (value != m_IsSingletonCreateSaftCheck)
                {
                    m_IsSingletonCreateSaftCheck = value;
                    Debug.LogEditorInfor(string.Format("对象创建状态改变为 {0}", m_IsSingletonCreateSaftCheck));
                }
            }
        }



    }
}