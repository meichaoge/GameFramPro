using GameFramePro.ResourcesEx.Reference;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// 加载得到的资源对象
    /// </summary>
    public sealed class LoadAssetResult<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// 加载得到的资源
        /// </summary>
        private T mLoadAssetInstance { get; set; }

        public string mLoadAssetInstanceName { get { return IsLoadAssetEnable ? mLoadAssetInstance.name : string.Empty; } }

        /// <summary>
        /// 表示这个得到的资源是否有效
        /// </summary>
        public bool IsLoadAssetEnable { get { return mLoadAssetInstance != null; } }


        #region 构造函数

        public LoadAssetResult(T loadAssetInstance)
        {
            mLoadAssetInstance = loadAssetInstance;
        }

        #endregion


        /// <summary>
        /// 获取或者设置加载得到的对象
        /// </summary>
        /// <param name="targetComponent">关联到哪个组件</param>
        /// <param name="ConnectAssetReferenceComponentAtc">如果这个操作返回false 则资源关联失败，否则会记录引用关系</param>
        public void ReferenceWithComponent(Component targetComponent, Func<T, bool> ConnectAssetReferenceComponentAtc)
        {
            if (mLoadAssetInstance == null || targetComponent == null || ConnectAssetReferenceComponentAtc == null)
                return;
            bool isSuccess = ConnectAssetReferenceComponentAtc(mLoadAssetInstance);
            if (isSuccess)
            {
                ReferenceAssetManager.S_Instance.StrongReferenceWithComponent(mLoadAssetInstance, targetComponent);
            }
        }


    }
}