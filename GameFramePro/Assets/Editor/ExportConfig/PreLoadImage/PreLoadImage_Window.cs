using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using GameFramePro.Upgrade;
using UnityEditor;


namespace GameFramePro.EditorEx
{
    public class PreLoadImage_Window : EditorWindow
    {
        [MenuItem("工具和扩展/导出资源配置/生成预加载图片配置3")]
        private static void OnOpenPreloadImageAssetConfigWin()
        {
            var win = GetWindow<PreLoadImage_Window>("生成活动图片资源预加载配置");
            win.minSize = new Vector2(600, 500);
            win.minSize = new Vector2(600, 800);
            win.Initialed();
            win.Show();
        }

        private void Initialed()
        {
            ConfigRelativePath = $"Assets/Editor/ExportConfig/PreLoadImage/{ConstDefine.S_PreloadImgConfiFileName}";
            mPreloadImageExportTopPath = ConstDefine.S_ExportRealPath.CombinePathEx(ConstDefine.S_PreLoadTextureTopDirectoryName); //导出的图片资源顶层目录
            mDeleteItemKey = string.Empty;
            outPutLanguageSupport = new List<UpgradeLanguage>(5);

            foreach (var language in System.Enum.GetValues(typeof(UpgradeLanguage)))
            {
                var targetLanguage = (UpgradeLanguage) System.Enum.Parse(typeof(UpgradeLanguage), language.ToString());
                outPutLanguageSupport.Add(targetLanguage);
            }
        }

        #region 路径配置

        /// <summary>/// 配置文件生成后的保存目录/// </summary>
        private string ConfigRelativePath;

        private string mPreloadImageExportTopPath;

        #endregion

        #region 导出设置

        private TextAsset configAsset = null;
        private PreloadImgConfigInfor mPreloadImgConfigInfor = null;
        private Vector2 mScrollView = Vector2.zero;


        private int mDataCount = 0; //数据的个数

        private string mDeleteItemKey = string.Empty;

        /// <summary>/// 导出支持的语言类型/// </summary>
        private List<UpgradeLanguage> outPutLanguageSupport = null;

        private UpgradeLanguage outExportLanguage = UpgradeLanguage.Chinese; //导出的语言类型

        #endregion


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(10);

            #region 第一行 默认的保存配置文件的路径

            EditorGUILayout.BeginHorizontal("box");
            if (configAsset == null)
                LoadDefaultConfigTestAsset();
            EditorGUILayout.ObjectField(new GUIContent("缓存上一次操作的配置文件"), configAsset, typeof(TextAsset), false);

            if (GUILayout.Button("显示配置详情", GUILayout.ExpandWidth(true)))
                ShowConfigView();

            EditorGUILayout.EndHorizontal();

            #endregion

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical("box");


            #region 选择需要配置的语言并自动加载图片

            EditorGUILayout.BeginHorizontal();
            outExportLanguage = (UpgradeLanguage) EditorGUILayout.EnumPopup("选择需要配置的导出图片语言", outExportLanguage, GUILayout.Width(400));

            if (GUILayout.Button("快速加载当前语言文件中夹资源", GUILayout.ExpandWidth(true)))
            {
                string topDirectory = $"{mPreloadImageExportTopPath}/{outExportLanguage}";
                if (EditorUtility.DisplayDialog("提示", $"快速加载目录{topDirectory}下所有的png图片，并且覆盖现有的配置,使用前请确保包含的数据个数清空为0", "确定", "取消"))
                {
                    OpenDirectoryGetAllFiles(topDirectory,outExportLanguage);
                }
            }

            EditorGUILayout.EndHorizontal();

            #endregion


            #region 清空配置和一键加载文件夹资源

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("清空配置数据", GUILayout.ExpandWidth(true)))
            {
                ClearConfig();
            }

            if (GUILayout.Button("选择需要加载文件夹资源", GUILayout.ExpandWidth(true)))
            {
                string directoryPath = EditorUtility.OpenFolderPanel("打开获取所有文件夹中的资源", Application.dataPath, "");
                if (string.IsNullOrEmpty(directoryPath)) return;
                OpenDirectoryGetAllFiles(directoryPath,outExportLanguage);
            }


            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            #endregion


            EditorGUILayout.EndVertical();

            ///导出设置
            GUILayout.Space(5);

