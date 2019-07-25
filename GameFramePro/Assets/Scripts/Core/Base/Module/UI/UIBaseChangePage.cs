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

        public UIBaseChangePage()
        {
            mUIPageTypeEnum = UIPageTypeEnum.ChangePage;
            MaxAliveAfterInActivte = 10;
        }


    }
}