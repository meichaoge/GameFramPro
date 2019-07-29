using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramePro.ResourcesEx;
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

        #region TextAsset/lua 文件的加载

        private static Dictionary<string, string> mAllCacheTextInfor = new Dictionary<string, string>();//缓存所有加载的文本资源
        /// <summary>
        /// 文本资源的加载(一般是配置文件或者lua 文件)
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string LoadTextAssettSync(string assetPath, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                    return resultStr;
            }
            BaseLoadAssetRecord assetRecord = LoadAssetSync(assetPath);
            if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor != null)
            {
                resultStr=assetRecord.LoadUnityObjectAssetInfor.LoadTextAssetContent();
                mAllCacheTextInfor[assetPath] = resultStr;
                return resultStr;
            }
            Debug.LogError("获取文本 资源失败 " + assetPath);
            return resultStr;

        }

        public static void LoadTextAssettAsync(string assetPath, System.Action<string> textAssetAction, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                {
                    if (textAssetAction != null) textAssetAction(resultStr);
                    return;
                }
            }
            LoadAssetAsync(assetPath, (assetRecord) =>
            {
                if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor != null)
                {
                    resultStr = assetRecord.LoadUnityObjectAssetInfor.LoadTextAssetContent();
                    mAllCacheTextInfor[assetPath] = resultStr;
                    if (textAssetAction != null) textAssetAction(resultStr);
                    return;
                }
                Debug.LogError("获取文本 资源失败 " + assetPath);
                if (textAssetAction != null) textAssetAction(null);
                return;
            });

        }
        #endregion


        #region Sprite 加载

        /// <summary>
        /// 同步 从指定路径加载一个sprite 并赋值给 targetImage 认的 是从加载资源的记录中取得SpriteRender 组件然后赋给参数targetImage
        /// </summary>
        /// <param name="assetPath">当assetPath 为null 时候设置targetImage为空。否则正常加载对应路径的资源</param>
        /// <param name="targetImage"></param>
        /// <param name="getAssetReference">定义了如何从当前组件 targetImage 的所有引用资源链表中找到自己想要的修改的那个引用记录</param>
        /// <param name="getAssetFromRecordAction"></param>
        public static void LoadSpriteAssetSync<T>(string assetPath, T targetImage, Action<UnityEngine.Object> AfterReferenceAction = null, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null,
            GetCurReferenceHandler<T> getAssetReference = null) where T : Image
        {
            if (getAssetFromRecordAction == null)
                getAssetFromRecordAction = SpriteAssetReference<T>.GetSpriteFromSpriteRender; //指定如何从一个组件的所有当前资源引用中找到对应的引用的方法

            if (getAssetReference == null)
                getAssetReference = SpriteAssetReference<T>.GetSpriteAssetReference; //指定如何从一个组件的所有当前资源引用中找到对应的引用的方法

            BaseLoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.LoadUnityObjectAssetInfor == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction, AfterReferenceAction);
        }

        /// <summary>
        ///  异步 从指定路径加载一个sprite 并赋值给 targetImage 认的 是从加载资源的记录中取得SpriteRender 组件然后赋给参数targetImage
        /// </summary>
        /// <param name="assetPath">当assetPath 为null 时候设置targetImage为空。否则正常加载对应路径的资源</param>
        /// <param name="targetImage"></param>
        /// <param name="getAssetReference">定义了如何从当前组件 targetImage 的所有引用资源链表中找到自己想要的修改的那个引用记录</param>
        /// <param name="getAssetFromRecordAction"></param>
        public static void LoadSpriteAssetAsync<T>(string assetPath, T targetImage, Action<UnityEngine.Object> AfterReferenceAction = null, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null, 
            GetCurReferenceHandler<T> getAssetReference = null) where T : Image
        {
            if (getAssetFromRecordAction == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须设置如何从加载的资源中获取需要的资源的方法");
                getAssetFromRecordAction = SpriteAssetReference<T>.GetSpriteFromSpriteRender;
            }
            if (getAssetReference == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须指定如何从一个组件的所有当前资源引用中找到对应的引用的方法");
                getAssetReference = SpriteAssetReference<T>.GetSpriteAssetReference;
            }
            if (string.IsNullOrEmpty(assetPath))
            {
                AssetReferenceController.CreateOrAddReference<T>(targetImage, null, getAssetReference, getAssetFromRecordAction);
                return;
            }


            BaseLoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor!=null)
            {
                AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction);
                return;
            }

            LoadAssetAsync(assetPath, (record) => { AssetReferenceController.CreateOrAddReference<T>(targetImage, record, getAssetReference, getAssetFromRecordAction, AfterReferenceAction); });
        }

        /// <summary>
        /// 从 copyImage 中复制 图片到targetImage中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyImage"></param>
        /// <param name="targetImage"></param>
        public static void LoadSpriteAssetAsync<T>(T copyImage, T targetImage, Action<UnityEngine.Object> AfterReferenceAction = null, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null, 
            GetCurReferenceHandler<T> getAssetReference = null) where T : Image
        {
            IAssetReference assetReference = AssetReferenceController.GetAssetReference<T>(copyImage, SpriteAssetReference<T>.GetSpriteAssetReference);
            BaseLoadAssetRecord assetRecord = null;
            if (assetReference != null)
                assetRecord = assetReference.CurLoadAssetRecord as BaseLoadAssetRecord;
            AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, SpriteAssetReference<T>.GetSpriteAssetReference, SpriteAssetReference<T>.GetSpriteFromSpriteRender, AfterReferenceAction);
        }
        #endregion

        #region GameObject加载
        public static void LoadGameObjectAssetSync(string assetPath, Transform targetParent, Action<UnityEngine.Object> AfterReferenceAction = null, GetAssetFromRecordHandler<Transform> getAssetFromRecordAction = null, GetCurReferenceHandler<Transform> getAssetReference = null)
        {
            if (getAssetFromRecordAction == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须设置如何从加载的资源中获取需要的资源的方法");
                getAssetFromRecordAction = GameObjectAssetReference.GetGameObjectInstance;
            }
            if (getAssetReference == null)
            {
                //Debug.LogError("LoadSpriteAssetSync Fail, 必须指定如何从一个组件的所有当前资源引用中找到对应的引用的方法");
                getAssetReference = GameObjectAssetReference.GetTransformAssetReference;
            }

            BaseLoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.LoadUnityObjectAssetInfor == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<Transform>(targetParent, assetRecord, getAssetReference, getAssetFromRecordAction, AfterReferenceAction);
        }

        #endregion

        #region AudioClip 加载
        public static void LoadAudioClipAssetSync(string assetPath, AudioSource targetAudioSource, Action<UnityEngine.Object> AfterReferenceAction = null, GetAssetFromRecordHandler<AudioSource> getAssetFromRecordAction = null, GetCurReferenceHandler<AudioSource> getAssetReference = null) 
        {
            if (getAssetFromRecordAction == null)
            {
                //Debug.LogError("LoadAudioClipAssetSync Fail, 必须设置如何从加载的资源中获取需要的资源的方法");
                getAssetFromRecordAction = AudioClipAssetReference.GetAudioClipFromAsset;
            }
            if (getAssetReference == null)
            {
                //Debug.LogError("LoadAudioClipAssetSync Fail, 必须指定如何从一个组件的所有当前资源引用中找到对应的引用的方法");
                getAssetReference = AudioClipAssetReference.GetAudioClipAssetReference;
            }

            BaseLoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.LoadUnityObjectAssetInfor == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<AudioSource>(targetAudioSource, assetRecord, getAssetReference, getAssetFromRecordAction, AfterReferenceAction);
        }

        #endregion

        #endregion



        #region 资源加载&释放接口
        /// <summary>
        /// 缓存中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private static BaseLoadAssetRecord LoadAssetFromCache(string assetPath)
        {
            BaseLoadAssetRecord asset = LocalResourcesManager.S_Instance.LoadAssetFromCache(assetPath);
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
        private static BaseLoadAssetRecord LoadAssetSync(string assetPath)
        {
            BaseLoadAssetRecord record = null;

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
        private static void LoadAssetAsync(string assetPath, System.Action<BaseLoadAssetRecord> loadCallback)
        {
            BaseLoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
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


        /// <summary>
        /// 卸载 Resources 资源
        /// </summary>
        /// <param name="assetRecord"></param>
        public static void UnLoadResourceAsset(BaseLoadAssetRecord assetRecord)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadResourceAsset Fail!! 参数是null ");
                return;
            }

            if (assetRecord.AssetLoadedType >= LoadedAssetTypeEnum.AssetBundle_UnKnown)
            {
                Debug.LogError("UnLoadResourceAsset Fail!! 当前资源不是Resource 资源 " + assetRecord.AssetUrl);
                return;
            }

            Debug.LogEditorInfor("释放Resources 资源 " + assetRecord.AssetUrl);
           ( assetRecord.LoadUnityObjectAssetInfor as ResourceLoadUnityAssetInfor).UnLoadAsResourcesAsset();
        }
        /// <summary>
        /// 卸载 AssetBundle 资源加载
        /// </summary>
        /// <param name="assetRecord"></param>
        /// <param name="isUnloadAllLoadedObjects"></param>
        public static void UnLoadAssetBundle(BaseLoadAssetRecord assetRecord, bool isUnloadAllLoadedObjects)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadAssetBundle Fail!! 参数是null ");
                return;
            }

            if (assetRecord.AssetLoadedType <= LoadedAssetTypeEnum.LocalStore_UnKnown)
            {
                Debug.LogError("UnLoadAssetBundle Fail!! 当前资源不是 AssetBundle 资源 " + assetRecord.AssetUrl);
                return;
            }

            Debug.LogEditorInfor("释放 AssetBundle 资源 " + assetRecord.AssetUrl);
            (assetRecord.LoadUnityObjectAssetInfor as BundleLoadUnityAssetInfor).UnLoadAsAssetBundleAsset(isUnloadAllLoadedObjects);
        }

        #endregion




    }
}