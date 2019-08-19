using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Text;
using System.Linq;

namespace GameFramePro.EditorEx
{
    /// <summary>/// 管理Unity 的图片资源/// </summary>
    public class TextureAssetEditor
    {
        /// <summary>
        /// 记录导入UI图片的时候哪些图片格式可能有问题
        /// </summary>
        private static string s_UITextureFormatErrorRecod { get { return "Editor/ArtUITextureImportInfor.txt"; } }


        #region 检测Art目录下的资源是否都正确设置了PackingName

        [MenuItem("Assets/工具和扩展/图片管理/检测 Assets&Art&UI 目录下的图片是否被正确设置")]
        public static void AutoCheckArtTextureImportSetting()
        {
            string[] allTextur2DAssetsGuid = AssetDatabase.FindAssets("t:texture2d", new string[] { "Assets/Art/UI" });
            int currentCount = 0;
            int recordCount = 0;
            StringBuilder builder = StringUtility.GetStringBuilder();
            foreach (var assetGuid in allTextur2DAssetsGuid)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                EditorUtility.DisplayProgressBar("Checking...", "texture..." + assetPath, 1f * currentCount / allTextur2DAssetsGuid.Length);
                TextureImporter textureImport = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImport.textureType != TextureImporterType.Sprite)
                {
                    ++recordCount;
                    builder.Append(string.Format("{0:100} \t \t   Error TextureType: {1}", assetPath, textureImport.textureType));
                    builder.Append(System.Environment.NewLine);
                    continue;
                }
                string packName = IOUtility.GetPathDirectoryNameByDeep(assetPath);
                if (textureImport.spritePackingTag != packName)
                {
                    ++recordCount;
                    builder.Append(string.Format("{0:100}  \t \t  Error PackingName: {1}", assetPath, textureImport.textureType));
                    builder.Append(System.Environment.NewLine);
                    continue;
                }
            }
            EditorUtility.ClearProgressBar();
            string saveRecordPath = Application.dataPath.CombinePathEx(s_UITextureFormatErrorRecod);
            IOUtility.CreateOrSetFileContent(saveRecordPath, builder.ToString(), false);
            if (EditorUtility.DisplayDialog("提示", string.Format("查找完毕，已经在目录{0}生成文件，共找到{1}个需要修正的地方", saveRecordPath, recordCount), "确定"))
            {
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(System.IO.Path.Combine("Assets", s_UITextureFormatErrorRecod));
                AssetDatabase.OpenAsset(asset.GetInstanceID(), 0);
            }

