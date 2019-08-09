using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    /// <summary>/// 进度条页面/// </summary>
    public class UIProgressChangePage : UIBaseChangePage
    {
        #region UI

        private Image m_ProgressBar;
        private Text m_ProgressText;
        private Text m_ProgressMessageText;
        private Text m_ProgressTipText;

        #endregion


        #region 基类重写

        public void ShowProgress(string message, float progress, string tips = ConstDefine.S_StringEmpty)
        {
            ShowProgressView(message, progress, tips);
        }

        protected override void OnInitialed()
        {
            base.OnInitialed();
            GetUIComponentReference();
            //TODO
        }

        protected override void OnBeforeVisible()
        {
            base.OnBeforeVisible();
            ShowProgressView(string.Empty, 0f, String.Empty);
        }

        /// <summary>/// 根据挂载的 UGUIComponentReference 脚本生成对应的引用配置/// </summary>
        private void GetUIComponentReference()
        {
            m_ProgressBar = GetComponentByName<Image>("ProgressBar");
            m_ProgressText = GetComponentByName<Text>("ProgressText");
            m_ProgressMessageText = GetComponentByName<Text>("ProgressMessageText");
            m_ProgressTipText = GetComponentByName<Text>("ProgressTipText");
        }

        #endregion

        #region  创建视图

        /// <summary>/// 显示进度信息/// </summary>
        private void ShowProgressView(string message, float progress, string tips)
        {
            Debug.Log($"progress={progress}");

            m_ProgressBar.fillAmount = progress;
            m_ProgressMessageText.text = message;
            m_ProgressTipText.text = tips;
            m_ProgressText.text = $"{(int) (progress * 100)}%100";
        }

        #endregion

        #region  按钮点击事件

        //TODO

        #endregion
    }
}
