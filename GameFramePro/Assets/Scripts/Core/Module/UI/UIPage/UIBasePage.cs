using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.UI
{
    //UI 界面类型
    public enum UIPageTypeEnum
    {
        None = 0, //未知的初始状态 不可用
        ChangePage, //页面
        PopWindow, //弹窗
        Widget, //组件 没有自己的声明周期，只被父组件管理 不被 UIPageManager 记录
    }

    //页面的状态
    public enum UIPageStateEnum
    {
        None = -1, //初始状态
        Initialed = 0, //初始化
        Showing,
        Hide = 10,
        Destroyed, //已经被隐藏
    }

    /// <summary>
    /// UI界面的基类
    /// </summary>
    public class UIBasePage
    {
        /// <summary>/// 标示是否在关联的预制体销毁时候释放 UIBasePage 的内存。现在的页面类型UI 都应该是false,使得能够回退到上一个界面/// </summary>
        public bool IsReleaseOnDestroyPageInstance { get; protected set; } = true;

        public bool mIsActivite
        {
            get { return ConnectGameObjectInstance == null ? false : ConnectGameObjectInstance.IsActivity; }
        }

        //标示是否正确的引用着预制体实例
        public bool IsPrefabInstanceEnable
        {
            get { return ConnectGameObjectInstance != null && ConnectGameObjectInstance.IsReferenceAssetEnable; }
        }

        #region UI 界面相关数据

        public string PageName { get; protected set; }
        public UIPageStateEnum mUIPageState { get; protected set; } = UIPageStateEnum.None; //页面的状态
        public UIPageTypeEnum mUIPageTypeEnum { get; protected set; } = UIPageTypeEnum.None; //界面类型 必须正确设置
        public BaseBeReferenceGameObjectInformation ConnectGameObjectInstance { get; protected set; } //关联的预制体实例

        public List<UIBaseWidget> mAllContainWidgets; //所有关联属于这个页面的组件，关闭的时候会一起被关闭 可能包含多个同名组件

        #endregion


        #region 辅助功能扩展功能（协程+UI组件引用+延迟销毁+音效控制组件）

        private readonly List<CoroutineEx> mAllRuningCoroutine = new List<CoroutineEx>();
        public float MaxAliveAfterInActivte { get; protected set; } = 0f; //当值等于0时候表示立刻销毁 ;小于0表示长存，其他值标示隐藏后的秒数
        public float RecordInvisibleRealTime { get; protected set; } = 0f; //不可见时候的时间

        protected readonly UGUIComponent mUGUIComponent = new UGUIComponent(); //用于查找获取预制体上的组件
        protected AudioController mAudioController { get; } = new AudioController(); //音效组件

        #endregion

        #region 构造函数和初始化 和 IDisposable 接口

        public UIBasePage()
        {
            mUIPageState = UIPageStateEnum.Initialed;
        }

        protected virtual void BaseUIPageInitialed(string pageName, UIPageTypeEnum pageType, BaseBeReferenceGameObjectInformation baseBeReferenceInstance)
        {
            PageName = pageName;
            mUIPageTypeEnum = pageType;
            ConnectGameObjectInstance = baseBeReferenceInstance;
            mUIPageState = UIPageStateEnum.Initialed;

            UGUIComponentReference uguiComponentReference = ConnectGameObjectInstance.GetComponent<UGUIComponentReference>();
            if (uguiComponentReference != null)
                mUGUIComponent.InitailedComponentReference(PageName, ConnectGameObjectInstance, uguiComponentReference);
            else
                mUGUIComponent.InitailedComponentReference(PageName, ConnectGameObjectInstance, null);
        }

        #endregion

        #region UI界面外部控制接口

        /// <summary>
        /// 被实例化创建后调用 除非被销毁否则只会调用一次
        /// </summary>
        public void InstantiatePage()
        {
            OnInitialed();
        }

        /// <summary>
        /// 只能由 UIPageManager 调用
        /// </summary>
        /// <param name="parameter"></param>
        public void ShowPage()
        {
            if (IsPrefabInstanceEnable == false)
            {
                Debug.LogError(string.Format("ShowPage Page {0} 关联的预制体为null", PageName));
                return;
            }

            switch (mUIPageState)
            {
                case UIPageStateEnum.None:
                    Debug.LogError("ShowPage Fail,没有初始化 {0}", PageName);
                    break;
                case UIPageStateEnum.Initialed:
                case UIPageStateEnum.Hide:
                    OnBeforeVisible();
                    ConnectGameObjectInstance.SetActive(true);
                    mUIPageState = UIPageStateEnum.Showing;
                    UIPageManagerUtility.S_Instance.UnRegisterUIBasePageInvisible(this);
                    OnAfterVisible();
                    break;
                case UIPageStateEnum.Showing:
                    Debug.LogError("ShowPage Fail,正在显示中  {0}", PageName);
                    break;
                case UIPageStateEnum.Destroyed:
                    Debug.LogError("ShowPage Fail,已经被销毁了 {0}", PageName);
                    break;
                default:
                    Debug.LogError("未知的状态 " + mUIPageState);
                    break;
            }
        }

        /// <summary>
        /// 关闭界面 
        /// </summary>
        /// <param name="isForceDestroyed">=true 时候会立刻销毁自身释放引用，false 时候则会等待一段时间后被回收</param>
        public void HidePage(bool isForceDestroyed)
        {
            if (IsPrefabInstanceEnable == false)
            {
                Debug.LogError(string.Format("HidePage Page {0} 关联的预制体为null", PageName));
                return;
            }

            switch (mUIPageState)
            {
                case UIPageStateEnum.None:
                    Debug.LogError("ShowPage Fail,没有初始化 {0}", PageName);
                    break;
                case UIPageStateEnum.Initialed:
                    Debug.LogError("ShowPage Fail,没有被显示过 {0}", PageName);
                    break;
                case UIPageStateEnum.Showing:
                    OnBeforeInVisible();
                    ConnectGameObjectInstance.SetActive(false);
                    mUIPageState = UIPageStateEnum.Hide;
                    OnAfterInVisible(isForceDestroyed);
                    break;
                case UIPageStateEnum.Hide:
                    Debug.LogError("ShowPage Fail,已经隐藏了 {0} ", PageName);
                    break;
                case UIPageStateEnum.Destroyed:
                    Debug.LogError("ShowPage Fail,已经被销毁了 {0}", PageName);
                    break;
                default:
                    Debug.LogError("未知的状态 " + mUIPageState);
                    break;
            }
        }

        /// <summary>
        /// 当UIBasePage 关联的对象不存在时候，恢复到初始的状态 进行数据重置然后重新关联新创建的预制体实例
        /// </summary>
        public void ResetPageForReConnectPageInstance()
        {
            if (IsPrefabInstanceEnable)
            {
                Debug.LogError("ResetPage 只能在关联的预制体对象不存在时候调用");
                return;
            }

            OnBeforeDestroyed();
            OnAfterDestroyed();
            mUIPageState = UIPageStateEnum.None;
        }


        /// <summary>
        ///  释放引用的对象和UIbasePage 对象
        /// </summary>
        public void DestroyAndRelease()
        {
            OnBeforeDestroyed();
            ConnectGameObjectInstance.ReduceReference();
            mUIPageState = UIPageStateEnum.Destroyed;
            OnAfterDestroyed();
        }

        #endregion


        #region UI 界面内部实现接口

        /// <summary>
        /// 每次界面关联的的预制体被创建时候调用
        /// </summary>
        protected virtual void OnInitialed()
        {
        }

        /// <summary>
        /// 调用SetActive(true )之前调用这个
        /// </summary>
        protected virtual void OnBeforeVisible()
        {
        }

        /// <summary>
        /// 调用SetActive(true )之后调用这个
        /// </summary>
        protected virtual void OnAfterVisible()
        {
        }

        /// <summary>
        /// 调用SetActive(false )之前调用这个
        /// </summary>
        protected virtual void OnBeforeInVisible()
        {
            if (mAudioController != null)
                mAudioController.StopAllAudios(false); //背景音保留
            if (mAllContainWidgets != null && mAllContainWidgets.Count != 0)
            {
                foreach (var widget in mAllContainWidgets)
                {
                    if (widget == null) continue;
                    if (widget.mIsActivite == false) continue;
                    if (widget.mParentUIPage != this)
                    {
#if UNITY_EDITOR
                        Debug.LogError("关闭ui 发现组件{0} 的所属父节点不是自身{1}，而是{2},是否是直接修改了组件的父级", widget.PageName, PageName, widget.mParentUIPage.PageName);
#endif
                        continue;
                    }

                    widget.HidePage(false); //这里需要传入参数 标示是否是由父节点关闭自身的
                }
            }
        }

        /// <summary>
        /// 调用SetActive(false )之后调用这个
        /// </summary>
        protected virtual void OnAfterInVisible(bool isForceDestroyed)
        {
            if (isForceDestroyed)
            {
                DestroyAndRelease();
            }
            else
            {
                RecordInvisibleRealTime = AppManager.S_Instance.CurrentRealTime; //记录不可见的时间
                UIPageManagerUtility.S_Instance.RegisterUIBasePageInvisible(this);
            }
        }

        /// <summary>
        /// 在被销毁前时候释放自身
        /// </summary>
        protected virtual void OnBeforeDestroyed()
        {
            //页面没有销毁UIBasePage 对象，其他的需要销毁
            mUGUIComponent.ReleaseReference(IsReleaseOnDestroyPageInstance);
            if (mAllContainWidgets != null)
            {
                foreach (var widget in mAllContainWidgets)
                {
                    if (widget == null) continue;
                    if (widget.mParentUIPage != this) continue;
                    widget.DestroyAndRelease();
                }

                mAllContainWidgets.Clear();
            }

            if (mAllRuningCoroutine.Count != 0)
            {
                StopAllCoroutine();
                mAllRuningCoroutine.Clear();
            }
        }

        /// <summary>
        /// 在被销毁后执行
        /// </summary>
        protected virtual void OnAfterDestroyed()
        {
        }

        #endregion

        #region 子组件

        /// <summary>
        /// 由于考虑到有多个同名的  UIBaseWidget 这里不会检测是否存在
        /// </summary>
        /// <param name="widgetName"></param>
        /// <param name="widget"></param>
        public virtual void AddWidget(string widgetName, UIBaseWidget widget)
        {
            if (mAllContainWidgets == null)
                mAllContainWidgets = new List<UIBaseWidget>();
            widget.SetWidgetParent(this);
            mAllContainWidgets.Add(widget);

            //UIBaseWidget recordWidget = null;
            //if (mAllContainWidgets.TryGetValue(widgetName, out recordWidget))
            //{
            //    if (recordWidget == null)
            //    {
            //        recordWidget = widget;
            //        return;
            //    }
            //    if (recordWidget.ConnectPageInstance == null)
            //    {
            //        UIPageManager.HideWidget(recordWidget.PageName, recordWidget);
            //        Debug.LogError("AddWidget 组件{0} 的记录中关联的预制体已经销毁，重新关联", widgetName);
            //        recordWidget = widget;
            //        return;
            //    }

            //    if (recordWidget != widget)
            //    {
            //        Debug.LogError("AddWidget Fail,Exit Widget of Name {0} is {1},new widget is {2}", widgetName, recordWidget.PageName, widget.PageName);
            //        recordWidget.SetWidgetParent(null);
            //        UIPageManager.HideWidget(recordWidget.PageName, recordWidget);
            //        recordWidget = widget;
            //    }
            //}
            //else
            //{
            //    mAllContainWidgets[widgetName] = widget;
            //}
        }

        public virtual void RemoveWidget(UIBaseWidget widget)
        {
            if (mAllContainWidgets == null)
                return;
            mAllContainWidgets.Remove(widget);

            //UIBaseWidget widget = null;
            //if (mAllContainWidgets.TryGetValue(widgetName, out widget))
            //{
            //    widget.SetWidgetParent(null);
            //    mAllContainWidgets.Remove(widgetName);
            //}
        }

        #endregion


        #region 辅助功能接口

        #region 根据对象名或者路径获取对象

        protected T GetComponentByName<T>(string gameObjectName) where T : Component
        {
            if (mUGUIComponent != null)
                return mUGUIComponent.GetComponentByName<T>(gameObjectName);
            Debug.LogError("没有正确的初始化UGUI 组件索引系统");
            return null;
        }

        protected T GetComponentByPath<T>(string gameObjectName, string path) where T : Component
        {
            if (mUGUIComponent != null)
                return mUGUIComponent.GetComponentByPath<T>(gameObjectName, path);
            Debug.LogError("没有正确的初始化UGUI 组件索引系统");
            return null;
        }

        protected GameObject GetGameObjectByName(string gameObjectName)
        {
            Component target = GetComponentByName<Component>(gameObjectName);
            if (target != null)
                return target.gameObject;
            Debug.LogError("没有正确的初始化UGUI 组件索引系统");
            return null;
        }

        protected GameObject GetComponentByPath(string gameObjectName, string path)
        {
            Component target = GetComponentByPath<Component>(gameObjectName, path);
            if (target != null)
                return target.gameObject;
            Debug.LogError("没有正确的初始化UGUI 组件索引系统");
            return null;
        }

        #endregion

        #region 协程相关

        protected virtual void StartCoroutine(IEnumerator routine)
        {
            mAllRuningCoroutine.Add(AsyncManager.StartCoroutineEx(routine));
        }

        protected virtual void StopCoroutine(CoroutineEx routine)
        {
            mAllRuningCoroutine.Remove(routine);
            AsyncManager.StopCoroutineEx(routine);
        }

        protected virtual void StopAllCoroutine()
        {
            foreach (var routine in mAllRuningCoroutine)
            {
                if (routine != null && routine.CoroutineState == CoroutineExStateEnum.Running)
                    AsyncManager.StopCoroutineEx(routine);
            }

            mAllRuningCoroutine.Clear();
        }

        #endregion

        #endregion
    }
}
