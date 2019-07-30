using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 弹窗的基类
    /// </summary>
    public class UIBasePopWindow : UIBasePage
    {
        public UIBaseChangePage mBelongChangePage { get; protected set; } = null;


        #region 初始化和设置所属于的页面

        public UIBasePopWindow():base()
        {
            mUIPageTypeEnum = UIPageTypeEnum.PopWindow;
            MaxAliveAfterInActivte = 5;
            mBelongChangePage = null;
        }


        public virtual void UIPageInitialed(string pageName, UIPageTypeEnum pageType, UIBaseChangePage belongChangePage, ReferenceGameObjectAssetInfor referenceInstance)
        {
            BaseUIPageInitialed(pageName, pageType, referenceInstance);
            ChangeBelongChangePage(belongChangePage);
        }

        public virtual void ChangeBelongChangePage(UIBaseChangePage belongChangePage)
        {
            if (mBelongChangePage == belongChangePage)
                return;
            if (mBelongChangePage != null)
            {
#if UNITY_EDITOR
                Debug.LogError("弹窗 {0} 之前属于{1} 现在改成{2}  确认是否有问题 ", PageName, mBelongChangePage.PageName, belongChangePage.PageName);
#endif
                mBelongChangePage.RemovePopWindow(PageName);
            }

            mBelongChangePage = belongChangePage;

            if (mBelongChangePage != null)
                mBelongChangePage.AddPopWindow(PageName);
        }


        #endregion




    }
}