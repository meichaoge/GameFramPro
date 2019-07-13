using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{
    /// <summary>
    /// 需要自己手动挂在 第一个执行的脚本
    /// </summary>
    public class AppManager : Single_Mono_NotDestroy<AppManager>
    {
        private void Start()
        {
            Debug.Log("AppManager--->>>>");
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                AssetBundleUpgradeManager.S_Instance.BeginCheckAssetBundleAssetState();
            }
        }


    }
}