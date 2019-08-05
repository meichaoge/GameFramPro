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
            if (obj == null) return;
            ResourcesTracker.UnRegisterTraceResources(obj);
            ReleaseComponentReference(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            if (obj == null) return;
            ResourcesTracker.UnRegisterTraceResources(obj);
            ReleaseComponentReference(obj);
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

        #region 资源加载&释放接口

        //释放一个对象& 组件(根据类型判断) 上的所有引用
        public static void ReleaseComponentReference(Object targetObject)
        {
            if (targetObject == null) return;
            if (targetObject is Component)
                ReferenceAssetUtility.ReleaseComponentReference(targetObject as Component);
            else if (targetObject is GameObject)
                ReferenceAssetUtility.ReleaseGameObjectReference(targetObject as GameObject);
        }


        /// <summary>/// 缓存中加载资源/// </summary>
        private static LoadAssetAssetRecord LoadAssetFromCache(string assetPath)
        {
            LoadAssetAssetRecord asset = LocalResourcesManager.S_Instance.LoadAssetFromCache(assetPath);
            if (asset != null)
                return asset;
            asset = AssetBundleManager.S_Instance.LoadSubAssetFromCache(assetPath);
            return asset;
        }

        /// <summary>/// 同步下载资源接口/// </summary>
        public static LoadAssetAssetRecord LoadAssetSync(string assetPath)
        {
            LoadAssetAssetRecord record = null;

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

        /// <summary>/// 异步下载资源的接口/// </summary>
        public static void LoadAssetAsync(string assetPath, System.Action<LoadAssetAssetRecord> loadCallback)
        {
            var assetRecord = LoadAssetFromCache(assetPath);
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


        /// <summary>/// 卸载 AssetBundle 资源加载/// </summary>
        public static void UnLoadAssetBundle(LoadAssetBundleAssetRecord assetRecord, bool isUnloadAllLoadedObjects)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadAssetBundle Fail!! 参数是null ");
                return;
            }

            Debug.LogEditorInfor("释放 AssetBundle 资源 " + assetRecord.AssetUrl);
            assetRecord.LoadAssetBundleInformation.UnLoadAsAssetBundleAsset(isUnloadAllLoadedObjects);
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

            var assetRecord = LoadAssetSync(assetPath);
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

            LoadAssetAsync(assetPath, (assetRecord) =>
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
            if (string.IsNullOrEmpty(assetPath))
                return SetImageSpriteFromRecord(targetImage, null, isAutoAttachReferenceAsset, isForceCreateInstance);
            return SetImageSpriteFromRecord(targetImage, LoadAssetSync(assetPath), isAutoAttachReferenceAsset, isForceCreateInstance);
        }

        /// <summary>/// 根据路径加载图片资源 异步接口 .isAutoAttachReferenceAsset=true标示默认关联指定的图片资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static void SetImageSpriteByPathAsync(Image targetImage, string assetPath, Action<BaseBeReferenceInformation> afterAttachSpriteAction, bool isAutoAttachReferenceAsset = true, bool isForceCreateInstance = false)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                var assetInfor = SetImageSpriteFromRecord(targetImage, null, isAutoAttachReferenceAsset, isForceCreateInstance);
                afterAttachSpriteAction?.Invoke(assetInfor);
            }
            else
            {
                LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    var assetInfor = SetImageSpriteFromRecord(targetImage, assetRecord, isAutoAttachReferenceAsset, isForceCreateInstance);
                    afterAttachSpriteAction?.Invoke(assetInfor);
                });
            }
        }

        /// <summary>/// 真实的赋值操作， 根据资源加载图片精灵  .isAutoAttachReferenceAsset=true标示默认关联指定的图片资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        private static BaseBeReferenceInformation SetImageSpriteFromRecord(Image targetImage, LoadAssetAssetRecord assetRecord, bool isAutoAttachReferenceAsset = true, bool isForceCreateInstance = false)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetImage, FindAssetReferenceUtility.GetSpriteAssetReference, isForceCreateInstance);
            if (assetRecord == null)
            {
                if (curReference != null)
                    ReferenceAssetUtility.ReleaseComponentReference(targetImage, curReference);

                if (isAutoAttachReferenceAsset)
                    targetImage.sprite = null;
                return null;
            }

            if (curReference != null && curReference.CurLoadAssetRecord.isReferenceEqual(assetRecord))
                return null;
            Sprite sp = assetRecord.LoadBasicObjectAssetInfor.LoadSpriteFromSpriteRender();
            BaseBeReferenceInformation baseBeReferenceInformation = new BaseBeReferenceInformation(sp, typeof(Sprite), assetRecord);

            ReferenceAssetAndRecord newReference = new ReferenceAssetAndRecord(assetRecord, baseBeReferenceInformation);

            if (curReference != null)
                curReference.ModifyComponentReference(targetImage, newReference);
            else
                ReferenceAssetUtility.AddObjectComponentReference(targetImage, newReference, isAutoAttachReferenceAsset);

            if (isAutoAttachReferenceAsset)
                targetImage.sprite = sp;
            return baseBeReferenceInformation;
        }


        /// <summary>/// 从 fromImage 上获取引用的的资源克隆到 toImage/// </summary>
        public static BaseBeReferenceInformation CloneImageSprite(Image fromImage, Image toImage, bool isAutoAttachReferenceAsset = true)
        {
            var fromsourceReference = ReferenceAssetUtility.GetComponentReference(fromImage, FindAssetReferenceUtility.GetSpriteAssetReference, false);
            var totargetReference = ReferenceAssetUtility.GetComponentReference(toImage, FindAssetReferenceUtility.GetSpriteAssetReference, false);

            if (fromsourceReference == null || fromsourceReference.IsReferenceEnable == false)
            {
                if (isAutoAttachReferenceAsset)
                {
                    if (totargetReference != null)
                        ReferenceAssetUtility.ReleaseComponentReference(toImage, totargetReference);
                    toImage.sprite = null;
                }

                return null;
            }

            if (isAutoAttachReferenceAsset)
            {
                if (totargetReference != null)
                    totargetReference.ModifyComponentReference(toImage, fromsourceReference);
                else
                    ReferenceAssetUtility.AddObjectComponentReference(toImage, fromsourceReference, isAutoAttachReferenceAsset);
                toImage.sprite = fromsourceReference.CurLoadAssetRecord.LoadBasicObjectAssetInfor.LoadSpriteFromSpriteRender();
            }

            return fromsourceReference.BeReferenceAsset;
        }

        #endregion

        #region GameObject加载

        /// <summary>/// 根据给定的路径 实例化一个对象在 指定的父节点上/// </summary>
        /// <param name="isForceCreateInstance">标示是否是强制创建一个新的对象</param>
        public static BaseBeReferenceGameObjectInformation InstantiateGameObjectByPathSync(Transform targetParent, string assetPath, bool isForceCreateInstance)
        {
            if (string.IsNullOrEmpty(assetPath))
                return InstantiateGameObjectFromRecord(targetParent, null, isForceCreateInstance);
            return InstantiateGameObjectFromRecord(targetParent, LoadAssetSync(assetPath), isForceCreateInstance);
        }

        /// <summary>/// 根据给定的路径 实例化一个对象在 指定的父节点上/// </summary>
        /// <param name="isForceCreateInstance">标示是否是强制创建一个新的对象</param>
        public static void InstantiateGameObjectByPathAsync(Transform targetParent, string assetPath, bool isForceCreateInstance, Action<BaseBeReferenceGameObjectInformation> afterInitialedInstanceAction = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                BaseBeReferenceGameObjectInformation gameObject = InstantiateGameObjectFromRecord(targetParent, null, isForceCreateInstance);
                afterInitialedInstanceAction?.Invoke(gameObject);
            }
            else
            {
                LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    BaseBeReferenceGameObjectInformation gameObject = InstantiateGameObjectFromRecord(targetParent, assetRecord, isForceCreateInstance);
                    afterInitialedInstanceAction?.Invoke(gameObject);
                });
            }
        }

        //内部操作  根据给定的路径 实例化一个对象在 指定的父节点上/
        private static BaseBeReferenceGameObjectInformation InstantiateGameObjectFromRecord(Transform targetParent, LoadAssetAssetRecord assetRecord, bool isForceCreateInstance)
        {
            if (assetRecord == null)
                return null;

            var curReference = ReferenceAssetUtility.GetComponentReference(targetParent, FindAssetReferenceUtility.GetGameObjectFromAssetReference, isForceCreateInstance, assetRecord.AssetUrl);

            if (curReference != null)
                return null;

            GameObject go = assetRecord.LoadBasicObjectAssetInfor.InstantiateInstance(targetParent);
            var referenceAssetInfor = new BaseBeReferenceGameObjectInformation(go, typeof(GameObject), assetRecord);

            var newReference = new ReferenceAssetAndRecord(assetRecord, referenceAssetInfor);
            ReferenceAssetUtility.AddObjectComponentReference(targetParent, newReference);

            return referenceAssetInfor;
        }

        #endregion

        #region Audio 加载

        /// <summary>/// 根据路径加载声音资源 同步接口 .isAutoAttachReferenceAsset=true标示默认关联指定的声音资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static BaseBeReferenceInformation GetAudioClipByPathSync(AudioSource targetAudioSources, string assetPath, bool isAutoAttachReferenceAsset, bool isForceCreateInstance = false)
        {
            if (string.IsNullOrEmpty(assetPath))
                return GetAudioClipFromRecord(targetAudioSources, null, isAutoAttachReferenceAsset, isForceCreateInstance);
            return GetAudioClipFromRecord(targetAudioSources, LoadAssetSync(assetPath), isAutoAttachReferenceAsset, isForceCreateInstance);
        }

        private static BaseBeReferenceInformation GetAudioClipFromRecord(AudioSource targetAudioSources, LoadAssetAssetRecord assetRecord, bool isAutoAttachReferenceAsset, bool isForceCreateInstance = false)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetAudioSources, FindAssetReferenceUtility.GetSAudioClipAssetReference, isForceCreateInstance);
            if (assetRecord == null)
            {
                if (curReference != null)
                    ReferenceAssetUtility.ReleaseComponentReference(targetAudioSources, curReference);
                return null;
            }

            if (curReference != null && curReference.CurLoadAssetRecord.isReferenceEqual(assetRecord))
                return null;
            var clip = assetRecord.LoadBasicObjectAssetInfor.LoadAudioClip();
            var referenceAssetInfor = new BaseBeReferenceInformation(clip, typeof(AudioClip), assetRecord);
            var newReference = new ReferenceAssetAndRecord(assetRecord, referenceAssetInfor);

            if (curReference != null)
                curReference.ModifyComponentReference(targetAudioSources, newReference);
            else
                ReferenceAssetUtility.AddObjectComponentReference(targetAudioSources, newReference, isAutoAttachReferenceAsset);

            if (isAutoAttachReferenceAsset)
                targetAudioSources.clip = clip;

            return referenceAssetInfor;
        }

        #endregion

        #endregion
        
    }
}
