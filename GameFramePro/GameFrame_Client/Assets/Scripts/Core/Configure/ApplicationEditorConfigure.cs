# if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using GameFramePro.Localization;
using UnityEditor;
using UnityEngine;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 用于生成一些在编辑器下会使用到的配置资源信息
    /// </summary>
    public class ApplicationEditorConfigure : ScriptableObject
    {
        public static readonly string S_ApplicationEditorConfigureName = "ApplicationEditorConfigureSettings"; //项目配置 .asset 资源文件名

        //生成的编辑器配置资源信息
        public static string S_ApplicationEditorConfigurePath
        {
            get { return $"Assets/Editor/{S_ApplicationEditorConfigureName}.asset"; }
        }

        private static ApplicationEditorConfigure s_Instance = null;

        public static ApplicationEditorConfigure S_Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = AssetDatabase.LoadAssetAtPath<ApplicationEditorConfigure>(S_ApplicationEditorConfigurePath);
                return s_Instance;
            }
        }

        //********配置*************

        [Space(20)] [Tooltip("设置 Android 图片导入时候的格式")]
        public TextureImporterFormat mTextureImportFormat_Android = TextureImporterFormat.ETC2_RGBA8;


        [Space(5)]    [Tooltip("设置 Standalone 图片导入时候的格式")] public TextureImporterFormat mTextureImportFormat_Standalone = TextureImporterFormat.RGBA32;


        [Space(5)] [Tooltip("设置 Ios 图片导入时候的格式")]
        public TextureImporterFormat mTexureImportFormat_Ios = TextureImporterFormat.ASTC_4x4;
    }


    public class ApplicationEditorConfigure_Editor
    {
        [MenuItem("工具和扩展/项目配置/生成编辑器配置.asset")]
        private static void CreateApplicationEditorConfigure()
        {
            ScriptableObjectUtility.CreateUnityAsset<ApplicationEditorConfigure>(ApplicationEditorConfigure.S_ApplicationEditorConfigurePath);
        }

        [MenuItem("工具和扩展/项目配置/移除不可用的Tags")]
        private static void RemoveAllUnUsingTags()
        {
            LayerAndTagManager.RemoveUnExitTags();
        }
    }
}
#endif