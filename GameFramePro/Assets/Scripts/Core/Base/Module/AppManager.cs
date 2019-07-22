using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using System.Reflection;
using System.Linq;

namespace GameFramePro
{
    /// <summary>
    /// 需要自己手动挂在 第一个执行的脚本 需要尽可能少的脚本直接依赖Mono  
    /// </summary>
    public class AppManager : Single_Mono_NotDestroy<AppManager>
    {
        //  private HashSet<IUpdateTick> mAllControllUpdateTicks = new HashSet<IUpdateTick>();


        private void Start()
        {
            Debug.Log("AppManager--->>>>");
            LocalizationManager.S_Instance.LoadDefaultLocalizationConfig();
            AssetBundleUpgradeManager.S_Instance.BeginUpdateAssetBundle();
        }


        private void Update()
        {
            UpdateTick(Time.realtimeSinceStartup);
        }



        #region 总的刷新控制
        private void UpdateTick(float realtimeSinceStartup)
        {
            Loom.S_Instance.UpdateTick(realtimeSinceStartup);
            XluaManager.S_Instance.UpdateTick(realtimeSinceStartup);
            DownloadManager.S_Instance.UpdateTick(realtimeSinceStartup);
            AssetDelayDeleteManager.S_Instance.UpdateTick(realtimeSinceStartup);
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