using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找
    /// </summary>
    public class UGUIComponentReference : MonoBehaviour
    {
        //TODO 增加编辑器下的按钮操作和检测逻辑

        [SerializeField]
        private List<Component> mAllSerilizeUGUIComponents = new List<Component>();
        public List<Component> AllSerilizeUGUIComponents { get { return mAllSerilizeUGUIComponents; } }
        private Dictionary<string, Component> mAllSerilizeUGUIComponentMap = new Dictionary<string, Component>();


        private void Awake()
        {
            TryMapUGUIComponent();
        }

        /// <summary>
        /// 映射序列化的UGUI 组件
        /// </summary>
        /// <returns></returns>
        private bool TryMapUGUIComponent()
        {
            mAllSerilizeUGUIComponentMap.Clear();
            foreach (var item in mAllSerilizeUGUIComponents)
            {
                if (item == null)
                {
                    Debug.LogError("TryMapUGUIComponent 包含无效的引用，请去除！");
                    return false;
                }

                if (mAllSerilizeUGUIComponentMap.ContainsKey(item.gameObject.name))
                {
#if UNITY_EDITOR
                    Debug.LogError("TryMapUGUIComponent 包含重复名称的引用，请去除！" + item.gameObject.name);
#endif
                    return false;
                }
                mAllSerilizeUGUIComponentMap[item.gameObject.name] = item;
            }
            return true;
        }

        public T GetComponentByName<T>(string gameObjectName) where T : Component
        {
            Component component = null;
            if(mAllSerilizeUGUIComponentMap.TryGetValue(gameObjectName,out component))
            {
                if (component is T)
                    return component as T;
            }
            return null;
        }


    }
}