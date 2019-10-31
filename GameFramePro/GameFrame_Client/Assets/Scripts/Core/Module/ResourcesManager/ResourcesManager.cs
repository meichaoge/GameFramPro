using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramePro.ResourcesEx;
using Object = UnityEngine.Object;
using GameFramePro.ResourcesEx.Reference;
using GameFramePro.UI;

namespace GameFramePro
{

    /// <summary>
    /// 资源加载渠道选择
    /// </summary>
    public enum LoadAssetChannelUsage
    {
        /// <summary>
        /// 编辑器下根据 默认编辑器下非Android 和 IOS 是通过配置 ，其他平台根据 优先AssetBundle,然后Local
        /// </summary>
        Default = 0, //默认的 根据 
        LocalResourcesPriority, //Resources 资源优先加载
        LocalResourcesOnly, // 只加载Resources 资源

        AssetBundlePriority = 10,  //AssetBundle 资源优先加载
        AssetBundleOnly,  //只加载 AssetBundle 
    }


    /// <summary>/// 这里有ResourcesManger 提供的对外访问接口和实现/// </summary>
    public sealed class ResourcesManager : Single<ResourcesManager>
    {
        #region 对象的创建和销毁逻辑（实例化对象 (这里对内部的GameObject.Instantiate<T> 做了一层封装，主要是想后期能够监控对象的创建TODO)）

        /// <summary>/// 标记为不会被销毁的对象/// </summary>
        public static void MarkNotDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail, Parameter is Null");
                return;
            }

