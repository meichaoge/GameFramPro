using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.UI
{
    /// <summary>/// UI 的管理器，负责UI的创建和界面的显示隐藏/// </summary>
    public static class UIPageManager
    {
        //****UI 页面的属性
        private static Transform mUIChangePage;
        private static Transform mUIPopWindow;
        public static Camera S_UICamera { get; private set; }

        public static void InitialedPageManager()
        {
            S_UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIChangePage = GameObject.Find("UIChangePage").transform;
            mUIPopWindow = GameObject.Find("UIPopWindow").transform;
            if (S_UICamera == null)
                Debug.LogError("UI 相机初始化失败 确认是否指定了名为 UICamera 的UI相机");
        }


        #region UIBaseChangePage 页面的接口  (单独缓存创建的页面实例)

        public static UIBaseChangePage CurUIBaseChangePage { get; private set; } = null; //当前正在显示的页面 可能为null
        private static Stack<string> s_ChangePageRecord = new Stack<string>(); // 记录打开页面的顺序，处理向上返回时候有用  key=页面名称
        private static Dictionary<string, UIBaseChangePage> s_AllAliveUIChangePages = new Dictionary<string, UIBaseChangePage>(); //记录所有现在还在内存中的页面

        /// <summary>/// 切换到 另一个界面/// </summary>
        public static T OpenChangePage<T>(string pageName, string pagePath) where T : UIBaseChangePage, new()
        {
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("OpenChangePage Fail,Parameter is null");
                return null;
            }

            if (CurUIBaseChangePage != null && (pageName == CurUIBaseChangePage.PageName))
            {
                Debug.LogError("页面无法切换到自身 " + pageName);
                return null;
            } //自己切换到自己

            T targetPage = TryGetUIChangePageFromCache(pageName) as T; //首先检测缓存中的数据
            //创建界面
            if (targetPage == null || targetPage.IsPrefabInstanceEnable == false)
            {
                CreateUIPageInstance(pageName, pagePath, mUIChangePage, (goInstance) =>
                {
                    if (goInstance != null)
                    {
                        if (targetPage == null)
                            targetPage = new T();
                        else
                            targetPage.ResetPageForReConnectInstance();

                        targetPage.UIChangePageInitialed(pageName, pagePath, UIPageTypeEnum.ChangePage, goInstance);
                        TryCacheUIChangePage(targetPage);
                        targetPage.InstantiatePage(); //初始化

#if UNITY_EDITOR
                        var debugShowScript = goInstance.GetAddComponentEx<Debug_ShowUIPageInfor>();
                        debugShowScript.Initialed(targetPage);
#endif
                    }
                });
            }


            if (targetPage == null || targetPage.IsPrefabInstanceEnable == false)
            {
                Debug.LogError($"创建页面 pageName={pageName} pagePath={pagePath} ");
                return null;
            }


            if (CurUIBaseChangePage != null)
            {
                CurUIBaseChangePage.HidePage(false);
                RecordChangePage(CurUIBaseChangePage.PageName);
            }

            CurUIBaseChangePage = targetPage;
            targetPage.ShowPage();

            return targetPage;
        }

        /// <summary>/// 返回到上一个界面/// </summary>
        public static void BackPage()
        {
            if (s_ChangePageRecord.Count == 0) return;
            string perviousPageName = string.Empty;
            UIBaseChangePage previouPage = null;

            while (true)
            {
                perviousPageName = s_ChangePageRecord.Pop();
                if (string.IsNullOrEmpty(perviousPageName))
                    continue;
                previouPage = TryGetUIChangePageFromCache(perviousPageName);
                if (previouPage == null)
                {
                    Debug.LogError($"无法返回到上一个界面 {perviousPageName} ，没有被记录的页面");
                    return;
                }

                if (previouPage.IsPrefabInstanceEnable == false)
                {
                    CreateUIPageInstance(previouPage.PageName, previouPage.mPagePath, mUIChangePage, (goInstance) =>
                    {
                        if (goInstance != null)
                        {
                            previouPage.ResetPageForReConnectInstance();

                            previouPage.UIChangePageInitialed(previouPage.PageName, previouPage.mPagePath, UIPageTypeEnum.ChangePage, goInstance);
                            previouPage.InstantiatePage(); //初始化

#if UNITY_EDITOR
                            var debugShowScript = goInstance.GetAddComponentEx<Debug_ShowUIPageInfor>();
                            debugShowScript.Initialed(previouPage);
#endif
                        }
                    });
                }


                if (previouPage.IsPrefabInstanceEnable == false)
                    continue; //说明上一个页面加载失败

                if (CurUIBaseChangePage != null)
                {
                    CurUIBaseChangePage.HidePage(false);
                    RecordChangePage(previouPage.PageName);
                }

                CurUIBaseChangePage = previouPage;
                previouPage.ShowPage();
                return;
            }
        }


        /// <summary>/// 这里后期可能会处理成值记录一次/// </summary>
        private static void RecordChangePage(string pageName)
        {
            s_ChangePageRecord.Push(pageName);
        }

        /// <summary>/// 保存创建的页面引用/// </summary>
        private static void TryCacheUIChangePage(UIBaseChangePage page)
        {
            if (page == null)
            {
                Debug.LogError("缓存的页面是空");
                return;
            }

            if (s_AllAliveUIChangePages.TryGetValue(page.PageName, out var cachePage))
            {
                Debug.LogError($"重复 缓存的页面 {page.PageName}");
                return;
            }

            s_AllAliveUIChangePages[page.PageName] = page;
        }

        /// <summary>/// 从缓存中获取一个指定名称的页面/// </summary>
        public static UIBaseChangePage TryGetUIChangePageFromCache(string pageName)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("TryGetUIChangePageFromCache Fail,Parameter is null");
                return null;
            }

            if (s_AllAliveUIChangePages.TryGetValue(pageName, out var baseChangePage))
                return baseChangePage;
            return null;
        }

        #endregion


        #region UIBasePopWindow 弹窗的接口 (单独缓存创建的弹窗实例)

        private static Dictionary<string, List<UIBasePopWindow>> s_AllAcivityPopWIndows = new Dictionary<string, List<UIBasePopWindow>>(10); //所有还在内存的弹窗

        /// <summary>/// 打开弹窗 isCreateInstanceIfIsShowing 标示已经有一个同名的弹窗存在并且现实中是否需要创建一个新的实例 默认不需要  /// </summary> 
        public static T ShowPopWindow<T>(string pageName, string pagePath, bool isBelongCurPage, bool isCreateInstanceIfIsShowing = false) where T : UIBasePopWindow, new()
        {
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("ShowPopWindow Fail,Parameter is null");
                return null;
            }

            T targetPage = null;
            if (isCreateInstanceIfIsShowing)
                targetPage = TryGetUIPopWindowFromCache(pageName, -1) as T; //获取隐藏的弹窗 如果没有则创建
            else
                targetPage = TryGetUIPopWindowFromCache(pageName, 0) as T;


            if (targetPage != null)
            {
                targetPage.ChangeBelongChangePage(isBelongCurPage ? CurUIBaseChangePage : null);
                targetPage.ShowPage();
                return targetPage;
            } //取出打开的界面

            CreateUIPageInstance(pageName, pagePath, mUIPopWindow, (gameObjectReference) =>
            {
                if (gameObjectReference != null)
                {
                    targetPage = new T();
                    targetPage.UIPageInitialed(pageName, targetPage.mUIPageTypeEnum, isBelongCurPage ? CurUIBaseChangePage : null, gameObjectReference);

                    TryCacheUIPopWindow(targetPage);
                    targetPage.InstantiatePage(); //初始化
                    targetPage.ShowPage();
#if UNITY_EDITOR
                    var debugShowScript = gameObjectReference.GetAddComponentEx<Debug_ShowUIPageInfor>();
                    debugShowScript.Initialed(targetPage);
#endif
                }
            });
            return targetPage;
        }

        /// <summary>/// 隐藏一个弹窗 有可能有多个同名的窗口 所以这里不提供关闭弹窗的接口 /// </summary>
        public static void HidePopwindow(string pageName, bool isForceDestroyed = false)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("HidePopWindow Fail, parameter id null");
                return;
            }

            UIBasePopWindow target = TryGetUIPopWindowFromCache(pageName, 1);
            HidePopwindow(target, isForceDestroyed);
        }

        /// <summary>/// 隐藏一个弹窗/// </summary>
        public static void HidePopwindow(UIBasePopWindow popWindow, bool isForceDestroyed = false)
        {
            if (popWindow == null)
            {
                Debug.LogError("HidePopWindow Fail,parameter is null");
                return;
            }

            if (popWindow.mIsActivite == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"HidePopWindow Fail, 弹窗 {popWindow.PageName} 已经关闭了");
#endif
                return;
            }

            popWindow.HidePage(isForceDestroyed);
        }

        /// <summary>/// 关闭所有的弹窗/// </summary>
        public static void HideAllPopWindows()
        {
            foreach (var popWindowInstances in s_AllAcivityPopWIndows.Values)
            {
                if (popWindowInstances == null) continue;
                foreach (var popWindow in popWindowInstances)
                {
                    if (popWindow == null) continue;
                    if (popWindow.mIsActivite == false) continue;
                    popWindow.HidePage(false);
                }

                popWindowInstances.Clear();
            }

            s_AllAcivityPopWIndows.Clear();
        }


        //保存创建的弹窗引用
        public static void TryCacheUIPopWindow(UIBasePopWindow popWindow)
        {
            if (s_AllAcivityPopWIndows.TryGetValue(popWindow.PageName, out var allBasePopWindows) == false)
            {
                allBasePopWindows = new List<UIBasePopWindow>();
                s_AllAcivityPopWIndows[popWindow.PageName] = allBasePopWindows;
            }

            allBasePopWindows.Add(popWindow);
        }

        /// <summary>/// 从缓存中获取一个指定名称的页面/// </summary>
        /// <param name="getPopWindowType">=0 标示不区分是显示还是隐藏的，>0标示选择看见的弹窗 ; <0标示选择不可见得弹窗</param>
        public static UIBasePopWindow TryGetUIPopWindowFromCache(string popWindowName, int getPopWindowType = 0)
        {
            if (string.IsNullOrEmpty(popWindowName))
            {
                Debug.LogError("TryGetUIPopWindowFromCache Fail,Parameter is null");
                return null;
            }

            if (s_AllAcivityPopWIndows.TryGetValue(popWindowName, out var allBasePopWindows) == false)
                return null;

            if (allBasePopWindows == null || allBasePopWindows.Count == 0)
                return null;

            foreach (var popWindow in allBasePopWindows)
            {
                if (getPopWindowType == 0)
                    return popWindow;
                if (getPopWindowType > 0)
                    if (popWindow.mIsActivite)
                        return popWindow;

                if (getPopWindowType < 0)
                    if (popWindow.mIsActivite == false)
                        return popWindow;
            }

            return null;
        }

        /// <summary>/// 只在弹窗被真正销毁时候调用/// </summary>
        public static void RemoveUIPopWindowFromCache(UIBasePopWindow popWindow)
        {
            if (s_AllAcivityPopWIndows.TryGetValue(popWindow.PageName, out var allBasePopWindows))
            {
                if (allBasePopWindows == null || allBasePopWindows.Count == 0) return;

                for (int dIndex = allBasePopWindows.Count - 1; dIndex >= 0; dIndex--)
                {
                    if (allBasePopWindows[dIndex] == popWindow)
                    {
                        allBasePopWindows.RemoveAt(dIndex);
                        return;
                    }
                }
            }
        }

        #endregion


        #region UIBaseWidget 组件接口 不提供关闭组件的接口 由每个父级自己管理

        /// <summary>/// 不会检测是否有缓存 每次都是重新创建 不受UIPageManager 管理生命周期，只受到父级页面管理/// </summary>
        public static T CreateWidgetInstance<T>(string pageName, string pagePath, UIBasePage parentPage, Transform parentTrans) where T : UIBaseWidget, new()
        {
            if (string.IsNullOrEmpty(pageName) || string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("CreateWidgetPage Fail,Parameter is null");
                return null;
            }

            T targetWidget = null;

            CreateUIPageInstance(pageName, pagePath, parentTrans, (gameObjectReference) =>
            {
                if (gameObjectReference != null)
                {
                    targetWidget = new T();

                    targetWidget.UIPageInitialed(pageName, UIPageTypeEnum.Widget, parentPage, gameObjectReference);
                    targetWidget.InstantiatePage(); //初始化

#if UNITY_EDITOR
                    var debugShowScript = gameObjectReference.GetAddComponentEx<Debug_ShowUIPageInfor>();
                    debugShowScript.Initialed(targetWidget);
#endif
                }
            });

            targetWidget.ShowPage();
            return targetWidget;
        }

        #endregion

        #region 辅助工具

        /// <summary>/// 创建页面实例/// </summary>
        private static void CreateUIPageInstance(string pageName, string pagePath, Transform parent, Action<GameObject> afterInitialedInstanceAction)
        {
            GameObject go = ResourcesManager.InstantiateAssetSync(pagePath, parent, false);
            if (go != null)
            {
                go.name = pageName;
            }
            afterInitialedInstanceAction?.Invoke(go);
        }

        #endregion
    }
}