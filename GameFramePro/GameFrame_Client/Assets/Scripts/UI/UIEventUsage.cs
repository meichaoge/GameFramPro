using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// UI 模块与其他模块交互的事件枚举定义
    /// </summary>
    public enum UIEventUsage
    {
        #region 基础事件不要删除
        NotifyChangePage = 0,  //页面切换
        NotifyShowPopWindow = 1, //显示弹窗
        NotifyHidePopWindow = 2, //关闭弹窗
        NotifyAllHidePopWindow = 3, //关闭所欲弹窗


        NotifySwitchLanguage = 10, //通知修改了语言
        #endregion
        OnResponse_Login=20,                //点击登录事件

    }
}
