using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.UI
{
    //UI 界面类型
    public enum UIPageTypeEnum
    {
        None = 0,  //未知的初始状态 不可用
        ChangePage, //页面
        PopWindow, //弹窗
        Widget, //组件 没有自己的声明周期，只被父组件管理 不被 UIPageManager 记录
    }

    //页面的状态
    public enum UIPageStateEnum
    {
        None = -1, //初始状态
        Initialed = 0,//初始化
        Showing,
        Hide = 10,
        Destroyed, //已经被隐藏
    }

    /// <summary>
    /// UI界面的基类
    /// </summary>
    public class UIBasePage
    {

        #region UI 数据
        public UIPageStateEnum mUIPageState { get; protected set; } = UIPageStateEnum.None; //页面的状态
        public UIPageTypeEnum mUIPageTypeEnum { get; protected set; } = UIPageTypeEnum.None;  //界面类型 必须正确设置
        public string PageName { get; protected set; }
        public GameObject ConnectPageInstance { get; protected set; } //关联的预制体实例
        public bool mIsActivite { get { return ConnectPageInstance == null ? false : ConnectPageInstance.activeSelf; } }

        public Dictionary<string, UIBaseWidget> mAllContainWidgets; //所有关联属于这个页面的组件，关闭的时候会一起被关闭

        #endregion


        #region 辅助扩展功能
        protected List<Coroutine> mAllRuningCoroutine = new List<Coroutine>();

        public float MaxAliveAfterInActivte { get; protected set; } = 0f;  //当值等于0时候表示立刻销毁 ;小于0表示长存，其他值标示隐藏后的秒数
        public float RecordInvisibleRealTime { get; protected set; } = 0f; //不可见时候的时间
        #endregion

        #region 构造函数和初始化
        public UIBasePage()
        {
            mUIPageState = UIPageStateEnum.Initialed;
        }

        protected virtual void InitialedBaseUIPage(string pageName, UIPageTypeEnum pageType, GameObject instance)
        {
            PageName = pageName;
            mUIPageTypeEnum = pageType;
            ConnectPageInstance = instance;
            mUIPageState = UIPageStateEnum.Initialed;
        }
        #endregion

        #region UI界面外部控制接口
        /// <summary>
        /// 只能由 UIPageManager 调用
        /// </summary>
        /// <param name="parameter"></param>
        public void ShowPage()
        {
            if (ConnectPageInstance == null)
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
                    ConnectPageInstance.SetActive(true);
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
            if (ConnectPageInstance == null)
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
                    ConnectPageInstance.SetActive(false);
                    mUIPageState = UIPageStateEnum.Hide;
                    OnAfterInVisible();
                    if (isForceDestroyed)
                        DestroyAndRelease();
                    else
                    {
                        RecordInvisibleRealTime = AppManager.S_Instance.CurrentRealTime; //记录不可见的时间
                        UIPageManagerUtility.S_Instance.RegisterUIBasePageInvisible(this);
                    }

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
        ///恢复到初始的状态 不关联其他对象
        /// </summary>
        public void ResetPage()
        {
            OnBeforeDestroyed();
            ConnectPageInstance = null;
            OnAfterDestroyed();
            mUIPageState = UIPageStateEnum.None;
        }


        /// <summary>
        ///  只能由 UIPageManager 调用
        /// </summary>
        public void DestroyAndRelease()
        {
            OnBeforeDestroyed();
            ResourcesManager.Destroy(ConnectPageInstance);
            mUIPageState = UIPageStateEnum.Destroyed;
            OnAfterDestroyed();
        }
        #endregion


        #region UI 界面内部实现接口


        /// <summary>
        /// 调用SetActive(true )之前调用这个
        /// </summary>
        protected virtual void OnBeforeVisible() { }
        /// <summary>
        /// 调用SetActive(true )之后调用这个
        /// </summary>
        protected virtual void OnAfterVisible() { }

        /// <summary>
        /// 调用SetActive(false )之前调用这个
        /// </summary>
        protected virtual void OnBeforeInVisible()
        {
            if (mAllContainWidgets != null && mAllContainWidgets.Count != 0)
            {
                foreach (var widget in mAllContainWidgets.Values)
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
        protected virtual void OnAfterInVisible()
        {

        }

        /// <summary>
        /// 在被销毁前时候释放自身
        /// </summary>
        protected virtual void OnBeforeDestroyed()
        {
            if (mAllContainWidgets != null)
                mAllContainWidgets.Clear();
            if (mAllRuningCoroutine.Count != 0)
            {
                StopAllCoroutines();
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
        public bool IsContainWidget(string widgetName)
        {
            UIBaseWidget recordWidget = null;
            if (mAllContainWidgets.TryGetValue(widgetName, out recordWidget))
            {
                if (recordWidget != null && recordWidget.ConnectPageInstance != null)
                    return true;
                Debug.LogError("包含的子组件 {0} 不存在 {1}或者引用为null ", widgetName, (recordWidget == null));
            }
            return false;
        }

        public virtual void AddWidget(string widgetName, UIBaseWidget widget)
        {
            if (mAllContainWidgets == null)
                mAllContainWidgets = new Dictionary<string, UIBaseWidget>();


            widget.SetWidgetParent(this);

            UIBaseWidget recordWidget = null;
            if (mAllContainWidgets.TryGetValue(widgetName, out recordWidget))
            {
                if (recordWidget == null)
                {
                    recordWidget = widget;
                    return;
                }
                if (recordWidget.ConnectPageInstance == null)
                {
                    UIPageManager.HideWidget(recordWidget.PageName, recordWidget);
                    Debug.LogError("AddWidget 组件{0} 的记录中关联的预制体已经销毁，重新关联", widgetName);
                    recordWidget = widget;
                    return;
                }

                if (recordWidget != widget)
                {
                    Debug.LogError("AddWidget Fail,Exit Widget of Name {0} is {1},new widget is {2}", widgetName, recordWidget.PageName, widget.PageName);
                    recordWidget.SetWidgetParent(null);
                    UIPageManager.HideWidget(recordWidget.PageName, recordWidget);
                    recordWidget = widget;
                }
            }
            else
            {
                mAllContainWidgets[widgetName] = widget;
            }

        }
        public virtual void RemoveWidget(string widgetName)
        {
            if (mAllContainWidgets == null)
                return;

            UIBaseWidget widget = null;
            if (mAllContainWidgets.TryGetValue(widgetName, out widget))
            {
                widget.SetWidgetParent(null);
                mAllContainWidgets.Remove(widgetName);
            }
        }
        #endregion


        #region 辅助功能接口
        protected virtual void StartCoroutine(IEnumerator routine)
        {
            mAllRuningCoroutine.Add(AsyncManager.S_Instance.StartCoroutineEx(routine));
        }

        protected virtual void StopCoroutine(Coroutine routine)
        {
            mAllRuningCoroutine.Remove(routine);
            AsyncManager.S_Instance.StopCoroutineEx(routine);
        }

        protected virtual void StopAllCoroutines()
        {
            foreach (var routine in mAllRuningCoroutine)
                AsyncManager.S_Instance.StopCoroutineEx(routine);
            mAllRuningCoroutine.Clear();
        }
        #endregion

    }

}