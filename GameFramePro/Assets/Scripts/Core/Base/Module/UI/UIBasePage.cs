using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.UI
{
    public enum UIPageTypeEnum
    {
        None=0,  //未知的初始状态 不可用
        ChangePage, //页面
        PopWindow, //弹窗
    }


    /// <summary>
    /// UI界面的基类
    /// </summary>
    public class UIBasePage
    {
        #region Data
        protected List<Coroutine> mAllRuningCoroutine = new List<Coroutine>();
        protected UIPageTypeEnum mUIPageTypeEnum = UIPageTypeEnum.None;  //界面类型 必须正确设置
        public string PageName { get; protected set; }
        public GameObject ConnectPageInstance { get; protected set; } //关联的预制体实例
        public bool mIsActivite { get { return ConnectPageInstance.activeSelf; } }

        public int MaxAliveAfterInActivte { get; protected set; } = 0;  //当值等于0时候表示立刻销毁 ;=-1表示长存，其他值标示隐藏后的分钟数
        #endregion

        public virtual void InitialedUIPage(string pageName, UIPageTypeEnum pageType, GameObject instance)
        {
            PageName = pageName;
            mUIPageTypeEnum = pageType;
            ConnectPageInstance = instance;
        }




        #region UI 界面接口

        public virtual void ShowPage(params object[] parameter)
        {

        }

        public virtual void HidePage(params object[] parameter)
        {
            OnExit();
        }

        protected virtual void OnEnter()
        {

        }
        protected virtual void OnExit()
        {
            
            StopAllCoroutines();
        }

        protected virtual void OnDestroyed()
        {
        }


        #endregion

        #region 辅助功能接口
        protected virtual void StartCoroutine(IEnumerator routine)
        {
            mAllRuningCoroutine.Add(AsyncManager.S_Instance.StartCoroutineEx(routine));
        }

        protected virtual void StopCoroutine(Coroutine routine)
        {
            mAllRuningCoroutine.Remove(routine);
            AsyncManager.S_Instance.StopCoroutineEx(routine);
        }

        protected virtual void StopAllCoroutines()
        {
            foreach (var routine in mAllRuningCoroutine)
                AsyncManager.S_Instance.StopCoroutineEx(routine);
            mAllRuningCoroutine.Clear();
        }
        #endregion

    }
}