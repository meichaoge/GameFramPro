using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 对象池的接口
    /// </summary>
    public interface INativeObjectPool<T>  where T :  new()
    {
        System.Action<T> BeforGetAction { get; }
        System.Action<T> BeforeRecycleAction { get; }
        Stack<T> PoolContainer { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="beforGetAction">获取前要进行的操作</param>
        /// <param name="beforeRecycleAction">回收前要做的操作</param>
        void InitialedPool(int capacity, System.Action<T> beforGetAction, System.Action<T> beforeRecycleAction);

        /// <summary>
        /// 不需要时候清理自己
        /// </summary>
        void ReleasPool();

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        T GetItemFromPool();

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="item"></param>
        void RecycleItemToPool(T item);
    }
}