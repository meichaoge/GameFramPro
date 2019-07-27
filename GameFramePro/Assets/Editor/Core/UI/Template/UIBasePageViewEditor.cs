using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameFramePro.UI;

namespace GameFramePro.EditorEx
{
    public class UIBasePageViewEditor
    {

        [MenuItem("Assets/Tools/辅助/创建继承 UIBaseChangePage 页面脚本")]
        private static void CreateUIBaseChangePageViewScript()
        {
            string ScriptName = "输入脚本名称";
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                        ScriptName = Selection.objects[0].name;
            }
            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(savePath, UIPageTypeEnum.ChangePage);
        }
        [MenuItem("Assets/Tools/辅助/创建继承 UIBaseWidget 组件脚本")]
        private static void CreateUIBaseWidgetViewScript()
        {

            string ScriptName = "输入脚本名称";
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                        ScriptName = Selection.objects[0].name;
            }
            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(savePath, UIPageTypeEnum.Widget);
        }
        [MenuItem("Assets/Tools/辅助/创建继承 UIBasePopWindow 弹窗脚本")]
        private static void CreateUIBasePopWindowViewScript()
        {
            string ScriptName = "输入脚本名称";
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                        ScriptName = Selection.objects[0].name;
            }
            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(savePath, UIPageTypeEnum.PopWindow);
        }






        private static void CreateUIBasePageScriptByPath(string saveScriptPath, UIPageTypeEnum type)
        {
            string tempViewPath = string.Empty;
            switch (type)
            {
                case UIPageTypeEnum.ChangePage:
                    tempViewPath = "Assets/Editor/Core/UI/Template/UIBaseChangePage.tpl.txt";
                    break;
                case UIPageTypeEnum.PopWindow:
                    tempViewPath = "Assets/Editor/Core/UI/Template/UIBasePopWindow.tpl.txt";
                    break;
                case UIPageTypeEnum.Widget:
                    tempViewPath = "Assets/Editor/Core/UI/Template/UIBaseWidget.tpl.txt";
                    break;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    break;
            }
            TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(tempViewPath);



            string fileName = System.IO.Path.GetFileNameWithoutExtension(saveScriptPath);
            string contentData = asset.text.Replace("#CLASSNAME#", fileName).Replace("#UIPARAMETER#", string.Empty);
            string fileRealPath = string.Format("{0}/{1}", IOUtility.GetFilePathParentDirectory(Application.dataPath, 1), saveScriptPath);

          //  Debug.Log("savePath=" + saveScriptPath);
            if (System.IO.File.Exists(saveScriptPath))
            {
                if (EditorUtility.DisplayDialog("脚本创建冲突", string.Format("是否覆盖{0} 的脚本文件", saveScriptPath), "替换", "取消"))
                {
                    IOUtility.CreateOrSetFileContent(fileRealPath, contentData, false);
                }
                else
                {
                    Debug.Log("文件创建冲突 取消创建脚本 " + saveScriptPath);
                    return;
                }
            }
            else
            {
                IOUtility.CreateOrSetFileContent(fileRealPath, contentData, false);
            }

            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(saveScriptPath));
            Debug.Log(string.Format("创建继承BasePage 脚本{0}成功！保存在路径{1}", fileName, saveScriptPath));


        }


    }
}