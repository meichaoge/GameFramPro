using System.Collections;
using System.Collections.Generic;
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
}
