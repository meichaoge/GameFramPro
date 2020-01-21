using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace GameFramePro.UI
{

    public class UILoginTipPopWindow:UIBasePopWindow
    {
        private Text mNameText;
        private Transform mContent;
        private Button mTestButton;
        private Button mCloseWidgetButton;

        private UITipInforWidget mUITipInforWidget1;
        private UITipInforWidget mUITipInforWidget2;


        protected override void OnInitialed()
        {
            base.OnInitialed();
            if (mUITipInforWidget1 != null)
                mUITipInforWidget1.HidePage(true);

            mUITipInforWidget1 = UIPageManager.CreateWidgetInstance<UITipInforWidget>(NameDefine.UITipInforWidgetName,PathDefine.UITipInforWidgetPath,
                this, mContent);
        }
        protected override void GetUIComponentReference()
        {
            //  base.GetUIComponentReference();
            mNameText = GetComponentByName<Text>("NameText");
            mContent = GetComponentByPath<Transform>("Content", "Content");
            mTestButton = GetComponentByName<Button>("TestButton");
        }
        protected override void RegisterButtonEvent()
        {
        //    base.RegisterButtonEvent();
            mTestButton.onClick.AddListener(OnTestButtonClick);
            mCloseWidgetButton = GetComponentByName<Button>("CloseWidgetButton");
            mCloseWidgetButton.onClick.AddListener(OnCloseWidgetButtonClick);
        }

        private void OnTestButtonClick()
        {
            if (mUITipInforWidget2 == null)
                mUITipInforWidget2 = UIPageManager.CreateWidgetInstance<UITipInforWidget>(NameDefine.UITipInforWidgetName, PathDefine.UITipInforWidgetPath,
                this, mContent);
        }


        private void OnCloseWidgetButtonClick()
        {
            if(mUITipInforWidget2!=null)
            {
                mUITipInforWidget2.HidePage(true);
                mUITipInforWidget2 = null;
            }
        }
    }
}