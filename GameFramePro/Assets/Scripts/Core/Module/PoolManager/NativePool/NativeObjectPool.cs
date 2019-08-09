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
    public class NativeObjectPool<T> : INativeObjectPool<T> where T : new()
    {
        public System.Action<T> BeforeGetAction { get; protected set; } = null;
        public System.Action<T> BeforeRecycleAction { get; protected set; } = null;
        public Stack<T> PoolContainer { get; protected set; } = null;


        public NativeObjectPool(int capacity, System.Action<T> beforeGetAction, System.Action<T> beforeeRecycleAction)
        {
            BeforeGetAction = beforeGetAction;
            BeforeRecycleAction = beforeeRecycleAction;
            PoolContainer = new Stack<T>(capacity);
            PoolObjectManager.TrackPoolManager(typeof(T), this);
        }


        public void ReleasePool()
        {
            if (PoolContainer.Count > 0)
            {
                T target ;
                while (PoolContainer.Count>0)
                {
                    target = PoolContainer.Pop();
                    BeforeRecycleAction?.Invoke(target);
                }
            }
            PoolContainer = null;
            BeforeGetAction = BeforeRecycleAction = null;

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


            BeforeGetAction?.Invoke(result);

            return result;
        }

        public void RecycleItemToPool(T item)
        {
            if (item == null)
            {
                Debug.LogError("RecycleItemToPool Fail, Target parameter item is null");
                return;
            }

            BeforeRecycleAction?.Invoke(item);
            PoolContainer.Push(item);
        }

      


    }
}
