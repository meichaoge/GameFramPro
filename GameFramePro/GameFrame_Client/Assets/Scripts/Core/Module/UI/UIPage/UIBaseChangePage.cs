using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>/// 页面 同一个时刻只能有一个页面是显示状态，只能从一个切换到另一个/// </summary>
    /// 页面不能释放 UIBasePage  对象，否则无法知道怎么返回到上一个页面
    public class UIBaseChangePage : UIBasePage
    {
        public HashSet<string> mAllContainSubPopWindows { get; protected set; } //所有关联属于这个页面的弹窗，关闭的时候会一起呗关闭
        public string mPagePath { get; private set; } = string.Empty; //页面创建时候的路径 需要记录以便于返回时候能够创建出来

        #region 初始化

        protected UIBaseChangePage():base()
        {
            mUIPageTypeEnum = UIPageTypeEnum.ChangePage;
            MaxAliveAfterInActivte = 10;
            IsReleaseOnDestroyPageInstance = false;
        }

        public virtual void UIChangePageInitialed(string pageName, string pagePath, UIPageTypeEnum pageType, GameObject goInstance)
        {
            BaseUIPageInitialed(pageName, pageType, goInstance);
            mPagePath = pagePath;
            IsReleaseOnDestroyPageInstance = false;
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
                HashSet<string> temp = new HashSet<string>(mAllContainSubPopWindows);
                foreach (var page in temp)
                {
                    //   Debug.Log($"关闭页面关联弹窗{page}");
                    UIPageManager.HidePopwindow(page);
                }
                mAllContainSubPopWindows.Clear();
            }
        }
        #endregion


    }
}
