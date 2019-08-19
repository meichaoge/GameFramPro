using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace GameFramePro.Localization
{
    /// <summary>/// 本地化组建的类型/// </summary>
    internal enum LocalizationComponentUsage
    {
        RectTransform,
        Text,
        Image,
    }

    internal abstract partial class LocalizationBase : MonoBehaviour
    {
        #region 变量属性

#if UNITY_EDITOR
        [Header("当前编辑时语言类型,可以更改调整设置配置信息")] public LocalizationLanguage CurEnditorLanguageType;
#endif

        [SerializeField] [Header("<<<<当前显示的语言类型 不要修改>>>>")]
        protected LocalizationLanguage mShowLanguage;

        internal abstract LocalizationComponentUsage LocalizationUsage { get; set; }

        #endregion


        /// <summary>/// 切换语言/// </summary>
        public virtual void ChangeLanguageType(LocalizationLanguage Language)
        {
            ShowView(Language);
        }


        #region 视图操作

        /// <summary>/// 显示的内容由父节点LocalizationManager 管理显示/// </summary>
        protected virtual void Awake()
        {
            mShowLanguage = LocalizationHelper.GetCurLocalizationLanguage(); //初始时候是未知的 确保第一次一定会刷新
            ShowView(mShowLanguage);
        }


        /// <summary>/// 初始化视图/// </summary>
        internal abstract bool ShowView(LocalizationLanguage languageType);

        #endregion
    }


#if UNITY_EDITOR
    internal abstract partial class LocalizationBase
    {
        #region 菜单

        [MenuItem("CONTEXT/LocalizationBase/新建当前语言下的配置")]
        private static void NewLanguageConfigView(UnityEditor.MenuCommand cmd)
        {
            LocalizationBase current = cmd.context as LocalizationBase;
            if (current == null)
                return;

            if (current.NewViewConfig(current.CurEnditorLanguageType))
                Debug.Log("新建当前语言的配置完成！");
        }


        [MenuItem("CONTEXT/LocalizationBase/保存当前语言下的配置")]
        private static void SaveCurrentView(UnityEditor.MenuCommand cmd)
        {
            LocalizationBase current = cmd.context as LocalizationBase;
            if (current == null)
                return;

            if (current.SaveView(current.CurEnditorLanguageType))
                Debug.Log("保存当前语言下的配置完成");
            else
                Debug.LogError("无法获取制定的信息");
        }

        [MenuItem("CONTEXT/LocalizationBase/显示当前语言下的视图")]
        private static void ShowCurrentView(UnityEditor.MenuCommand cmd)
        {
            LocalizationBase current = cmd.context as LocalizationBase;
            if (current == null)
                return;

            current.transform.localPosition += Vector3.one;
            if (current.ShowView(current.CurEnditorLanguageType))
                Debug.Log("刷新当前语言下的视图完成");
            current.transform.localPosition -= Vector3.one; //为了强制刷新
        }

        #endregion


        /// <summary>/// 重置/// </summary>
        protected virtual void Reset()
        {
            CurEnditorLanguageType = LocalizationHelper.GetCurLocalizationLanguage();
            var parentLocalizationManagers = transform.GetComponentsInParent<LocalizationRootController>();
            foreach (var item in parentLocalizationManagers)
            {
                item.OnChildAttachOrRemoveComponent();

                if (parentLocalizationManagers.Length > 1)
                    Debug.Log($"通知父节点{item.gameObject.name}  管理器数刷新");
            }
        }


        /// <summary>/// 保存视图/// </summary>
        internal abstract bool SaveView(LocalizationLanguage Language);

        /// <summary>/// 新建一个当前语言对应的配置 ，如果已经存在则返回/// </summary>
        internal abstract bool NewViewConfig(LocalizationLanguage languageType);
    }

    [CustomEditor(typeof(LocalizationBase))]
    internal class LocalizationBase_Editor : Editor
    {
        private LocalizationBase mLocalizationBase = null;

        private void OnEnable()
        {
            mLocalizationBase = target as LocalizationBase;
        }

        public override void OnInspectorGUI()
        {
            #region 操作

            GUILayout.BeginVertical();

            if (GUILayout.Button("新建当前语言下的配置", GUILayout.Height(30)))
            {
                if (mLocalizationBase.NewViewConfig(mLocalizationBase.CurEnditorLanguageType))
                    Debug.Log("新建当前语言的配置完成！");
            }

            if (GUILayout.Button("保存当前语言下的配置", GUILayout.Height(30)))
            {
                if (mLocalizationBase.SaveView(mLocalizationBase.CurEnditorLanguageType))
                    Debug.Log("保存当前语言下的配置完成");
                else
                    Debug.LogError("无法获取制定的信息");
            }

            if (GUILayout.Button("显示当前语言下的视图", GUILayout.Height(30)))
            {
                mLocalizationBase.transform.localPosition += Vector3.one;
                if (mLocalizationBase.ShowView(mLocalizationBase.CurEnditorLanguageType))
                    Debug.Log("刷新当前语言下的视图完成");
                mLocalizationBase.transform.localPosition -= Vector3.one; //为了强制刷新
            }

            GUILayout.EndVertical();

            #endregion

            GUILayout.Space(30);
            base.OnInspectorGUI();
        }
    }

#endif
}
