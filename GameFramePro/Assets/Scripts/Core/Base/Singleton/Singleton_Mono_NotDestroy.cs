﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro;



/// <summary>
/// 场景加载不会被销毁的单例Mono类
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton_Mono_NotDestroy<T> : Singleton_Mono<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        ResourcesManager.Instance.MarkNotDestroyOnLoad(gameObject);
    }

}
