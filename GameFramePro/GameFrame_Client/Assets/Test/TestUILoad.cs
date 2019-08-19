using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.UI;
using GameFramePro;

public class TestUILoad :MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIPageManager.OpenChangePage<UIHomeChangePage>(NameDefine.UIHomeChangePageName, PathDefine.UIHomeChangePagePath); //切换页面
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            UIPageManager.ShowPopWindow<UILoginTipPopWindow>(NameDefine.UILoginTipPopWindowName, PathDefine.UILoginTipPopWindowPath, true); //弹出弹窗
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UIPageManager.OpenChangePage<UILoginChangePage>(NameDefine.UILoginChangePageName, PathDefine.UILoginChangePagePath); //返回登录页面
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIPageManager.HidePopwindow(NameDefine.UILoginTipPopWindowName); //弹出弹窗
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UIPageManager.BackPage();  //回退
        }
    }

}
