using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace GameFramePro.UI
{
    /// <summary>
    /// 管理在首页需要自动弹出的弹窗
    /// </summary>
    public static class UIHomeAutoPopupManager
    {
        public static int AutoPopTimeSpace { get; set; } = 3600;  //两次登录时间如果大于这个值(单位秒) 则会弹出界面 否则不弹出

        /// <summary>/// 存放首页需要自动弹出的所有页面/// key：页面权重编号  value：页面名字/// </summary>
        private static List<UIAutoPopupMarkInfor> s_AllWillPushPopPages = new List<UIAutoPopupMarkInfor>();


        private static int mLastLoginInTimeRecord = int.MinValue;
        //上一次自动弹出的时间
        public static int LastLoginInTimeRecord
        {
            get
            {
                if (mLastLoginInTimeRecord == int.MinValue)
                    mLastLoginInTimeRecord = PlayerPrefsManager.GetInt(PlayerPrefsKeyDefine.LastLoginTimeRecord, 0);

                if (mLastLoginInTimeRecord == 0)
                {
                    mLastLoginInTimeRecord = DateTime_Ex.NowToTimestampLocal_Second();
                    PlayerPrefsManager.SetInt(PlayerPrefsKeyDefine.LastLoginTimeRecord, mLastLoginInTimeRecord);
                }
                return mLastLoginInTimeRecord;
            }
        }

        public static DateTime LastLoginInDateTimeRecord { get { return DateTime_Ex.TimestampToDateTime(LastLoginInTimeRecord); } }

        //标识是否已经超过上次自动弹出的时间
        public static bool IsTimeOutToAutoPopup { get { return (DateTime.Now - LastLoginInDateTimeRecord).TotalSeconds >= AutoPopTimeSpace; } }

        public static string TargetHomePageName { get { return "Home"; } }

        public static bool IsAtHomePage { get { return UIPageManager.IsCurPageMatch(TargetHomePageName); } }  //判断是否在某个指定的页面 TODO

        //判断是否包含指定的弹窗
        public static bool IsContainPopupInfor(UIAutoPopupMarkInfor popupMarkInfor)
        {
            foreach (var item in s_AllWillPushPopPages)
            {
                if (item.mWidget == popupMarkInfor.mWidget && popupMarkInfor.mPagePrefabName == item.mPagePrefabName && popupMarkInfor.mPageID == item.mPageID)
                    return true;
            }
            return false;
        }

        //监听相关的UI事件
        public static void RegisterUIEvent()
        {
            EventManager.RegisterMessageHandler<string, string>((int)UIEventUsage.NotifyChangePage, OnNotifyChangePage);
        }

        private static void OnNotifyChangePage(int messageID, string previousPage, string currentPage)
        {
            if (TargetHomePageName == currentPage)
            {
                EventManager.RegisterMessageHandler<string>((int)UIEventUsage.NotifyHidePopWindow, OnNotifyHidePopWindow);
            }
            else
            {
                EventManager.UnRegisterMessageHandler<string>((int)UIEventUsage.NotifyHidePopWindow, OnNotifyHidePopWindow);
            }
        }

        private static void OnNotifyHidePopWindow(int messageID, string hidePageName)
        {
            if (UIPageManager.IsShowingPopWindow)
                return;
            CheckAndAutoPopup();
        }



        /// <summary>/// 记录需要弹出的界面 会按照权重排序
        /// </summary>
        /// <param name="isAllTime">=true 则每次都会弹出 否则一定间隔后才会弹出</param>
        public static void AddPushPageToMap(float key, string PageName, string pagePath, int pageID, object otherInfor, bool isAllTime = false)
        {
            //无时间限制弹窗 +两次登录时间如果大于这个值(单位分钟) 则会弹出界面 否则不弹出 
            if (isAllTime || IsTimeOutToAutoPopup)
            {
                RecordPoppage(new UIAutoPopupMarkInfor(PageName, pagePath, pageID, key, otherInfor));
            }
        }

        /// <summary>/// 添加弹窗到队列中 如果已经存在一个同名弹窗则附加ActivityId组合成名字/// </summary>
        private static void RecordPoppage(UIAutoPopupMarkInfor uIAutoPopupMarkInfor)
        {
            if (uIAutoPopupMarkInfor == null) return;

            if (IsContainPopupInfor(uIAutoPopupMarkInfor))
            {
                Debug.LogError($"存在重复的配置  权重相同 key={uIAutoPopupMarkInfor.mWidget} poppageName={uIAutoPopupMarkInfor.mPagePrefabName}  activityId={uIAutoPopupMarkInfor.mPageID}");
                return;
            }

            //if (s_AllWillPushPopPageDic.ContainsValue(uIAutoPopupMarkInfor.wi))
            //{
            //    Debug.Log($"存在重复的弹窗名 {poppageName},后添加的使用{poppageName}_{activityId}命名");
            //    poppageName = $"{poppageName}_{activityId}";
            //}

            s_AllWillPushPopPages.Add(uIAutoPopupMarkInfor);
            s_AllWillPushPopPages.Sort((a, b) => { return b.mWidget.CompareTo(a.mWidget); });
            //             Debug.Log$"---------{key}-----------{poppageName}");
        }

        public static void RemovePopPage(string pageName, float widget, int id)
        {
            for (int dex = s_AllWillPushPopPages.Count - 1; dex >= 0; dex--)
            {
                var item = s_AllWillPushPopPages[dex];
                if (item.mWidget == widget && item.mPagePrefabName == pageName && item.mPageID == id)
                {
                    s_AllWillPushPopPages.RemoveAt(dex);
                    return;
                }
            }
        }
        #region 弹出弹窗

        //检测是否能够弹窗弹窗 如果有则弹出
        public static void CheckAndAutoPopup()
        {
            if (s_AllWillPushPopPages.Count == 0)
                return;
            if (IsAtHomePage == false) return;
            if (UIPageManager.IsShowingPopWindow) return;

            UIAutoPopupMarkInfor popupMarkInfor = s_AllWillPushPopPages[s_AllWillPushPopPages.Count - 1];
            s_AllWillPushPopPages.RemoveAt(s_AllWillPushPopPages.Count - 1);

            OnPopupMarkInfor(popupMarkInfor);
        }


        private static void OnPopupMarkInfor(UIAutoPopupMarkInfor popupMarkInfor)
        {

        }

        #endregion

    }
}