﻿using GameFramePro.ResourcesEx;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// 这里只有ResourcesManger 提供的对外访问接口和实现
    /// </summary>
    public class ResourcesManager : Single<ResourcesManager>
    {
        #region 对象的创建和销毁逻辑（实例化对象 (这里对内部的GameObject.Instantiate<T> 做了一层封装，主要是想后期能够监控对象的创建TODO)）

        // 实例化一个对象
        public static GameObject Instantiate(string goName)
        {
            GameObject go = new GameObject(goName);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        //标记为不会被销毁的对象
        public static bool MarkNotDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail, Parameter is Null");
                return false;
            }

            GameObject.DontDestroyOnLoad(go);
            return true;
        }

        public static void Destroy(UnityEngine.Object obj)
        {
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.DestroyImmediate(obj);
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original) where T : Object
        {
            T go = GameObject.Instantiate<T>(original, Vector3.zero, Quaternion.identity);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            T go = Instantiate<T>(original, parent, true);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            T go = Instantiate<T>(original, position, rotation, null);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            T go = GameObject.Instantiate<T>(original, position, rotation, parent);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            T go = GameObject.Instantiate<T>(original, parent, worldPositionStays);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }


        #endregion


        #region 资源加载接口
        #region Sprite 加载
    
        /// <summary>
        /// 从指定路径加载一个sprite 并赋值给 targetImage 认的 是从加载资源的记录中取得SpriteRender 组件然后赋给参数targetImage
        /// </summary>
        /// <param name="assetPath">当assetPath 为null 时候设置targetImage为空。否则正常加载对应路径的资源</param>
        /// <param name="targetImage"></param>
        /// <param name="getAssetReference">定义了如何从当前组件 targetImage 的所有引用资源链表中找到自己想要的修改的那个引用记录</param>
        /// <param name="getAssetFromRecordAction"></param>
        public static void LoadSpriteAssetSync<T>(string assetPath, T targetImage,Action<T, ILoadAssetRecord> getAssetFromRecordAction=null, Func<LinkedList<IAssetReference>, IAssetReference> getAssetReference=null) where T:Image
        {
            if (getAssetFromRecordAction == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须设置如何从加载的资源中获取需要的资源的方法");
                //return;
                getAssetFromRecordAction = SpriteAssetReference.GetSpriteFromSpriteRender;
            }
            if (getAssetReference == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须指定如何从一个组件的所有当前资源引用中找到对应的引用的方法");
                getAssetReference = SpriteAssetReference.GetSpriteAssetReference;
            }
         
            ILoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.TargetAsset == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction);
        }

        public static void LoadSpriteAssetAsync<T>(string assetPath, T targetImage, Action<T, ILoadAssetRecord> getAssetFromRecordAction=null, Func<LinkedList<IAssetReference>, IAssetReference> getAssetReference=null) where T : Image
        {
            if (getAssetFromRecordAction == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须设置如何从加载的资源中获取需要的资源的方法");
                getAssetFromRecordAction = SpriteAssetReference.GetSpriteFromSpriteRender;
            }
            if (getAssetReference == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须指定如何从一个组件的所有当前资源引用中找到对应的引用的方法");
                getAssetReference = SpriteAssetReference.GetSpriteAssetReference;
            }
            if (string.IsNullOrEmpty(assetPath))
            {
                AssetReferenceController.CreateOrAddReference<T>(targetImage, null, getAssetReference, getAssetFromRecordAction);
                return;
            }


            ILoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.TargetAsset != null)
            {
                AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction);
                return;
            }

            LoadAssetAsync(assetPath, (record)=> { AssetReferenceController.CreateOrAddReference<T>(targetImage, record, getAssetReference, getAssetFromRecordAction); });
        }


        #endregion

        #endregion



        #region 资源加载接口
        /// <summary>
        /// 缓存中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private static ILoadAssetRecord LoadAssetFromCache(string assetPath)
        {
            ILoadAssetRecord asset = LocalResourcesManager.S_Instance.LoadAssetFromCache(assetPath);
            if (asset != null)
                return asset;
        //    asset = AssetBundleManager.S_Instance.LoadAssetFromCache(assetPath);
            return asset;
        }

        /// <summary>
        /// 同步下载资源接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private static ILoadAssetRecord  LoadAssetSync(string assetPath)
        {
            ILoadAssetRecord record = null;

            if (AppSetting.S_IsLoadResourcesAssetPriority)
            {
                record = LocalResourcesManager.S_Instance.LoadAssetSync(assetPath);
                if (record == null)
                    record = AssetBundleManager.S_Instance.LoadAssetSync(assetPath);
            }
            else
            {
               record = AssetBundleManager.S_Instance.LoadAssetSync(assetPath);
                if (record == null)
                    record = LocalResourcesManager.S_Instance.LoadAssetSync(assetPath);
            }
            return record;
        }


        /// <summary>
        /// 异步下载资源的接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <param name="loadCallback"></param>
        public static void LoadAssetAsync(string assetPath, System.Action<ILoadAssetRecord> loadCallback)
        {
            ILoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }

            if (AppSetting.S_IsLoadResourcesAssetPriority)
                LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetPath, loadCallback, null);
            else
                AssetBundleManager.S_Instance.LoadAssetAsync(assetPath, loadCallback);
        }
        #endregion



        #region 辅助接口



        #endregion

    }
}