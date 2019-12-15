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
        public System.Action<GameObject> BeforeGetAction { get; protected set; }
        public System.Action<GameObject> BeforeRecycleAction { get; protected set; }
        public HashSet<int> PollItemRecord { get; protected set; } = new HashSet<int>();
        public Stack<GameObject> PoolContainer { get; protected set; }
        public GameObject PrefabTarget { get; protected set; }
        public string PoolNameUri { get; protected set; }
        public bool mIsDontDestroyed { get; protected set; }

        private Transform mPoolItemRecycleParent = null;

        /// <summary>
        /// 挂载缓存的PrefabTarget实例对象 (当池对象呗回收时候挂在在这里)
        /// </summary>
        public Transform PoolItemRecycleParent
        {
            get
            {
                if (mPoolItemRecycleParent == null)
                {
                    mPoolItemRecycleParent = PoolObjectManager.GetUnityPoolManagerTransParent(PoolNameUri);
                    if (mIsDontDestroyed && mPoolItemRecycleParent != null)
                        ResourcesManagerUtility.MarkNotDestroyOnLoad(mPoolItemRecycleParent.gameObject);
                }
                return mPoolItemRecycleParent;
            }
        }


        #region 构造函数
        private UnityGameObjectPool() { }
        /// <summary>
        /// 创建池管理器  
        /// </summary>
        /// <param name="poolNameUri">唯一标识一个池管理器</param>
        public UnityGameObjectPool(string poolNameUri, bool isMarkDontDestroyedOnLoad = true)
        {
            PoolNameUri = poolNameUri + System.Guid.NewGuid().ToString();
            mIsDontDestroyed = isMarkDontDestroyedOnLoad;
            if (mIsDontDestroyed && PoolItemRecycleParent != null)
                ResourcesManagerUtility.MarkNotDestroyOnLoad(PoolItemRecycleParent.gameObject);
        }

        /// <summary>
        /// 初始化对象池 并且在元素回收时候会吧元素放在 ItemParent 节点下，某些情况下可以避免修改父节点带来的开销
        /// </summary>
        /// <param name="poolNameUri"></param>
        /// <param name="ItemParent"></param>
        public UnityGameObjectPool(string poolNameUri, Transform ItemParent, bool isMarkDontDestroyedOnLoad = true)
        {
            PoolNameUri = poolNameUri + System.Guid.NewGuid().ToString();
            mIsDontDestroyed = isMarkDontDestroyedOnLoad;
            mPoolItemRecycleParent = ItemParent;

            //SetPoolItemRecycleParent(ItemParent);
            if (mIsDontDestroyed && ItemParent != null)
                ResourcesManagerUtility.MarkNotDestroyOnLoad(ItemParent.gameObject);
        }

        ///// <summary>
        ///// 设置 元素回收时候会吧元素放在 ItemParent 节点下，某些情况下可以避免修改父节点带来的开销
        ///// </summary>
        ///// <param name="ItemParent"></param>
        //public void SetPoolItemRecycleParent(Transform ItemParent)
        //{
        //    mPoolItemRecycleParent = ItemParent;
        //    if (mIsDontDestroyed && ItemParent != null)
        //        ResourcesManagerUtility.MarkNotDestroyOnLoad(ItemParent.gameObject);
        //}
        #endregion


        public void InitialedPool(int capacity, GameObject prefabTarget, Action<GameObject> beforGetAction, Action<GameObject> beforeRecycleAction)
        {
            BeforeGetAction = beforGetAction;
            BeforeRecycleAction = beforeRecycleAction;
            PrefabTarget = prefabTarget;
            if (PrefabTarget == null)
                Debug.LogError(string.Format("InitialedPool Fail, parameter prefabTarget is null of PoolType={0}", typeof(GameObject)));

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
                    BeforeRecycleAction?.Invoke(target);

                    ResourcesManagerUtility.Destroy(target);
                } //移除对象
            } //删除缓存对象


            PollItemRecord.Clear();
            PrefabTarget = null;
            BeforeGetAction = BeforeRecycleAction = null;
            PoolContainer = null;

            PoolObjectManager.UnTrackPoolManager_Mono<GameObject>(this);
            PoolObjectManager.RecycleUnityPoolManagerTransParent(PrefabTarget.name);
            //不能销毁自身
            //   ResourcesManagerUtility.Destroy(this); //销毁自己
        }

        public GameObject GetItemFromPool(Transform parent)
        {
            GameObject resultObject = null;
            while (PoolContainer.Count > 0)
            {
                resultObject = PoolContainer.Pop();
                if (resultObject != null)
                    break;
            }

            if (resultObject == null)
                resultObject = ResourcesManagerUtility.Instantiate<GameObject>(PrefabTarget, parent, false);
            else
            {
                int hashCode = resultObject.GetHashCode();
                PollItemRecord.Remove(hashCode);

                if (resultObject.transform.parent != parent)
                    resultObject.transform.SetParent(parent);
            }

            if (resultObject != null && resultObject.activeSelf == false)
                resultObject.SetActive(true);

            BeforeGetAction?.Invoke(resultObject);
            return resultObject;
        }


        public void RecycleItemToPool(GameObject item)
        {
            if (item == null)
            {
                Debug.LogError("RecycleItemToPool Fail,Parameter item is null");
                return;
            }
            int hashCode = item.GetHashCode();
            if (PollItemRecord.Contains(hashCode))
            {
                Debug.LogError($"检测到重复缓存对象{item} 请确保只回收一次");
                return;
            }
            PollItemRecord.Add(hashCode);

            if (BeforeRecycleAction != null)
                BeforeRecycleAction(item);

            item.transform.SetParent(PoolItemRecycleParent);
            if (item != null && item.activeSelf == true)
                item.SetActive(false);
            PoolContainer.Push(item);
        }
    }
}