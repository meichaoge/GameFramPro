using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 页面 同一个时刻只能有一个页面是显示状态，只能从一个切换到另一个
    /// </summary>
    public class UIBaseChangePage : UIBasePage
    {
        public HashSet<string> mAllContainSubPopWindows { get; protected set; } //所有关联属于这个页面的弹窗，关闭的时候会一起呗关闭
        public string mPagePath { get; protected set; } = string.Empty; //页面创建时候的路径 需要记录以便于返回时候能够创建出来

        #region 初始化

        public UIBaseChangePage():base()
        {
            mUIPageTypeEnum = UIPageTypeEnum.ChangePage;
            MaxAliveAfterInActivte = 60;
        }

        public virtual void InitialedUIPage(string pageName, string pagePath, UIPageTypeEnum pageType, GameObject instance)
        {
            InitialedBaseUIPage(pageName, pageType, instance);
            mPagePath = pagePath;
        }



        #endregion

        #region 关联子弹窗
        public virtual void AddPopWindow(string popWindowName)
        {
            if (mAllContainSubPopWindows == null)
                mAllContainSubPopWindows = new HashSet<string>();

            if (mAllContainSubPopWindows.Contains(popWindowName) == false)
                mAllContainSubPopWindows.Add(popWindowName);
        }

        public virtual void RemovePopWindow(string popWindowName)
        {
            if (mAllContainSubPopWindows == null)
                return;

            mAllContainSubPopWindows.Remove(popWindowName);
        }
        #endregion


        #region 基类重写
        protected override void OnBeforeInVisible()
        {
            base.OnBeforeInVisible();

            if (mAllContainSubPopWindows != null && mAllContainSubPopWindows.Count != 0)
            {
                foreach (var page in mAllContainSubPopWindows)
                    UIPageManager.HidePopwindow(page);
                mAllContainSubPopWindows.Clear();
            }
        }
        #endregion


    }
}