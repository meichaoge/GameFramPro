using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{
    /// <summary>
    /// 管理Unity  GameObjec 对象池对象
    /// </summary>
    public class UnityGameObjectPool : IUnityGameObjectPool
    {
        public System.Action<GameObject> BeforGetAction { get; private set; }
        public System.Action<GameObject> BeforeRecycleAction { get; private set; }
        public Stack<GameObject> PoolContainer { get; private set; }
        public GameObject PrefabTarget { get; private set; }

        private Transform mPoolManagerTarget = null;
        //挂载缓存的PrefabTarget实例对象
        public Transform PoolManagerTarget { get { if (mPoolManagerTarget == null) mPoolManagerTarget = UnityMonoObjectPoolHelper.S_Instance.GetUnityPoolManagerTransParent(PrefabTarget.name); return mPoolManagerTarget; } }





        public void InitialedPool(int capacity, GameObject prefabTarget, Action<GameObject> beforGetAction, Action<GameObject> beforeRecycleAction)
        {
            BeforGetAction = beforGetAction;
            BeforeRecycleAction = beforeRecycleAction;
            PrefabTarget = prefabTarget;
            if (PrefabTarget == null)
            {
                Debug.LogError(string.Format("InitialedPool Fail, parameter prefabTarget is null of PoolType={0}", typeof(GameObject)));
            }
            PoolContainer = new Stack<GameObject>(capacity);
            PoolObjectManager.TrackPoolManager_Mono<GameObject>(this);
        }

        public void ReleasPool()
        {
            if (PoolContainer.Count > 0)
            {
                GameObject target = null;
                while (PoolContainer.Count > 0)
                {
                    target = PoolContainer.Pop();
                    if (target == null) continue;
                    if (BeforeRecycleAction != null)
                        BeforeRecycleAction(target);

                    ResourcesManager.Destroy(target);
                }//移除对象
            } //删除缓存对象


            PrefabTarget = null;
            BeforGetAction = BeforeRecycleAction = null;
            PoolContainer = null;

            PoolObjectManager.UnTrackPoolManager_Mono<GameObject>(this);
            UnityMonoObjectPoolHelper.S_Instance.RecycleUnityPoolManagerTransParent(PrefabTarget.name);
            ResourcesManager.Destroy(PoolManagerTarget.gameObject); //销毁自己
        }

        public GameObject GetItemFromPool()
        {
            GameObject resultObject = null;
            while (PoolContainer.Count > 0)
            {
                resultObject = PoolContainer.Pop();
                if (resultObject == null)
                    continue;
            }

            if (resultObject == null)
                resultObject = ResourcesManager.Instantiate<GameObject>(PrefabTarget, PoolManagerTarget,false);

            if (BeforGetAction != null)
                BeforGetAction(resultObject);


            return resultObject;
        }


        public void RecycleItemToPool(GameObject item)
        {
            if (item == null)
            {
                Debug.LogError("RecycleItemToPool Fail,Parameter item is null");
                return;
            }

            if (BeforeRecycleAction != null)
                BeforeRecycleAction(item);

            item.transform.SetParent(PoolManagerTarget);
            PoolContainer.Push(item);
        }



    }
}