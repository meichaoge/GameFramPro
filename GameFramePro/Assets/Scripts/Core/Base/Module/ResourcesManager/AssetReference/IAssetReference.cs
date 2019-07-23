using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.ResourcesEx;

namespace GameFramePro
{

    /// <summary>
    /// 记录资源被引用情况
    /// </summary>
    public interface IAssetReference
    {
        /// <summary>
        /// 标示当前引用哪个资源
        /// </summary>
        ILoadAssetRecord CurLoadAssetRecord { get; }
   
        /// <summary>
        /// 被引用的资源信息
        /// </summary>
        BaseBeReferenceAssetInfor ReferenceAssetInfor { get; }

        //#region   编辑器下显示数据
        //void UpdateView();
        //#endregion
    }
}