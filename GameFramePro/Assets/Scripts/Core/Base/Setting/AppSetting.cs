using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 配置应用的设置
    /// </summary>
    public class AppSetting : Sington_Mono_AutoCreateNotDestroy<AppSetting>
    {
        [Header("标示是否追踪资源的创建的开关, 以便需要的时候输出当前使用的资源，分析性能")]
        [SerializeField]
        private bool m_IsTraceRecourceCreate = true;
        public bool IsTraceResourceCreate
        {
            get { return m_IsTraceRecourceCreate; }
            set { if (value != m_IsTraceRecourceCreate) { m_IsTraceRecourceCreate = value; Debug.LogEditorInfor(string.Format("追踪资源状态改变为 {0}", m_IsTraceRecourceCreate)); } }
        }
    }
}