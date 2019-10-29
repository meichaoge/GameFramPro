using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;
using GameFramePro.UI;

namespace GameFramePro.Upgrade
{
    /// <summary>/// 负责控制整个项目的需要更新的模块信息/// </summary>
    public class AppUpgradeManager : Single<AppUpgradeManager>, IUpgradeModule
    {
        /// <summary>/// 主代码版本号/// </summary>
        public string AppVersion { get; private set; } = "1.0.1";

        /// <summary>/// 资源版本号/// </summary>
        public string AppAssetsVersion { get; private set; } = "a 1.0.1"; // a 代表资源

        private UIProgressChangePage mUIProgressChangePage = null; //下载进度


        #region IUpgradeModule 接口的实现 (这里主要用于进入应用的时候的更新逻辑)

        public event System.Action OnBeginUpgradeEvent;
        public event System.Action<string, float> OnUpgradeProcessEvent;
        public event System.Action<string> OnUpgradeFailEvent;
        public event System.Action OnUpgradeSuccessEvent;
        public event System.Action OnReBeginUpgradeEvent;
        public float CurProcess { get; private set; }
        public UpgradeStateUsage mUpgradeState { get; private set; } = UpgradeStateUsage.Initialed;

        public IEnumerator OnBeginUpgrade()
        {
            mUpgradeState = UpgradeStateUsage.Begin;
            OnBeginUpgradeEvent?.Invoke();
            Debug.LogInfor("开始应用启动时候的更新流程");

            mUIProgressChangePage = UIPageManager.OpenChangePage<UIProgressChangePage>(NameDefine.UIProgressChangePageName, PathDefine.UIProgressChangePagePath);
            mUIProgressChangePage.ShowProgress("开始应用启动时候的更新流程", 0);

            //AssetBundle 更新下载
            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent += OnUpgradeProcess;
            var assetBundleSuperCoroutine = new SuperCoroutine(AssetBundleUpgradeManager.S_Instance.OnBeginUpgrade());
            yield return assetBundleSuperCoroutine.WaitDone(true);
            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent -= OnUpgradeProcess;

            //预加载图片
            PreLoadTextureManager.S_Instance.OnUpgradeProcessEvent += OnUpgradeProcess;
            var preloadTextureSuperCoroutine = new SuperCoroutine(PreLoadTextureManager.S_Instance.OnBeginUpgrade());
            yield return preloadTextureSuperCoroutine.WaitDone(true);
            PreLoadTextureManager.S_Instance.OnUpgradeProcessEvent -= OnUpgradeProcess;


            OnUpgradeProcess("更新完成", 1f);
        }

        public void OnUpgradeProcess(string message, float process)
        {
            mUpgradeState = UpgradeStateUsage.Upgrading;
            CurProcess = process;
            Debug.LogInfor($"OnUpgradeProcess process={process}  message={message}  ");
            mUIProgressChangePage.ShowProgress(message, process);
            OnUpgradeProcessEvent?.Invoke(message, process);
        }

        public void OnUpgradeFail(string message)
        {
            mUpgradeState = UpgradeStateUsage.UpgradeFail;
            Debug.LogError(message);
            OnUpgradeFailEvent?.Invoke(message);
        }

        public void OnUpgradeSuccess()
        {
            mUpgradeState = UpgradeStateUsage.UpgradeSuccess;
            Debug.LogError("下载 成功 ");
            mUIProgressChangePage.ShowProgress("应用更新完成", 1);
            OnUpgradeSuccessEvent?.Invoke();
        }

        public IEnumerator OnReBeginUpgrade()
        {
            mUpgradeState = UpgradeStateUsage.Begin;
            Debug.LogError("重新 下载 ");
            OnReBeginUpgradeEvent?.Invoke();
            yield break;
        }


        #region 下载成功回调

        #endregion

        #endregion
    }
}