using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using System.Reflection;
using System.Linq;
using GameFramePro.UI;

namespace GameFramePro
{
    /// <summary>
    /// 需要自己手动挂在 第一个执行的脚本 需要尽可能少的脚本直接依赖Mono  
    /// </summary>
    public class AppManager : Single_Mono_NotDestroy<AppManager>
    {
        //  private HashSet<IUpdateTick> mAllControllUpdateTicks = new HashSet<IUpdateTick>();

        public float CurrentRealTime { get { return Time.realtimeSinceStartup; } } //启动到现在的时间

        private IEnumerator Start()
        {
            Debug.Log("AppManager--->>>>");
            UIPageManager.InitialedPageManager();

            LocalizationManager.S_Instance.LoadDefaultLocalizationConfig();
            AssetBundleUpgradeManager.S_Instance.BeginUpdateAssetBundle();

            yield return null;

            UIPageManager.OpenChangePage<UILoginChangePage>(NameDefine.UILoginChangePageName, PathDefine.UILoginChangePagePath);
        }


        private void Update()
        {
            UpdateTick(Time.realtimeSinceStartup);

            if (Input.GetKeyDown(KeyCode.A))
            {
                UIPageManager.OpenChangePage<UIHomeChangePage>(NameDefine.UIHomeChangePageName, PathDefine.UIHomeChangePagePath); //切换页面
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                UIPageManager.ShowPopwindow<UILoginTipPopWindow>(NameDefine.UILoginTipPopWindowName, PathDefine.UILoginTipPopWindowPath, true); //弹出弹窗
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UIPageManager.OpenChangePage<UILoginChangePage>(NameDefine.UILoginChangePageName, PathDefine.UILoginChangePagePath); //返回登录页面
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                UIPageManager.BackPage();  //回退
            }
        }



        #region 总的刷新控制
        private void UpdateTick(float realtimeSinceStartup)
        {
            Loom.S_Instance.UpdateTick(realtimeSinceStartup);
            XluaManager.S_Instance.UpdateTick(realtimeSinceStartup);
            DownloadManager.S_Instance.UpdateTick(realtimeSinceStartup);
            AssetDelayDeleteManager.S_Instance.UpdateTick(realtimeSinceStartup);
            UIPageManagerUtility.S_Instance.UpdateTick(realtimeSinceStartup);

            //foreach (var updateTick in mAllControllUpdateTicks)
            //{
            //    updateTick.Tick(realtimeSinceStartup);
            //}
        }

        ////注册需要定时刷新的模块
        //public void RegisterUpdateTick(IUpdateTick ticker)
        //{
        //    if (mAllControllUpdateTicks.Contains(ticker))
        //    {
        //        Debug.LogError("RegisterUpdateTick Fail!! Already Exit " + ticker.GetType());
        //        return;
        //    }
        //    mAllControllUpdateTicks.Add(ticker);
        //}
        //public void UnRegisterUpdateTick(IUpdateTick ticker)
        //{
        //    if (mAllControllUpdateTicks.Contains(ticker))
        //    {
        //        mAllControllUpdateTicks.Remove(ticker);
        //        return;
        //    }
        //    Debug.LogError("UnRegisterUpdateTick Fail!! Not Exit " + ticker.GetType());
        //}

        //private void GetAllNeedControllUpdateInstance()
        //{
        //    var assembly = Assembly.Load("Assembly-CSharp");
        //    var types = assembly.GetTypes();

        //    foreach (var type in types)
        //    {
        //        if (type.GetInterfaces().Contains(typeof(IUpdateTick)))
        //        {

        //            continue;
        //        }
        //    }
        //}

        #endregion

    }
}