using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 应用控制管理器
    /// </summary>
    public class ApplicationManager : Single<ApplicationManager>
    {
        public ApplicationDynamicConfigure mApplicationDynamicConfigure { get; set; } = new ApplicationDynamicConfigure();

        private ApplicatonProperty ApplicatonPropertySettings = null;

        public ApplicatonProperty mApplicatonPropertySettings
        {
            get
            {
                if (ApplicatonPropertySettings == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"这里需要修改加载方式");
                    ApplicatonPropertySettings = Resources.Load<ApplicatonProperty>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicatonPropertyName));
#else
   Debug.LogError($"还没有实现加载.asset");
#endif
                }

                return ApplicatonPropertySettings;
            }
        }


        private ApplicationConfigure ApplicationConfigureSettings = null;

        public ApplicationConfigure mApplicationConfigureSettings
        {
            get
            {
                if (ApplicationConfigureSettings == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"这里需要修改加载方式");
                    ApplicationConfigureSettings = Resources.Load<ApplicationConfigure>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicationConfigureName));
#else
   Debug.LogError($"还没有实现加载.asset");
#endif
                }

                return ApplicationConfigureSettings;
            }
        }
    }
}