using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameFramePro.UI
{
    /// <summary>/// 通用的显示瓢字的弹窗/// </summary>
    public class UIGeneralTipPopWindow : UIBasePopWindow
    {
        #region UI

        private Text m_TipText;
        private CanvasGroup m_Content;

        #endregion

        #region 基类重写

        /// <summary>/// 显示瓢字提示/// </summary>
        public void ShowTipMessage(string message)
        {
            ShowTipView(message);
        }


        protected override void OnInitialed()
        {
            base.OnInitialed();
            GetUIComponentReference();
        }

        protected override void OnBeforeVisible()
        {
            base.OnBeforeVisible();
            ShowTipView(string.Empty);
        }

        protected override void OnAfterVisible()
        {
            base.OnAfterVisible();
            DoContentFadeEffect();
        }

        /// <summary>/// 根据挂载的 UGUIComponentReference 脚本生成对应的引用配置/// </summary>
        private void GetUIComponentReference()
        {
            m_TipText = GetComponentByName<Text>("TipText");
            m_Content = GetComponentByName<CanvasGroup>("Content");
        }

        private void DoContentFadeEffect()
        {
            m_Content.alpha = 0;
            m_Content.DOFade(1, 0.5F);

            m_Content.DOFade(0, 0.5F).SetDelay(1).OnComplete(() => { HidePage(false); });
        }

        #endregion

        #region  创建视图

        private void ShowTipView(string message)
        {
            m_TipText.text = message;
        }

        #endregion

        #region  点击事件或者操作

        //TODO

        #endregion
    }
}
