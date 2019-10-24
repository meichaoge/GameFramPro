using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 专用于处理Unity MonoBehaviour 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public  interface IUnityGameObjectPool : IUnityObjectPool 
    {
        System.Action<GameObject> BeforeGetAction { get; }
        System.Action<GameObject> BeforeRecycleAction { get; }
        Stack<GameObject> PoolContainer { get; }
        GameObject PrefabTarget { get; }
        Transform PoolManagerTarget { get; } //池对象管理器自身


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="prefabTarget">当对象不足时按照这个创建</param>
        /// <param name="beforGetAction">获取前要进行的操作</param>
        /// <param name="beforeRecycleAction">回收前要做的操作</param>
        void InitialedPool(int capacity, GameObject prefabTarget, System.Action<GameObject> beforGetAction, System.Action<GameObject> beforeRecycleAction);

        /// <summary>
        /// 不需要时候清理自己
        /// </summary>
        void ReleasPool();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        GameObject GetItemFromPool();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="item"></param>
        void RecycleItemToPool(GameObject item);
    }
}