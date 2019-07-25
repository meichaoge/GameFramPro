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
        private class UIChangePageInfor
        {
            public string mPageName;
            public string mPagePath;

            public UIChangePageInfor(string name,string path)
            {
                mPageName = name;
                mPagePath = path;
            }

        }

        #region 记录页面的数据
        //key=UI 界面名称
        private static Dictionary<string, UIBasePage> s_AllAliveUIPages = new Dictionary<string, UIBasePage>(); //记录所有现在还在内存中的界面

        #endregion

        #region UIBaseChangePage 页面的接口
        /// <summary>
        /// 记录打开页面的顺序，处理向上返回时候有用  key=页面名称
        /// </summary>
        private static Stack<string> s_ChangePageRecord = new Stack<string>();//

        /// <summary>
        /// 所有当前还在内存的弹窗  key changePageName  Vlaue=包含的弹窗
        /// </summary>
        private static Dictionary<string, List<string>> s_AllAlivePopwindow = new Dictionary<string, List<string>>(); //

        /// <summary>
        /// 记录所有打开的界面信息 key= 页面名称 
        /// </summary>
        private static Dictionary<string, UIChangePageInfor> s_AllCreateChangePageInfor = new Dictionary<string, UIChangePageInfor>(); // 

        public static UIBaseChangePage CurUIBaseChangePage { get; private set; } = null; //当前正在显示的页面 可能为null

        /// <summary>
        /// 切换到 另一个界面
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pageName"></param>
        /// <param name="pagePath"></param>
        /// <returns></returns>
        public static T OpenChangePage<T>(string pageName, string pagePath) where T : UIBaseChangePage,new ()
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
            UIBasePage cachePage = TryGetUIPageFromCache(pageName);
            if (cachePage != null)
            {
                targetPage = cachePage as T;
                TryHideChangePage(CurUIBaseChangePage);

                CurUIBaseChangePage = targetPage;
                cachePage.ShowPage();
                RecordChangePage(pageName);
                return targetPage;
            } //取出打开的界面

            ResourcesManager.LoadGameObjectAssetSync(pagePath, null, (obj) =>
            {
                if (obj != null)
                {
                    TryHideChangePage(CurUIBaseChangePage);
                    RecordCreateChangePage(pageName, pagePath);
                    targetPage = new T();
                    targetPage.InitialedUIPage(pageName, UIPageTypeEnum.ChangePage, obj as GameObject);

                    TryCacheUIPageFrom(pageName, targetPage);
                    targetPage.ShowPage();
                }
            });
            return targetPage;
        }

        private static bool TryHideChangePage(UIBaseChangePage page)
        {
            if (page == null) return false;

            List<string> allChildPopWindows = null;
            if (s_AllAlivePopwindow.TryGetValue(page.PageName,out allChildPopWindows)&& allChildPopWindows!=null)
            {
                foreach (var popWindowName in allChildPopWindows)
                {
                    var childPopwindow = TryGetUIPageFromCache(popWindowName);
                    if (childPopwindow == null || childPopwindow.mIsActivite == false)
                        continue;
                    childPopwindow.HidePage();
                }
            }
            allChildPopWindows.Clear();
            page.HidePage();
            return true;
        }
   

        //这里后期可能会处理成值记录一次
        private static void RecordChangePage(string pageName)
        {
            s_ChangePageRecord.Push(pageName);
        }
        #endregion

        #region UIBasePopWindow 弹窗的接口

        public static T ShowPopwindow<T>(string pageName, string pagePath,bool isBelongCurPage=false) where T : UIBasePopWindow, new()
        {
            if (string.IsNullOrEmpty(pageName) | string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("ShowPopwindow Fail,Parameter is null");
                return null;
            }

            if (CurUIBaseChangePage == null )
            {
                Debug.LogError("当前没有打开任何页面 " + pageName);
                return null;
            } //自己切换到自己



            T targetPage = null;
            UIBasePage cachePage = TryGetUIPageFromCache(pageName);
            if (cachePage != null)
            {
                targetPage = cachePage as T;
                cachePage.ShowPage();
                RecordPopPage(pageName, isBelongCurPage);
                return targetPage;
            } //取出打开的界面

            ResourcesManager.LoadGameObjectAssetSync(pagePath, null, (obj) =>
            {
                if (obj != null)
                {
                    RecordPopPage(pageName, isBelongCurPage);
                    targetPage = new T();
                    targetPage.InitialedUIPage(pageName, UIPageTypeEnum.ChangePage, obj as GameObject);

                    TryCacheUIPageFrom(pageName, targetPage);
                    targetPage.ShowPage();
                }
            });
            return targetPage;
        }


        //这里后期可能会处理成值记录一次
        private static void RecordPopPage(string pageName,bool isBelongCurPage)
        {
            if (isBelongCurPage)
            {
                List<string> allChildPopPage = null;
                if(s_AllAlivePopwindow.TryGetValue(CurUIBaseChangePage.PageName,out allChildPopPage)==false)
                {
                    allChildPopPage = new List<string>();
                    s_AllAlivePopwindow[CurUIBaseChangePage.PageName] = allChildPopPage;
                }

                if (allChildPopPage.Contains(pageName) == false)
                {
                    allChildPopPage.Add(pageName);
                    return;
                }
            }
        }
        #endregion


        #region 辅助工具



        private static void RecordCreateChangePage(string pageName,string pagePath)
        {
            UIChangePageInfor pageInfor = null;
            if (s_AllCreateChangePageInfor.TryGetValue(pageName,out pageInfor)==false)
            {
                s_AllCreateChangePageInfor[pageName] = new UIChangePageInfor(pageName, pagePath);
            }
        }


        #endregion


        #region 公共的接口

        public static bool  TryCacheUIPageFrom(string pageName, UIBasePage page)
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


        #endregion

    }
}