            GameObject.DontDestroyOnLoad(go);
            return;
        }

        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj == null) return;
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj == null) return;
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.DestroyImmediate(obj);
        }


        /// <summary>///  实例化一个对象/// </summary>
        public static GameObject Instantiate(string goName)
        {
            GameObject go = new GameObject(goName);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
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

        #region 资源加载&释放接口

        public static void UnLoadAsset(UnityEngine.Object asset)
        {
            try
            {
                if (asset == null)
                {
                    Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                    return;
                }

#if UNITY_EDITOR
                Debug.LogInfor($"卸载资源  {asset.name}:{asset.GetType()}");
#endif
                Resources.UnloadAsset(asset);
            }
            catch (Exception e)
            {
                Debug.LogInfor($"卸载资源  {asset.name}:{asset.GetType()}  异常{e}");
            }

        }

        /// <summary>
        /// 强制系统回收资源
        /// </summary>
        public static void UnloadAllUnusedAssets()
        {
            try
            {
                UIPageManagerUtility.S_Instance.UnLoadInVisibleUI(Time.realtimeSinceStartup);
                AssetDelayDeleteManager.S_Instance.UnLoadNoReferenceAssets(Time.realtimeSinceStartup);
                Resources.UnloadUnusedAssets();

                Debug.LogInfor($"主动清理所有没有引用的资源{DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                Debug.LogError($"主动清理所有没有引用的资源{DateTime.UtcNow}  异常{e}");
            }

        }

        public static void UnLoadAssetBundle(AssetBundle asset, bool isUnloadAllLoadedObjects)
        {
            if (asset == null)
            {
                Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                return;
            }
            try
            {
#if UNITY_EDITOR
                Debug.Log($"释放AssetBundle 资源{asset.name}");
#endif
                asset.Unload(isUnloadAllLoadedObjects);
            }
            catch (Exception e)
            {
                Debug.Log($"释放AssetBundle 资源{asset.name} 异常{e}");
            }
        }

        #endregion

        #region 扩展资源加载接口

        /// <summary>
        /// 扩展了资源加载的方式 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetBundleUri"></param>
        /// <param name="targetComponent">引用加载资源的组件</param>
        /// <param name="ConnectAssetReferenceComponentAtc">得到资源后的操作，第一个参数标识是否加载成功，第三个参数标识是否需要引用这个资源</param>
        /// <param name="loadAssetChannel">加载的渠道</param>
        /// <returns></returns>
        public static bool LoadAssetSync_Ex<T>(string assetBundleUri, Component targetComponent, System.Action<bool, T> ConnectAssetReferenceComponentAtc, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default) where T : UnityEngine.Object
        {
            LoadAssetResult<T> result = LoadAssetSync<T>(assetBundleUri, loadAssetChannel);
            if (result == null)
            {
                Debug.LogError($"加载资源{assetBundleUri} 失败");
                ConnectAssetReferenceComponentAtc?.Invoke(false, null);
                return false;
            }
            return result.ReferenceWithComponent(targetComponent, resultAsset => ConnectAssetReferenceComponentAtc?.Invoke(true, resultAsset));
        }


        /// <summary>
        /// 通用异步加载接口
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="completeCallback"></param>
        /// <param name="isForceReload"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadAssetAsync_Ex<T>(string assetBundleUri, Component targetComponent, System.Action<bool, T> ConnectAssetReferenceComponentAtc, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default) where T : UnityEngine.Object
        {
            System.Action<LoadAssetResult<T>> completeCallback = (loadResult) =>
            {
                if (loadResult == null)
                {
                    ConnectAssetReferenceComponentAtc?.Invoke(false, null);
                    return;
                }

                loadResult.ReferenceWithComponent(targetComponent, resultAsset => ConnectAssetReferenceComponentAtc?.Invoke(true, resultAsset));
            };

            LoadAssetRecordAsync(assetBundleUri, (assetRecord) =>
            {
                if (assetRecord == null)
                {
                    completeCallback?.Invoke(null);
                    return;
                }

                T result = null;
                if (typeof(T) == typeof(Sprite))
                    result = GetSpriteAssetFromLoadAsset(assetRecord.GetLoadAsset()) as T;
                else
                    result = assetRecord.GetLoadAsset() as T;
                ReferenceAssetManager.S_Instance.AddWeakReference(result, assetRecord);
                completeCallback?.Invoke(new LoadAssetResult<T>(result));
            }, loadAssetChannel);

        }

        #endregion

        #region 资源加载接口

        /// <summary>
        /// 同步加载指定的资源 并且没有增加引用计数，如果需要使用这个资源需要调用 ReferenceWithComponent接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath"></param>
        /// <param name="isForceReload"></param>
        /// <returns></returns>
        public static LoadAssetResult<T> LoadAssetSync<T>(string assetBundleUri, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default) where T : UnityEngine.Object
        {
            var objectAssetRecord = LoadAssetRecordSync(assetBundleUri,loadAssetChannel);
            if (objectAssetRecord == null)
                return null;
            T result = null;
            if (typeof(T) == typeof(Sprite))
                result = GetSpriteAssetFromLoadAsset(objectAssetRecord.GetLoadAsset()) as T;
            else
                result = objectAssetRecord.GetLoadAsset() as T;

            ReferenceAssetManager.S_Instance.AddWeakReference(result, objectAssetRecord);
            return new LoadAssetResult<T>(result);
        }


        /// <summary>
        /// 通用异步加载接口
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="completeCallback"></param>
        /// <param name="isForceReload"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadAssetAsync<T>(string assetBundleUri, System.Action<LoadAssetResult<T>> completeCallback, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default) where T : UnityEngine.Object
        {
            if (completeCallback != null)
            {
                LoadAssetRecordAsync(assetBundleUri, (assetRecord) =>
                {
                    if (assetRecord == null)
                    {
                        completeCallback?.Invoke(null);
                        return;
                    }

                    T result = null;
                    if (typeof(T) == typeof(Sprite))
                        result = GetSpriteAssetFromLoadAsset(assetRecord.GetLoadAsset()) as T;
                    else
                        result = assetRecord.GetLoadAsset() as T;
                    ReferenceAssetManager.S_Instance.AddWeakReference(result, assetRecord);
                    completeCallback?.Invoke(new LoadAssetResult<T>(result));
                }, loadAssetChannel);
            }
            else
            {
                LoadAssetRecordAsync(assetBundleUri, null, loadAssetChannel);
            }
        }


        /// <summary>
        /// 从加载的资源中获取精灵对象
        /// </summary>
        /// <param name="objectAsset"></param>
        /// <returns></returns>
        private static Sprite GetSpriteAssetFromLoadAsset(Object objectAsset)
        {
            if (objectAsset == null)
                return null;
            GameObject go = objectAsset as GameObject;
            if (go == null)
                return null;
            SpriteRenderer renderer = go.GetComponent<SpriteRenderer>();
            if (renderer != null)
                return renderer.sprite;
            return null;
        }


        /// <summary>
        /// 实例化GameObject 专用接口 
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="parent"></param>
        /// <param name="localPositon"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static GameObject InstantiateAssetSync(string assetBundleUri, Transform parent, Vector3 localPositon, Quaternion rotation, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {
            var objectAssetRecord = LoadAssetRecordSync(assetBundleUri, loadAssetChannel);
            if (objectAssetRecord == null)
                return null;

            GameObject prefab = objectAssetRecord.GetLoadAsset() as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"加载资源{assetBundleUri} 失败");
                return null;
            }

            GameObject go = Instantiate(prefab, localPositon, rotation, parent);
            if (go == null)
                return null;
            ReferenceAssetManager.S_Instance.AddWeakReference(go, objectAssetRecord);
            ReferenceAssetManager.S_Instance.StrongReferenceWithComponent<Transform>(go, go.transform);
            return go;
        }
        public static GameObject InstantiateAssetSync(string assetBundleUri, Transform parent, bool worldPositionStays, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {
            var objectAssetRecord = LoadAssetRecordSync(assetBundleUri, loadAssetChannel);
            if (objectAssetRecord == null)
                return null;
            GameObject prefab = objectAssetRecord.GetLoadAsset() as GameObject;
            if (prefab == null)
            {
                Debug.LogError($"加载资源{assetBundleUri} 失败");
                return null;
            }

            GameObject go = Instantiate(prefab, parent, worldPositionStays);
            if (go == null)
                return null;
            ReferenceAssetManager.S_Instance.AddWeakReference(go, objectAssetRecord);
            ReferenceAssetManager.S_Instance.StrongReferenceWithComponent<Transform>(go, go.transform);
            return go;
        }

        /// <summary>
        /// 异步实例化对象
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="completeCallback"></param>
        /// <param name="parent"></param>
        /// <param name="localPositon"></param>
        /// <param name="rotation"></param>
        public static void InstantiateAssetAsync(string assetBundleUri, System.Action<GameObject> completeCallback, Transform parent, Vector3 localPositon, Quaternion rotation, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {
            if (completeCallback != null)
            {
                LoadAssetRecordAsync(assetBundleUri, (assetRecord) =>
                {
                    if (assetRecord == null)
                    {
                        completeCallback?.Invoke(null);
                        return;
                    }

                    GameObject prefab = assetRecord.GetLoadAsset() as GameObject;
                    if (prefab == null)
                    {
                        Debug.LogError($"加载资源{assetBundleUri} 失败");
                        completeCallback.Invoke(null);
                        return;
                    }

                    GameObject go = Instantiate(prefab, localPositon, rotation, parent);
                    if (go == null)
                    {
                        completeCallback.Invoke(null);
                        return;
                    }

                    ReferenceAssetManager.S_Instance.AddWeakReference(go, assetRecord);
                    ReferenceAssetManager.S_Instance.StrongReferenceWithComponent<Transform>(go, go.transform);
                    completeCallback.Invoke(go);
                }, loadAssetChannel);
            }
            else
            {
                LoadAssetRecordAsync(assetBundleUri, null, loadAssetChannel);
            }
        }
        public static void InstantiateAssetAsync(string assetBundleUri, System.Action<GameObject> completeCallback, Transform parent, bool worldPositionStays, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {
            if (completeCallback != null)
            {
                LoadAssetRecordAsync(assetBundleUri, (assetRecord) =>
                {
                    if (assetRecord == null)
                    {
                        completeCallback?.Invoke(null);
                        return;
                    }

                    GameObject prefab = assetRecord.GetLoadAsset() as GameObject;
                    if (prefab == null)
                    {
                        Debug.LogError($"加载资源{assetBundleUri} 失败");
                        completeCallback.Invoke(null);
                    }
                    else
                    {
                        GameObject go = Instantiate(prefab, parent, worldPositionStays);
                        ReferenceAssetManager.S_Instance.AddWeakReference(go, assetRecord);
                        ReferenceAssetManager.S_Instance.StrongReferenceWithComponent<Transform>(go, go.transform);
                        //  ReferenceAssetManager.S_Instance.AddStrongReference<GameObject>(go.transform, prefab, assetRecord);
                        completeCallback.Invoke(go);
                    }
                }, loadAssetChannel);
            }
            else
            {
                LoadAssetRecordAsync(assetBundleUri, null, loadAssetChannel);
            }
        }

        /// <summary>/// 同步下载资源接口/// </summary>
        private static ILoadAssetRecord LoadAssetRecordSync(string assetBundleUri, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {
            ILoadAssetRecord record = null;

            switch (loadAssetChannel)
            {
                case LoadAssetChannelUsage.Default:
                    #region 默认的加载规则
#if UNITY_EDITOR && !UNITY_IPHONE && !UNITY_ANDROID

                    if (AppEntryManager.S_Instance.mIsLoadResourcesAssetPriority)
                    {
                        record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
                        if (record == null)
                            record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
                    }
                    else
                    {
                        record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
                        if (record == null)
                            record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
                    }
#else
            record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
            if (record == null)
                record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
        
#endif
                    #endregion
                    break;
                case LoadAssetChannelUsage.LocalResourcesPriority:
                    record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
                    if (record == null)
                        record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
                    break;
                case LoadAssetChannelUsage.LocalResourcesOnly:
                    record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
                    break;
                case LoadAssetChannelUsage.AssetBundlePriority:
                    record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
                    if (record == null)
                        record = LocalResourcesManager.S_Instance.ResourcesLoadAssetSync(assetBundleUri);
                    break;
                case LoadAssetChannelUsage.AssetBundleOnly:
                    record = AssetBundleManager.S_Instance.AssetBundleLoadAssetSync(assetBundleUri);
                    break;
                default:
                    Debug.LogError($"LoadAssetRecordSync 没有处理的加载流程{loadAssetChannel}");
                    break;
            }

            return record;
        }

        /// <summary>/// 异步下载资源的接口/// </summary>
        private static void LoadAssetRecordAsync(string assetBundleUri, System.Action<ILoadAssetRecord> loadCallback, LoadAssetChannelUsage loadAssetChannel = LoadAssetChannelUsage.Default)
        {

            switch (loadAssetChannel)
            {
                case LoadAssetChannelUsage.Default:

                    #region 默认的规则
#if UNITY_EDITOR && !UNITY_IPHONE && !UNITY_ANDROID
                    if (AppEntryManager.S_Instance.mIsLoadResourcesAssetPriority)
                    {
                        LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (assetRecord) =>
                        {
                            if (assetRecord != null)
                                loadCallback?.Invoke(assetRecord);
                            else
                            {
                                AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetBundleAsset) => { loadCallback?.Invoke(assetBundleAsset); });
                            }
                        }, null);
                    }
                    else
                    {
                        AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetRecord) =>
                        {
                            if (assetRecord != null)
                                loadCallback?.Invoke(assetRecord);
                            else
                            {
                                LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (localAsset) => { loadCallback?.Invoke(localAsset); }, null);
                            }
                        });
                    }
