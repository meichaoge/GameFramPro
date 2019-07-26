using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.UI
{
    /// <summary>
    /// 辅助 UIPageManager ，用于定时清理长时间不被引用的界面
    /// </summary>
    public class UIPageManagerUtility : Single<UIPageManagerUtility>, IUpdateTimeTick
    {
        private LinkedList<UIBasePage> mAllInVisiblePage = new LinkedList<UIBasePage>();
        private List<UIBasePage> mTempInVisiblePage = new List<UIBasePage>(); //用于记录那些页面不可见了



        #region IUpdateTick 接口实现

        protected float lastRecordTime = 0; //上一次记录的时间
        public float TickPerTimeInterval { get; private set; } = 30; //约等于1分钟检测一次

        public bool CheckIfNeedUpdateTick(float curTime)
        {
            if (lastRecordTime == 0f)
            {
                lastRecordTime = curTime;
                return true;
            }

            if(curTime- lastRecordTime>= TickPerTimeInterval)
                return true;

            return false;
        }
        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick(currentTime) == false)
                return;
            lastRecordTime = currentTime;

            #region 刷新
            if (mTempInVisiblePage != null&& mTempInVisiblePage.Count>0)
            {
                foreach (var item in mTempInVisiblePage)
                    mAllInVisiblePage.AddLast(item);
                mTempInVisiblePage.Clear();
            }
            if (mAllInVisiblePage.Count > 0)
            {
                var targetNode = mAllInVisiblePage.First;
                while (targetNode != null)
                {
                    var next = targetNode.Next;
                    if (targetNode.Value.mUIPageState != UIPageStateEnum.Hide)
                    {
                        mAllInVisiblePage.Remove(targetNode);
                        targetNode = next;
                        continue;
                    }//移除非隐藏状态的界面


                    if (currentTime - targetNode.Value.RecordInvisibleRealTime >= targetNode.Value.MaxAliveAfterInActivte)
                    {
                        UIPageManager.RemoUIPageCacheRecord(targetNode.Value.PageName);
                        mAllInVisiblePage.Remove(targetNode);
                        targetNode = next;
                        continue;
                    }
                    else
                    {
                        targetNode = next;
                        continue;
                    }
                }
            }
         

            #endregion
        }

        #endregion


        #region 记录隐藏了页面 以及定时刷新状态

        //注册等待超时删除的页面
        public void RegisterUIBasePageInvisible(UIBasePage page)
        {
            if (page == null)
            {
                Debug.LogError("RegisterUIBasePageInvisible 失败，参数is null");
                return;
            }

            if (page.MaxAliveAfterInActivte <0)
                return; //长存的页面不销毁

            mTempInVisiblePage.Add(page);
        }

        //取消注册等待超时删除的页面
        public void UnRegisterUIBasePageInvisible(UIBasePage page)
        {
            if (page == null)
            {
                Debug.LogError("RegisterUIBasePageInvisible 失败，参数is null");
                return;
            }

            if (page.MaxAliveAfterInActivte <0)
                return; //长存的页面不销毁

            mTempInVisiblePage.Remove(page);
        }
        #endregion


    }
}