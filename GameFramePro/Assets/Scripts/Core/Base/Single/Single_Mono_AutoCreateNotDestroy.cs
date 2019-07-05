using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;


/// <summary>
/// 单例的Mono  类 如果不存在则会自动创建一个空对象并附加这个脚本,并且不会再场景加载时候销毁
/// </summary>
/// <typeparam name="T"></typeparam>
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif
public class Single_Mono_AutoCreateNotDestroy<T> : Single_Mono_AutoCreate<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        ResourcesManager.MarkNotDestroyOnLoad(gameObject);
    }





}
