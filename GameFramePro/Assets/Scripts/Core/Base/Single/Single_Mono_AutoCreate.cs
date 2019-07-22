using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;


/// <summary>
/// 单例的Mono  类 如果不存在则会自动创建一个空对象并附加这个脚本
/// </summary>
/// <typeparam name="T"></typeparam>
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class Single_Mono_AutoCreate<T> : Single_Mono<T> where T : MonoBehaviour   
{
    public new static T S_Instance
    {
        get
        {
            if (s_Instance == null)
            {
                GameObject go = ResourcesManager.Instantiate(typeof(T).Name);
                s_Instance = go.GetAddComponentEx<T>();
                ResourcesTracker.RegisterTraceResources(s_Instance, TraceResourcesStateEnum.Singtion);
            }
            return s_Instance;
        }
    }

    /// <summary>
    /// 这里需要重写父类实现 因为初始化的时间不一样
    /// </summary>
    protected override void Awake()
    {
        GetInstance(false);  //Make sure the other Component is destroyed
    }

   

 




}
