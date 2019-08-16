using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using GameFramePro.Localization;

namespace GameFramePro.EditorEx
{
    /// <summary>/// 本地化多语言配置窗口/// </summary>
    public class LocalizationConfig_Win : EditorWindow
    {
        [MenuItem("工具和扩展/Localization/多语言配置窗口")]
        static void OpenLocalizationConfigWin()
        {
            LocalizationConfig_Win win = EditorWindow.GetWindow<LocalizationConfig_Win>("多语言本地化配置");
            win.minSize = new Vector2(400, 600);
            win.Show();
            win.InitialedLocalizationConfigWindow();
        }

        private void InitialedLocalizationConfigWindow()
        {
            mLocalizationConfigExcelPath = PlayerPrefsManager.GetString(EditorPlayerPrefsKeyDefine.S_LastSelectLocalizationExcelPath);
            if (string.IsNullOrEmpty(mLocalizationConfigExcelPath))
                mLocalizationConfigExcelPath = Application.dataPath;
            if (string.IsNullOrEmpty(mLocalizationConfigExcelExportPath))
                mLocalizationConfigExcelExportPath = ConstDefine.S_ResourcesRealPath.CombinePathEx(ConstDefine.S_LocalizationDirectoryName);
        }

        #region Data
        private DataSet mLocalizationExcelData = null;

        private  string mLocalizationConfigExcelPath;  //需要解析的本地化Excel配置路径
        private static string mLocalizationConfigExcelExportPath;  //解析的本地化Excel 导出文件路径
        private static readonly string s_LocalizationKeyReg = @"^(_@)+[0-9A-Za-z_]+$";

        private static Language mExportLanguageType = Language.zh_CN; //导出的语言类型
        private static ExportFormatEnum mExportFormatType = ExportFormatEnum.Csv; //导出的配置格式类型

        private bool mIsFormatEnable = false; //标示Excel 是否格式合法
        #endregion

        private void OnDisable()
        {
            mLocalizationExcelData = null;
        }

        private void OnGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");

