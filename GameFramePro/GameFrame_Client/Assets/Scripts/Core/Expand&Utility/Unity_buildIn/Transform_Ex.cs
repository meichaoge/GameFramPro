﻿using GameFramePro;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Transform 扩展类
/// </summary>
public static class Transform_Ex
{
    public static T GetAddComponent<T>(this Transform target) where T : Component
    {
        if (target == null)
        {
            Debug.LogError("GetAddComponent  Fail,Target Transform Is Null");
            return null;
        }

        Debug.Log(typeof(T));
        T result = target.GetComponent<T>();
        if (result == null)
            result = target.gameObject.AddComponent<T>();
        return result;
    }

    /// <summary>
    /// 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    public static void ResetTransProperty(this Transform target)
    {
        target.ResetTransProperty(Vector3.zero, Vector3.one, Quaternion.identity);
    }

    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos)
    {
        target.ResetTransProperty(transpos, Vector3.one, Quaternion.identity);
    }

    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    /// <param name="localScale"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos, Vector3 localScale)
    {
        target.ResetTransProperty(transpos, localScale, Quaternion.identity);
    }

    /// <summary>
    /// / 重置 TransForm 位置属性
    /// </summary>
    /// <param name="target"></param>
    /// <param name="transpos"></param>
    /// <param name="localScale"></param>
    /// <param name="rataion"></param>
    public static void ResetTransProperty(this Transform target, Vector3 transpos, Vector3 localScale, Quaternion rataion)
    {
        target.localPosition = transpos;
        target.localScale = localScale;
        target.rotation = rataion;
    }

    /// <summary>
    /// 获取组件先后对于根节点的路径
    /// </summary>
    /// <param name="target"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static string GetTransRelativePathToRoot(this Transform target, Transform root)
    {
        string path = string.Empty;
        List<StringBuilder> stringBuilders = new List<StringBuilder>();
        Transform trans = target;
        while (trans != null)
        {
            if (trans == root)
                break;
            StringBuilder builder = new StringBuilder();
            builder.Append(trans.name);
            stringBuilders.Add(builder);

            trans = trans.parent;
        }

        stringBuilders.Reverse();
        StringBuilder pathBuilder = new StringBuilder();
        for (int dex = 0; dex < stringBuilders.Count; dex++)
        {
            pathBuilder.Append(stringBuilders[dex]);
            if (dex != stringBuilders.Count - 1)
                pathBuilder.Append("/");
        }

        path = pathBuilder.ToString();
        return path;
    }


    /// <summary>
    /// 获取所有的直接子节点
    /// </summary>
    /// <param name="includeInactive">是否包含不可见节点 默认包含</param>
    /// <returns></returns>
    public static List<Transform> GetAllDirectChildsTrans(this Transform target, bool includeInactive = true)
    {
        if (target == null)
        {
            Debug.LogError("当前参数为null");
            return null;
        }

        List<Transform> allChildTrans = new List<Transform>(target.childCount);
        if (target.childCount == 0)
            return allChildTrans;

        for (int dex = 0; dex < target.childCount; dex++)
        {
            var Trans = target.GetChild(dex);
            if (includeInactive == false && Trans.gameObject.activeSelf == false)
                continue;
            allChildTrans.Add(Trans);
        }

        return allChildTrans;
    }

    /// <summary>
    /// 获取所有的直接子节点
    /// </summary>
    /// <param name="includeInactive">是否包含不可见节点 默认包含</param>
    /// <returns></returns>
    public static List<RectTransform> GetAllDirectChildsRectTrans(this Transform target, bool includeInactive = true)
    {
        if (target == null)
        {
            Debug.LogError("当前参数为null");
            return null;
        }

        List<RectTransform> allChildTrans = new List<RectTransform>(target.childCount);
        if (target.childCount == 0)
            return allChildTrans;

        for (int dex = 0; dex < target.childCount; dex++)
        {
            var rectTransform = target.GetChild(dex) as RectTransform;
            if (rectTransform == null) continue;
            if (includeInactive == false && rectTransform.gameObject.activeSelf == false)
                continue;
            allChildTrans.Add(rectTransform);
        }

        return allChildTrans;
    }


#if UNITY_EDITOR
    /// <summary>
    /// 获取组件先后对于根节点的路径
    /// </summary>
    /// <param name="target"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static string GetTransRelativePathToRoot_Editor(this Transform target, Transform root)
    {
        string path = string.Empty;
        List<string> stringBuilders = new List<string>();
        Transform trans = target;
        while (trans != null)
        {
            if (trans == root)
                break;
            stringBuilders.Add(trans.name);
            trans = trans.parent;
        }

        stringBuilders.Reverse();
        StringBuilder pathBuilder = new StringBuilder();
        for (int dex = 0; dex < stringBuilders.Count; dex++)
        {
            pathBuilder.Append(stringBuilders[dex]);
            if (dex != stringBuilders.Count - 1)
                pathBuilder.Append("/");
        }

        path = pathBuilder.ToString();
        return path;
    }
#endif
}