            #region 一键加载配置

            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal("Box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button($"一键加载 {ConstDefine.S_PreLoadTextureTopDirectoryName} 目录下所有的配置图片", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
            {
                string[] topLanguageEnumes = System.IO.Directory.GetDirectories(mPreloadImageExportTopPath);
                foreach (var item in topLanguageEnumes)
                {
                    mDataCount = 0;
                    string DirectoryName = System.IO.Path.GetFileNameWithoutExtension(item);
                    outExportLanguage = (UpgradeLanguage) System.Enum.Parse(typeof(UpgradeLanguage), DirectoryName);

                    OpenDirectoryGetAllFiles(item,outExportLanguage);
                    if (mPreloadImgConfigInfor == null)
                    {
                        Debug.LogError("配置文件为null");
                        return;
                    }

                    if (CheckItemAvaliable())
                        return;

                    SaveConfig(true, outExportLanguage);
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            #endregion


            #region 显示所有的项

            if (mPreloadImgConfigInfor == null)
                mPreloadImgConfigInfor = new PreloadImgConfigInfor();

            mPreloadImgConfigInfor.GetTotalSize();

            if (string.IsNullOrEmpty(mDeleteItemKey) == false)
            {
                mPreloadImgConfigInfor.AllPreloadImgConfig.Remove(mDeleteItemKey);
                mDeleteItemKey = string.Empty;
                mDataCount--;
            }

            if (string.IsNullOrEmpty(mPreloadImgConfigInfor.Version))
                mPreloadImgConfigInfor.Version = "1.0.1";
            mPreloadImgConfigInfor.Version = EditorGUILayout.TextField("版本号:", mPreloadImgConfigInfor.Version);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"包含的数据个数: {mPreloadImgConfigInfor.AllPreloadImgConfig.Count}", GUILayout.Width(200));
            EditorGUILayout.LabelField($"资源总大小:{mPreloadImgConfigInfor.GetTotalSizeForShow()}", GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("新增一个项"))
            {
            //    mPreloadImgConfigInfor.AllPreloadImgConfig.Add( );
            }
            
            EditorGUILayout.EndHorizontal();

            #region 显示数据

            mScrollView = EditorGUILayout.BeginScrollView(mScrollView, GUILayout.Width(Screen.width - 70));

            foreach (var preloadImageInfor in mPreloadImgConfigInfor.AllPreloadImgConfig.Values)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("资源名:", GUILayout.Width(100));
                preloadImageInfor.mTextureRelativePath = EditorGUILayout.TextField(preloadImageInfor.mTextureRelativePath);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("资源MD5:", GUILayout.Width(100));
                preloadImageInfor.mAssetMD5 = EditorGUILayout.TextField(preloadImageInfor.mAssetMD5);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("资源大小(byte):", GUILayout.Width(100));
                preloadImageInfor.mAssetSize = EditorGUILayout.IntField(preloadImageInfor.mAssetSize);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("计算资源MD5", GUILayout.Width(100)))
                {
                    string path = EditorUtility.OpenFilePanel("选择.png|.jpg 图片", Application.dataPath, "*.*");
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        string extensionName = System.IO.Path.GetExtension(path);
                        if (extensionName != ".png" && extensionName != ".jpg")
                        {
                            Debug.LogError(extensionName + "选择的图片资源不是png 或者jpg：" + path);
                        }
                        else
                        {
                            preloadImageInfor.mAssetMD5 = MD5Helper.GetFileMD5OutLength(path, out var size);
                            preloadImageInfor.mTextureRelativePath = System.IO.Path.GetFileNameWithoutExtension(path);
                            preloadImageInfor.mAssetSize = (int) size;
                            Debug.Log($"获取{path}资源的MD5={preloadImageInfor.mAssetMD5}");
                        }
                    }
                }

                if (GUILayout.Button("删除这一项", GUILayout.Width(100)))
                {
                    mDeleteItemKey = preloadImageInfor.mTextureRelativePath;
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();

            #endregion

            EditorGUILayout.EndHorizontal();

            #endregion


            #region 操作

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("保存配置文件", GUILayout.Width(120), GUILayout.Height(40)))
            {
                if (mPreloadImgConfigInfor == null)
                {
                    Debug.LogError("配置文件为null");
                    return;
                }

                if (CheckItemAvaliable())
                    return;

                SaveConfig(true, outExportLanguage);
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            #endregion


            EditorGUILayout.EndVertical();
        }


        #region 实现

        private void ClearConfig()
        {
            mDataCount = 0;

            if (mPreloadImgConfigInfor == null)
                mPreloadImgConfigInfor = new PreloadImgConfigInfor();
            mPreloadImgConfigInfor.AllPreloadImgConfig = new Dictionary<string, PreloadImgInfor>();
        }

        private void SaveConfig(bool isSaveConfig, UpgradeLanguage targetLanguage)
        {
            if (isSaveConfig == false) return;
            string data = SerializeManager.SerializeObject(mPreloadImgConfigInfor);
            string savePath = $"{Application.dataPath.Replace(ConstDefine.S_AssetsName, "")}/{ConfigRelativePath}";

            IOUtility.CreateOrSetFileContent(savePath, data, false);
            OutExportConfigOfLanguage(targetLanguage.ToString(), data);
        }


