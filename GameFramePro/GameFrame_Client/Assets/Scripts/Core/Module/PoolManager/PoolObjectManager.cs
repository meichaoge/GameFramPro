using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 管理生成的对象池
    /// </summary>
    public static class PoolObjectManager
    {


        #region 原生对象池接口
        private static Dictionary<Type, INativeObjectPool<object>> mAllNativeObjectPoolManagers = new Dictionary<Type, INativeObjectPool<object>>();


        public static void TrackPoolManager<T>(INativeObjectPool<T> poolManager) where T : new()
        {
            TrackPoolManager(typeof(T), poolManager);
        }
        public static void TrackPoolManager<T>(Type type, INativeObjectPool<T> poolManager) where T : new()
        {
       //     if (Application.isPlaying == false) return; //过滤编辑器代码

            INativeObjectPool<object> pool = null;

            if (mAllNativeObjectPoolManagers.TryGetValue(type, out pool))
            {
                Debug.LogError($"TrackPoolManager Fail,Already Exit Pool of Type {type}");
                return;
            }
            mAllNativeObjectPoolManagers[type] = poolManager as INativeObjectPool<object>;
        }



        public static void UnTrackPoolManager<T>(INativeObjectPool<T> poolManager) where T : new()
        {
            UnTrackPoolManager(typeof(T), poolManager);
        }

        public static void UnTrackPoolManager<T>(Type type, INativeObjectPool<T> poolManager) where T : new()
        {
    //        if (Application.isPlaying == false) return; //过滤编辑器代码
            INativeObjectPool<object> pool = null;

            if (mAllNativeObjectPoolManagers.TryGetValue(type, out pool))
            {
                mAllNativeObjectPoolManagers.Remove(type);
                return;
            }
            Debug.LogError($"UnTrackPoolManager Fail,Not Exit Pool of Type {type}");
        }

        #endregion


        #region Unity  对象池接口

        private static Dictionary<Type, IUnityObjectPool> mAllUnityObjectPoolManagers = new Dictionary<Type, IUnityObjectPool>();

        public static void TrackPoolManager_Mono<T>(IUnityObjectPool poolManager) where T : UnityEngine.Object
        {
            TrackPoolManager_Mono<T>(typeof(T), poolManager);
        }
        public static void TrackPoolManager_Mono<T>(Type type, IUnityObjectPool poolManager) where T : UnityEngine.Object
        {
            if (Application.isPlaying == false) return; //过滤编辑器代码
            IUnityObjectPool pool = null;

            if (mAllUnityObjectPoolManagers.TryGetValue(type, out pool))
            {
                Debug.LogError(string.Format("TrackPoolManager Fail,Already Exit Pool of Type {0}", type));
                return;
            }
            mAllUnityObjectPoolManagers[type] = poolManager as IUnityObjectPool;
        }



        public static void UnTrackPoolManager_Mono<T>(IUnityObjectPool poolManager) where T : UnityEngine.Object
        {
            UnTrackPoolManager_Mono<T>(typeof(T), poolManager);
        }

        public static void UnTrackPoolManager_Mono<T>(Type type, IUnityObjectPool poolManager) where T : UnityEngine.Object
        {
            if (Application.isPlaying == false) return; //过滤编辑器代码
            IUnityObjectPool pool = null;

            if (mAllUnityObjectPoolManagers.TryGetValue(type, out pool))
            {
                mAllUnityObjectPoolManagers.Remove(type);
                return;
            }
            Debug.LogError(string.Format("UnTrackPoolManager_Mono Fail,Not Exit Pool of Type {0}", type));

        }

        #endregion


        #region 辅助生成 UnityMonoObjectPool 
        private static readonly string R_UnityPoolManagerPrefix = "UnityPoolManger_"; //Unity 对象池父节点名称前缀
        private static readonly Dictionary<string, Transform> mAllPoolMonoObjects = new Dictionary<string, Transform>();

      //  protected override bool IsNotDestroyedOnLoad { get; } = true; //标示不会一起销毁


        /// <summary>
        /// 获取Unity 对象池管理器所属的Hierachy 父节点
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public static Transform GetUnityPoolManagerTransParent(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("GetUnityPoolManagerTransParent Fail,Parameter poolName is null");
                return null;
            }

            poolName = string.Format("{0}{1}", R_UnityPoolManagerPrefix, poolName);
            Transform parent = null;
            if (mAllPoolMonoObjects.TryGetValue(poolName, out parent))
            {
                if (parent != null)
                    return parent;
            }

            parent = ResourcesManagerUtility.Instantiate(poolName).transform;
        //    parent.SetParent(transform, false);
            mAllPoolMonoObjects[poolName] = parent;
            return parent;
        }

        /// <summary>
        /// 销毁对象池时候回收父节点
        /// </summary>
        /// <param name="poolName"></param>
        public static void RecycleUnityPoolManagerTransParent(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("RecycleUnityPoolManagerTransParents Fail,Parameter poolName is null");
                return;
            }

            poolName = string.Format("{0}{1}", R_UnityPoolManagerPrefix, poolName);
            Transform parent = null;
            if (mAllPoolMonoObjects.TryGetValue(poolName, out parent))
            {
                if (parent != null)
                {
                    mAllPoolMonoObjects.Remove(poolName);
                    ResourcesManagerUtility.Destroy(parent.gameObject);
                    return;
                }
            }

            Debug.LogError(string.Format("RecycleUnityPoolManagerTransParent Fail,the poolName={0} connect Transform Not Record Or Aleady Destroyed", poolName));
            return;
        }
        #endregion
    }
}
