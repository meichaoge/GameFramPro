using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;

#endif

namespace GameFramePro.Localization
{
    /// <summary>/// 本地化组件管理/// </summary>
    [DisallowMultipleComponent]
    public partial class LocalizationRootController : MonoBehaviour
    {
        #region 变量属性

        private bool mIsInitialed = false;

#if UNITY_EDITOR
        [Header("编辑器条件下语言类型")] [SerializeField]
        internal LocalizationLanguage CurEditorLanguageType;

#endif

        //[Header("所有的TextMeshPro本地化组件")]
        //public List<LocalizationTextMeshPro> allLocalizationTextMeshPros = new List<LocalizationTextMeshPro>();

        [Header("所有的文本本地化组件")] [SerializeField]
        internal List<LocalizationText> allLocalizationTexts = new List<LocalizationText>();

        [Header("所有的RectTransform本地化组件")] [SerializeField]
        internal List<LocalizationRectTransform> allLocalizationRectTransforms = new List<LocalizationRectTransform>();


        [Header("所有的Image本地化组件")] [SerializeField]
        internal List<LocalizationImage> allLocalizationImages = new List<LocalizationImage>();

        private LocalizationLanguage m_lastLanguageType = LocalizationLanguage.Unknow;

        #endregion


        private void Awake()
        {
            m_lastLanguageType = LocalizationLanguage.Unknow;
            SwitchOrShowLanguage(LocalizationHelper.GetCurLocalizationLanguage());
        }


        /// <summary>/// 初始化视图/// </summary>
        private void InitView(LocalizationLanguage languageType)
        {
            foreach (var localization in allLocalizationRectTransforms)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }

            foreach (var localization in allLocalizationImages)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }

            foreach (var localization in allLocalizationTexts)
            {
                if (localization == null)
                    continue;
                localization.ChangeLanguageType(languageType);
            }
        }

        /// <summary>/// 对外接口/// </summary>
        internal void SwitchOrShowLanguage(LocalizationLanguage showLanguage)
        {
            if (mIsInitialed == false)
                GetAllLocalizations();

            if (m_lastLanguageType == showLanguage)
                return;
            InitView(showLanguage);
            m_lastLanguageType = showLanguage;
        }
    }


#if UNITY_EDITOR

    public partial class LocalizationRootController
    {
        #region 菜单

        [MenuItem("CONTEXT/LocalizationRootController/统计所有本地化组件")]
        private static void GetAllLocalizations(MenuCommand cmd)
        {
            var current = cmd.context as LocalizationRootController;
            if (current == null)
            {
                Debug.LogError("统计失败！");
                return;
            }

            current.GetAllLocalizations();
            Debug.Log("统计所有本地化组件完成!!!");
        }


        [MenuItem("CONTEXT/LocalizationRootController/显示当前语言对应的视图")]
        private static void ShowLocalizations(MenuCommand cmd)
        {
            var current = cmd.context as LocalizationRootController;
            if (current == null)
            {
                Debug.LogError("显示当前语言对应的视图失败！");
                return;
            }

            current.SwitchOrShowLanguage(current.CurEditorLanguageType);
            Debug.Log("切换语言完成" + current.CurEditorLanguageType);
        }

        #endregion

        private void Reset()
        {
            CurEditorLanguageType = LocalizationHelper.GetCurLocalizationLanguage();
            var childManager = transform.GetComponentsInChildren<LocalizationRootController>(true);
            if (childManager.Length == 1)
            {
                GetAllLocalizations();
                return;
            }

            Debug.LogError("存在多个 LocalizationRootController 管理脚本会导致异常");
            foreach (var item in childManager)
            {
                Debug.Log($"节点{item.gameObject.name} 上有管理器脚本");
            }
        }

        private void OnValidate()
        {
            GetAllLocalizations();
            //        if (m_lastLanguageType == currentLanguageType)
            //            return;
            //        InitView(currentLanguageType);
            //        m_lastLanguageType = currentLanguageType;

            //#if UNITY_EDITOR
            //        Debug.Log("切换语言完成！");
            //#endif
        }

        /// <summary>/// 当子节点挂在了本地化脚本时候刷新引用/// </summary>
        public void OnChildAttachOrRemoveComponent()
        {
            GetAllLocalizations();
        }

        /// <summary>/// 获取所有的本地化组件/// </summary>
        internal void GetAllLocalizations()
        {
            mIsInitialed = true;
            LocalizationBase[] allLocalizationBases = gameObject.GetComponentsInChildren<LocalizationBase>(true);
            allLocalizationRectTransforms.Clear();
            allLocalizationImages.Clear();
            allLocalizationTexts.Clear();

            foreach (var localizationComponent in allLocalizationBases)
            {
                switch (localizationComponent.LocalizationUsage)
                {
                    case LocalizationComponentUsage.RectTransform:
                        allLocalizationRectTransforms.Add(localizationComponent as LocalizationRectTransform);
                        break;
                    case LocalizationComponentUsage.Text:
                        allLocalizationTexts.Add(localizationComponent as LocalizationText);
                        break;
                    case LocalizationComponentUsage.Image:
                        allLocalizationImages.Add(localizationComponent as LocalizationImage);
                        break;
                    default:
                        Debug.LogError($"没有定义的本地化组件类型{localizationComponent.LocalizationUsage}");
                        break;
                }
            }
        }
    }
    
    
    
    [CustomEditor(typeof(LocalizationRootController))]
    public class LocalizationRootController_Editor : Editor
    {
        private LocalizationRootController mLocalizationRootController;

        private void OnEnable()
        {
            mLocalizationRootController = target as LocalizationRootController;
        }


        public override void OnInspectorGUI()
        {
            #region 操作

            GUILayout.BeginVertical();

            if (GUILayout.Button("统计所有本地化组件", GUILayout.Height(30)))
            {
                mLocalizationRootController.GetAllLocalizations();
            }

            if (GUILayout.Button("显示当前语言对应的视图", GUILayout.Height(30)))
            {
                mLocalizationRootController.SwitchOrShowLanguage(mLocalizationRootController.CurEditorLanguageType);
            }

            GUILayout.EndVertical();

            #endregion

            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
#endif
}
