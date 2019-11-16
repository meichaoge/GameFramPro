using GameFramePro.ResourcesEx.Reference;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// 加载得到的资源对象 如果要使用对应的资源 请调用  ReferenceWithComponent 接口
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
        /// <param name="targetComponent">关联到哪个组件，这个参数为null 的话不关联任何组件</param>
        /// <param name="ConnectAssetReferenceComponentAtc">则资源会记录引用关系(如果引用的对象是空，则默认不会引用)</param>
        /// <returns>返回值标识是否成功的引用了资源 </returns>
        public bool ReferenceWithComponent(Component targetComponent, System.Action<T> ConnectAssetReferenceComponentAtc)
        {
            if (ConnectAssetReferenceComponentAtc == null)
                return false;
            ConnectAssetReferenceComponentAtc(mLoadAssetInstance);

            if (targetComponent == null)
                return false;
            if (mLoadAssetInstance == null || IsLoadAssetEnable == false)
                return false;
            return ReferenceAssetManager.StrongReferenceWithComponent(mLoadAssetInstance, targetComponent);
        }



    }
}