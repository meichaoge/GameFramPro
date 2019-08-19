using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.UI
{
    /// <summary>/// 用于提供辅助UI 显示/// </summary>
    public class UIPageHelpUtility : Single<UIPageHelpUtility>
    {
        /// <summary>/// 显示通用的瓢字(可能存在多个最后一个参数为true)/// </summary>
        public static void ShowTipMessage(string message)
        {
            var uiGeneralTipPopWindow = UIPageManager.ShowPopWindow<UIGeneralTipPopWindow>(NameDefine.UIGeneralTipPopWindowName, PathDefine.UIGeneralTipPopWindowPath,false,true);
            uiGeneralTipPopWindow.ShowTipMessage(message);
        }
    }
}
