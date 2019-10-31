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

            mUIProgressChangePage = UIPageManager.OpenChangePage<UIProgressChangePage>(NameDefine.UIProgressChangePageName, PathDefine.UIProgressChangePagePath,LoadAssetChannelUsage.LocalResourcesPriority);
            mUIProgressChangePage.ShowProgress("开始应用启动时候的更新流程", 0);

            //AssetBundle 更新下载
            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent += OnUpgradeProcess;
            var assetBundleSuperCoroutine = new SuperCoroutine(AssetBundleUpgradeManager.S_Instance.OnBeginUpgrade());
            yield return assetBundleSuperCoroutine.WaitDone(true);
            AssetBundleUpgradeManager.S_Instance.OnUpgradeProcessEvent -= OnUpgradeProcess;

            if(AssetBundleUpgradeManager.S_Instance.mUpgradeState!= UpgradeStateUsage.UpgradeSuccess)
            {
                OnUpgradeFail("资源下载失败");
                yield break;
            }


            //预加载图片
            PreLoadTextureManager.S_Instance.OnUpgradeProcessEvent += OnUpgradeProcess;
            var preloadTextureSuperCoroutine = new SuperCoroutine(PreLoadTextureManager.S_Instance.OnBeginUpgrade());
            yield return preloadTextureSuperCoroutine.WaitDone(true);
            PreLoadTextureManager.S_Instance.OnUpgradeProcessEvent -= OnUpgradeProcess;

            if (PreLoadTextureManager.S_Instance.mUpgradeState != UpgradeStateUsage.UpgradeSuccess)
            {
                OnUpgradeFail("资源下载失败");
                yield break;
            }
            OnUpgradeProcess("更新完成", 1f);


            //更细成功
            OnUpgradeSuccess();
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
            Debug.LogInfor("所有的资源更新完成-- 可以进入游戏 ");
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