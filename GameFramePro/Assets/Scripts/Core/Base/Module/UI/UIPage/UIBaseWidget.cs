using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 组件 不能单独存在 只能是页面或者弹窗等的子组件,可以包含子组件
    /// </summary>
    public class UIBaseWidget: UIBasePage
    {
        public UIBasePage mParentUIPage { get; protected set; }

        #region  初始化
        public UIBaseWidget():base()
        {
            mUIPageTypeEnum = UIPageTypeEnum.Widget;
            MaxAliveAfterInActivte = 0;
        }
        public virtual void InitialedUIPage(string pageName, UIPageTypeEnum pageType, UIBasePage parent, GameObject instance)
        {
            InitialedBaseUIPage(pageName, pageType, instance);
            SetWidgetParent(parent);
        }

        public virtual void SetWidgetParent(UIBasePage parentPage)
        {
            //            if (mParentUIPage != null)
            //            {
            //#if UNITY_EDITOR
            //                Debug.LogError("组件 {0} 之前属于父级{1} 现在改成{2}  确认是否有问题 ", PageName, parentPage.PageName, mParentUIPage.PageName);
            //#endif
            //                mParentUIPage.RemoveWidget(PageName);
            //            }

            mParentUIPage = parentPage;

            //if (mParentUIPage != null)
            //    mParentUIPage.AddWidget(PageName, this);
        }


        #endregion


    }
}