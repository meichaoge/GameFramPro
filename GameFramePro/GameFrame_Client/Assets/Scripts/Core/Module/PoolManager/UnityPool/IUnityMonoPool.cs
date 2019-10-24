using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 缓存池中存储的对象
    /// </summary>
    public class UnityMonoPoolItemInfor
    {
        public GameObject mTargetObject = null;
        public MonoBehaviour mTargetScript = null; //关联的脚本
    }


    /// <summary>
    /// 负责 带有继承自MonoBehavior 脚本的Unity 对象
    /// </summary>
    public interface IUnityMonoPool<T> : IUnityObjectPool where T : MonoBehaviour
    {
        System.Action<T> BeforeGetAction { get; }
        System.Action<T> BeforeRecycleAction { get; }
        Stack<UnityMonoPoolItemInfor> PoolContainer { get; }
        GameObject PrefabTarget { get; }
        Transform PoolManagerTarget { get; } //池对象管理器自身


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="prefabTarget">当对象不足时按照这个创建</param>
        /// <param name="beforGetAction">获取前要进行的操作</param>
        /// <param name="beforeRecycleAction">回收前要做的操作</param>
        void InitialedPool(int capacity, GameObject prefabTarget, System.Action<T> beforGetAction, System.Action<T> beforeRecycleAction);

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