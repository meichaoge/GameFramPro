using GameFramePro;
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
    public static string GetTransRelativePathToRoot(this Transform target,Transform root)
    {
        string path = string.Empty;
        List<StringBuilder> stringBuilders = StringUtility.GetStringBuilderList();
        Transform trans = target;
        while (trans != null)
        {
            if (trans == root)
                break;
            StringBuilder builder = StringUtility.GetStringBuilder();
            builder.Append(trans.name);
            stringBuilders.Add(builder);

            trans = trans.parent;

        }
        stringBuilders.Reverse();
        StringBuilder pathBuilder= StringUtility.GetStringBuilder();
        for (int dex = 0; dex < stringBuilders.Count; dex++)
        {
            pathBuilder.Append(stringBuilders[dex]);
            if (dex != stringBuilders.Count - 1)
                pathBuilder.Append("/");
        }
        path = pathBuilder.ToString();
        StringUtility.ReleaseStringBuilder(pathBuilder);
        StringUtility.ReleaseStringBuilderList(stringBuilders);
        return path;
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
