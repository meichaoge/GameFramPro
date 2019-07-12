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
        /// 收到资源没有引用的消息(可能是强制删除了)
        /// </summary>
        /// <param name="infor"></param>
        void NotifyAssetNoReference(ILoadAssetRecord infor);

        /// <summary>
        /// 当一个记录的引用对应已经被销毁时候仍然增加或者减少引用时候调用
        /// </summary>
        /// <param name="record"></param>
        void MarkTargetAssetNull(ILoadAssetRecord record);

    }
}