using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    /// <summary>
    /// 特指UGUI  组件
    /// </summary>
    public class UGUIComponent : IUIComponent
    {
        private Dictionary<string, GameObject> mAllUIPageInstanceChildNodes = new Dictionary<string, GameObject>();  //缓存所有获取过的UI节点


        private Dictionary<string, Component> mAllReferenceComponent = new Dictionary<string, Component>(); //所有脚本中引用的组件
        private Dictionary<string, string> mNamePathMapInfor;

        private Transform mConnectTrans = null;
        public Transform ConnectTrans
        {
            get
            {
                if (mConnectTrans == null)
                {
                    UIBasePage page = UIPageManager.TryGetUIPageFromCache(mConnectUIBasePageName);
                    if (page != null)
                        mConnectTrans = page.ConnectPageInstance.transform;
                }
                return mConnectTrans;
            }
        }



        private UGUIComponentReference mUGUIComponentReference;
        private string mConnectUIBasePageName;
        public void InitailedComponentReference(string uiBasePageName, UGUIComponentReference uguiComponenentReference)
        {
            mUGUIComponentReference = uguiComponenentReference;
            mConnectUIBasePageName = uiBasePageName;
        }

        #region 接口实现

        public void AddButtonListenner(string buttonName, Delegate click)
        {
            throw new NotImplementedException();
        }

        public void RemoveButtonListenner(string buttonName)
        {
            throw new NotImplementedException();
        }

        public void SetSprite(string imageName, Sprite sp)
        {
            throw new NotImplementedException();
        }

        public void SetText(string textName, string textStr)
        {
            throw new NotImplementedException();
        }

        public T GetComponentByName<T>(string gameObjectName) where T : Component
        {
            T component = null;
            if (mUGUIComponentReference != null)
                component = mUGUIComponentReference.GetComponentByName<T>(gameObjectName);
            if (component != null)
                return component; //先看序列化的数据
            component = GetCacheComponentByName<T>(gameObjectName);
            if (component != null)
                return component; //在看缓存的数据

            //最后找到映射的路径查找引用
            if (mNamePathMapInfor == null)
                mNamePathMapInfor = ComponentUtility.GetGameObjectPathByName(mConnectUIBasePageName);
            if (mNamePathMapInfor == null)
            {
                mNamePathMapInfor = ComponentUtility.GetGameObjectNamePathMap(ConnectTrans.gameObject);
            }

            if (mNamePathMapInfor == null)
            {
                Debug.LogError("获取UI界面节点路径映射失败");
                return null;
            }
            string path = string.Empty;
            if (mNamePathMapInfor.TryGetValue(gameObjectName, out path))
                return      FindComponentByPath<T>(gameObjectName, path);
            Debug.LogError("没有找到{0} 上节点{0} 的路径名称映射", mConnectUIBasePageName, gameObjectName);
            return null;
        }

        public T GetComponentByPath<T>(string gameObjectName,string path) where T : Component
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("获取UI界面节点路径映射失败,参数为null");
                return null;
            }

            T result = GetCacheComponentByName<T>(gameObjectName);
            if (result != null)
                return result;

            return FindComponentByPath<T>(gameObjectName, path);

        }



        #endregion


        #region 辅助
        public T FindComponentByPath<T>(string gameObjectName, string path) where T : Component
        {
            Transform trans = ConnectTrans.Find(path);
            if (trans == null)
            {
                Debug.LogError("获取UI界面节点路径映射失败,指定路径不存在子节点 " + path);
                return null;
            }
            T result = trans.GetComponent<T>();

            return result;
        }

        /// <summary>
        /// 获取缓存的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObjectName"></param>
        /// <returns></returns>
        private T GetCacheComponentByName<T>(string gameObjectName) where T : Component
        {
            Component component = null;
            if (mAllReferenceComponent.TryGetValue(gameObjectName, out component))
            {
                if (component is T)
                    return component as T;
            }
            return null;
        }
        //public GameObject FindChildGameObjectByName(string name)
        //{

            //}

            #endregion



    }


}