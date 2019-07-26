using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.UI
{
    /// <summary>
    /// UI 的管理器
    /// </summary>
    public static class UIPageManager
    {
        #region UI 页面父节点
        private static Camera s_UICamera;
        private static Transform mUIChangePage;
        private static Transform mUIPopWindow;

        #endregion


        public static void InitialedPageManager()
        {
            s_UICamera = GameObject.Find("UICamera").GetComponent<Camera>();
            mUIChangePage = GameObject.Find("UIChangePage").transform;
            mUIPopWindow = GameObject.Find("UIPopWindow").transform;
        }


        #region 记录页面的数据
        //key=UI 界面名称
        private static Dictionary<string, UIBasePage> s_AllAliveUIPages = new Dictionary<string, UIBasePage>(); //记录所有现在还在内存中的界面

        #endregion


        #region UIBaseChangePage 页面的接口
        private static Stack<string> s_ChangePageRecord = new Stack<string>();// 记录打开页面的顺序，处理向上返回时候有用  key=页面名称
        public static UIBaseChangePage CurUIBaseChangePage { get; private set; } = null; //当前正在显示的页面 可能为null

        /// <summary>
        /// 切换到 另一个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageName"></param>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public static T OpenChangePage<T>(string pageName, string pagePath) where T : UIBaseChangePage, new()
        {
            if (string.IsNullOrEmpty(pageName) | string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("OpenChangePage Fail,Parameter is null");
                return null;
            }

            if (CurUIBaseChangePage != null && (pageName == CurUIBaseChangePage.PageName))
            {
                Debug.LogError("页面无法切换到自身 " + pageName);
                return null;
            } //自己切换到自己

            T targetPage = null;

            targetPage = TryGetUIPageFromCache(pageName) as T; //首先检测缓存中的数据
            //创建界面
            if (targetPage == null || targetPage.ConnectPageInstance == null)
            {
                CreateUIPageInstance(pageName, pagePath, mUIChangePage, (obj) =>
                {
                    if (obj != null)
                    {
                        if (targetPage == null)
                            targetPage = new T();
                        else
                            targetPage.ResetPage();

                        targetPage.InitialedUIPage(pageName, pagePath, UIPageTypeEnum.ChangePage, obj as GameObject);
                        TryCacheUIPage(pageName, targetPage);

#if UNITY_EDITOR
                        var debugShowScript = (obj as GameObject).GetAddComponentEx<Debug_ShowUIPageInfor>();
                        debugShowScript.Initialed(targetPage);
#endif

                    }
                });
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


        /// <summary>
        /// 返回到上一个界面
        /// </summary>
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
                previouPage = TryGetUIPageFromCache(perviousPageName) as UIBaseChangePage;
                if (previouPage == null)
                {
                    Debug.LogError("无法返回到上一个界面 {0} ，没有被记录的页面", perviousPageName);
                    return;
                }
                if (previouPage.ConnectPageInstance == null)
                {
                    CreateUIPageInstance(previouPage.PageName, previouPage.mPagePath, mUIChangePage, (obj) =>
                    {
                        if (obj != null)
                        {
                            previouPage.ResetPage();

                            previouPage.InitialedUIPage(previouPage.PageName, previouPage.mPagePath, UIPageTypeEnum.ChangePage, obj as GameObject);
                            TryCacheUIPage(previouPage.PageName, previouPage);
#if UNITY_EDITOR
                            var debugShowScript = (obj as GameObject).GetAddComponentEx<Debug_ShowUIPageInfor>();
                            debugShowScript.Initialed(previouPage);
#endif
                        }
                    });
                }


                if (previouPage.ConnectPageInstance == null)
                    continue;  //说明上一个页面加载失败

                if (CurUIBaseChangePage != null)
                    CurUIBaseChangePage.HidePage(false);

                CurUIBaseChangePage = previouPage;
                previouPage.ShowPage();
                return;
            }
        }


        //这里后期可能会处理成值记录一次
        private static void RecordChangePage(string pageName)
        {
            s_ChangePageRecord.Push(pageName);
        }
        #endregion



        #region UIBasePopWindow 弹窗的接口

        private static HashSet<string> s_AllAcivityPopWIndows = new HashSet<string>(); //所有可见的弹窗
        //打开弹窗
        public static T ShowPopwindow<T>(string pageName, string pagePath, bool isBelongCurPage) where T : UIBasePopWindow, new()
        {
            if (string.IsNullOrEmpty(pageName) | string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("ShowPopwindow Fail,Parameter is null");
                return null;
            }

            if (CurUIBaseChangePage == null)
            {
                Debug.LogError("当前没有打开任何页面 " + pageName);
                return null;
            } //自己切换到自己

            T targetPage = null;
            UIBasePage cachePage = TryGetUIPageFromCache(pageName);
            if (cachePage != null)
            {
                targetPage = cachePage as T;
                if(isBelongCurPage)
                    targetPage.ChangeBelongChangePage(CurUIBaseChangePage);
                else
                    targetPage.ChangeBelongChangePage(null);

                cachePage.ShowPage();
                RecordPopPage(pageName, true);
                return targetPage;
            } //取出打开的界面

            CreateUIPageInstance(pageName, pagePath, mUIPopWindow, (obj) =>
             {
                 if (obj != null)
                 {
                     RecordPopPage(pageName, true);
                     targetPage = new T();
                     if (isBelongCurPage)
                         targetPage.InitialedUIPage(pageName, targetPage.mUIPageTypeEnum, CurUIBaseChangePage, obj as GameObject);
                     else
                         targetPage.InitialedUIPage(pageName, targetPage.mUIPageTypeEnum, null, obj as GameObject);

                     TryCacheUIPage(pageName, targetPage);
                     targetPage.ShowPage();
#if UNITY_EDITOR
                     var debugShowScript = (obj as GameObject).GetAddComponentEx<Debug_ShowUIPageInfor>();
                     debugShowScript.Initialed(targetPage);
#endif
                 }
             });
            return targetPage;
        }

        /// <summary>
        /// 隐藏一个弹窗
        /// </summary>
        /// <param name="pageName"></param>
        public static void HidePopwindow(string pageName, bool isForceDestroyed = false)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("HidePopwindow Fail, parameter id null");
                return;
            }

            if (s_AllAcivityPopWIndows.Contains(pageName)==false)
            {
#if UNITY_EDITOR
                Debug.LogError("弹窗{0} 已经关闭了 ，并不需要重复关闭", pageName);
#endif
                return;
            }

            UIBasePage popWindow = TryGetUIPageFromCache(pageName);
            HidePopwindow(popWindow as UIBasePopWindow, isForceDestroyed, false);
        }

        /// <summary>
        /// 隐藏一个弹窗
        /// </summary>
        /// <param name="pageName"></param>
        public static void HidePopwindow(UIBasePopWindow popWindow, bool isForceDestroyed = false, bool isNeedCheckState = true)
        {
            if (popWindow == null)
            {
                Debug.LogError("HidePopwindow Fail,parameter is null");
                return;
            }

            if (isNeedCheckState)
            {
                if (s_AllAcivityPopWIndows.Contains(popWindow.PageName) == false)
                {
#if UNITY_EDITOR
                    Debug.LogError("弹窗{0} 已经关闭了 ，并不需要重复关闭", popWindow.PageName);
#endif
                    return;
                }
            }

            if (popWindow.mIsActivite == false)
            {
#if UNITY_EDITOR
                Debug.LogError("HidePopwindow Fail, 弹窗 {0} 已经关闭了", popWindow.PageName);
#endif
                return;
            }

            popWindow.HidePage(isForceDestroyed);
            RecordPopPage(popWindow.PageName, false);
        }

        /// <summary>
        /// 关闭所有的弹窗
        /// </summary>
        public static void HideAllPopWindows()
        {
            foreach (var pageName in s_AllAcivityPopWIndows)
            {
                if (string.IsNullOrEmpty(pageName))
                    continue;

                UIBasePage popWindow = TryGetUIPageFromCache(pageName);
                if (popWindow == null || popWindow.mIsActivite == false)
                    continue;

                popWindow.HidePage(false);
            }
            s_AllAcivityPopWIndows.Clear();
        }


        //这里后期可能会处理成值记录一次
        private static void RecordPopPage(string pageName, bool isAddRecord)
        {
            if (isAddRecord)
            {
                if (s_AllAcivityPopWIndows.Contains(pageName) == false)
                    s_AllAcivityPopWIndows.Add(pageName);
            }
            else
            {
                if (s_AllAcivityPopWIndows.Contains(pageName))
                    s_AllAcivityPopWIndows.Remove(pageName);
            }
        }

        #endregion


        #region UIBaseWidget 组件接口

        public static void HideWidget(string pageName, UIBasePage senderPage)
        {

        }
        #endregion

        #region 辅助工具

        //创建页面实例
        private static void CreateUIPageInstance(string pageName, string pagePath, Transform parent, Action<UnityEngine.Object> afterInitialedInstanceAction)
        {
            ResourcesManager.LoadGameObjectAssetSync(pagePath, parent, afterInitialedInstanceAction);
        }


        #endregion


        #region 公共的接口
        //保存创建的页面引用
        public static bool TryCacheUIPage(string pageName, UIBasePage page)
        {
            if (s_AllAliveUIPages.ContainsKey(pageName))
                return false;

            s_AllAliveUIPages[pageName] = page;
            return true;
        }

        //从缓存中获取一个指定名称的页面
        public static UIBasePage TryGetUIPageFromCache(string pageName)
        {
            if (string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("TryGetUIPageFromCache Fail,Parameter is null");
                return null;
            }
            UIBasePage basePage = null;
            if (s_AllAliveUIPages.TryGetValue(pageName, out basePage))
                return basePage;
            return basePage;
        }

        public static void RemoUIPageCacheRecord(string pageName)
        {
            UIBasePage page = null;
            if(s_AllAliveUIPages.TryGetValue(pageName,out page)&&page!=null)
            {
                if(page.mUIPageTypeEnum!= UIPageTypeEnum.ChangePage)
                {
                    page.DestroyAndRelease();
                    page = null;
                    s_AllAliveUIPages.Remove(pageName);
                }
                else
                {
                    page.DestroyAndRelease();
                }//页面只取消关联预制体 不销毁  UIBasePage 对象
            }
        }
        #endregion

    }
}