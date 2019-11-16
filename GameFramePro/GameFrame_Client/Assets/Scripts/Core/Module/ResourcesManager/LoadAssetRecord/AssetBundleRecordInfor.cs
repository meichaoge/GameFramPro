using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 所有加载的AssetBundle 信息
    /// </summary>
    [System.Serializable]
    internal sealed class AssetBundleRecordInfor
    {
        /// <summary>
        /// 被创建的资源引用的次数 (间接依赖+直接依赖)
        /// </summary>
        public int mTotalReferenceCount { get { return mDependencesOtherABundleRecords.Count + mBeDependencesABundleRecord.Count; } }


        public string mAssetBundleNameUri { get; private set; } //得到AssetBundle 的文件名称
        private AssetBundle mAssetBundle { get; set; }

        public bool IsAssetBundleEnable { get { return mAssetBundle != null; } }
        public int mAssetBundleInstanceID { get { return mAssetBundle == null ? -1 : mAssetBundle.GetInstanceID(); } }


        /// <summary>
        /// 所有依赖的 其他 AssetBundle 记录
        /// </summary>
        private Dictionary<string, AssetBundleRecordInfor> mDependencesOtherABundleRecords { get; set; } = new Dictionary<string, AssetBundleRecordInfor>(10);

        /// <summary>
        /// 所有被其他AssetBundle 依赖的记录(相当于被其他创建的资源间接依赖)
        /// </summary>
        private Dictionary<string, AssetBundleRecordInfor> mBeDependencesABundleRecord { get; set; } = new Dictionary<string, AssetBundleRecordInfor>(10);

        /// <summary>
        /// 所有从当前这个AssetBundle 中加载的资源记录 (直接依赖)
        /// </summary>
        private Dictionary<string, LoadAssetBundleAssetRecord> mABundleLoadAssetRecords { get; set; } = new Dictionary<string, LoadAssetBundleAssetRecord>(10);


        #region 构造函数

        static AssetBundleRecordInfor()
        {
            s_AssetBundleRecordInforrPoolMgr = new NativeObjectPool<AssetBundleRecordInfor>(50, null, OnBeforeRecycleAssetBundleRecordInfor);
        }

        public AssetBundleRecordInfor()
        {
        }

        #endregion

        #region 对象池

        private static NativeObjectPool<AssetBundleRecordInfor> s_AssetBundleRecordInforrPoolMgr;

        //private static void OnBeforeGeAssetBundleRecordInfor(AssetBundleRecordInfor record)
        //{
        //}

        private static void OnBeforeRecycleAssetBundleRecordInfor(AssetBundleRecordInfor record)
        {
            if (record == null) return;
    
            //移除被依赖
            foreach (var beReferenceABundle in record.mBeDependencesABundleRecord.Values)
            {
                if (beReferenceABundle == null) continue;
                beReferenceABundle.RemoveDependenceAssetBudleRecord(record);
            }
            record.mBeDependencesABundleRecord.Clear();

            //移除加载的资源
            foreach (var assetBundleAssetRecord in record.mABundleLoadAssetRecords.Values)
            {
                if (assetBundleAssetRecord == null) continue;
                LoadAssetBundleAssetRecord.ReleaseAssetBundleRecordInfor(assetBundleAssetRecord);
            }
            record.mABundleLoadAssetRecords.Clear();

            //移除依赖
            foreach (var dependencesAssetBundleRecord in record.mDependencesOtherABundleRecords.Values)
            {
                if (dependencesAssetBundleRecord == null) continue;
                dependencesAssetBundleRecord.NotifyRemoveBeReferenceByABundleRecord(record);
            }
            record.mDependencesOtherABundleRecords.Clear();


            ResourcesManagerUtility.UnLoadAssetBundle(record.mAssetBundle, false);
            record.mAssetBundle = null;
        }


        /// <summary>
        /// 获取 AssetBundleRecordInfor 实例对象
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static AssetBundleRecordInfor GetAssetBundleRecordInfor(string assetBundleNameUri, AssetBundle assetBundle)
        {
            var assetBundleRecordInfor = s_AssetBundleRecordInforrPoolMgr.GetItemFromPool();
            assetBundleRecordInfor.mAssetBundleNameUri = assetBundleNameUri;
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


        #region 加载AssetBundle 中指定的资源

        /// <summary>
        /// 加载AssetBundle 中指定的资源
        /// </summary>
        /// <param name="assetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName) where T : Object
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError($"加载AssetBundle 中资源失败，参数为null ");
                return null;
            }

            if (mAssetBundle == null)
                return null;
            return mAssetBundle.LoadAsset<T>(assetName);
        }

        #endregion



        #region AssetBundle 之间的依赖处理+ 加载的资源对AssetBundle 的依赖

        /// <summary>
        /// 增加 依赖其他的AssetBundle记录 (依赖的AsetBundle 也会记录自己)
        /// </summary>
        /// <param name="otherABundleRecord"></param>
        public void AddDependenceAssetBudleRecord(AssetBundleRecordInfor otherABundleRecord)
        {
            if (IsAssetBundleEnable==false || otherABundleRecord == null || otherABundleRecord.IsAssetBundleEnable == false)
                return;

            otherABundleRecord.NotifyAddBeReferenceByABundleRecord(this); //通知其他的记录被依赖

            if (mDependencesOtherABundleRecords.TryGetValue(otherABundleRecord.mAssetBundleNameUri, out var dependenceAssetBundle) || dependenceAssetBundle != null && dependenceAssetBundle.IsAssetBundleEnable)
                return;

            if (dependenceAssetBundle != null && dependenceAssetBundle.IsAssetBundleEnable == false)
                ReleaseAssetBundleRecordInfor(dependenceAssetBundle);
            mDependencesOtherABundleRecords[otherABundleRecord.mAssetBundleNameUri] = otherABundleRecord;
        }

        /// <summary>
        /// 减少 依赖其他的AssetBundle记录
        /// </summary>
        /// <param name="otherDependenceAssetBundleRecord"></param>
        public void RemoveDependenceAssetBudleRecord(AssetBundleRecordInfor otherABundleRecord)
        {
            if (otherABundleRecord == null)
                return;
            otherABundleRecord.NotifyRemoveBeReferenceByABundleRecord(this); //通知其他的记录被移除依赖

            mDependencesOtherABundleRecords.Remove(otherABundleRecord.mAssetBundleNameUri);
        }



        /// <summary>
        /// 通知 增加自己被其他的AssetBundle 资源引用
        /// </summary>
        /// <param name="beReferenceAssetBundleRecordInfor"></param>
        private void NotifyAddBeReferenceByABundleRecord(AssetBundleRecordInfor beReferenceAssetBundleRecordInfor)
        {
            if (beReferenceAssetBundleRecordInfor == null || beReferenceAssetBundleRecordInfor.IsAssetBundleEnable == false)
                return;

            mBeDependencesABundleRecord[beReferenceAssetBundleRecordInfor.mAssetBundleNameUri] = beReferenceAssetBundleRecordInfor;
        }

        /// 通知 去除自己被其他的AssetBundle 资源引用
        /// </summary>
        /// <param name="beReferenceAssetBundleRecordInfor"></param>
        private void NotifyRemoveBeReferenceByABundleRecord(AssetBundleRecordInfor beReferenceAssetBundleRecordInfor)
        {
            if (beReferenceAssetBundleRecordInfor == null || beReferenceAssetBundleRecordInfor.IsAssetBundleEnable == false)
                return;

            mBeDependencesABundleRecord.Remove(beReferenceAssetBundleRecordInfor.mAssetBundleNameUri);
        }

        #endregion

        #region 记录从当前AssetBundle 加载的资源
        /// <summary>
        /// 通知 有从当前AssetBundle 中加载的资源依赖当前对象
        /// </summary>
        /// <param name="loadAssetBundleAssetRecord"></param>
        public void NotifyAddBeReferenceByLoadAsset(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null || loadAssetBundleAssetRecord.IsRecordEnable == false)
                return;
            mABundleLoadAssetRecords[loadAssetBundleAssetRecord.mAssetFullUri] = loadAssetBundleAssetRecord;
        }

        /// <summary>
        /// 通知 取消从当前AssetBundle 中加载的资源依赖当前对象
        /// </summary>
        /// <param name="loadAssetBundleAssetRecord"></param>
        public void NotifyRemoveBeReferenceByLoadAsset(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null || loadAssetBundleAssetRecord.IsRecordEnable == false)
                return;
            mABundleLoadAssetRecords.Remove(loadAssetBundleAssetRecord.mAssetFullUri);
        }

        #endregion
    }
}