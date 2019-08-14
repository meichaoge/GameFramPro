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

        public event OnBeginUpgradeDelegate OnBeginUpgradeEvent;
        public event OnUpgradeProcessDelegate OnUpgradeProcessEvent;
        public event OnUpgradeFailDelegate OnUpgradeFailEvent;
        public event OnUpgradeSuccessDelegate OnUpgradeSuccessEvent;
        public event OnReBeginUpgradeDelegate OnReBeginUpgradeEvent;
        public float CurProcess { get; }

        public IEnumerator OnBeginUpgrade()
        {
            Debug.LogInfor("开始应用启动时候的更新流程");

            mUIProgressChangePage = UIPageManager.OpenChangePage<UIProgressChangePage>(NameDefine.UIProgressChangePageName, PathDefine.UIProgressChangePagePath);
            mUIProgressChangePage.ShowProgress("开始应用启动时候的更新流程", 0);


            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent += OnUpgradeProcess;
            var assetBundleCoroutineEx = new SuperCoroutine(AssetBundleUpgradeManager.S_Instance.OnBeginUpgrade());
            yield return assetBundleCoroutineEx.WaitDone(true);
            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent -= OnUpgradeProcess;



            OnUpgradeProcess("更新完成", 1f);
        }

        public void OnUpgradeProcess(string message, float process)
        {
            Debug.LogInfor($"OnUpgradeProcess process={process}  message={message}  ");
            mUIProgressChangePage.ShowProgress(message, process);
        }

        public void OnUpgradeFail()
        {
            Debug.LogError("下载失败 ");
        }

        public void OnUpgradeSuccess()
        {
            Debug.LogError("下载 成功 ");
            mUIProgressChangePage.ShowProgress("应用更新完成", 1);
        }

        public IEnumerator OnReBeginUpgrade()
        {
            Debug.LogError("重新 下载 ");
            yield break;
        }


        #region 下载成功回调

        #endregion

        #endregion
    }
}
