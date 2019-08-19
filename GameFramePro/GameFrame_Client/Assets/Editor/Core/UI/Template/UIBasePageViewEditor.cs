using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using GameFramePro.UI;

namespace GameFramePro.EditorEx
{
    public class UIBasePageViewEditor
    {
        [MenuItem("Assets/工具和扩展/辅助/创建继承 UIBaseChangePage 页面脚本")]
        private static void CreateUIBaseChangePageViewScript()
        {
            string ScriptName = "输入脚本名称";
            GameObject selectPrefab = null;
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                    {
                        selectPrefab = Selection.objects[0] as GameObject;
                        ScriptName = Selection.objects[0].name;
                    }
            }

            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(selectPrefab, savePath, UIPageTypeEnum.ChangePage);
        }

        [MenuItem("Assets/工具和扩展/辅助/创建继承 UIBaseWidget 组件脚本")]
        private static void CreateUIBaseWidgetViewScript()
        {
            string ScriptName = "输入脚本名称";
            GameObject selectPrefab = null;
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                        if (Selection.objects[0].name.StartsWith("UI"))
                        {
                            selectPrefab = Selection.objects[0] as GameObject;
                            ScriptName = Selection.objects[0].name;
                        }
            }

            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(selectPrefab, savePath, UIPageTypeEnum.Widget);
        }

        [MenuItem("Assets/工具和扩展/辅助/创建继承 UIBasePopWindow 弹窗脚本")]
        private static void CreateUIBasePopWindowViewScript()
        {
            string ScriptName = "输入脚本名称";
            GameObject selectPrefab = null;
            if (Selection.objects.Length > 0)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.objects[0]);
                if (assetPath.EndsWith(ConstDefine.S_PrefabExtension))
                    if (Selection.objects[0].name.StartsWith("UI"))
                    {
                        selectPrefab = Selection.objects[0] as GameObject;
                        ScriptName = Selection.objects[0].name;
                    }
            }

            string savePath = EditorUtility.SaveFilePanelInProject("选择脚本保存的目录", ScriptName, "cs", string.Empty, Application.dataPath);
            if (string.IsNullOrEmpty(savePath))
                return;

            CreateUIBasePageScriptByPath(selectPrefab, savePath, UIPageTypeEnum.PopWindow);
        }


        /// <summary>/// 根据给定的 模板文件路径和 模板类型创建指定类型的脚本/// </summary>
        private static void CreateUIBasePageScriptByPath(GameObject go, string saveScriptPath, UIPageTypeEnum type)
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

            string uiDefineStr = string.Empty;
            string uiReferenceStr = string.Empty;
            if (go != null)
            {
                UGUIComponentReference.GetComponentReferenceByConfig(go, out uiDefineStr, out uiReferenceStr);

                //拷贝到剪贴板
                TextEditor te = new TextEditor();
                te.text = uiDefineStr + System.Environment.NewLine + uiReferenceStr;
                te.SelectAll();
                te.Copy();
                Debug.LogEditorInfor($"自动生成 预制体对象 {go} 的组件引用配置为{System.Environment.NewLine}  {te.text}");
            }


            string fileName = System.IO.Path.GetFileNameWithoutExtension(saveScriptPath);
            string contentData = asset.text.Replace("#CLASSNAME#", fileName).Replace("#UIPARAMETER#", uiDefineStr).Replace("#UICOMPONENTREFERENCE#", uiReferenceStr);

            string fileRealPath = $"{IOUtility.GetFilePathParentDirectory(Application.dataPath, 1)}/{saveScriptPath}";

            //  Debug.Log("savePath=" + saveScriptPath);
            if (System.IO.File.Exists(saveScriptPath))
            {
                if (EditorUtility.DisplayDialog("脚本创建冲突", $"是否覆盖{saveScriptPath} 的脚本文件", "替换", "取消"))
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
            Debug.Log($"创建继承BasePage 脚本{fileName}成功！保存在路径{saveScriptPath}");
        }
    }
}
