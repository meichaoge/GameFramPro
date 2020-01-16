using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using GameFramePro.Localization;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameFramePro
{
    // public delegate void OnLanguageChangedHandler(Language curLanguage);

    /// <summary>/// 导出的配置文件格式/// </summary>
    [System.Flags]
    public enum ExportFormatEnum
    {
        Csv = 1,
        Json = 2,
        Xml = 4,
    }


    /// <summary>///本地化管理器/// </summary>
    public static class LocalizationManager
    {
        #region 支持的本地化语言


        /// <summary>
        /// 当前的语言
        /// </summary>
        public static Language CurLanguage { get; private set; } = Language.zh_CN;

        ///// <summary>
        ///// 本地化语言改变事件
        ///// </summary>
        //private static List<OnLanguageChangedHandler> mAllLanguageChangeHandlers = new List<OnLanguageChangedHandler>();

        public static readonly string s_LocalizationKeyReg = @"^(@)+[A-Za-z0-9_]+$";

        #endregion

        #region 本地化语言配置文件

        private static Dictionary<Language, Dictionary<string, string>> mAllSupportLanguageConfig = new Dictionary<Language, Dictionary<string, string>>();
        private static bool mIsReadyGetLocalization = false; //标示是否加载完配置语言

        #endregion


        #region 本地化语言的设置

        //获取默认的语言
        private static void GetDefaultLanguage()
        {
            int lastSelectedLanguage = PlayerPrefsManager.GetInt(PlayerPrefsKeyDefine.LocalizationLanguage_Key);
            if (lastSelectedLanguage != 0)
            {
                CurLanguage = (Language)lastSelectedLanguage;
                return;
            }

            //TODO  
            Debug.LogError("需要获取各个平台系统语言环境");

            CurLanguage = Language.zh_CN;
            PlayerPrefsManager.SetInt(PlayerPrefsKeyDefine.LocalizationLanguage_Key, (int)CurLanguage);
        }

        /// <summary>
        /// 尝试切换到语言 language
        /// </summary>
        /// <param name="language"></param>
        public static void ChangeLanguage(Language language)
        {
            if (CurLanguage == language)
                return;
            CurLanguage = language;
            EventManager.TriggerMessage((int)UIEventUsage.NotifySwitchLanguage);
            //foreach (var handler in mAllLanguageChangeHandlers)
            //{
            //    if (handler != null)
            //        handler.Invoke(CurLanguage);
            //}
        }

        //        /// <summary>
        //        /// 监听切换语言
        //        /// </summary>
        //        /// <param name="handler"></param>
        //        public static void RegisterLanguageChangeEvent(OnLanguageChangedHandler handler)
        //        {
        //            if (handler == null)
        //            {
        //                Debug.LogError("RegisterLanguageChangeEvent Fail,Parameter is null");
        //                return;
        //            }

        //            foreach (var handlerItem in mAllLanguageChangeHandlers)
        //            {
        //                if (handlerItem.Equals(handler))
        //                {
        //#if UNITY_EDITOR
        //                    Debug.LogError("RegisterLanguageChangeEvent Fail,重复注册切换语言事件" + handler.Method.Name);
        //#endif
        //                    return;
        //                }
        //            }

        //            mAllLanguageChangeHandlers.Add(handler);
        //        }

        //        /// <summary>
        //        ///  取消监听切换语言
        //        /// </summary>
        //        /// <param name="handler"></param>
        //        public static void UnRegisterLanguageChangeEvent(OnLanguageChangedHandler handler)
        //        {
        //            if (handler == null)
        //            {
        //                Debug.LogError("UnRegisterLanguageChangeEvent Fail,Parameter is null");
        //                return;
        //            }

        //            for (int dex = mAllLanguageChangeHandlers.Count - 1; dex >= 0; dex--)
        //            {
        //                if (mAllLanguageChangeHandlers[dex].Equals(handler))
        //                {
        //                    mAllLanguageChangeHandlers.RemoveAt(dex);
        //                    return;
        //                }
        //            }
        //#if UNITY_EDITOR
        //            Debug.LogError("UnRegisterLanguageChangeEvent Fail,不存在正在监听的切换语言事件" + handler.Method.Name);
        //#endif
        //        }

        /// <summary>
        /// 判断参数的key 是否合法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool CheckIfMatchLocalizationKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            return Regex.IsMatch(key, s_LocalizationKeyReg, RegexOptions.Singleline);
        }
        #endregion


        #region 本地化加载配置文件

        /// <summary>
        /// 根据指定的语言和格式获取本地化文件名
        /// </summary>
        /// <param name="format"></param>
        /// <param name="targetLanguage"></param>
        /// <returns></returns>
        public static string GetLocalizationConfigFileName(ExportFormatEnum format, Language targetLanguage)
        {
            switch (format)
            {
                case ExportFormatEnum.Csv:
                    return string.Format("{0}_{1}{2}", ConstDefine.S_LocalizationDirectoryName, targetLanguage.ToString(), ".csv");
                case ExportFormatEnum.Json:
                    return string.Format("{0}_{1}{2}", ConstDefine.S_LocalizationDirectoryName, targetLanguage.ToString(), ".json");
                case ExportFormatEnum.Xml:
                    return string.Format("{0}_{1}{2}", ConstDefine.S_LocalizationDirectoryName, targetLanguage.ToString(), ".xml");
                default:
                    Debug.LogError("S_LocalizationConfigFileName Fail,无法识别的格式 " + format);
                    return string.Empty;
            }
        }

        /// <summary>
        /// 加载默认的语言 本地化配置，信息不一定是最新的，只是保证一开始能正确显示
        /// </summary>
        public static void LoadDefaultLocalizationConfig()
        {
            LoadLocalizationConfig(CurLanguage);
        }

        /// <summary>
        /// 加载多个语言版本的配置文件
        /// </summary>
        private static void LoadLocalizationConfig(Language language)
        {
            string filePath = string.Format("{0}/{1}", ConstDefine.S_LocalizationDirectoryName, GetLocalizationConfigFileName(ApplicationManager.S_Instance.mLocalizationExportFormatType, language));
            string content = string.Empty;

            ResourcesManagerUtility.LoadAssetSync<TextAsset>(filePath.GetPathWithOutExtension(), null, (isSuccess, textAsset) =>
            {
                if (isSuccess && textAsset != null && string.IsNullOrEmpty(textAsset.text) == false)
                {
                    content = textAsset.text;
                }
            });
            GetLocalizationConfigByFormat(content, language, ApplicationManager.S_Instance.mLocalizationExportFormatType);
            Debug.LogInfor(string.Format("完成加载语言 {0}的本地化配置", language));
        }

        /// <summary>
        /// 根据key 获取 本地化的内容(默认是当前的语言)
        /// </summary>
        public static string GetLocalizationByKey(string key)
        {
            return GetLocalizationByKey(key, CurLanguage);
        }

        /// <summary>
        /// 根据key 获取 本地化的内容(默认是当前的语言)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string GetLocalizationByKey(string key, Language language)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("GetLocalizationByKey Fail,parameter key  is Null");
                return string.Empty;
            }

            string result = string.Empty;

            Dictionary<string, string> languageLocalizationInfor = null;
            if (mAllSupportLanguageConfig.TryGetValue(language, out languageLocalizationInfor) == false)
            {
                LoadLocalizationConfig(language);
                Debug.LogError("TODO 转换配置配置");
            }

            if (languageLocalizationInfor != null)
            {
                if (languageLocalizationInfor.TryGetValue(key, out result))
                    return result;
            }

            Debug.LogError(string.Format("GetLocalizationByKey Fail,No Key={0} Of Language={1}", key, language));
            return result;
        }

        #endregion

        #region 解析获得的本地化配置

        /// <summary>
        /// 根据选择的语言和对应的导出格式解析配置文件
        /// </summary>
        /// <param name="content"></param>
        /// <param name="language"></param>
        /// <param name="exportFormat"></param>
        private static void GetLocalizationConfigByFormat(string content, Language language, ExportFormatEnum exportFormat)
        {
            switch (exportFormat)
            {
                case ExportFormatEnum.Csv:
                    mAllSupportLanguageConfig[language] = GetLocalizationConfigByCsv(content);
                    break;
                case ExportFormatEnum.Json:
                    mAllSupportLanguageConfig[language] = GetLocalizationConfigByJson(content);
                    break;
                case ExportFormatEnum.Xml:
                    mAllSupportLanguageConfig[language] = GetLocalizationConfigByXml(language, content);
                    break;
                default:
                    Debug.LogError("GetLocalizationConfigByFormat Fail,没有定义的类型");
                    break;
            }
        }

        private static Dictionary<string, string> GetLocalizationConfigByCsv(string content)
        {
            Dictionary<string, string> allConfig = new Dictionary<string, string>();
            string[] allLines = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string key, localizationValue;
            for (int dex = 0; dex < allLines.Length; dex++)
            {
                if (string.IsNullOrEmpty(allLines[dex])) continue;

                if (CsvUtility.ReadLineCsv<string, string>(allLines[dex], out key, out localizationValue))
                    allConfig[key] = localizationValue;
                else
                    Debug.LogError(string.Format("解析CSV 文件失败，第{0}行数据异常 ", dex));
            }

            return allConfig;
        }

        private static Dictionary<string, string> GetLocalizationConfigByJson(string content)
        {
            Dictionary<string, string> allConfig = SerializeManager.DeserializeObject<Dictionary<string, string>>(content);
            return allConfig;
        }

        private static Dictionary<string, string> GetLocalizationConfigByXml(Language language, string content)
        {
            Dictionary<string, string> allConfig = new Dictionary<string, string>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true; //忽略文档里面的注释

            XmlNode root = doc.SelectSingleNode("Table");
            if (root != null)
            {
                foreach (var RowChildNode in root.ChildNodes)
                {
                    XmlNode RowNode = (XmlNode)RowChildNode;

                    string key = RowNode.SelectSingleNode("Key").InnerText;
                    string localizationValue = RowNode.SelectSingleNode(language.ToString()).InnerText;
                    allConfig[key] = localizationValue;
                }
            }

            return allConfig;
        }

        #endregion


        #region 编辑器扩展
#if UNITY_EDITOR
        public static string LocalizationKeyAssetPath = "Assets/Editor/Core/Localization/localizationKey.txt";
        public static string LocalizationKeyAssetDatePath = "Editor/Core/Localization/localizationKey.txt";

        private static HashSet<string> s_AllLocalizationKeys = new HashSet<string>();
        public static void LoadDefaultLocalizationKey(ref HashSet<string> dataContainer)
        {
            s_AllLocalizationKeys.Clear();
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(LocalizationManager.LocalizationKeyAssetPath);
            if (textAsset == null)
            {
                Debug.LogError($"加载本地化key 失败{LocalizationKeyAssetPath}");
                return;
            }
            dataContainer = SerializeManager.DeserializeObject_NewtonJson<HashSet<string>>(textAsset.text);
        }

        public static bool IsExitLocalizationKey(string key)
        {
            if (s_AllLocalizationKeys.Count == 0)
                LoadDefaultLocalizationKey(ref s_AllLocalizationKeys);

            if (s_AllLocalizationKeys.Count == 0)
            {
                Debug.LogError($"加载本地化key 失败");
                return false;
            }
            return s_AllLocalizationKeys.Contains(key);
        }

#endif
        #endregion

    }
}
