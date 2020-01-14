using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 组件 不能单独存在 只能是页面或者弹窗等的子组件,可以包含子组件
    /// 当父组件销毁时候子组件就销毁
    /// </summary>
    public class UIBaseWidget : UIBasePage
    {
        public UIBasePage mParentUIPage { get; protected set; }

        #region  初始化

        public UIBaseWidget() : base()
        {
            mUIPageTypeEnum = UIPageTypeEnum.Widget;
            MaxAliveAfterInActivte = 0;
        }


        public virtual void UIPageInitialed(string pageName, UIPageTypeEnum pageType, UIBasePage parent, GameObject baseBeReferenceInstance)
        {
            BaseUIPageInitialed(pageName, pageType, baseBeReferenceInstance);
            SetWidgetParent(parent);
        }

        public virtual void SetWidgetParent(UIBasePage parentPage)
        {
            if (mParentUIPage == parentPage)
                return;

            if (mParentUIPage != null)
                mParentUIPage.RemoveWidget(this);
            mParentUIPage = parentPage;
            if (parentPage != null)
                parentPage.AddWidget(PageName, this);
        }

        #endregion

        #region 基类重写

        /// <summary>
        /// 组件的声明周期只跟关联的父级组件相关 不需要再后台计时
        /// </summary>
        /// <param name="isForceDestroyed"></param>
        protected override void OnAfterInVisible(bool isForceDestroyed)
        {
            if (isForceDestroyed)
                DestroyAndRelease();
        }

        protected override void OnBeforeDestroyed()
        {
            if (mParentUIPage != null)
                mParentUIPage.RemoveWidget(this);
            base.OnBeforeDestroyed();
        }

        #endregion
    }
}