using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using GameFramePro;


/// <summary>
/// 非Mono的单例泛型类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Single<T> where T : class, new()
{
    protected static T s_Instance = default(T);
    private static object obj = new object();
    public static T S_Instance
    {
        get
        {
            if (s_Instance == null)
            {
                lock (obj)
                {
                    Type _type = typeof(T);
                    BindingFlags flag = BindingFlags.NonPublic | BindingFlags.Instance;    //***反射获取 非静态 InitialSingleton 方法 并调用
                    MethodInfo _infor = _type.GetMethod("InitialSingleton", flag); //反射获得InitialSingleton 方法
                    object obj = Activator.CreateInstance(_type);

                    ResourcesTracker.RegistTraceNativeobject(obj,TraceNativeobjectStateEnum.Singtion);

                    s_Instance = (T)obj;
                    if (s_Instance == null)
                        Debug.LogError("GetInstance Fail .... " + _type);

                    _infor.Invoke(obj, null); //调用方法
                }
            }
            return s_Instance;
        }
    }


    /// <summary>
    /// 初始化单例实例的接口(只会调用一次除非对象被销毁)
    /// </summary>
    protected virtual void InitialSingleton() { }

    public virtual void DisposeInstance()
    {
        ResourcesTracker.UnRegistTraceNativeobject(s_Instance);
        s_Instance = null;
    }

}
