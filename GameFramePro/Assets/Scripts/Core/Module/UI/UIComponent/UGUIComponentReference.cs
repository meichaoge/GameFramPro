using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace GameFramePro.UI
{
#if UNITY_EDITOR
    /// <summary>/// 关联的资源类型 名称也是对应的 Tag/// </summary>
    [System.Serializable]
    public enum UGUIComponentTypeEnum
    {
        Transform,
        RectTransform,
        UGUIText,
        UGUIImage,
        UGUIButton,
        UGUIInputField,
        UGUIDropDown,
    }
#endif

    /// <summary>/// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找/// </summary>
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

        [SerializeField] private List<UGUIComponentSerializationInfor> mSerilizeUGUIComponents = new List<UGUIComponentSerializationInfor>();

        public List<UGUIComponentSerializationInfor> AllSerilizeUGUIComponents
        {
            get { return mSerilizeUGUIComponents; }
        }

        private Dictionary<string, Component> mAllSerilizeUGUIComponentMap = new Dictionary<string, Component>();


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

        #region 编辑器菜单功能

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

        /// <summary>/// 根据指定的需要关联的组件类型 指定组件/// </summary>
        public void GetComponentByTypeDefine()
        {
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == null)
                    continue;

                AttachComponentByType(ref item.mReferenceComponent, ref item.mComponentType, item.mReferenceComponent.transform);
            }
        }

        //根据选择的参数类型获取指定对象的指定组件 第一个参数要是引用类型
        public static void AttachComponentByType(ref Component targetComponent, ref UGUIComponentTypeEnum type, Transform target)
        {
            Component temp = null;
            switch (type)
            {
                case UGUIComponentTypeEnum.Transform:
                    temp = target;
                    break;
                case UGUIComponentTypeEnum.RectTransform:
                    temp = target as  RectTransform;
                    break;
                case UGUIComponentTypeEnum.UGUIText:
                    temp = target.GetComponent<Text>();
                    break;
                case UGUIComponentTypeEnum.UGUIImage:
                    temp = target.GetComponent<Image>();
                    break;
                case UGUIComponentTypeEnum.UGUIButton:
                    temp = target.GetComponent<Button>();
                    break;
                case UGUIComponentTypeEnum.UGUIInputField:
                    temp = target.GetComponent<InputField>();
                    break;
                case UGUIComponentTypeEnum.UGUIDropDown:
                    temp = target.GetComponent<Dropdown>();
                    break;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    break;
            }

            if (temp == null)
            {
                Debug.LogError($"组件 {targetComponent.name} 没有指定类型{type} 类型的组件可以关联 自动关联成 Transform 组件");
                type = UGUIComponentTypeEnum.Transform;
                temp = targetComponent.transform;
            }

            targetComponent = temp;
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

        /// <summary>/// 根据子节点标注的Tag 来自动关联引用/// </summary>
        public void AutoRecordReferenceWithTag()
        {
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
                        AttachComponentByType(ref item.mReferenceComponent,ref item.mComponentType, item.mReferenceComponent.transform);
                        break;
                    }
                }

                if (isExit == false)
                {
                    UGUIComponentSerializationInfor infor = new UGUIComponentSerializationInfor();
                    infor.mComponentType = type;
                    AttachComponentByType(ref infor.mReferenceComponent,ref  infor.mComponentType, trans);
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
                case UGUIComponentTypeEnum.RectTransform:
                    return targetComponent is RectTransform;
                case UGUIComponentTypeEnum.UGUIText:
                    return targetComponent is Text;
                case UGUIComponentTypeEnum.UGUIImage:
                    return targetComponent is Image;
                case UGUIComponentTypeEnum.UGUIButton:
                    return targetComponent is Button;
                case UGUIComponentTypeEnum.UGUIInputField:
                    return targetComponent is InputField;
                case UGUIComponentTypeEnum.UGUIDropDown:
                    return targetComponent is Dropdown;
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


        public static void ShowComponentReferenceByConfig(GameObject go)
        {
            GetComponentReferenceByConfig(go, out var uiDefineStr, out var uiReferenceStr);
            //拷贝到剪贴板
            TextEditor te = new TextEditor();
            te.text = uiDefineStr + System.Environment.NewLine + uiReferenceStr;
            te.SelectAll();
            te.Copy();
            Debug.LogEditorInfor($"自动生成 预制体对象 {go} 的组件引用配置,并且复制到剪切板中... 内容如下:  \n   {te.text}");
        }

        /// <summary>/// 根据给定的对象上的  UGUIComponentReference 组件生成对应的代码/// </summary>
        /// <param name="go"></param>
        /// <param name="uiDefineString">输出得到的 UI 组件名定义</param>
        /// <param name="getReferenceString">输出得到的 获取UI组件的定义</param>
        public static void GetComponentReferenceByConfig(GameObject go, out string uiDefineString, out string getReferenceString)
        {
            uiDefineString = getReferenceString = string.Empty;

            if (go == null)
            {
                Debug.LogError("获取指定对象上UI定义失败 ，指定对象为null");
                return;
            }

            if (go.GetComponentsInChildren<UGUIComponentReference>().Length > 1)
            {
                Debug.LogError($"当前选择的对象{go.name}  有多个 UGUIComponentReference 组件 无法自动生成对应的配置");
                return;
            }

            UGUIComponentReference componentReference = go.GetComponent<UGUIComponentReference>();
            if (componentReference == null)
            {
                Debug.LogError($"当前选择的对象{go.name}  没有 UGUIComponentReference 组件 无法自动生成对应的配置");
                return;
            }

            StringBuilder uiDefineBuilder = new StringBuilder(10);
            StringBuilder uiGetReferenceBuilder = new StringBuilder(10);

            foreach (var serilizeUguiComponent in componentReference.AllSerilizeUGUIComponents)
            {
                string compionentName = serilizeUguiComponent.mReferenceComponent.name;
                string uiDefineName = $"m_{compionentName}";
                switch (serilizeUguiComponent.mComponentType)
                {
                    case UGUIComponentTypeEnum.Transform:
                        uiDefineBuilder.Append($"\t  private  Transform {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<Transform>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.RectTransform:
                        uiDefineBuilder.Append($"\t  private  RectTransform {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<RectTransform>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UGUIText:
                        uiDefineBuilder.Append($"\t  private  Text {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<Text>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UGUIImage:
                        uiDefineBuilder.Append($"\t  private  Image {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<Image>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UGUIButton:
                        uiDefineBuilder.Append($"\t  private  Button {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<Button>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UGUIInputField:
                        uiDefineBuilder.Append($"\t private  InputField {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<InputField>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UGUIDropDown:
                        uiDefineBuilder.Append($"\t private  Dropdown {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<Dropdown>(\"{compionentName}\"); \n \t");
                        break;
                    default:
                        Debug.LogError($"无法生成指定的组件类型{serilizeUguiComponent.mComponentType}  的声明方式");
                        break;
                }
            }


            uiDefineBuilder.Append(System.Environment.NewLine);
            uiGetReferenceBuilder.Append(System.Environment.NewLine);


            uiDefineString = uiDefineBuilder.ToString();
            getReferenceString = uiGetReferenceBuilder.ToString();
        }

        //private void OnValidate()
        //{
        //    CheckIfContainSameNameObject();
        //    GetComponentByTypeDefine();
        //}
#endif

        #endregion
    }
}
