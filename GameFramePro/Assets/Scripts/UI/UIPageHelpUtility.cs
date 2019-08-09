using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.UI
{
    /// <summary>/// 用于提供辅助UI 显示/// </summary>
    public class UIPageHelpUtility : Single<UIPageHelpUtility>
    {
        /// <summary>/// 显示通用的瓢字/// </summary>
        public static void ShowTipMessage(string message)
        {
            var uiGeneralTipPopWindow = UIPageManager.ShowPopwindow<UIGeneralTipPopWindow>(NameDefine.UITipInforWidgetName, PathDefine.UIGeneralTipPopWindowPath,false);
            uiGeneralTipPopWindow.ShowTipMessage(message);
        }
    }
}