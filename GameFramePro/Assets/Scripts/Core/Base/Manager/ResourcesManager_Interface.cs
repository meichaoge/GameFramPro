using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 这里只有ResourcesManger 提供的对外访问接口和实现
    /// </summary>
    public partial class ResourcesManager : Singleton_Static<ResourcesManager>
    {

        #region  实例化对象 (这里对内部的GameObject.Instantiate<T> 做了一层封装，主要是想后期能够监控对象的创建TODO)
        // 实例化一个对象
        public GameObject Instantiate(string goName)
        {
            return new GameObject(goName);
        }

        // 实例化一个对象
        public T Instantiate<T>(T original) where T : Object
        {
            return GameObject.Instantiate<T>(original, Vector3.zero, Quaternion.identity);
        }

        // 实例化一个对象
        public T Instantiate<T>(T original, Transform parent) where T : Object
        {
            return Instantiate<T>(original, parent, true);
        }

        // 实例化一个对象
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            return Instantiate<T>(original, position, rotation, null);
        }

        // 实例化一个对象
        public T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            return GameObject.Instantiate<T>(original, position, rotation, parent);
        }

        // 实例化一个对象
        public T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            return GameObject.Instantiate<T>(original, parent, worldPositionStays);
        }
        #endregion

        #region  记录不会销毁的对象
        /// <summary>
        /// 记录不会被销毁的对象
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public bool MarkNotDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail, Parameter is Null");
                return false;
            }

            if (AppSetting.Instance.IsTraceResourceCreate)
            {
                if (m_AllNotDestroyOnLoadObjects.Contains(go))
                {
                    Debug.LogError("MarkNotDestroyOnLoad Fail,Already Record " + go.name);
                    return false;
                }
                m_AllNotDestroyOnLoadObjects.Add(go);
            }
          
            GameObject.DontDestroyOnLoad(go);
            return true;
        }
        #endregion
    }
}