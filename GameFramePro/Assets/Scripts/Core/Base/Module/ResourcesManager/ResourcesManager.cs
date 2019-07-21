using GameFramePro.ResourcesEx;
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

        #region TextAsset/lua 文件的加载

        private static Dictionary<string, string> mAllCacheTextInfor = new Dictionary<string, string>();//缓存所有加载的文本资源
        /// <summary>
        /// 文本资源的加载(一般是配置文件或者lua 文件)
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string LoadTextAssettSync(string assetPath, bool isForceReload = false)
        {
            string reseultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out reseultStr) && string.IsNullOrEmpty(reseultStr) == false)
                    return reseultStr;
            }
            ILoadAssetRecord assetRecord = LoadAssetSync(assetPath);
            if (assetRecord != null && assetRecord.TargetAsset != null)
            {
                if (assetRecord.TargetAsset is TextAsset)
                {
                    reseultStr = (assetRecord.TargetAsset as TextAsset).text;
                    mAllCacheTextInfor[assetPath] = reseultStr;
                    return reseultStr;
                }
            }
            Debug.LogError("获取文本 资源失败 " + assetPath);
            return reseultStr;
        }

        public static void LoadTextAssettAsync(string assetPath, System.Action<string> textAssetAction, bool isForceReload = false)
        {
            string reseultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out reseultStr) && string.IsNullOrEmpty(reseultStr) == false)
                {
                    if (textAssetAction != null) textAssetAction(reseultStr);
                    return;
                }
            }
            LoadAssetAsync(assetPath, (assetRecord) =>
            {
                if (assetRecord != null && assetRecord.TargetAsset != null)
                {
                    if (assetRecord.TargetAsset is TextAsset)
                    {
                        reseultStr = (assetRecord.TargetAsset as TextAsset).text;
                        mAllCacheTextInfor[assetPath] = reseultStr;
                        if (textAssetAction != null) textAssetAction(reseultStr);
                        return;
                    }
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
        public static void LoadSpriteAssetSync<T>(string assetPath, T targetImage, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null, GetCurReferenceHandler<T> getAssetReference = null) where T : Image
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

            ILoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.TargetAsset == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction);
        }
        /// <summary>
        ///  异步 从指定路径加载一个sprite 并赋值给 targetImage 认的 是从加载资源的记录中取得SpriteRender 组件然后赋给参数targetImage
        /// </summary>
        /// <param name="assetPath">当assetPath 为null 时候设置targetImage为空。否则正常加载对应路径的资源</param>
        /// <param name="targetImage"></param>
        /// <param name="getAssetReference">定义了如何从当前组件 targetImage 的所有引用资源链表中找到自己想要的修改的那个引用记录</param>
        /// <param name="getAssetFromRecordAction"></param>
        public static void LoadSpriteAssetAsync<T>(string assetPath, T targetImage, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null, GetCurReferenceHandler<T> getAssetReference = null) where T : Image
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


            ILoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.TargetAsset != null)
            {
                AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, getAssetReference, getAssetFromRecordAction);
                return;
            }

            LoadAssetAsync(assetPath, (record) => { AssetReferenceController.CreateOrAddReference<T>(targetImage, record, getAssetReference, getAssetFromRecordAction); });
        }

        /// <summary>
        /// 从 copyImage 中复制 图片到targetImage中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="copyImage"></param>
        /// <param name="targetImage"></param>
        public static void LoadSpriteAssetAsync<T>(T copyImage, T targetImage, GetAssetFromRecordHandler<T> getAssetFromRecordAction = null, GetCurReferenceHandler<T> getAssetReference = null) where T : Image
        {
            IAssetReference assetReference = AssetReferenceController.GetAssetReference<T>(copyImage, SpriteAssetReference<T>.GetSpriteAssetReference);
            ILoadAssetRecord assetRecord = null;
            if (assetReference != null)
                assetRecord = assetReference.CurLoadAssetRecord;
            AssetReferenceController.CreateOrAddReference<T>(targetImage, assetRecord, SpriteAssetReference<T>.GetSpriteAssetReference, SpriteAssetReference<T>.GetSpriteFromSpriteRender);
        }
        #endregion

        #region GameObject加载
        public static void LoadGameObjectAssetSync(string assetPath, Transform targetParent, GetAssetFromRecordHandler<Transform> getAssetFromRecordAction = null, GetCurReferenceHandler<Transform> getAssetReference = null)
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

            ILoadAssetRecord assetRecord = null;
            if (string.IsNullOrEmpty(assetPath) == false)
            {
                assetRecord = LoadAssetFromCache(assetPath);
                if (assetRecord == null || assetRecord.TargetAsset == null)
                    assetRecord = LoadAssetSync(assetPath);
            }
            AssetReferenceController.CreateOrAddReference<Transform>(targetParent, assetRecord, getAssetReference, getAssetFromRecordAction);
        }

        #endregion

        #region Material 加载

        #endregion

        #endregion



        #region 资源加载&释放接口
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
        private static ILoadAssetRecord LoadAssetSync(string assetPath)
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
        private static void LoadAssetAsync(string assetPath, System.Action<ILoadAssetRecord> loadCallback)
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


        /// <summary>
        /// 卸载 Resources 资源
        /// </summary>
        /// <param name="assetRecord"></param>
        public static void UnLoadResourceAsset(ILoadAssetRecord assetRecord)
        {
            if (assetRecord == null)
            {
                Debug.LogErrorFormat("UnLoadResourceAsset Fail!! 参数是null ");
                return;
            }

            if (assetRecord.AssetLoadedType >= LoadedAssetTypeEnum.AssetBundle_UnKnown)
            {
                Debug.LogErrorFormat("UnLoadResourceAsset Fail!! 当前资源不是Resource 资源 " + assetRecord.AssetUrl);
                return;
            }

            Debug.LogEditorInfor("释放Resources 资源 " + assetRecord.AssetUrl);
            if (assetRecord.TargetAsset != null)
                Resources.UnloadAsset(assetRecord.TargetAsset);
        }
        /// <summary>
        /// 卸载 AssetBundle 资源加载
        /// </summary>
        /// <param name="assetRecord"></param>
        /// <param name="isUnloadAllLoadedObjects"></param>
        public static void UnLoadAssetBundle(ILoadAssetRecord assetRecord, bool isUnloadAllLoadedObjects)
        {
            if (assetRecord == null)
            {
                Debug.LogErrorFormat("UnLoadAssetBundle Fail!! 参数是null ");
                return;
            }

            if (assetRecord.AssetLoadedType <= LoadedAssetTypeEnum.LocalStore_UnKnown)
            {
                Debug.LogErrorFormat("UnLoadAssetBundle Fail!! 当前资源不是 AssetBundle 资源 " + assetRecord.AssetUrl);
                return;
            }

            Debug.LogEditorInfor("释放 AssetBundle 资源 " + assetRecord.AssetUrl);
            if (assetRecord.TargetAsset != null)
            {
                (assetRecord.TargetAsset as AssetBundle).Unload(isUnloadAllLoadedObjects);
            }
        }

        #endregion




    }
}