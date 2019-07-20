using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// Resources&AssetBundl&LocalStore 都必须继承这个接口
    /// </summary>
    public interface IAssetManager
    {
        /// <summary>
        /// 被标记为无引用后最多存在这个长时间(单位秒)就要被销毁
        /// </summary>
        float MaxAliveTimeAfterNoReference { get; }


        /// <summary>
        /// 管理的资源引用次数改变
        /// </summary>
        /// <param name="record"></param>
        void NotifyAssetReferenceChange(ILoadAssetRecord record);

        /// <summary>
        /// 一个资源没有被引用一段时间后彻底释放自己 需要删除记录
        /// </summary>
        /// <param name="record"></param>
        void NotifyAssetRelease (ILoadAssetRecord record);

    }
}