#else
             AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetRecord) =>
                {
                    if (assetRecord != null)
                        loadCallback?.Invoke(assetRecord);
                    else
                    {
                        LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (localAsset) => { loadCallback?.Invoke(localAsset); }, null);
                    }
                });
#endif
                    #endregion
                    break;
                case LoadAssetChannelUsage.LocalResourcesPriority:
                    LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (assetRecord) =>
                    {
                        if (assetRecord != null)
                            loadCallback?.Invoke(assetRecord);
                        else
                        {
                            AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetBundleAsset) => { loadCallback?.Invoke(assetBundleAsset); });
                        }
                    }, null);
                    break;
                case LoadAssetChannelUsage.LocalResourcesOnly:
                    LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (assetRecord) =>
                    {
                        loadCallback?.Invoke(assetRecord);
                    }, null);
                    break;
                case LoadAssetChannelUsage.AssetBundlePriority:
                    AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetRecord) =>
                    {
                        if (assetRecord != null)
                            loadCallback?.Invoke(assetRecord);
                        else
                        {
                            LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetBundleUri, (localAsset) => { loadCallback?.Invoke(localAsset); }, null);
                        }
                    });
                    break;
                case LoadAssetChannelUsage.AssetBundleOnly:
                    AssetBundleManager.S_Instance.AssetBundleLoadAssetAsync(assetBundleUri, (assetRecord) =>
                    {
                        loadCallback?.Invoke(assetRecord);
                    });
                    break;
                default:
                    Debug.LogError($" LoadAssetRecordAsync 没有处理的加载流程{loadAssetChannel}");
                    break;
            }





        }

        #endregion




        #region 引用关系处理

        /// <summary>
        /// 释放某个组件对某个资源的依赖
        /// </summary>
        /// <param name="targetComponent"></param>
        /// <param name="targetReferenceAsset"></param>
        public static void ReleaseComponentReferenceAsset<T>(Component targetComponent, Object targetReferenceAsset) where T : UnityEngine.Object
        {
            ReferenceAssetManager.S_Instance.ReduceStrongReference<T>(targetComponent, targetReferenceAsset);
        }


        /// <summary>
        /// 某个Object 被释放移除所有的依赖
        /// </summary>
        /// <param name="targetObject"></param>
        public static void ReduceGameObjectReference(GameObject targetObject)
        {
            ReferenceAssetManager.S_Instance.RemoveGameObjectReference(targetObject);
        }

        #endregion
    }
}