            StringUtility.ReleaseStringBuilder(builder);
        }
        #endregion

        #region 设置图片的导入格式 和PackName

        private static bool CheckIfSelectAnyAsset()
        {
            var selectObjects = Selection.objects;
            if (selectObjects.Length == 0)
            {
                Debug.LogError("请至少选择一个图片文件或者文件夹");
                return false;
            }
            return true;
        }


        [MenuItem("Assets/工具和扩展/图片管理/包含的图片设置 PackName(忽略Default 格式资源)", false, 25)]
        private static void AutoSetUITexturesPakeNames()
        {
            if (CheckIfSelectAnyAsset() == false) return;

            SetSelectAssetTexturePakeNameOrFormat(Selection.objects, true, false,false);
        }
        [MenuItem("Assets/工具和扩展/图片管理/包含的图片设置 格式(忽略Default 格式资源)", false, 26)]
        private static void AutoSetUITexturesFormat()
        {
            if (CheckIfSelectAnyAsset() == false) return;

            SetSelectAssetTexturePakeNameOrFormat(Selection.objects, false, true,false);
        }


        [MenuItem("Assets/工具和扩展/图片管理/包含的图片设置 PackName(自动导入)", false, 20)]
        private static void AutoSetUITexturesPakeNames_AutoImport()
        {
            if (CheckIfSelectAnyAsset() == false) return;

            SetSelectAssetTexturePakeNameOrFormat(Selection.objects, true, false, true);
        }

        [MenuItem("Assets/工具和扩展/图片管理/包含的图片设置 格式(EditorApplicationSetting 下定义)(自动导入)", false, 15)]
        private static void AutoSetUITexturesFormat_AutoImport()
        {
            if (CheckIfSelectAnyAsset() == false) return;

            SetSelectAssetTexturePakeNameOrFormat(Selection.objects, false, true, true);
        }

        [MenuItem("Assets/工具和扩展/图片管理/包含的图片设置 PackName和格式(EditorApplicationSetting 下定义)(自动导入)", false, 10)]
        private static void AutoSetUITexturesPakeNamesAndFormat_AutoImport()
        {
            if (CheckIfSelectAnyAsset() == false) return;

            SetSelectAssetTexturePakeNameOrFormat(Selection.objects, true, true, true);
        }

        /// <summary>
        /// 设置图片资源的格式和PackName
        /// </summary>
        /// <param name="selectObjects"></param>
        /// <param name="isSetPackName">想要设置的packName</param>
        /// <param name="isSetFormat">格式设置是否有效</param>
        /// <param name="isAutoImport">z是都在没有导入资源的时候自动导入图片</param>
        private static void SetSelectAssetTexturePakeNameOrFormat(Object[] selectObjects, bool isSetPackName, bool isSetFormat, bool isAutoImport )
        {
            foreach (var selectObj in selectObjects)
            {
                string selectObjectRelativePath = AssetDatabase.GetAssetPath(selectObj);
                if (IOUtility.IsDirectoryPath(selectObjectRelativePath) == false)
                {
                    string packName = IOUtility.GetPathDirectoryNameByDeep(selectObjectRelativePath);
                    SetTextureImportSettingOfPlamt(selectObjectRelativePath, TextureImporterType.Sprite, isAutoImport, EditorApplicationSetting.S_DefaultTetureImportFormat, isSetFormat, packName, isSetPackName);
                }
                else
                {
                    string[] allContainAssetGuids = AssetDatabase.FindAssets("t:texture2d", new string[] { selectObjectRelativePath });
                    int currentCount = 0;
                    int realTextureCount = 0;
                    foreach (var assetGuid in allContainAssetGuids)
                    {
                        ++currentCount;
                        string assetRelativePath = AssetDatabase.GUIDToAssetPath(assetGuid);
                        if (assetRelativePath.EndsWith(".meta"))
                            Debug.Log("assetRelativePath=" + assetRelativePath);

                        EditorUtility.DisplayProgressBar("..", "Import Texture.." + assetRelativePath, currentCount * 1f / allContainAssetGuids.Length);
                        string extesion = System.IO.Path.GetExtension(assetRelativePath);
                        if (CheckIsTexture(assetRelativePath) == false)
                            continue;  //过滤非图片资源
                        ++realTextureCount;

                        string packName = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(assetRelativePath));
                        SetTextureImportSettingOfPlamt(assetRelativePath, TextureImporterType.Sprite, isAutoImport, EditorApplicationSetting.S_DefaultTetureImportFormat, isSetFormat, packName, isSetPackName);

                    }
                    Debug.LogEditorInfor(string.Format("目录 {0} 下设置了 {1} 个图片(.jpg|.png)", selectObjectRelativePath, realTextureCount));
                    EditorUtility.ClearProgressBar();
                }
            }
        }


        #endregion

        #region 关联图片预制体
        [MenuItem("Assets/工具和扩展/图片管理/包含的图片关联SpriteRender (过滤Default 格式资源)", false, 30)]
        private static void AutoSetUITexturesConnectSpriteRender()
        {
            var selectObjects = Selection.objects;
            if (selectObjects.Length == 0)
            {
                Debug.LogError("请至少选择一个图片文件或者文件夹");
                return;
            }

            SetSelectAssetTextureSpriteRenderConnect(selectObjects);
        }


        /// <summary>
        /// 关联SpriteRender 
        /// </summary>
        /// <param name="selectObjects"></param>
        private static void SetSelectAssetTextureSpriteRenderConnect(Object[] selectObjects)
        {
            string saveDirectoryPath = EditorUtility.SaveFolderPanel("选择生成关联Sprite预制体的保存文件夹", ConstDefine.S_ResourcesRealPath, "不需要设置，根据Sprite名称设置");
            if (string.IsNullOrEmpty(saveDirectoryPath))
                return;

            if (IOUtility.IsContainDirectory(saveDirectoryPath, ConstDefine.S_AssetsName) == false)
            {
                Debug.LogError("请确保选择保存目录在Assets 下,不合理的保存路径 " + saveDirectoryPath);
                return;
            }
            saveDirectoryPath = IOUtility.GetPathFromSpecialDirectoryName(saveDirectoryPath, ConstDefine.S_AssetsName);
           // Debug.LogEditorInfor("saveDirectoryPath=" + saveDirectoryPath);
            foreach (var selectObj in selectObjects)
            {
                string selectObjectRelativePath = AssetDatabase.GetAssetPath(selectObj);
                string directoryName = IOUtility.GetPathDirectoryNameByDeep(selectObjectRelativePath);

                if (IOUtility.IsDirectoryPath(selectObjectRelativePath) == false)
                {
                    if (SpriteAssetConnectSpriteRender(selectObjectRelativePath, saveDirectoryPath, directoryName))
                        AssetDatabase.Refresh();
                }//选择单个文件
                else
                {
                    string[] allContainAssetGuids = AssetDatabase.FindAssets("t:texture2d", new string[] { selectObjectRelativePath });
                    int currentCount = 0;
                    int connectTextureCount = 0;
                    try
                    {
                        foreach (var assetGuid in allContainAssetGuids)
                        {
                            ++currentCount;
                            string assetRelativePath = AssetDatabase.GUIDToAssetPath(assetGuid);

                            EditorUtility.DisplayProgressBar("..", "Import Texture.." + assetRelativePath, currentCount * 1f / allContainAssetGuids.Length);
                            string extesion = System.IO.Path.GetExtension(assetRelativePath);
                            if (CheckIsTexture(assetRelativePath) == false)
                                continue;  //过滤非图片资源

                            if (SpriteAssetConnectSpriteRender(assetRelativePath, saveDirectoryPath, directoryName))
                                ++connectTextureCount;
                        }

                        if (currentCount > 0)
                            AssetDatabase.Refresh();
                        Debug.LogEditorInfor(string.Format("目录 {0} 下关联了 {1} 个精灵资源", selectObjectRelativePath, connectTextureCount));
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogEditorError(e);
                    }
                    finally
                    {
                        EditorUtility.ClearProgressBar();
                    }
                }
            }
        }


        private static bool SpriteAssetConnectSpriteRender(string assetPath, string saveDirectoryPath, string assetRelativeDirectoryName)
        {
            TextureImporter import = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (import.textureType != TextureImporterType.Sprite)
            {
                Debug.LogEditorInfor(string.Format("SpriteAssetConnectSpriteRender Error, Wrong Type {0}  of assetPath={1}", import.textureType, assetPath));
                return false;
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            string assetRelativePath = IOUtility.GetPathFromSpecialDirectoryName(assetPath, assetRelativeDirectoryName, true); //获取当前资源相对性选择目录的路径
            assetRelativePath = IOUtility.GetPathWithOutExtension(assetRelativePath);
            string spriteRendePrefabPath = saveDirectoryPath.CombinePathEx(assetRelativePath); //合并得到最终在Unity 保存生成的Prefab资源的路径
            string spriteRenderAssetPrefabPath = string.Format("{0}.prefab", spriteRendePrefabPath);
            GameObject assetPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(spriteRenderAssetPrefabPath);
            if (assetPrefab != null)
            {
                if (EditorUtility.DisplayDialog("提示", string.Format("路径{0} 已经存在同名的资源{1}. 是否直接更新关联的Sprite", spriteRenderAssetPrefabPath, assetPrefab.name), "更新", "忽略"))
                {
                    Debug.LogEditorInfor("忽略生成关联预制体Sprite 资源; " + assetPath);
                    return false;
                }
                var spriteRender = assetPrefab.GetComponent<SpriteRenderer>();

                if (spriteRender != null)
                {
                    spriteRender.sprite = sprite;
                    return true;
                }
            }//存在这个生成的预制体
            IOUtility.CheckOrCreateDirectory(System.IO.Path.GetDirectoryName(spriteRendePrefabPath)); //创建目录


            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
            //***创建预制体并关联图片
            GameObject go = ResourcesManager.Instantiate(assetName);
            go.GetAddComponentEx<SpriteRenderer>().sprite = sprite;

            PrefabUtility.SaveAsPrefabAsset(go, spriteRenderAssetPrefabPath);
            ResourcesManager.DestroyImmediate(go);
            return true;
        }

        #endregion


        #region 辅助和实现


        /// <summary>
        /// 判断指定的资源目录是否是图片
        /// </summary>
        /// <param name="assetRelativePath"></param>
        /// <returns></returns>
        private static bool CheckIsTexture(string assetRelativePath)
        {
            string extesion = System.IO.Path.GetExtension(assetRelativePath);
            if (extesion != ".jpg" && extesion != ".png")
                return false;
            return true;
        }

      

        /// <summary>
        /// 设置多个平台下的图片导入格式
        /// </summary>
        /// <param name="import"></param>
        private static void SetTextureImportSettingOfPlamt(string assetPath, TextureImporterType textureType, bool isAutoImport, TextureImporterFormat format, bool isFormatSetEnable, string packingTag, bool isPackingTagEnable = false)
        {
            TextureImporter import = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (import == null)
            {
                Debug.LogError("SetTextureImportSetting  Fail assetPath=" + assetPath);
                return;
            }
            if (isAutoImport == false && import.textureType != textureType)
            {
                Debug.LogError("SetTextureImportSetting  Fail assetPath=" + assetPath);
                return;
            }
            bool isNeedReImport = false;  //标示是否需要导入 避免重复导入


            int width = 0;
            int height = 0;
            EditorHelperUtility.GetOriginalSize(import, out width, out height);
            int width_Power2 = MathfUtility.GetMinMaxPowerNumber(width);
            int height_Power2 = MathfUtility.GetMinMaxPowerNumber(height);

            if(isAutoImport && import.textureType != textureType)
            {
                import.textureType = textureType;
                isNeedReImport = true;
            }
            import.mipmapEnabled = false;

            if (isPackingTagEnable)
            {
                if (import.spritePackingTag != packingTag)
                {
                    isNeedReImport = false;
                    import.spritePackingTag = packingTag;
                }
            }

            if (isFormatSetEnable)
            {
                isNeedReImport = SetTextureImportFormatOfPlamt(isNeedReImport, import, format, width_Power2, height_Power2);
            }

            if (isNeedReImport)
                AssetDatabase.ImportAsset(import.assetPath);
        }

        /// <summary>
        /// 设置多个平台下的图片导入格式
        /// </summary>
        /// <param name="import"></param>
        /// <param name="textureType"></param>
        /// <param name="packingTag"></param>
        /// <param name="format"></param>
        private static bool SetTextureImportFormatOfPlamt(bool isNeedReimport, TextureImporter import, TextureImporterFormat format, int width_Power2, int height_Power2)
        {
            //Default 
            TextureImporterPlatformSettings defaultSetting = import.GetPlatformTextureSettings("Standalone");
            if (isNeedReimport || defaultSetting.format != format)
            {
                isNeedReimport = true;
                defaultSetting.format = format;
                defaultSetting.overridden = true;
                defaultSetting.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
                defaultSetting.maxTextureSize = Mathf.Max(width_Power2, height_Power2);
                import.SetPlatformTextureSettings(defaultSetting);
            }


            //Android 
            TextureImporterPlatformSettings AndroidSetting = import.GetPlatformTextureSettings("Android");
            if (isNeedReimport || AndroidSetting.format != format)
            {
                isNeedReimport = true;
                AndroidSetting.format = format;
                AndroidSetting.overridden = true;
                AndroidSetting.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
                AndroidSetting.maxTextureSize = Mathf.Max(width_Power2, height_Power2);
                import.SetPlatformTextureSettings(AndroidSetting);
            }


            //IOS
            TextureImporterPlatformSettings iPhoneSetting = import.GetPlatformTextureSettings("iPhone");
            if (isNeedReimport || iPhoneSetting.format != format)
            {
                isNeedReimport = true;
                iPhoneSetting.format = format;
                iPhoneSetting.overridden = true;
                iPhoneSetting.resizeAlgorithm = TextureResizeAlgorithm.Bilinear;
                iPhoneSetting.maxTextureSize = Mathf.Max(width_Power2, height_Power2);
                iPhoneSetting.format = format;
                import.SetPlatformTextureSettings(iPhoneSetting);
            }

            return isNeedReimport;
        }

        #endregion




    }
}
