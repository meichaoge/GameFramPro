# if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 创建序列化资源 Asset 工具类
    /// </summary>
    public static class ScriptableObjectUtility
    {
        /// <summary>
        /// 创建Unity 序列化资源Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void CreateUnityAsset<T>(string title, string directoryPath, string fileName) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            string assetPathAndName = EditorUtility.SaveFilePanel(title, directoryPath, fileName, "asset");
            if (string.IsNullOrEmpty(assetPathAndName)) return;
            var fileRelativeAssetPath = assetPathAndName.Substring(assetPathAndName.IndexOf("Assets"));

            Debug.Log("CreateUnityAsset >>>path :" + fileRelativeAssetPath);
            string fileDirectory = System.IO.Path.GetDirectoryName(fileRelativeAssetPath);
            if (System.IO.Directory.Exists(fileDirectory) == false)
            {
                Debug.LogEditorInfor($"创建目录{fileDirectory}");
                Directory.CreateDirectory(fileDirectory);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, fileRelativeAssetPath); //创建资源Asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }


        /// <summary>
        /// 创建 以  Assets/开始的 路径下的 序列化资源
        /// </summary>
        /// <param name="fileRelativeAssetPath"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreateUnityAsset<T>(string fileRelativeAssetPath) where T : ScriptableObject
        {
            if (string.IsNullOrEmpty(fileRelativeAssetPath))
            {
                Debug.LogEditorError($"请输入正确的保存 .asset 资源的路径");
                return;
            }

            var localAsset = AssetDatabase.LoadAssetAtPath(fileRelativeAssetPath, typeof(ScriptableObject));
            if (localAsset != null)
            {
                if (EditorUtility.DisplayDialog("提示", $"在路径{fileRelativeAssetPath} 已经存在一个同名资源，是否替换!", "替换", "取消") == false)
                    return;
            }


            T asset = ScriptableObject.CreateInstance<T>();
            string fileDirectory = System.IO.Path.GetDirectoryName(fileRelativeAssetPath);
            if (System.IO.Directory.Exists(fileDirectory) == false)
            {
                Debug.LogEditorInfor($"创建目录{fileDirectory}");
                Directory.CreateDirectory(fileDirectory);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, fileRelativeAssetPath); //创建资源Asset
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;


            Debug.LogEditorInfor($"创建{typeof(T)}类型的.asset 资源成功，保存在路径{fileRelativeAssetPath}");
        }
    }
}

#endif