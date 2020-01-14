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


#if UNITY_EDITOR


    /// <summary>/// 关联的资源类型 名称也是对应的 Tag/// </summary>
    ///  每次增加需要依次修改 AttachComponentByType/CheckReferenceComponentType/GetComponentReferenceByConfig 实现
    ///  以及UIViewEditor 脚本中 TagObjs 定义
    [System.Serializable]
    public enum UGUIComponentTypeEnum
    {
        GameObject,
        Transform,
        RectTransform,
        UGUIText,
        UGUIImage,
        UGUIButton,
        UGUIInputField,
        UGUIDropDown,
        CanvasGroup,
        UILoopScrollRect_Vertical,  //循环ScrollRect组件
        UILoopScrollRect_Horizontial,  //循环ScrollRect组件
        ScrollRect,  //Unity 自带的 ScrollRect组件

    }


    /// <summary>/// 挂载在预制体上 编辑器配置哪些组件的引用可以序列化，主要使用这个避免每次查找/// </summary>
    public partial class UGUIComponentReference
    {
        public partial class UGUIComponentSerializationInfor
        {
            //辅助获取指定类型的组件
            public UGUIComponentTypeEnum mComponentType = UGUIComponentTypeEnum.Transform;
        }

        [CustomPropertyDrawer(typeof(UGUIComponentSerializationInfor))]
        public class UGUIComponentSerializationInforPropertyDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                EditorGUI.BeginProperty(position, label, property); // 开始绘制属性
                // 获取属性前值 label, 就是显示在此属性前面的那个名称 label
                //   position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                float EditorLableWidth = EditorGUIUtility.labelWidth; //需要先保存这个值 然后还原

                float lableWidth = 5;
                //  var labelRect = new Rect(position.x, position.y, lableWidth, position.height); //显示前面的Lable

                float contentInforWidth = position.width - lableWidth; //内容区间的宽度

                var mReferenceComponentRect = new Rect(position.x + lableWidth, position.y, contentInforWidth * 0.6f, position.height);
                var mComponentTypeRect = new Rect(mReferenceComponentRect.x + mReferenceComponentRect.width, position.y, contentInforWidth * 0.4f, position.height);

                //      EditorGUI.LabelField(labelRect, label);
                EditorGUIUtility.labelWidth = contentInforWidth * 0.2f; //控制每个属性文本区间大小
                EditorGUI.PropertyField(mReferenceComponentRect, property.FindPropertyRelative("mReferenceComponent"), new GUIContent("Component"));
                EditorGUI.PropertyField(mComponentTypeRect, property.FindPropertyRelative("mComponentType"), new GUIContent("ComponentType"));


                EditorGUIUtility.labelWidth = EditorLableWidth;
                EditorGUI.EndProperty(); // 完成绘制
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                return base.GetPropertyHeight(property, label);
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

        /// <summary>/// 根据子节点标注的Tag 来自动关联引用/// </summary>
        public void AutoRecordReferenceWithTag()
        {
            Transform[] allChildTrans = transform.GetComponentsInChildren<Transform>(true);
            foreach (var trans in allChildTrans)
            {
                if (string.IsNullOrEmpty(trans.gameObject.tag) || trans.gameObject.tag == "Untagged") continue;
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
                        AttachComponentByType(ref item.mReferenceComponent, ref item.mComponentType, item.mReferenceComponent.transform);
                        break;
                    }
                }

                if (isExit == false)
                {
                    UGUIComponentSerializationInfor infor = new UGUIComponentSerializationInfor();
                    infor.mComponentType = type;
                    AttachComponentByType(ref infor.mReferenceComponent, ref infor.mComponentType, trans);
                    mSerilizeUGUIComponents.Add(infor);
                }
            }
        }

        /// <summary>/// 检测序列化的组件名是否有重复/// </summary>
        public void CheckIfContainSameNameComponent()
        {
            HashSet<string> allSerializeGameObjectName = new HashSet<string>();
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == false)
                    continue;
                if (allSerializeGameObjectName.Contains(item.mReferenceComponent.name))
                {
                    Debug.LogError($"检测到序列化两个相同名称{item.mReferenceComponent.name} 的对象，会导致无法通过名称索引，请修改 ");
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

        /// <summary>/// 获取所有序列化的组件的脚本定义字符串/// </summary>
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

        /// <summary>/// 设置序列化的组件对象的Tag/// </summary>
        public void AutoSetSerializaComponentTag()
        {
            foreach (var item in mSerilizeUGUIComponents)
            {
                if (item == null || item.mReferenceComponent == null)
                    continue;

                item.mReferenceComponent.gameObject.tag = item.mComponentType.ToString();
            }
        }


        /// <summary>/// 根据选择的参数类型获取指定对象的指定组件 第一个参数要是引用类型/// </summary>
        private static void AttachComponentByType(ref Component targetComponent, ref UGUIComponentTypeEnum type, Transform target)
        {
            Component temp = null;
            switch (type)
            {
                case UGUIComponentTypeEnum.GameObject:
                    temp = target;
                    break;
                case UGUIComponentTypeEnum.Transform:
                    temp = target;
                    break;
                case UGUIComponentTypeEnum.RectTransform:
                    temp = target as RectTransform;
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
                case UGUIComponentTypeEnum.CanvasGroup:
                    temp = target.GetComponent<CanvasGroup>();
                    break;
                case UGUIComponentTypeEnum.UILoopScrollRect_Vertical:
                    temp = target.GetComponent<LoopVerticalScrollRect>();
                    break;
                case UGUIComponentTypeEnum.UILoopScrollRect_Horizontial:
                    temp = target.GetComponent<LoopHorizontalScrollRect>();
                    break;
                case UGUIComponentTypeEnum.ScrollRect:
                    temp = target.GetComponent<ScrollRect>();
                    break;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    break;
            }

            if (temp == null)
            {
                Debug.LogError($"组件 {targetComponent.name} 没有指定类型{type} 类型的组件 target={target.name}.可以关联 自动关联成 Transform 组件");
                type = UGUIComponentTypeEnum.Transform;
                temp = targetComponent.transform;
            }

            targetComponent = temp;
        }

        //根据选的的类型判断引用组件是否正确
        private bool CheckReferenceComponentType(ref Component targetComponent, UGUIComponentTypeEnum type)
        {
            if (targetComponent == null) return true;

            switch (type)
            {
                case UGUIComponentTypeEnum.GameObject:
                    return targetComponent is Transform;
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
                case UGUIComponentTypeEnum.CanvasGroup:
                    return targetComponent is CanvasGroup;
                case UGUIComponentTypeEnum.UILoopScrollRect_Vertical:
                    return targetComponent is LoopVerticalScrollRect;
                case UGUIComponentTypeEnum.UILoopScrollRect_Horizontial:
                    return targetComponent is LoopHorizontalScrollRect;
                case UGUIComponentTypeEnum.ScrollRect:
                    return targetComponent is ScrollRect;
                default:
                    Debug.LogError("没有定义的类型 " + type);
                    return false;
            }
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

            foreach (var serilizeUguiComponent in componentReference.mSerilizeUGUIComponents)
            {
                if (serilizeUguiComponent == null || serilizeUguiComponent.mReferenceComponent == null)
                {
                    Debug.LogError($"引用空对象,标签 {serilizeUguiComponent.mComponentType}");
                    continue;
                }

                string compionentName = serilizeUguiComponent.mReferenceComponent.name;
                string uiDefineName = $"m_{compionentName}";
                switch (serilizeUguiComponent.mComponentType)
                {
                    case UGUIComponentTypeEnum.GameObject:
                        uiDefineBuilder.Append($"\t  private  GameObject {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetGameObjectByName(\"{compionentName}\"); \n \t");
                        break;
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
                    case UGUIComponentTypeEnum.CanvasGroup:
                        uiDefineBuilder.Append($"\t private  CanvasGroup {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<CanvasGroup>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UILoopScrollRect_Vertical:
                        uiDefineBuilder.Append($"\t private  LoopVerticalScrollRect {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<LoopVerticalScrollRect>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.UILoopScrollRect_Horizontial:
                        uiDefineBuilder.Append($"\t private  LoopHorizontalScrollRect {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<LoopHorizontalScrollRect>(\"{compionentName}\"); \n \t");
                        break;
                    case UGUIComponentTypeEnum.ScrollRect:
                        uiDefineBuilder.Append($"\t private  ScrollRect {uiDefineName} ; \n \t");
                        uiGetReferenceBuilder.Append($"\t  {uiDefineName} =GetComponentByName<ScrollRect>(\"{compionentName}\"); \n \t");
                        break;
                    default:
                        Debug.LogError($"无法生成指定的组件类型{serilizeUguiComponent.mComponentType}  的声明方式");
                        break;
                }
            }


            uiDefineBuilder.Append(System.Environment.NewLine);
            //   uiGetReferenceBuilder.Append(System.Environment.NewLine);

            uiDefineString = uiDefineBuilder.ToString();
            getReferenceString = uiGetReferenceBuilder.ToString();
        }
    }


    [CustomEditor(typeof(UGUIComponentReference))]
    public class UGUIComponentReference_Editor : Editor
    {
        private SerializedProperty mSerilizeUGUIComponents;
        private UGUIComponentReference mUGUIComponentReference;

        private void OnEnable()
        {
            mSerilizeUGUIComponents = serializedObject.FindProperty("mSerilizeUGUIComponents");
            mUGUIComponentReference = target as UGUIComponentReference;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginVertical();
            GUILayout.Space(5);

            if (GUILayout.Button("根据ComponentTypeEnum创建Tag", GUILayout.Height(25)))
                mUGUIComponentReference.AddAllUGUIComponentTypeTag();

            if (GUILayout.Button("自动序列化标记Tag 的组件&设置序列化组件对象的Tag", GUILayout.Height(25)))
            {
                mUGUIComponentReference.AutoRecordReferenceWithTag();
                mUGUIComponentReference.AutoSetSerializaComponentTag();
                Undo.RegisterFullObjectHierarchyUndo(mUGUIComponentReference.gameObject, "SerializeTgs");
            }


            if (GUILayout.Button("检测是否有重名的对象", GUILayout.Height(25)))
                mUGUIComponentReference.CheckIfContainSameNameComponent();

            if (GUILayout.Button("获取当前对象的UI定义", GUILayout.Height(25)))
                UGUIComponentReference.ShowComponentReferenceByConfig(mUGUIComponentReference.gameObject);

            GUILayout.Space(5);
            GUILayout.EndVertical();


            if (EditorGUILayout.PropertyField(mSerilizeUGUIComponents))
            {
                EditorGUI.indentLevel++;
                // 设置元素个数
                mSerilizeUGUIComponents.arraySize = EditorGUILayout.IntField("Size", mSerilizeUGUIComponents.arraySize);

                for (int dex = 0; dex < mSerilizeUGUIComponents.arraySize; dex++)
                {
                    var componentItem = mSerilizeUGUIComponents.GetArrayElementAtIndex(dex);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("+"), GUILayout.Width(25)))
                    {
                        mSerilizeUGUIComponents.InsertArrayElementAtIndex(dex + 1);
                        var newComponentItem = mSerilizeUGUIComponents.GetArrayElementAtIndex(dex + 1);
                        newComponentItem.FindPropertyRelative("mReferenceComponent").objectReferenceValue = null;
                        newComponentItem.FindPropertyRelative("mComponentType").intValue = 0;
                        break;
                    }

                    if (GUILayout.Button(new GUIContent("-"), GUILayout.Width(25)))
                    {
                        mSerilizeUGUIComponents.DeleteArrayElementAtIndex(dex);
                        break;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(componentItem, true);
                    mUGUIComponentReference.GetComponentByTypeDefine();
                    EditorGUI.EndChangeCheck();

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;
            serializedObject.ApplyModifiedProperties();
            //       base.OnInspectorGUI();
        }
    }


#endif
}
