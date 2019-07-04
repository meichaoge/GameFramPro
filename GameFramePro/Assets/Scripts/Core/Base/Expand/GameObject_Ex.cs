using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// GameObject 类扩展
/// </summary>
public static class GameObject_Ex 
{
    public static T GetAddComponent<T>(this GameObject target) where T : Component
    {
        if (target == null)
        {
            Debug.LogError("GetAddComponent  Fail,Target GameObject Is Null");
            return null;
        }

        T result = target.GetComponent<T>();
        if (result == null)
            result = target.AddComponent<T>();
        return result;
    }
}
