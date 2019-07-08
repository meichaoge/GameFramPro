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
        int MaxAliveTimeAfterNoReference { get; }

        /// <summary>
        /// 收到资源没有引用的消息
        /// </summary>
        /// <param name="infor"></param>
        void NotifyAssetNoReference(BaseLoadedAssetInfor infor);

        /// <summary>
        /// 强制删除一个对象
        /// </summary>
        /// <param name="infor"></param>
        void NotifyAssetForceDelete(BaseLoadedAssetInfor infor);
    }
}