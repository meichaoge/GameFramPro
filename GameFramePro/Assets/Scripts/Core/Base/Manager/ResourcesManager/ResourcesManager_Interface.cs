using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 这里只有ResourcesManger 提供的对外访问接口和实现
    /// </summary>
    public partial class ResourcesManager : Single<ResourcesManager>
    {
        #region 对象的创建和销毁逻辑（实例化对象 (这里对内部的GameObject.Instantiate<T> 做了一层封装，主要是想后期能够监控对象的创建TODO)）

        // 实例化一个对象
        public static GameObject Instantiate(string goName)
        {
            GameObject go = new GameObject(goName);
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
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

            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.NotDestroyGameObject);

            GameObject.DontDestroyOnLoad(go);
            return true;
        }

        public static void Destroy(Object obj)
        {
            ResourcesTracker.UnRegistTraceResources(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(Object obj)
        {
            ResourcesTracker.UnRegistTraceResources(obj);
            GameObject.DestroyImmediate(obj);
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original) where T : Object
        {
            T go = GameObject.Instantiate<T>(original, Vector3.zero, Quaternion.identity);
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            T go = Instantiate<T>(original, parent, true);
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            T go = Instantiate<T>(original, position, rotation, null);
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
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
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
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
            ResourcesTracker.RegistTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }


        #endregion

        #region 资源加载接口实现
        ///// <summary>
        ///// 加载项目资源的接口，屏蔽内部Resources/AssetBundle 加载，隐藏内部同步/异步加载逻辑
        ///// </summary>
        ///// <param name="assetpath"></param>
        ///// <param name="loadCallback"></param>
        //private void LoadResourcesAsset(string assetpath, System.Action<UnityEngine.Object> loadCallback)
        //{
        //    if (string.IsNullOrEmpty(assetpath))
        //    {
        //        Debug.LogError("LoadResourcesAsset Fail,parameter assetpath is null");
        //        if (loadCallback != null) loadCallback.Invoke(null);
        //        return;
        //    }
        //}
        #endregion

      

    }
}