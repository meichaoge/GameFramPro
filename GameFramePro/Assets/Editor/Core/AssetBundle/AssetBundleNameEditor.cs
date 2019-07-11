using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 管理和生成AssetBundleName
    /// </summary>
    public class AssetBundleNameEditor
    {
       
        //打包需要过滤的资源扩展名
        private readonly static HashSet<string> mNeedIgnoreAssetExtension = new HashSet<string>() { ".meta", ".dll", ".cs" };

        //private static string s_AssetBundleNameRule = "资源按照所属的文件夹设置AssetBundleName\n" +
        //    "为了确保目录名不重复，如果是Resources下资源，则目录名为相对于Resources的路径，否则为相对于Assets目录\n" +
        //    "目录层级之间使用符号 &取代目录符号\n";

        private static string s_AssetBundleNameRule = "资源按照所属的文件夹设置AssetBundleName,相同父目录的文件会打包到同一个AssetBundle 中\n";
   

        [MenuItem("Assets/Tools/设置AssetBundleName/文件夹所有资源设置对应的AssetBundleName(递归)")]
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
                    if (string.IsNullOrEmpty(System.IO.Path.GetExtension(assetRelativePathToAssets)))
                    {
                        string assetBundleName =System.IO.Path.GetFileNameWithoutExtension(assetRelativePathToAssets);
                        SetAssetBundleName(assetRelativePathToAssets, assetBundleName);
                        Debug.LogEditorInfor(string.Format("目录: {0:50} \t assetBundleName={1} ", assetRelativePathToAssets, assetBundleName));
                    }
                    else
                    {
                        string assetBundleName = IOUtility.GetPathDirectoryNameByDeep(assetRelativePathToAssets);
                        SetAssetBundleName(assetRelativePathToAssets, assetBundleName);
                        Debug.LogEditorInfor(string.Format("资源: {0:50} \t assetBundleName={1} ", assetRelativePathToAssets, assetBundleName));
                    }
                }
                Debug.LogEditorInfor(string.Format("选择文件{0} 设置了{1}个资源名", assetPath, containAssets.Length));
                EditorUtility.ClearProgressBar();
            }
            AssetDatabase.Refresh();
        }

        //设置AssetBundleName 信息
        private static bool  SetAssetBundleName(string assetPath,string assetBundleName)
        {
            AssetImporter import = AssetImporter.GetAtPath(assetPath);
            if (import.assetBundleName!= assetBundleName)
            {
                import.assetBundleName = assetBundleName;
                import.SaveAndReimport();
                return true;
            }
            return false;
        }

     


    }
}