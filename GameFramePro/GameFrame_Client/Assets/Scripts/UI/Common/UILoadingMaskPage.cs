using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace GameFramePro.UI
{
    /// <summary>
    /// 公用的Mask 遮罩层
    /// </summary>
    public class UILoadingMaskPage : UIBasePopWindow
    {
        #region UI
        private Image m_loading;
        private Image m_loadingblackmask;

        #endregion

        public uint ReferenceCount { get; private set; } = 0;

        public float mMaskViewDelayShow { get; protected set; } = 0;
        private float mTimeTicked { get; set; } = 0;
        public bool mIsShowingMaskView { get; private set; } = false;
        public Tweener mLoadingTweener { get; private set; } = null;

        #region 基类重写

        protected override void OnInitialed()
        {
            base.OnInitialed();
            MaxAliveAfterInActivte = -1;  //不销毁\
            RegisterUpdateCallback(UpdateCallbackUsage.Update, false);
        }

        /// <summary>/// 根据挂载的 UGUIComponentReference 脚本生成对应的引用配置/// </summary>
        protected override void GetUIComponentReference()
        {
            m_loading = GetComponentByName<Image>("loading");
            m_loadingblackmask = GetComponentByName<Image>("loadingblackmask");
        }

        protected override void OnRegisterUpdateCallback()
        {
            // base.OnRegisterUpdateCallback();
            if (mMaskViewDelayShow <= 0)
                return;
            mTimeTicked += Time.deltaTime;
        }
        protected override void OnBeforeInVisible()
        {
            HideMaskView();
            base.OnBeforeInVisible();
        }

        protected override void OnAfterVisible()
        {
            base.OnAfterVisible();
            mTimeTicked = 0;
        }
        #endregion

        #region  创建视图
        private void ShowMaskView()
        {
            mIsShowingMaskView = true;
            m_loadingblackmask.enabled = true;
            m_loading.gameObject.SetActive(true);
            m_loading.transform.localEulerAngles = Vector3.zero;

            if (mLoadingTweener != null)
                mLoadingTweener.Kill();

            mLoadingTweener = m_loading.transform.DORotate(new Vector3(0, 0, -360), 0.9f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }

        private void HideMaskView()
        {
            mIsShowingMaskView = false;
            m_loadingblackmask.enabled = false;
            m_loading.gameObject.SetActive(false);
            mLoadingTweener.Kill();
            mLoadingTweener = null;
            mTimeTicked = 0;
        }
        #endregion

        #region  点击事件或者操作
        #endregion

        #region 其他
        public void AddReference(float maskViewShowDelay = 2)
        {
            ++ReferenceCount;
            mMaskViewDelayShow = maskViewShowDelay;
            bool isShow = maskViewShowDelay <= 0f || mTimeTicked >= maskViewShowDelay;
            if (mIsShowingMaskView == isShow)
                return;
            if (isShow)
                ShowMaskView();
            else
                HideMaskView();
        }
        public void ReduceReference()
        {
            --ReferenceCount;
        }
        #endregion

    }
}
