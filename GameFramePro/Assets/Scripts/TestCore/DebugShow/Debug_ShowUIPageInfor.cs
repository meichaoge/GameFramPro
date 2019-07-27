using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.UI;

public class Debug_ShowUIPageInfor :MonoBehaviour
{
    public UIPageStateEnum mUIPageState;
    public UIPageTypeEnum mUIPageTypeEnum;
    public string PageName;
    public GameObject ConnectPageInstance;
    public float RecordInvisibleRealTime;
    public float MaxAliveAfterInActivte;

    public List<UIBaseWidget> mAllContainWidgets=new List<UIBaseWidget>();
    public List<string> mAllContainSubPopWindows = new List<string>();
    public string mPagePath;


    private UIBasePage mTarget;

    public void Initialed(UIBasePage page)
    {
        mTarget = page;
    }



    private void Update()
    {
        Debug_ShowUIPage();
    }

    public void Debug_ShowUIPage()
    {
        if (mTarget == null) return;

        mUIPageState = mTarget.mUIPageState;
        mUIPageTypeEnum = mTarget.mUIPageTypeEnum;
        PageName = mTarget.PageName;
        ConnectPageInstance = mTarget.ConnectPageInstance;
        RecordInvisibleRealTime = mTarget.RecordInvisibleRealTime;
        MaxAliveAfterInActivte = mTarget.MaxAliveAfterInActivte;
        mAllContainWidgets.Clear();
        if (mTarget.mAllContainWidgets != null)
            mAllContainWidgets.AddRange(mTarget.mAllContainWidgets);
        if (mTarget.mUIPageTypeEnum == UIPageTypeEnum.ChangePage)
        {
            UIBaseChangePage changePage = mTarget as UIBaseChangePage;
            mPagePath = changePage.mPagePath;

            mAllContainSubPopWindows.Clear();
            if (changePage.mAllContainSubPopWindows != null)
                mAllContainSubPopWindows.AddRange(changePage.mAllContainSubPopWindows);
        }

    }



}
