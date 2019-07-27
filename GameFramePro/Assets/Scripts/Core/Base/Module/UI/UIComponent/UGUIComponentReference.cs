using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GameFramePro.UI
{


    /// <summary>
    /// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找
    /// </summary>
    public class UGUIComponentReference : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// 关联的资源类型
        /// </summary>
        [System.Serializable]
        public enum UGUIComponentTypeEnum
        {
            Transform,
            Text,
            Image,
            Button,
        }
#endif

        [System.Serializable]
        public class UGUIComponentSerializationInfor
        {
            public Component mReferenceComponent = null;
#if UNITY_EDITOR
            //辅助获取指定类型的组件
            public UGUIComponentTypeEnum mComponentType = UGUIComponentTypeEnum.Transform;
#endif
        }


        //TODO 增加编辑器下的按钮操作和检测逻辑
        [SerializeField]
        private List<UGUIComponentSerializationInfor> mSerilizeUGUIComponents = new List<UGUIComponentSerializationInfor>();

        //[SerializeField]
        //private List<Component> mAllSerilizeUGUIComponents = new List<Component>();
        public List<UGUIComponentSerializationInfor> AllSerilizeUGUIComponents { get { return mSerilizeUGUIComponents; } }
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
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == false)
                {
                    Debug.LogError("TryMapUGUIComponent 包含无效的引用，请去除！");
                    return false;
                }

                if (mAllSerilizeUGUIComponentMap.ContainsKey(item.mReferenceComponent.name))
                {
#if UNITY_EDITOR
                    Debug.LogError("TryMapUGUIComponent 包含重复名称的引用，请去除！" + item.mReferenceComponent.name);
#endif
                    return false;
                }
                mAllSerilizeUGUIComponentMap[item.mReferenceComponent.name] = item.mReferenceComponent;
            }
            return true;
        }

        public T GetComponentByName<T>(string gameObjectName) where T : Component
        {
            Component component = null;
            if (mAllSerilizeUGUIComponentMap.TryGetValue(gameObjectName, out component))
            {
                if (component is T)
                    return component as T;
            }
            return null;
        }




#if UNITY_EDITOR

        //检测对象名重复
        public void CheckIfContainSameNameObject()
        {
            HashSet<string> allSerializeGameObjectName = new HashSet<string>();
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == false)
                    continue;
                if (allSerializeGameObjectName.Contains(item.mReferenceComponent.name))
                {
                    Debug.LogError("检测到序列化两个相同名称的对象，会导致无法通过名称索引，请修改 " + item.mReferenceComponent.name);
                    return;
                }
                allSerializeGameObjectName.Add(item.mReferenceComponent.name);
            }
        }

        /// <summary>
        /// 根据指定的需要关联的组件类型 指定组件
        /// </summary>
        public void GetComponentByTypeDefine()
        {
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == null)
                    continue;

                switch (item.mComponentType)
                {
                    case UGUIComponentTypeEnum.Transform:
                        item.mReferenceComponent = item.mReferenceComponent.transform;
                        break;
                    case UGUIComponentTypeEnum.Text:
                        item.mReferenceComponent = item.mReferenceComponent.transform.GetComponent<Text>();
                        break;
                    case UGUIComponentTypeEnum.Image:
                        item.mReferenceComponent = item.mReferenceComponent.transform.GetComponent<Image>();
                        break;
                    case UGUIComponentTypeEnum.Button:
                        item.mReferenceComponent = item.mReferenceComponent.transform.GetComponent<Button>();
                        break;
                    default:
                        Debug.LogError("没有定义的类型 " + item.mComponentType);
                        break;
                }
            }
        }

        private void OnValidate()
        {
            CheckIfContainSameNameObject();
            GetComponentByTypeDefine();
        }
#endif


    }
}