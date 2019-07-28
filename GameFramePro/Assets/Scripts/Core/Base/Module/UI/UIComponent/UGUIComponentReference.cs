using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace GameFramePro.UI
{
#if UNITY_EDITOR
    /// <summary>
    /// 关联的资源类型 名称也是对应的 Tag
    /// </summary>
    [System.Serializable]
    public enum UGUIComponentTypeEnum
    {
        Transform,
        UGUIText,
        UGUIImage,
        UGUIButton,
    }
#endif

    /// <summary>
    /// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找
    /// </summary>
    public class UGUIComponentReference : MonoBehaviour
    {

        [System.Serializable]
        public class UGUIComponentSerializationInfor
        {
            public Component mReferenceComponent = null;
#if UNITY_EDITOR
            //辅助获取指定类型的组件
            public UGUIComponentTypeEnum mComponentType = UGUIComponentTypeEnum.Transform;
#endif
        }

        [SerializeField]
        private List<UGUIComponentSerializationInfor> mSerilizeUGUIComponents = new List<UGUIComponentSerializationInfor>();
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

                AttachComponentByType(ref item.mReferenceComponent, item.mComponentType, item.mReferenceComponent.transform);
            }
        }

        //根据选择的参数类型获取指定对象的指定组件 第一个参数要是引用类型
        private static void AttachComponentByType(ref Component targetComponent, UGUIComponentTypeEnum type, Transform target)
        {
            switch (type)
            {
                case UGUIComponentTypeEnum.Transform:
                    targetComponent = target;
                    break;
                case UGUIComponentTypeEnum.UGUIText:
                    targetComponent = target.GetComponent<Text>();
                    break;
                case UGUIComponentTypeEnum.UGUIImage:
                    targetComponent = target.GetComponent<Image>();
                    break;
                case UGUIComponentTypeEnum.UGUIButton:
                    targetComponent = target.GetComponent<Button>();
                    break;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    break;
            }
        }


        //枚举类型转成对应的tag
        public void AddAllUGUIComponentTypeTag()
        {
            List<UGUIComponentTypeEnum> allUGUIComponentEnum = EnumUtility.GetEnumValue<UGUIComponentTypeEnum>();
            foreach (var item in allUGUIComponentEnum)
            {
                LayerAndTagManager.AddTag(item.ToString());
            }
        }

        /// <summary>
        /// 根据子节点标注的Tag 来自动关联引用
        /// </summary>
        public void AutoRecordReferenceWithTag()
        {
            //    targetGo = UnityEditor.PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
            Transform[] allChildTrans = transform.GetComponentsInChildren<Transform>(true);
            foreach (var trans in allChildTrans)
            {
                if (string.IsNullOrEmpty(trans.gameObject.tag)) continue;
                UGUIComponentTypeEnum type;
                if (System.Enum.TryParse(trans.gameObject.tag, out type) == false)
                    continue;

                bool isExit = false;
                foreach (var item in mSerilizeUGUIComponents)
                {
                    if (item == null) continue;
                    if (item.mReferenceComponent == null) continue;
                    if (item.mReferenceComponent.gameObject.transform == trans)
                    {
                        isExit = true;
                        item.mComponentType = type;
                        AttachComponentByType(ref item.mReferenceComponent, item.mComponentType, item.mReferenceComponent.transform);
                        break;
                    }
                }

                if (isExit == false)
                {
                    UGUIComponentSerializationInfor infor = new UGUIComponentSerializationInfor();
                    infor.mComponentType = type;
                    AttachComponentByType(ref infor.mReferenceComponent, infor.mComponentType, trans);
                    mSerilizeUGUIComponents.Add(infor);
                }
            }
        }

        //根据选的的类型判断引用组件是否正确
        private bool CheckReferenceComponentType(ref Component targetComponent, UGUIComponentTypeEnum type)
        {
            if (targetComponent == null) return true;

            switch (type)
            {
                case UGUIComponentTypeEnum.Transform:
                    return targetComponent is Transform;
                case UGUIComponentTypeEnum.UGUIText:
                    return targetComponent is Text;
                case UGUIComponentTypeEnum.UGUIImage:
                    return targetComponent is Image;
                case UGUIComponentTypeEnum.UGUIButton:
                    return targetComponent is Button;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    return false;
            }
        }

        //判断引用组件和类型是佛正确
        public void CheckSerializationComponentType()
        {
            for (int dex = 0; dex < mSerilizeUGUIComponents.Count; dex++)
            {
                var item = mSerilizeUGUIComponents[dex];
                if (CheckReferenceComponentType(ref item.mReferenceComponent, item.mComponentType) == false)
                    Debug.LogError("索引{0} 引用组件 {1} 不是指定的类型", dex, item.mReferenceComponent.gameObject.name);
            }

        }


        //private void OnValidate()
        //{
        //    CheckIfContainSameNameObject();
        //    GetComponentByTypeDefine();
        //}
#endif


    }
}