using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using GameFramePro.Localization;
using GameFramePro.UI;
using GameFramePro.Upgrade;
using GameFramePro.ResourcesEx.Reference;

namespace GameFramePro
{

    /// <summary>
    /// 需要自己手动挂在 第一个执行的脚本(作为程序执行的入口) 需要尽可能少的脚本直接依赖Mono.
    /// </summary>
    public class AppEntryManager : SingleMono<AppEntryManager>
    {
         private static HashSet<IUpdateTick> sAllControllUpdateTicks = new HashSet<IUpdateTick>();

#if UNITY_EDITOR
        [ReadOnly]
        public int ReadOnlyInt = 10;

        [Tooltip("标示是否在优先加载Resources下资源, =false 时候优先加载外部AssetBundle资源")]
        public bool mIsLoadResourcesAssetPriority = true;

#endif
        [Tooltip("标示是否启用 屏幕点击特效 (这里是在入口处判断，比ScreenClickManager 中的接口优先级和限制更高)")]
        public bool mIsClickEffectEnable = true;

        protected override bool IsNotDestroyedOnLoad { get; } = true; //标示不销毁

        public float CurrentRealTime
        {
            get { return Time.realtimeSinceStartup; }
        } //启动到现在的时间

        private IEnumerator Start()
        {
            Debug.Log("AppEntryManager--->>>>");
       

#if UNITY_EDITOR
            Debug.LogInfor($"Application.persistentDataPath={Application.persistentDataPath}");
#endif


            UIPageManager.InitialedPageManager();
        
            NetWorkManager.S_Instance.InitialedNetWork();

            var appUpgradeCoroutineEx = new SuperCoroutine(AppUpgradeManager.S_Instance.OnBeginUpgrade());
            yield return appUpgradeCoroutineEx.WaitDone(true); //等待完成项目的更新

            if (AppUpgradeManager.S_Instance.mUpgradeState != UpgradeStateUsage.UpgradeSuccess)
            {
                Debug.LogError($"更新失败！！");
                yield break;
            }
            yield return null;
            Debug.LogInfor($"更新资源完成 ----进入游戏");

            LocalizationManager.LoadDefaultLocalizationConfig();
            UIPageManager.OpenChangePage<UILoginChangePage>(NameDefine.UILoginChangePageName, PathDefine.UILoginChangePagePath);
            //       SocketClientHelper.BaseLoginTcpClient.Connect(IPAddress.Parse("127.0.0.1"), 2500, 5000);
        }


        private void Update()
        {
            UpdateTick(Time.realtimeSinceStartup);

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.D))
            {
                NetWorkManager.S_Instance.DisConnectSocket(SocketClientHelper.BaseTcpClientName);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                NetWorkManager.S_Instance.ReConnectSocket(SocketClientHelper.BaseTcpClientName);
            }
#endif
        }


        private void OnDisable()
        {
            NetWorkManager.S_Instance.CloseAllSocketClient();
        }

        #region 更新流程控制

        #endregion


        #region 总的刷新控制

        private void UpdateTick(float realtimeSinceStartup)
        {
            foreach (var updateTick in sAllControllUpdateTicks)
            {
                updateTick.UpdateTick(realtimeSinceStartup);
            }
        }


        ////注册需要定时刷新的模块
        public static void RegisterUpdateTick(IUpdateTick ticker)
        {
            if (sAllControllUpdateTicks.Contains(ticker))
            {
                Debug.LogError("RegisterUpdateTick Fail!! Already Exit " + ticker.GetType());
                return;
            }
            sAllControllUpdateTicks.Add(ticker);
        }
        public static void UnRegisterUpdateTick(IUpdateTick ticker)
        {
            if (sAllControllUpdateTicks.Contains(ticker))
            {
                sAllControllUpdateTicks.Remove(ticker);
                return;
            }
            Debug.LogError("UnRegisterUpdateTick Fail!! Not Exit " + ticker.GetType());
        }

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


        #region 事件检测系统

        #endregion
    }
}