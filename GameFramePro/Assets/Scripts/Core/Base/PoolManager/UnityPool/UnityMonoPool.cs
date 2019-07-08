using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 管理挂载了Unity 脚本的Mono 对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UnityMonoPool<T> : IUnityMonoPool<T> where T : MonoBehaviour
    {
        public Action<T> BeforGetAction { get; private set; }

        public Action<T> BeforeRecycleAction { get; private set; }

        public Stack<UnityMonoPoolItemInfor> PoolContainer { get; private set; }

        public GameObject PrefabTarget { get; private set; }

        private Transform mPoolManagerTarget = null;
        //挂载缓存的PrefabTarget实例对象
        public Transform PoolManagerTarget { get { if (mPoolManagerTarget == null) mPoolManagerTarget = UnityMonoObjectPoolHelper.S_Instance.GetUnityPoolManagerTransParent(PrefabTarget.name); return mPoolManagerTarget; } }


        public void InitialedPool(int capacity, GameObject prefabTarget, Action<T> beforGetAction, Action<T> beforeRecycleAction)
        {
            BeforGetAction = beforGetAction;
            BeforeRecycleAction = beforeRecycleAction;
            PrefabTarget = prefabTarget;
            if (PrefabTarget == null)
            {
                Debug.LogError(string.Format("InitialedPool Fail, parameter prefabTarget is null of PoolType={0}", typeof(GameObject)));
            }
            PoolContainer = new Stack<UnityMonoPoolItemInfor>(capacity);
            PoolObjectManager.TrackPoolManager_Mono<GameObject>(this);
        }

        public void ReleasPool()
        {
            if (PoolContainer.Count > 0)
            {
                UnityMonoPoolItemInfor itemInfor = null;

                while (PoolContainer.Count > 0)
                {
                    itemInfor = PoolContainer.Pop();
                    if (itemInfor == null || itemInfor.mTargetObject) continue;
                    if (BeforeRecycleAction != null)
                        BeforeRecycleAction(itemInfor.mTargetScript as T);

                    ResourcesManager.Destroy(itemInfor.mTargetObject);
                }//移除对象
            } //删除缓存对象


            PrefabTarget = null;
            BeforGetAction = BeforeRecycleAction = null;
            PoolContainer = null;

            PoolObjectManager.UnTrackPoolManager_Mono<GameObject>(this);
            UnityMonoObjectPoolHelper.S_Instance.RecycleUnityPoolManagerTransParent(PrefabTarget.name);
            ResourcesManager.Destroy(PoolManagerTarget.gameObject); //销毁自己
        }


        public T GetItemFromPool()
        {
            UnityMonoPoolItemInfor itemInfor = null;
            while (PoolContainer.Count > 0)
            {
                itemInfor = PoolContainer.Pop();
                if (itemInfor == null|| itemInfor.mTargetObject)
                    continue;
            }

            if (itemInfor == null)
            {
                itemInfor = new UnityMonoPoolItemInfor();
                itemInfor .mTargetObject= ResourcesManager.Instantiate<GameObject>(PrefabTarget, PoolManagerTarget, false);
                itemInfor.mTargetScript = itemInfor.mTargetObject.GetAddComponent<T>();
            }

            if (BeforGetAction != null)
                BeforGetAction(itemInfor.mTargetScript as T);


            return itemInfor.mTargetScript as T;
        }

    

        public void RecycleItemToPool(T item)
        {
            if (item == null)
            {
                Debug.LogError("RecycleItemToPool Fail,Parameter item is null");
                return;
            }

            if (BeforeRecycleAction != null)
                BeforeRecycleAction(item);

            item.transform.SetParent(PoolManagerTarget);
            UnityMonoPoolItemInfor itemInfor = new UnityMonoPoolItemInfor();
            itemInfor.mTargetObject = item.gameObject;
            itemInfor.mTargetScript = item;
            PoolContainer.Push(itemInfor);
        }

     
    }
}