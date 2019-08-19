using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;


namespace GameFramePro.EditorEx
{


    /// <summary>
    /// 检测预制体中 RaycastTarget 属性是否开启
    /// </summary>
    public class UIGraphicRaycastCheck
    {
        [MenuItem("工具和扩展/性能优化/检测预制体 RaycastTarget 属性 ")]
        private static void CheckRayCastTagEnable()
        {
            var resourcesPrefabs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/Resources" });
            int count = 0;
            GameObject root = new GameObject("root");
            foreach (var assetGuid in resourcesPrefabs)
            {
                ++count;
                string prefabAssetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                EditorUtility.DisplayProgressBar("分析中..", prefabAssetPath, 1f * count / resourcesPrefabs.Length);

                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
                GameObject go = GameObject.Instantiate(prefab, root.transform);
                go.name = prefab.name;
                Transform[] Childs = go.GetComponentsInChildren<Transform>(true);
                foreach (var child in Childs)
                {
                    Graphic[] targets = child.GetComponents<Graphic>();
                    if (targets == null || targets.Length == 0)
                        break;

                    foreach (var graphic in targets)
                    {
                        if (graphic.raycastTarget == false)
                            continue;
                        if (graphic.transform.GetComponent<Button>() != null)
                            continue;
                        string path = graphic.transform.GetTransRelativePathToRoot_Editor(go.transform);
                        Debug.LogFormat("预制体 {0} 节点 {1} 组件 {2} 可能需要设置 raycastTarget 为false!!!路径 {3}", go.name, graphic.transform.name, graphic.GetType(), path);
                    }
                }
                GameObject.DestroyImmediate(go);
            }


            GameObject.DestroyImmediate(root);
            EditorUtility.ClearProgressBar();
            Debug.Log("完成了Resources 下资源 raycastTarget 属性 检测");
        }




    }
}
