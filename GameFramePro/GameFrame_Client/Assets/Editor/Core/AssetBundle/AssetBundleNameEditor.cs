using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>/// 管理和生成AssetBundleName (按照相对于Resources 目录命名)/// </summary>
    public class AssetBundleNameEditor
    {

        [MenuItem("Assets/工具和扩展/ABundleName/文件夹设置对应的ABundleName(递归)")]
        private static void CreateAssetBundleNameOfDirectory()
        {
            if (Selection.objects.Length == 0)
            {
                Debug.LogEditorError("未选择任何有效的目录,无法设置对应的AssetBundleName");
                return;
            }

            foreach (var selectObject in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(selectObject);
                if (string.IsNullOrEmpty(System.IO.Path.GetExtension(assetPath)) == false)
                {
                    Debug.LogEditorError("请选择文件夹操作 " + assetPath);
                    continue;
                }


                string[] containAssets = AssetDatabase.FindAssets("", new string[] { assetPath });
                int currentCount = 0;
                foreach (var assetId in containAssets)
                {
                    ++currentCount;
                    EditorUtility.DisplayProgressBar("...", "Deal.." + assetPath, currentCount * 1f / containAssets.Length);
                    string assetRelativePathToAssets = AssetDatabase.GUIDToAssetPath(assetId);
                    if (IOUtility.IsDirectoryPath(System.IO.Path.GetFullPath(assetRelativePathToAssets)))
                    {
                        SetAssetBundleName(assetRelativePathToAssets, string.Empty);
                        continue;
                    }

                    string relativeResourcesPath = IOUtility.GetPathFromSpecialDirectoryName(assetRelativePathToAssets, ConstDefine.S_ResourcesName, false);
                    relativeResourcesPath = System.IO.Path.GetDirectoryName(relativeResourcesPath);
                    string assetBundleName = relativeResourcesPath;

                    //需要设置一个扩展名 否则打包时候复制资源失败
                    assetBundleName = assetBundleName + ConstDefine.S_AssetBundleExtension;
                    SetAssetBundleName(assetRelativePathToAssets, assetBundleName);
                    Debug.LogEditorInfor($"资源: {assetRelativePathToAssets:50} \t assetBundleName={assetBundleName} ");
                }

                Debug.LogEditorInfor($"选择文件{assetPath} 设置了{containAssets.Length}个资源名");
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh();
        }




        //打包需要过滤的资源扩展名
        private static readonly HashSet<string> mNeedIgnoreAssetExtension = new HashSet<string>() { ".meta", ".dll", ".cs" };

        // private static string s_AssetBundleNameRule = "资源相对于Resources 目录的目录命名\n";
        //    "为了确保目录名不重复，如果是Resources下资源，则目录名为相对于Resources的路径，否则为相对于Assets目录\n" +
        //    "目录层级之间使用符号 &取代目录符号\n";

        private static string s_AssetBundleNameRule = "资源相对于Resources 目录的目录命名\n";


        //设置AssetBundleName 信息
        private static bool SetAssetBundleName(string assetPath, string assetBundleName)
        {
            AssetImporter import = AssetImporter.GetAtPath(assetPath);
            if (import.assetBundleName != assetBundleName)
            {
                import.assetBundleName = assetBundleName;
                import.SaveAndReimport();
                return true;
            }

            return false;
        }



        //修改资源的AssetBundle Name
        internal class AssetBundleNameModifyWindow : EditorWindow
        {
            static string ShowAssetBundleName = string.Empty;
            static string newAssetBundleName = string.Empty;

            static HashSet<string> AllSelectAssetPaths = new HashSet<string>();
            static AssetBundleNameModifyWindow window;

            [MenuItem("Assets/工具和扩展/AssetBundleName/修改AssetBundle Name")]
            private static void ModifyAassetBundleName()
            {
                if (Selection.objects.Length == 0)
                {
                    Debug.LogEditorError("未选择任何有效的资源,无法设置对应的AssetBundleName");
                    return;
                }

                foreach (var selectObject in Selection.objects)
                {
                    string assetPath = AssetDatabase.GetAssetPath(selectObject);
                    if (string.IsNullOrEmpty(System.IO.Path.GetExtension(assetPath)))
                    {
                        //Debug.LogEditorError("请选择Project 中的资源操作 " + assetPath);
                        //return;
                        continue;
                    }

                    AssetImporter import = AssetImporter.GetAtPath(assetPath);
                    if (string.IsNullOrEmpty(ShowAssetBundleName))
                        ShowAssetBundleName = import.assetBundleName;
                    else
                    {
                        if (import.assetBundleName != null && import.assetBundleName != ShowAssetBundleName)
                        {
                            Debug.LogError($"选择的资源{assetPath} 的AssetBundle Name={import.assetBundleName} 不一致{ShowAssetBundleName}");
                       //     continue;
                        }
                    }
                    AllSelectAssetPaths.Add(assetPath);
                }

                if (AllSelectAssetPaths.Count == 0)
                {
                    Debug.LogEditorError("请选择Project 中的资源操作 " );
                    return;
                }

                newAssetBundleName = ShowAssetBundleName;
                window = EditorWindow.GetWindow<AssetBundleNameModifyWindow>("修改AssetBundle Name");
                window.minSize = new Vector2(400, 100);
                window.maxSize = new Vector2(600, 100);
                window.Show();
                Debug.Log($"选择了{AllSelectAssetPaths.Count} 个资源，默认的AssetBundleName={ShowAssetBundleName}");
            }


            private void OnGUI()
            {
                GUILayout.Space(10);
                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();
                GUILayout.Label("AssetBundle Name:", GUILayout.Width(120));
                newAssetBundleName = GUILayout.TextField(newAssetBundleName, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();

                GUILayout.Space(30);
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("确定",GUILayout.Height(30),GUILayout.Width(80)))
                {
                    if (newAssetBundleName != ShowAssetBundleName)
                    {
                        EditorUtility.DisplayProgressBar("修改AssetBundle Name", "Start...", 0.01f);
                        int dex = 0;
                        foreach (var assetPath in AllSelectAssetPaths)
                        {
                            ++dex;
                            AssetImporter import = AssetImporter.GetAtPath(assetPath);
                            import.assetBundleName = newAssetBundleName;
                            EditorUtility.DisplayProgressBar("修改AssetBundle Name", "Start...", 1f * dex / AllSelectAssetPaths.Count);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    Debug.Log($"修改了{AllSelectAssetPaths.Count} 个资源的AssetBundle Name 修改为:{newAssetBundleName}");
                    window.Close();
                }

                if (GUILayout.Button("取消", GUILayout.Height(30), GUILayout.Width(80)))
                {
                    window.Close();
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }




    }
}
