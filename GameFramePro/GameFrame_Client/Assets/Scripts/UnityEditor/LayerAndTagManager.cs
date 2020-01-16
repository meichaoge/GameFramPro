using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 管理编辑器下的 Tag 和 Layer 导入
/// Layer  导入目前有问题
/// </summary>
public static class LayerAndTagManager
{
#if UNITY_EDITOR

    public static void AddTag(string tag)
    {
        if (!isHasTag(tag))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name != "tags") continue;
                for (int i = 0; i < it.arraySize; i++)
                {
                    SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                    if (string.IsNullOrEmpty(dataPoint.stringValue))
                    {
                        dataPoint.stringValue = tag;
                        tagManager.ApplyModifiedProperties();
                        return;
                    }
                }
                it.InsertArrayElementAtIndex(it.arraySize);  //避免一开始不存在异常
                tagManager.ApplyModifiedProperties();
                it.GetArrayElementAtIndex(it.arraySize - 1).stringValue = tag;
                tagManager.ApplyModifiedProperties();

                break;
            }
        }
    }

    public static void RemoveUnExitTags()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty property = tagManager.GetIterator();
        while (property.NextVisible(true))
        {
            if (property.name != "tags") continue;
            for (int dex = property.arraySize - 1; dex >= 0; dex--)
            {
                SerializedProperty dataPoint = property.GetArrayElementAtIndex(dex);
                if (string.IsNullOrEmpty(dataPoint.stringValue))
                {
                    property.DeleteArrayElementAtIndex(dex);
                    tagManager.ApplyModifiedProperties();
                }
            }
            break;
        }
    }

    //移除所有的未定义的Tags
    public static void RemoveUnDefineTags(List<string> defineTags)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty property = tagManager.GetIterator();
        while (property.NextVisible(true))
        {
            if (property.name != "tags") continue;
            for (int dex = property.arraySize - 1; dex >= 0; dex--)
            {
                SerializedProperty dataPoint = property.GetArrayElementAtIndex(dex);
                if (string.IsNullOrEmpty(dataPoint.stringValue) || defineTags.Contains(dataPoint.stringValue) == false)
                {
                    property.DeleteArrayElementAtIndex(dex);
                    tagManager.ApplyModifiedProperties();
                }
            }
            break;
        }
    }

    public static void AddLayer(string layer)
    {
        if (!isHasLayer(layer))
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                Debug.Log(it.name);

                if (it.name.StartsWith("layers"))
                {
                    if (it.type == "string")
                    {
                        if (string.IsNullOrEmpty(it.stringValue))
                        {
                            it.stringValue = layer;
                            tagManager.ApplyModifiedProperties();
                            return;
                        }
                    }
                }
            }
        }
    }

    public static bool isHasTag(string tag)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.tags.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.tags[i] == tag)
                return true;
        }
        return false;
    }

    public static bool isHasLayer(string layer)
    {
        for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; i++)
        {
            if (UnityEditorInternal.InternalEditorUtility.layers[i] == layer)
                return true;
        }
        return false;
    }
#endif
}
