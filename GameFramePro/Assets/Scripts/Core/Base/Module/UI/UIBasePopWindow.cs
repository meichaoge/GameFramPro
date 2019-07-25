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


        public UIBasePopWindow()
        {
            mUIPageTypeEnum = UIPageTypeEnum.PopWindow;
            MaxAliveAfterInActivte = 5;
        }


    }
}