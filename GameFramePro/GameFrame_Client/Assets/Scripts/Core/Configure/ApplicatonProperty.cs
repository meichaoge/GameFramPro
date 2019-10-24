using System.Collections;
using System.Collections.Generic;
# if UNITY_EDITOR
using GameFramePro.EditorEx;
using UnityEditor;
#endif
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 应用的基础属性配置
    /// </summary>
    public class ApplicatonProperty : ScriptableObject
    {
        
        
        [Space(20)] [Tooltip("应用的版本号")] public string mAppVersion = "1.0.0";


        [Tooltip("应用的资源版本号")] public string mAppResourcesVersion = "1.0.0";
    }
    
    
    
    
# if UNITY_EDITOR
    /// <summary>
    /// ApplicatonProperty 配置
    /// </summary>
    public class ApplicationEditorConfigure_Editor
    {
        [MenuItem("工具和扩展/项目配置/生成 项目属性配置.asset")]
        private static void CreateApplicationEditorConfigure()
        {
            ScriptableObjectUtility.CreateUnityAsset<ApplicatonProperty>($"Assets/Resources/{string.Format(ConstDefine.S_ApplicationConfigureAssetPath,ConstDefine.S_ApplicatonPropertyName)}.asset");
        }
    }
#endif
    
    
}