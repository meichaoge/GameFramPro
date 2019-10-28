using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 应用控制管理器
    /// </summary>
    public static class ApplicationManager
    {
        public static ApplicationDynamicConfigure mApplicationDynamicConfigure { get; set; } = new ApplicationDynamicConfigure();

        private static ApplicatonProperty ApplicatonPropertySettings = null;

        public static ApplicatonProperty mApplicatonPropertySettings
        {
            get
            {
                if (ApplicatonPropertySettings == null)
                {
                    LoadAssetResult<ApplicatonProperty> result = ResourcesManager.LoadAssetSync<ApplicatonProperty>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicatonPropertyName));
                    result.ReferenceWithComponent(null, (applicationProperty) =>
                    {
                        ApplicatonPropertySettings = applicationProperty;
                        return false;
                    });

                    //#if UNITY_EDITOR
                    //                    Debug.LogEditorError($"这里需要修改加载方式");
                    //                    ApplicatonPropertySettings = Resources.Load<ApplicatonProperty>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicatonPropertyName));
                    //#else
                    //   Debug.LogError($"还没有实现加载.asset");
                    //#endif
                }

                return ApplicatonPropertySettings;
            }
        }


        private static ApplicationConfigure ApplicationConfigureSettings = null;

        public static ApplicationConfigure mApplicationConfigureSettings
        {
            get
            {
                if (ApplicationConfigureSettings == null)
                {
                    LoadAssetResult<ApplicationConfigure> result = ResourcesManager.LoadAssetSync<ApplicationConfigure>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicationConfigureName));
                    result.ReferenceWithComponent(null, (applicationSetting) =>
                    {
                        ApplicationConfigureSettings = applicationSetting;
                        return false;
                    });

                    //#if UNITY_EDITOR
                    //                    Debug.LogEditorError($"这里需要修改加载方式");
                    //                    ApplicationConfigureSettings = Resources.Load<ApplicationConfigure>(string.Format(ConstDefine.S_ApplicationConfigureAssetPath, ConstDefine.S_ApplicationConfigureName));
                    //#else
                    //   Debug.LogError($"还没有实现加载.asset");
                    //#endif
                }

                return ApplicationConfigureSettings;
            }
        }
    }
}