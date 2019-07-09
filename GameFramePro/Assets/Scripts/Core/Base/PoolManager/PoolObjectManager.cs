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
        private static Dictionary<Type, INativeObjectPool<object>> mAllNativeObjectPoolManagers = new Dictionary<Type, INativeObjectPool<object>>();

        private static Dictionary<Type, IUnityObjectPool> mAllUnityObjectPoolManagers = new Dictionary<Type, IUnityObjectPool>();

        #region 原生对象池接口


        public static void TrackPoolManager<T>(INativeObjectPool<T> poolManager) where T : new()
        {
            TrackPoolManager(typeof(T), poolManager);
        }
        public static void TrackPoolManager<T>(Type type, INativeObjectPool<T> poolManager) where T : new()
        {
            if (Application.isPlaying == false) return; //过滤编辑器代码

            INativeObjectPool<object> pool = null;

            if (mAllNativeObjectPoolManagers.TryGetValue(type, out pool))
            {
                Debug.LogError(string.Format("TrackPoolManager Fail,Already Exit Pool of Type {0}", type));
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
            if (Application.isPlaying == false) return; //过滤编辑器代码
            INativeObjectPool<object> pool = null;

            if (mAllNativeObjectPoolManagers.TryGetValue(type, out pool))
            {
                mAllNativeObjectPoolManagers.Remove(type);
                return;
            }
            Debug.LogError(string.Format("UnTrackPoolManager Fail,Not Exit Pool of Type {0}", type));
        }

        #endregion


        #region Unity  对象池接口


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

    }
}