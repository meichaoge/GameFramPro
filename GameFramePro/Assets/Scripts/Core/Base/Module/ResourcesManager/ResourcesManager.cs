using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramePro.ResourcesEx;
using Object = UnityEngine.Object;

namespace GameFramePro
{
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
            ResourcesUtility.ReleaseComponentReference(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj == null) return;
            ResourcesTracker.UnRegisterTraceResources(obj);
            ResourcesUtility.ReleaseComponentReference(obj);
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
            if (asset == null)
            {
                Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                return;
            }

#if UNITY_EDITOR
            Debug.LogInfor("卸载资源  " + asset.GetType() + "  " + asset.name);
#endif
            Resources.UnloadAsset(asset);
        }

        public static void UnLoadAssetBundle(AssetBundle asset, bool isUnloadAllLoadedObjects)
        {
            if (asset == null)
            {
                Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                return;
            }

            asset.Unload(isUnloadAllLoadedObjects);
        }

        #endregion

        #region 资源加载接口

        #region TextAsset/lua 文件的加载

        private static readonly Dictionary<string, string> mAllCacheTextInfor = new Dictionary<string, string>(); //缓存所有加载的文本资源

        /// <summary>/// 文本资源的加载(一般是配置文件或者lua 文件)/// </summary>
        public static string LoadTextAssetSync(string assetPath, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                    return resultStr;
            }

            var assetRecord = ResourcesUtility.LoadAssetSync(assetPath);
            if (assetRecord != null && assetRecord.IsReferenceEnable)
            {
                resultStr = assetRecord.LoadBasicObjectAssetInfor.LoadTextAssetContent();
                mAllCacheTextInfor[assetPath] = resultStr;
                return resultStr;
            }

            Debug.LogError("获取文本 资源失败 " + assetPath);
            return resultStr;
        }

        public static void LoadTextAssetAsync(string assetPath, System.Action<string> textAssetAction, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                {
                    textAssetAction?.Invoke(resultStr);
                    return;
                }
            }

            ResourcesUtility.LoadAssetAsync(assetPath, (assetRecord) =>
            {
                if (assetRecord != null && assetRecord.IsReferenceEnable)
                {
                    resultStr = assetRecord.LoadBasicObjectAssetInfor.LoadTextAssetContent();
                    mAllCacheTextInfor[assetPath] = resultStr;
                    textAssetAction?.Invoke(resultStr);
                    return;
                }

                Debug.LogError("获取文本 资源失败 " + assetPath);
                textAssetAction?.Invoke(null);
            });
        }

        #endregion

        #region Sprite 加载

        /// <summary>/// 根据路径加载图片资源 同步接口 .isAutoAttachReferenceAsset=true标示默认关联指定的图片资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static BaseBeReferenceInformation SetImageSpriteByPathSync(Image targetImage, string assetPath, bool isAutoAttachReferenceAsset = true, bool isForceCreateInstance = false)
        {
            LoadAssetAssetRecord targetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
                targetRecord = ResourcesUtility.LoadAssetSync(assetPath);
            return ResourcesUtility.SetImageSpriteFromRecord(targetImage, targetRecord, isAutoAttachReferenceAsset, isForceCreateInstance);
        }

        /// <summary>/// 根据路径加载图片资源 异步接口 .isAutoAttachReferenceAsset=true标示默认关联指定的图片资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static void SetImageSpriteByPathAsync(Image targetImage, string assetPath, Action<BaseBeReferenceInformation> afterAttachSpriteAction, bool isAutoAttachReferenceAsset = true, bool isForceCreateInstance = false)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                var assetInfor = ResourcesUtility.SetImageSpriteFromRecord(targetImage, null, isAutoAttachReferenceAsset, isForceCreateInstance);
                afterAttachSpriteAction?.Invoke(assetInfor);
            }
            else
            {
                ResourcesUtility.LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    var assetInfor = ResourcesUtility.SetImageSpriteFromRecord(targetImage, assetRecord, isAutoAttachReferenceAsset, isForceCreateInstance);
                    afterAttachSpriteAction?.Invoke(assetInfor);
                });
            }
        }

        /// <summary>/// 从 fromImage 上获取引用的的资源克隆到 toImage/// </summary>
        public static BaseBeReferenceInformation CloneImageSprite(Image fromImage, Image toImage, bool isAutoAttachReferenceAsset = true)
        {
            var fromsourceReference = ReferenceAssetUtility.GetComponentReference(fromImage, FindAssetReferenceUtility.GetSpriteAssetReference, false);
            var totargetReference = ReferenceAssetUtility.GetComponentReference(toImage, FindAssetReferenceUtility.GetSpriteAssetReference, false);

            //判断是否引用相同的资源 如果是就不需要克隆
            bool isReferenceAssetEqual = fromsourceReference == null && totargetReference == null;
            if (isReferenceAssetEqual == false)
                isReferenceAssetEqual = totargetReference != null && totargetReference.IsReferenceEqual(fromsourceReference); //判断是否引用相同的资源
            if (isReferenceAssetEqual)
            {
#if UNITY_EDITOR
                Debug.LogEditorInfor($"克隆对象与当前对象引用相同的资源");
#endif
                return totargetReference;
            }

            if (totargetReference != null)
            {
                if (isAutoAttachReferenceAsset)
                    ReferenceAssetUtility.ReleaseComponentReference(toImage, totargetReference); //释放当前引用资源 以及删除对象引用记录
            }

            if (fromsourceReference == null || fromsourceReference.IsReferenceAssetEnable == false)
            {
                if (isAutoAttachReferenceAsset)
                {
                    if (totargetReference != null)
                        ReferenceAssetUtility.ReleaseComponentReference(toImage, totargetReference);
                    toImage.sprite = null;
                }
                return null;
            }

            //注意这里第二个参数是创建一个新的对象 而不是直接使用源对象，避免由于源对象设置为null 导致的异常
            BaseBeReferenceInformation newBeReferenceInformation = new BaseBeReferenceInformation(fromsourceReference);
            ReferenceAssetUtility.AddObjectComponentReference(toImage, newBeReferenceInformation, isAutoAttachReferenceAsset);

            if (isAutoAttachReferenceAsset)
                toImage.sprite = fromImage.sprite;

            return newBeReferenceInformation;
        }

        #endregion

        #region GameObject加载

        /// <summary>/// 根据给定的路径 实例化一个对象在 指定的父节点上/// </summary>/// <param name="isForceCreateInstance">标示是否是强制创建一个新的对象</param>
        public static BaseBeReferenceGameObjectInformation InstantiateGameObjectByPathSync(Transform targetParent, string assetPath, bool isForceCreateInstance)
        {
            if (string.IsNullOrEmpty(assetPath))
                return ResourcesUtility.InstantiateGameObjectFromRecord(targetParent, null, isForceCreateInstance);
            return ResourcesUtility.InstantiateGameObjectFromRecord(targetParent, ResourcesUtility.LoadAssetSync(assetPath), isForceCreateInstance);
        }

        /// <summary>/// 根据给定的路径 实例化一个对象在 指定的父节点上/// </summary>
        /// <param name="isForceCreateInstance">标示是否是强制创建一个新的对象</param>
        public static void InstantiateGameObjectByPathAsync(Transform targetParent, string assetPath, bool isForceCreateInstance, Action<BaseBeReferenceGameObjectInformation> afterInitialedInstanceAction = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                BaseBeReferenceGameObjectInformation gameObject = ResourcesUtility.InstantiateGameObjectFromRecord(targetParent, null, isForceCreateInstance);
                afterInitialedInstanceAction?.Invoke(gameObject);
            }
            else
            {
                ResourcesUtility.LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    BaseBeReferenceGameObjectInformation gameObject = ResourcesUtility.InstantiateGameObjectFromRecord(targetParent, assetRecord, isForceCreateInstance);
                    afterInitialedInstanceAction?.Invoke(gameObject);
                });
            }
        }

        #endregion

        #region Audio 加载

        /// <summary>/// 根据路径加载声音资源 同步接口 .isAutoAttachReferenceAsset=true标示默认关联指定的声音资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static BaseBeReferenceInformation GetAudioClipByPathSync(AudioSource targetAudioSources, string assetPath, bool isAutoAttachReferenceAsset, bool isForceCreateInstance = false)
        {
            if (string.IsNullOrEmpty(assetPath))
                return ResourcesUtility.SetAudioClipFromRecord(targetAudioSources, null, isAutoAttachReferenceAsset, isForceCreateInstance);
            return ResourcesUtility.SetAudioClipFromRecord(targetAudioSources, ResourcesUtility.LoadAssetSync(assetPath), isAutoAttachReferenceAsset, isForceCreateInstance);
        }

        #endregion

        #endregion
    }
}
