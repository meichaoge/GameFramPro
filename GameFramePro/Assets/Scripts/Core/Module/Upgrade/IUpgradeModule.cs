using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.Upgrade
{
    /// <summary>///  开始更新/// </summary>
    public delegate void OnBeginUpgradeDelegate();

    /// <summary>///  更新进行中/// </summary>
    public delegate void OnUpgradeProcessDelegate(string message, float process);

    /// <summary>///  更新失败/// </summary>
    public delegate void OnUpgradeFailDelegate(string message);

    /// <summary>///  完成更新/// </summary>
    public delegate void OnUpgradeSuccessDelegate();

    /// <summary>/// 重新开始更新/// </summary>
    public delegate void OnReBeginUpgradeDelegate();


    /// <summary>/// 需要更新模块/// </summary>
    public interface IUpgradeModule
    {
        event OnBeginUpgradeDelegate OnBeginUpgradeEvent;

        event OnUpgradeProcessDelegate OnUpgradeProcessEvent;

        event OnUpgradeFailDelegate OnUpgradeFailEvent;

        event OnUpgradeSuccessDelegate OnUpgradeSuccessEvent;

        event OnReBeginUpgradeDelegate OnReBeginUpgradeEvent;

        /// <summary>/// 当前的下载进度 取值[0-1f]/// </summary>
        float CurProcess { get; }

        /// <summary>/// 开始更新/// </summary>
        IEnumerator OnBeginUpgrade();

        /// <summary>/// 更新进行中/// </summary>
        void OnUpgradeProcess(string message, float process);

        /// <summary>/// 更新失败/// </summary>
        void OnUpgradeFail();

        /// <summary>/// 更新成功/// </summary>
        void OnUpgradeSuccess();

        /// <summary>/// 重新开始成功/// </summary>
        IEnumerator OnReBeginUpgrade();
    }
}
