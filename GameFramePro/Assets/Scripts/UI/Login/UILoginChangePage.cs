using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    /// <summary>
    /// 登录页面
    /// </summary>
    public class UILoginChangePage : UIBaseChangePage
    {
        #region  UI
        Text mNameText;
        Button mLoginButton;
        #endregion


        #region 基类重写
        protected override void OnInitialed()
        {
            base.OnInitialed();
            mNameText = GetComponentByName<Text>("NameText");
            mLoginButton = GetComponentByName<Button>("LoginButton");
            mLoginButton.onClick.AddListener(OnLoginButtonClick);
        }
        #endregion


        #region 事件
        private void OnLoginButtonClick()
        {
            Debug.Log("OnLoginButtonClick");
        }

        #endregion

    }
}