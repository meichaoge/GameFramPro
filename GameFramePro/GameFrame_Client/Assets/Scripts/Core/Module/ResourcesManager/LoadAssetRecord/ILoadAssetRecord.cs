using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 资源加载的来源
    /// </summary>
    public enum LoadAssetSourceUsage
    {
        None,  //未知的需要处理
        ResourcesAsset,
        AssetBundleAsset,
    }

    /// <summary>
    /// 加载的资源记录
    /// </summary>
    public interface ILoadAssetRecord
    {
        /// <summary>
        ///完整的资源路径
        /// </summary>
        string mAssetFullUri { get; }

        /// <summary>
        /// 表示这个加载记录是否有效(资源是为null)
        /// </summary>
        bool IsRecordEnable { get; }

        /// <summary>
        /// 没有引用关系后最多存活的时间(单位秒) 小于0表示不会被自动销毁
        /// </summary>
        float mMaxAliveAfterNoReference { get; }

        /// <summary>
        /// 资源的加载来源
        /// </summary>
        LoadAssetSourceUsage mLoadAssetSourceUsage { get; }

     




        /// <summary>
        /// 获取加载的资源对象
        /// </summary>
        /// <returns></returns>
        Object GetLoadAsset();

        /// <summary>
        /// 获得加载资源的实例ID
        /// </summary>
        /// <returns></returns>
        int GetLoadAssetInstanceID();

        /// <summary>
        /// 清理释放加载的资源信息
        /// </summary>
        void ReleaseLoadAssetRecord();

    }
}