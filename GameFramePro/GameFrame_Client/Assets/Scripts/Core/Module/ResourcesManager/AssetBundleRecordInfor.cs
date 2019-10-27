using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 所有加载的AssetBundle 信息
    /// </summary>
    [System.Serializable]
    public class AssetBundleRecordInfor
    {
        public string mAssetBundleUri { get; set; } //得到AssetBundle 的路径
        public AssetBundle mAssetBundle { get; set; }
        public HashSet<string> mAllBeDepdencesAssetBundleUri = new HashSet<string>(); //所有依赖这个AssetBundle 的资源

        public bool IsAssetBundleEnable { get { return mAssetBundle != null; } }

        #region 对象池


        private static NativeObjectPool<AssetBundleRecordInfor> s_AssetBundleRecordInforrPoolMgr;
        private static void OnBeforeGeAssetBundleRecordInfor(AssetBundleRecordInfor record)
        {
        }

        private static void OnBeforeRecycleAssetBundleRecordInfor(AssetBundleRecordInfor record)
        {
            //     record.NotifyReleaseRecord(); //回收时候销毁引用
        }

        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <returns></returns>
        public static AssetBundleRecordInfor GetAssetBundleRecordInfor()
        {
            return s_AssetBundleRecordInforrPoolMgr.GetItemFromPool();
        }

        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static AssetBundleRecordInfor GetAssetBundleRecordInfor(string assetBundleUri, AssetBundle assetBundle)
        {
            var assetBundleRecordInfor = s_AssetBundleRecordInforrPoolMgr.GetItemFromPool();
            assetBundleRecordInfor.mAssetBundleUri = assetBundleUri;
            assetBundleRecordInfor.mAssetBundle = assetBundle;
            return assetBundleRecordInfor;
        }

        /// <summary>
        /// 释放 AssetBundleRecordInfor 对象
        /// </summary>
        /// <param name="assetBundleRecordInfor"></param>
        public static void ReleaseAssetBundleRecordInfor(AssetBundleRecordInfor assetBundleRecordInfor)
        {
            s_AssetBundleRecordInforrPoolMgr.RecycleItemToPool(assetBundleRecordInfor);
        }

        #endregion


        #region 构造函数
        static AssetBundleRecordInfor()
        {
            s_AssetBundleRecordInforrPoolMgr = new NativeObjectPool<AssetBundleRecordInfor>(50, OnBeforeGeAssetBundleRecordInfor, OnBeforeRecycleAssetBundleRecordInfor);
        }

        public AssetBundleRecordInfor() { }

        #endregion


        #region 对外接口

        /// <summary>
        /// 增加 被依赖的记录
        /// </summary>
        /// <param name="beDepdenceAssetBundle"></param>
        public void AddBeDepdenceAssetBudle(string beDepdenceAssetBundle)
        {
            if (string.IsNullOrEmpty(beDepdenceAssetBundle))
                return;

            if (mAllBeDepdencesAssetBundleUri.Contains(beDepdenceAssetBundle))
                return;
            mAllBeDepdencesAssetBundleUri.Add(beDepdenceAssetBundle);
        }

        /// <summary>
        /// 增加 被依赖的记录
        /// </summary>
        /// <param name="beDepdenceAssetBundle"></param>
        public void RemoveBeDepdenceAssetBudle(string beDepdenceAssetBundle)
        {
            if (string.IsNullOrEmpty(beDepdenceAssetBundle))
                return;
            mAllBeDepdencesAssetBundleUri.Remove(beDepdenceAssetBundle);
        }

        #endregion


    }
}