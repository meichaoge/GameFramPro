using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{
    /// <summary>
    /// 原生对象(非继承自Unity.Object 对象)对象池
    /// </summary>
    public class NativeObjectPool<T> : INativeObjectPool<T> where T :  new()
    {
        public System.Action<T> BeforGetAction { get; protected set; } = null;
        public System.Action<T> BeforeRecycleAction { get; protected set; } = null;
        public Stack<T> PoolContainer { get; protected set; } = null;

        
        public void InitialedPool(int capacity, System.Action<T> beforGetAction, System.Action<T> beforeRecycleAction)
        {
            BeforGetAction = beforGetAction;
            BeforeRecycleAction = beforeRecycleAction;
            PoolContainer = new Stack<T>(capacity);
            PoolObjectManager.TrackPoolManager(typeof(T), this);
        }
        public void ReleasPool()
        {
            if (PoolContainer.Count > 0)
            {
                T target ;
                while (PoolContainer.Count>0)
                {
                    target = PoolContainer.Pop();
                    if (BeforeRecycleAction != null)
                        BeforeRecycleAction(target);
                }
            }
            PoolContainer = null;
            BeforGetAction = BeforeRecycleAction = null;

            PoolObjectManager.UnTrackPoolManager(this);
        }

        public T GetItemFromPool()
        {
            T result = default(T);
            if (PoolContainer.Count != 0)
            {
                result = PoolContainer.Pop();
                while (result == null && PoolContainer.Count > 0)
                {
                    result = PoolContainer.Pop();
                }//循环获取一个可用的对象
            }
            if (result == null)
                result = new T();


            if (BeforGetAction != null)
                BeforGetAction(result);

            return result;
        }

        public void RecycleItemToPool(T item)
        {
            if (item == null)
            {
                Debug.LogError("RecycleItemToPool Fail, Target parameter item is null");
                return;
            }

            if (BeforeRecycleAction != null)
                BeforeRecycleAction(item);
            PoolContainer.Push(item);
        }

      


    }
}