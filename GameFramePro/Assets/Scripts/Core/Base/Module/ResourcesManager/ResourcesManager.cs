using GameFramePro.ResourcesEx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

        public static void Destroy(Object obj)
        {
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(Object obj)
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
        /// <summary>
        /// 缓存中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private static T LoadAssetFromCache<T>(string assetPath) where T : UnityEngine.Object
        {
            T asset = LocalResourcesManager.S_Instance.LoadAssetFromCache<T>(assetPath);
            if (asset != null)
                return asset;
            asset = AssetBundleManager.S_Instance.LoadAssetFromCache<T>(assetPath);
            return asset;
        }

        /// <summary>
        /// 同步下载资源接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public static UnityEngine.Object LoadAssetSync(string assetPath)
        {
            Object asset = LoadAssetFromCache<Object>(assetPath);
            if (asset != null)
                return asset;


            if (AppSetting.S_IsLoadResourcesAssetPriority)
                return LocalResourcesManager.S_Instance.ResourcesLoadAssetSync<Object>(assetPath);

            return AssetBundleManager.S_Instance.LoadAssetSync<Object>(assetPath);
        }

        /// <summary>
        /// 同步下载资源接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public static T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object
        {
            T asset = LoadAssetFromCache<T>(assetPath);
            if (asset != null)
                return asset;


            if (AppSetting.S_IsLoadResourcesAssetPriority)
                return LocalResourcesManager.S_Instance.ResourcesLoadAssetSync<T>(assetPath);
            return AssetBundleManager.S_Instance.LoadAssetSync<T>(assetPath);
        }

        /// <summary>
        /// 异步下载资源的接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <param name="loadCallback"></param>
        public static void LoadAssetAsync(string assetPath, System.Action<UnityEngine.Object> loadCallback)
        {
            Object asset = LoadAssetFromCache<Object>(assetPath);
            if (asset != null)
            {
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }

            if (AppSetting.S_IsLoadResourcesAssetPriority)
                LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetPath, loadCallback, null);
            else
                AssetBundleManager.S_Instance.LoadAssetAsync(assetPath, loadCallback);
        }
        #endregion



        #region 辅助接口
        /// <summary>
        /// 释放某个资源的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        public static void ReleaseReference<T>(T asset) where T : UnityEngine.Object
        {
            if (LocalResourcesManager.S_Instance.ReleaseReference<T>(asset))
                return;

            AssetBundleManager.S_Instance.ReleaseReference<T>(asset);
        }

        /// <summary>
        /// 在使用ResoucesMgr 后改变图片使用这个接口
        /// </summary>
        /// <param name="image"></param>
        /// <param name="sp"></param>
        public static void SetSprite(UnityEngine.UI.Image image, Sprite sp)
        {
            if (image == null)
            {
                Debug.LogError("SetSprite Fail,Parameter image is null");
                return;
            }
            if (image.sprite != null)
                ReleaseReference<Sprite>(image.sprite);
            image.sprite = sp;
        }
        #endregion

    }
}