        /// <summary>/// 输出数据到指定的目录/// </summary>
        private void OutExportConfigOfLanguage(string languageName, string data)
        {
            var outputDirectory = $"{ConstDefine.S_ExportRealPath}/{ConstDefine.S_PreLoadTextureTopDirectoryName}/{languageName}/{ConstDefine.S_PreloadImgConfiFileName}";

            IOUtility.CreateOrSetFileContent(outputDirectory, data);
            EditorUtility.DisplayDialog("提示", $"导出 语言{languageName} 预加载图片配置文件到目录{outputDirectory}中，注意查看", "已知晓");
            Debug.Log($"导出 语言{languageName} 预加载图片配置文件到目录{outputDirectory}中，注意查看");
        }


        /// <summary>/// 打开指定的文件夹并获取所有的文件/// </summary>
        private void OpenDirectoryGetAllFiles(string directoryPath,UpgradeLanguage language)
        {
            if (string.IsNullOrEmpty(directoryPath)) return;


            PreloadImgConfigInfor exitConfig = new PreloadImgConfigInfor();
            string configFilePath = $"{directoryPath}/{ConstDefine.S_PreloadImgConfiFileName}";
            if (System.IO.File.Exists(configFilePath))
            {
                string config = System.IO.File.ReadAllText(configFilePath);
                exitConfig = SerializeManager.DeserializeObject<PreloadImgConfigInfor>(config);
            }

            if (mPreloadImgConfigInfor == null)
                mPreloadImgConfigInfor = new PreloadImgConfigInfor();
            mPreloadImgConfigInfor.AllPreloadImgConfig.Clear();
            mDataCount = 0;

            string[] allFiles = System.IO.Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                string extension = Path.GetExtension(file);
                if (extension != ".png" && extension != ".jpg")
                    continue;

                PreloadImgInfor infor = new PreloadImgInfor
                {
                    mAssetMD5 = MD5Helper.GetFileMD5OutLength(file, out var size),
                    mAssetSize = (int) size,
                    mTextureRelativePath = file.Substring(directoryPath.Length + 1)
                };


                bool isNewAsset = true;
                foreach (var preloadImageConfig in exitConfig.AllPreloadImgConfig.Values)
                {
                    if (preloadImageConfig.mTextureRelativePath == infor.mTextureRelativePath)
                    {
                        isNewAsset = false;
                        if (preloadImageConfig.mAssetMD5 != infor.mAssetMD5 || preloadImageConfig.mAssetSize != infor.mAssetSize)
                            Debug.Log($"更新资源{infor.mTextureRelativePath}  MD5={infor.mAssetMD5}  大小={infor.mAssetSize}");
                        break;
                    }
                }

                if (isNewAsset)
                    Debug.Log($"添加新的资源{infor.mTextureRelativePath}  MD5={infor.mAssetMD5}  大小={infor.mAssetSize}");

                mPreloadImgConfigInfor.AllPreloadImgConfig.Add(infor.mTextureRelativePath, infor);
                mPreloadImgConfigInfor.mLanguage = language;
                mDataCount++;
            }
        }


        /// <summary>/// 加载默认的配置文件/// </summary>
        private void LoadDefaultConfigTestAsset()
        {
            if (configAsset != null)
            {
                if (mPreloadImgConfigInfor == null)
                    mPreloadImgConfigInfor = new PreloadImgConfigInfor();
                return;
            }

            configAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(ConfigRelativePath);
        }

        /// <summary>/// 显示数据/// </summary>
        private void ShowConfigView()
        {
            if (configAsset == null || string.IsNullOrEmpty(configAsset.text))
            {
                mPreloadImgConfigInfor = new PreloadImgConfigInfor();
                return;
            }

            mPreloadImgConfigInfor = SerializeManager.DeserializeObject<PreloadImgConfigInfor>(configAsset.text);
            mDataCount = mPreloadImgConfigInfor.AllPreloadImgConfig.Count;
            //   mPreloadImgConfigInfor.GetTotalSize();
        }

        /// <summary>/// 检测是否包含重复的名称、扩展名、目录/// </summary>
        private bool CheckItemAvaliable()
        {
            if (mPreloadImgConfigInfor == null || mPreloadImgConfigInfor.AllPreloadImgConfig.Count == 0)
                return false;
            HashSet<string> allAssetName = new HashSet<string>();
            foreach (var item in mPreloadImgConfigInfor.AllPreloadImgConfig.Values)
            {
                if (string.IsNullOrEmpty(item.mTextureRelativePath) || string.IsNullOrEmpty(item.mAssetMD5))
                {
                    Debug.LogError("---------没有配置文件名或者MD5" + item.mTextureRelativePath);
                    return true;
                }

                if (allAssetName.Contains(item.mTextureRelativePath) == false)
                    allAssetName.Add(item.mTextureRelativePath);
                else
                {
                    Debug.LogError($"包含重复的资源 :{item.mTextureRelativePath}");
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
