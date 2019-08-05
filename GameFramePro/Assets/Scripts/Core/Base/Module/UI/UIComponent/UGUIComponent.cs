using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameFramePro.UI
{
    /// <summary>
    /// 特指UGUI  组件
    /// </summary>
    public class UGUIComponent : IUIComponent
    {
        private Dictionary<string, Component> mAllReferenceComponent = new Dictionary<string, Component>(); //所有脚本中引用的组件
        private bool mIsInitialed = false; //标示是否初始化了获取了名称和路径的映射

        public Dictionary<string, string> NamePathMapInfor { get; protected set; } //缓存映射关系
    
        public BaseBeReferenceGameObjectInformation ConnectTransBaseBeReference { get; protected set; }
        //{
        //    get
        //    {
        //        if (mConnectTrans == null)
        //        {
        //            UIBasePage page = UIPageManager.TryGetUIPageFromCache(mConnectUIBasePageName);
        //            if (page != null)
        //                mConnectTrans = page.ConnectPageInstance.transform;
        //        }
        //        return mConnectTrans;
        //    }
        //}

        private UGUIComponentReference mUGUIComponentReference;
        private string mConnectUIBasePageName;
        public void InitailedComponentReference(string uiBasePageName, BaseBeReferenceGameObjectInformation connectPageInstance, UGUIComponentReference uguiComponenentReference)
        {
            mUGUIComponentReference = uguiComponenentReference;
            mConnectUIBasePageName = uiBasePageName;
            ConnectTransBaseBeReference = connectPageInstance;
        }

        #region 接口实现


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
            if (mIsInitialed == false)
            {
                NamePathMapInfor = ComponentUtility.GetGameObjectNamePathMap(ConnectTransBaseBeReference); //生成映射
                mIsInitialed = true;
            }

            if (NamePathMapInfor == null)
            {
                Debug.LogError("获取UI界面节点路径映射失败");
                return null;
            }
            string path = string.Empty;
            if (NamePathMapInfor.TryGetValue(gameObjectName, out path))
                return FindComponentByPath<T>(gameObjectName, path);
            Debug.LogError("没有找到{0} 上节点{0} 的路径名称映射", mConnectUIBasePageName, gameObjectName);
            return null;
        }

        public T GetComponentByPath<T>(string gameObjectName, string path) where T : Component
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

        public void ReleaseReference(bool isReleaseNamePathMap)
        {
            ConnectTransBaseBeReference.ReduceReference();
            mAllReferenceComponent.Clear();
            if (isReleaseNamePathMap)
            {
                if (NamePathMapInfor != null)
                    NamePathMapInfor.Clear();
                mIsInitialed = false;
            } //这里可以保留映射表以便于下次查询

            mUGUIComponentReference = null;
        }
        #endregion


        #region 辅助
        public T FindComponentByPath<T>(string gameObjectName, string path) where T : Component
        {
            T result = ConnectTransBaseBeReference.FindChildComponentByPath<T>(path);
            if (result == null)
            {
                Debug.LogError("获取UI界面节点路径映射失败,指定路径不存在子节点 " + path);
                return null;
            }
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


        #endregion



    }


}