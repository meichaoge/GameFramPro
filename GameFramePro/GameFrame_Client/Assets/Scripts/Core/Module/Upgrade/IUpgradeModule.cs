using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.Upgrade
{
    /// <summary>
    /// 更新模块的状态
    /// </summary>
    public enum UpgradeStateUsage
    {
        Initialed, //初始化 还没有开始
        Begin, //准备更新
        Upgrading, //更新中
        UpgradeFail, //更新失败
        UpgradeSuccess, //更新完成
    }


    /// <summary>/// 需要更新模块/// </summary>
    public interface IUpgradeModule
    {
        /// <summary>
        /// 开始更新
        /// </summary>
        event System.Action OnBeginUpgradeEvent;

        /// <summary>
        /// 更新进行中
        /// </summary>
        event System.Action<string, float> OnUpgradeProcessEvent;

        /// <summary>
        /// 更新失败
        /// </summary>
        event System.Action<string> OnUpgradeFailEvent;

        /// <summary>
        /// 完成更新
        /// </summary>
        event System.Action OnUpgradeSuccessEvent;

        /// <summary>
        /// 重新开始更新
        /// </summary>
        event System.Action OnReBeginUpgradeEvent;

        /// <summary>
        /// 当前的下载进度 取值[0-1f]
        ///  </summary>
        float CurProcess { get; }

        /// <summary>
        /// 更新模块的状态
        /// </summary>
        UpgradeStateUsage mUpgradeState { get; }
        /// <summary>
        /// 开始更新
        /// </summary>
        IEnumerator OnBeginUpgrade();

        /// <summary>
        /// 更新进行中
        /// </summary>
        void OnUpgradeProcess(string message, float process);

        /// <summary>
        ///  更新失败
        ///  </summary>
        void OnUpgradeFail(string message);

        /// <summary>
        /// 更新成功
        /// </summary>
        void OnUpgradeSuccess();

        /// <summary>
        ///  重新开始成功
        ///  </summary>
        IEnumerator OnReBeginUpgrade();
    }
}
