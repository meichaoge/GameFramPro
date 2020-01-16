using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using GameFramePro.Localization;

namespace GameFramePro.UI
{
    /// <summary>/// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找/// </summary>
    public partial class UGUIComponentReference : MonoBehaviour
    {
        [System.Serializable]
        public partial class UGUIComponentSerializationInfor
        {
            public Component mReferenceComponent = null;
        }

        [Header("序列化当前预制体中包含的组件，用于脚本访问")]
        [SerializeField]
        private List<UGUIComponentSerializationInfor> mSerilizeUGUIComponents = new List<UGUIComponentSerializationInfor>();
        private Dictionary<string, Component> mAllSerilizeUGUIComponentMap = new Dictionary<string, Component>();


        [Header("序列化需要本地化翻译的组件")]
        [SerializeField]
        [ReadOnly]
        private List<LocalizationKeyComponent> mAllLocalizationKeyComponents = new List<LocalizationKeyComponent>();

        public List<LocalizationKeyComponent> AllLocalizationKeyComponents { get { return mAllLocalizationKeyComponents; } }

        /// <summary>/// 根据组件名获取对应的组件/// </summary>
        public T GetComponentByName<T>(string gameObjectName) where T : Component
        {
            if (mAllSerilizeUGUIComponentMap.TryGetValue(gameObjectName, out var component))
            {
                if (component is T)
                    return component as T;
            }

            return null;
        }


        private void Awake()
        {
            TryMapUGUIComponent();
        }

        /// <summary>/// 映射序列化的UGUI 组件/// </summary>
        private bool TryMapUGUIComponent()
        {
            mAllSerilizeUGUIComponentMap.Clear();
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == false)
                {
                    Debug.LogError($"TryMapUGUIComponent 包含无效的引用  {gameObject.name}，请去除！");
                    return false;
                }

                if (mAllSerilizeUGUIComponentMap.ContainsKey(item.mReferenceComponent.name))
                {
#if UNITY_EDITOR
                    Debug.LogError($"TryMapUGUIComponent 包含重复名称 {item.mReferenceComponent.name} 的引用，请去除！");
#endif
                    return false;
                }

                mAllSerilizeUGUIComponentMap[item.mReferenceComponent.name] = item.mReferenceComponent;
            }

            return true;
        }




    }
}
