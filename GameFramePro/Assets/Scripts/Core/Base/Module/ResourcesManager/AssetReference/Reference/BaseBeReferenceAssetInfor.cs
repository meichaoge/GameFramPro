using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 被引用的资源信息
    /// </summary>
    [System.Serializable]
    public class BaseBeReferenceAssetInfor
    {
        /// <summary>
        ///  被引用的资源的实例ID
        /// </summary>
        public int ReferenceInstanceID;//{ get; protected set; }

        /// <summary>
        ///  被引用的资源的类型
        /// </summary>
        public Type ReferenceAssetType;// { get; protected set; }

        public UnityEngine.Object ReferenceAsset;// { get; protected set; }



        public BaseBeReferenceAssetInfor() { }

        public BaseBeReferenceAssetInfor(UnityEngine.Object asset, Type type)
        {
            ReferenceAsset = asset;
            ReferenceAssetType = type;
            if (asset != null)
                ReferenceInstanceID = asset.GetInstanceID();
        }



    }
}