            #region Excel 配置信息
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("需要解析的Excel路径:", GUILayout.Width(150));
            mLocalizationConfigExcelPath = GUILayout.TextField(mLocalizationConfigExcelPath, GUILayout.Width(Screen.width - 260));
            if (GUILayout.Button("选择Excel", GUILayout.Width(80)))
            {
                string selectExcelPath = EditorUtility.OpenFilePanel("选择本地化Excel 文件", mLocalizationConfigExcelPath, "*.*");
                if (string.IsNullOrEmpty(selectExcelPath) == false)
                {
                    mLocalizationConfigExcelPath = selectExcelPath;
                    PlayerPrefsManager.SetString(EditorPlayerPrefsKeyDefine.S_LastSelectLocalizationExcelPath, mLocalizationConfigExcelPath);
                    Debug.Log("选择本地化Excel 路径为：" + selectExcelPath);
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(3);
            GUIStyle rulerGUIStyle = new GUIStyle();
            rulerGUIStyle.normal.textColor = ColorExpand.HtmToColor("#1B7AD4");
            GUILayout.Label("Excel 中第一行为支持的本地化语言key,对应Language 枚举中的有效字符串", rulerGUIStyle);
            GUILayout.Space(2);
            GUILayout.Label("Excel 中第二行为本地户内容的格式，默认是<color=#FF0000>string </color> 类型", rulerGUIStyle);
            GUILayout.Space(2);
            GUILayout.Label(string.Format("Excel 中第一列的Key 必须以<color=#FF0000> {0}</color> 开头，否则会检测到格式错误", ConstDefine.S_LocalizationKeyFlag), rulerGUIStyle);
            GUILayout.EndVertical();
            #endregion

            #region 导出配置
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Excel中导出指定的语言:", GUILayout.Width(150));
            Language selectedLanguage = (Language)EditorGUILayout.EnumFlagsField(mExportLanguageType, GUILayout.Width(Screen.width - 180));
            if (selectedLanguage != mExportLanguageType)
            {
                Debug.LogEditorInfor("切换导出语言为 " + selectedLanguage);
                mExportLanguageType = selectedLanguage;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Excel中导出指定的格式:", GUILayout.Width(150));
            ExportFormatEnum selectedFromat = (ExportFormatEnum)EditorGUILayout.EnumFlagsField(mExportFormatType, GUILayout.Width(Screen.width - 180));
            if (selectedFromat != mExportFormatType)
            {
                Debug.LogEditorInfor("切换导出格式为 " + selectedFromat);
                mExportFormatType = selectedFromat;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Excel中导出的配置保存目录:", GUILayout.Width(150));
            GUILayout.TextField(mLocalizationConfigExcelExportPath, GUILayout.Width(Screen.width - 180));
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            #endregion


            #region 功能接口
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");

            #region 检测Excel 文件
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("检测Excel 文件格式", GUILayout.Height(35)))
            {
                LoadLocalizationExcel();
                CheckExcelLocalizationKey();
            }
            if (GUILayout.Button("Excel 导出指定语言的配置文件", GUILayout.Height(35)))
            {
                if (mIsFormatEnable == false)
                {
                    Debug.LogError(string.Format("请确保先进行了格式检测并且没有格式错误!!"));
                    return;
                }

                if (mExportLanguageType == 0)
                {
                    Debug.LogError("没有选择需要导出的语言");
                    return;
                }
                if (mExportFormatType == 0)
                {
                    Debug.LogError("没有选择需要导出的格式");
                    return;
                }

                List<Language> allLanguages = EnumUtility.GetEnumValue<Language>();
                if ((int)mExportLanguageType != -1)
                {
                    for (int dex = allLanguages.Count - 1; dex >= 0; dex--)
                    {
                        if ((allLanguages[dex] & mExportLanguageType) != allLanguages[dex])
                            allLanguages.RemoveAt(dex);
                    }
                }

                List<ExportFormatEnum> allexportFormats = EnumUtility.GetEnumValue<ExportFormatEnum>();
                if ((int)mExportFormatType != -1)
                {
                    for (int dex = allexportFormats.Count - 1; dex >= 0; dex--)
                    {
                        if ((allexportFormats[dex] & mExportFormatType) != allexportFormats[dex])
                            allexportFormats.RemoveAt(dex);
                    }
                }

                ExportExcel(allLanguages, allexportFormats);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            #endregion

            GUILayout.EndVertical();
            #endregion

            GUILayout.EndVertical();


        }

        #region 辅助接口
        /// <summary>
        /// 加载Excel 文件
        /// </summary>
        private void LoadLocalizationExcel()
        {
            //if (mLocalizationExcelData != null)
            //    return;  //这里需要注释避免修改没有即时获取
            mLocalizationExcelData = ExcelUtility.GetExcelData(mLocalizationConfigExcelPath);
            if (mLocalizationExcelData != null)
                Debug.LogEditorInfor(string.Format("加载路径 {0} 本地化Excel 成功!!", mLocalizationConfigExcelPath));
        }

        /// <summary>
        /// 检测Excel key 是否合法
        /// </summary>
        private void CheckExcelLocalizationKey()
        {
            if (mLocalizationExcelData == null)
            {
                mIsFormatEnable = false;
                return;
            }
            DataTable dataTable = mLocalizationExcelData.Tables[0];

            if (dataTable.Rows.Count < 2)
            {
                mIsFormatEnable = false;
                Debug.LogError("当前读取的表中不包含任何数据" + dataTable.TableName);
                return;
            }

            mIsFormatEnable = true;
            #region 获取一共有多少列数据
            int Column = 1;
            for (int dex = 1; dex < dataTable.Rows[0].ItemArray.Length; dex++)
            {
                var column = dataTable.Rows[0].ItemArray[dex];
                if (string.IsNullOrEmpty(column.ToString()))
                    break;
                ++Column;
            }
            Debug.Log(string.Format("一共有{0} 列有效的列", Column));
            #endregion


            StringBuilder stringBuilder = new StringBuilder();
            Dictionary<string, int> allKeyRecords = new Dictionary<string, int>();

            for (int row = 2; row < dataTable.Rows.Count; ++row)
            {
                string key = dataTable.Rows[row][0].ToString(); //每一行的第一列为本地化的key

                if (string.IsNullOrEmpty(key))
                {
                    mIsFormatEnable = false;
                    stringBuilder.Append(string.Format("Row={0}  Key 为null", row));
                    stringBuilder.Append(System.Environment.NewLine);
                    continue;
                }

                if(allKeyRecords.ContainsKey(key))
                {
                    mIsFormatEnable = false;
                    stringBuilder.Append(string.Format("Row={0}  Key={1} 重复的key", row, key));
                    stringBuilder.Append(System.Environment.NewLine);
                    continue;
                }
                else
                    allKeyRecords[key] = row;

                if (Regex.IsMatch(key, s_LocalizationKeyReg, RegexOptions.Singleline) == false)
                {
                    mIsFormatEnable = false;

                    stringBuilder.Append(string.Format("Row={0}  Key={1} 格式错误", row, key));
                    stringBuilder.Append(System.Environment.NewLine);
                    continue;
                }


            }

            if (mIsFormatEnable == false)
            {
                string content = stringBuilder.ToString();
                string errorFormatPath = Application.dataPath.CombinePathEx(ConstDefine.S_EditorName).CombinePathEx("localizationErrorFormat.txt");
                IOUtility.CreateOrSetFileContent(errorFormatPath, content, false);
                Debug.LogInfor("Excel 中部分Key 格式错误，已经输出保存到文件" + errorFormatPath);
                return;
            }
            Debug.LogEditorInfor("本地化语言表Excel 格式正确，可以导出");
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        private void ExportExcel(List<Language> languages, List<ExportFormatEnum> exportFormats)
        {
            string filePath = mLocalizationConfigExcelExportPath;
            foreach (var languagege in languages)
            {
                var allExportColumn = GetExportColumnByLanguage(languagege);
                foreach (var exportFormat in exportFormats)
                {
                    filePath = mLocalizationConfigExcelExportPath.CombinePathEx(LocalizationManager.GetLocalizationConfigFileName(exportFormat, languagege));
                    switch (exportFormat)
                    {
                        case ExportFormatEnum.Csv:
                            ExcelToCsvUtility.ConvertExcelToCsv(mLocalizationExcelData.Tables[0], filePath, allExportColumn);
                            break;
                        case ExportFormatEnum.Json:
                            ExcelToJsonUtility.ConvertExcelToJson(mLocalizationExcelData.Tables[0], filePath, allExportColumn);
                            break;
                        case ExportFormatEnum.Xml:
                            ExcelToXmlUtility.ConvertExcelToXml(mLocalizationExcelData.Tables[0], filePath, allExportColumn);
                            break;
                        default:
                            Debug.LogError("未知的格式 " + exportFormat);
                            break;
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        private int[] GetExportColumnByLanguage(Language language)
        {
            if (mLocalizationExcelData == null) return null;
            List<Language> allLanguage = EnumUtility.GetEnumValue<Language>();
            List<int> exportColumn = new List<int>();
            exportColumn.Add(0);
            Dictionary<Language, int> excelLanguageColumn = new Dictionary<Language, int>();
            for (int dex = 1; dex < mLocalizationExcelData.Tables[0].Rows[0].ItemArray.Length; dex++)
            {
                string firstRowColumnValue = mLocalizationExcelData.Tables[0].Rows[0].ItemArray[dex].ToString();
                Language curLanguage;
                if (System.Enum.TryParse(firstRowColumnValue, out curLanguage))
                    excelLanguageColumn[curLanguage] = dex;
                else
                    Debug.LogError("GetExportColumnByLanguage Fail,没有对应到 Language 的列索引" + dex);
            }

            int searchIndex = 0;
            if (excelLanguageColumn.TryGetValue(language, out searchIndex))
            {
                exportColumn.Add(searchIndex);
                return exportColumn.ToArray();
            }
            Debug.LogError("没有对应到语言的列 " + language);
            return null;
        }

        #endregion

    }
}
