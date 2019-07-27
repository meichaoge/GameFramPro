using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    public class UITipInforWidget:UIBaseWidget
    {
        private Text mNameText;

        protected override void OnInitialed()
        {
            base.OnInitialed();
            mNameText = GetComponentByName<Text>("TipText");
        }
    }
}