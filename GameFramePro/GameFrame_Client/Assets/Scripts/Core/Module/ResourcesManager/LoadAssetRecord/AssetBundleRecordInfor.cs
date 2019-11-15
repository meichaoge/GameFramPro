using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 所有加载的AssetBundle 信息
    /// </summary>
    [System.Serializable]
    public sealed class AssetBundleRecordInfor
    {
        /// <summary>
        /// 被创建的资源引用的次数 (间接依赖+直接依赖)
        /// </summary>
        public int mTotalReferenceCount
        {
            get { return mAllBeDependencesAssetBundleRecordInfors.Count + mAllAssetBundleLoadAssetRecords.Count; }
        }


        public string mAssetBundleNameUri { get; private set; } //得到AssetBundle 的文件名称
        private AssetBundle mAssetBundle { get; set; }

        public bool IsAssetBundleEnable
        {
            get { return mAssetBundle != null; }
        }
        public int mAssetBundleInstanceID
        {
            get { return mAssetBundle == null ? -1 : mAssetBundle.GetInstanceID(); }
        }


        /// <summary>
        /// 所有依赖的 其他 AssetBundle 记录
        /// </summary>
        private Dictionary<string, AssetBundleRecordInfor> mAllDependencesAssetBundleRecordInfors { get; set; } = new Dictionary<string, AssetBundleRecordInfor>(10);

        /// <summary>
        /// 所有被其他AssetBundle 依赖的记录(相当于被其他创建的资源间接依赖)
        /// </summary>
        private Dictionary<string, AssetBundleRecordInfor> mAllBeDependencesAssetBundleRecordInfors { get; set; } = new Dictionary<string, AssetBundleRecordInfor>(10);

        /// <summary>
        /// 所有从当前这个AssetBundle 中加载的资源记录 (直接依赖)
        /// </summary>
        private Dictionary<string, LoadAssetBundleAssetRecord> mAllAssetBundleLoadAssetRecords { get; set; } = new Dictionary<string, LoadAssetBundleAssetRecord>(10);



        #region 对象池

        private static NativeObjectPool<AssetBundleRecordInfor> s_AssetBundleRecordInforrPoolMgr;

        private static void OnBeforeGeAssetBundleRecordInfor(AssetBundleRecordInfor record)
        {
        }

        private static void OnBeforeRecycleAssetBundleRecordInfor(AssetBundleRecordInfor record)
        {
            if (record == null) return;
            ResourcesManager.UnLoadAssetBundle(record.mAssetBundle, false);
            record.mAssetBundle = null;
            record.mAllDependencesAssetBundleRecordInfors.Clear();
            record.mAllBeDependencesAssetBundleRecordInfors.Clear();

            foreach (var assetBundleAssetRecord in record.mAllAssetBundleLoadAssetRecords.Values)
            {
                if (assetBundleAssetRecord == null) continue;
                LoadAssetBundleAssetRecord.ReleaseAssetBundleRecordInfor(assetBundleAssetRecord);
            }

            record.mAllAssetBundleLoadAssetRecords.Clear();
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


        #region 构造函数

        static AssetBundleRecordInfor()
        {
            s_AssetBundleRecordInforrPoolMgr = new NativeObjectPool<AssetBundleRecordInfor>(50, OnBeforeGeAssetBundleRecordInfor, OnBeforeRecycleAssetBundleRecordInfor);
        }

        public AssetBundleRecordInfor()
        {
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
        /// 增加 依赖其他的AssetBundle记录
        /// </summary>
        /// <param name="otherDependenceAssetBundleRecord"></param>
        public void AddDependenceAssetBudleRecord(AssetBundleRecordInfor otherDependenceAssetBundleRecord)
        {
            if (otherDependenceAssetBundleRecord == null || otherDependenceAssetBundleRecord.IsAssetBundleEnable == false)
                return;

            otherDependenceAssetBundleRecord.NotifyAddBeReferenceByAssetBundleRecord(this); //通知其他的记录被依赖

            if (mAllDependencesAssetBundleRecordInfors.ContainsKey(otherDependenceAssetBundleRecord.mAssetBundleNameUri))
                return;
            mAllDependencesAssetBundleRecordInfors[otherDependenceAssetBundleRecord.mAssetBundleNameUri] = otherDependenceAssetBundleRecord;
        }

        /// <summary>
        /// 减少 依赖其他的AssetBundle记录
        /// </summary>
        /// <param name="otherDependenceAssetBundleRecord"></param>
        public void RemoveDependenceAssetBudleRecord(AssetBundleRecordInfor otherDependenceAssetBundleRecord)
        {
            if (otherDependenceAssetBundleRecord == null)
                return;
            otherDependenceAssetBundleRecord.NotifyRemoveBeReferenceByAssetBundleRecord(this); //通知其他的记录被移除依赖

            mAllDependencesAssetBundleRecordInfors.Remove(otherDependenceAssetBundleRecord.mAssetBundleNameUri);
        }

        /// <summary>
        /// 减少所有 依赖其他的AssetBundle记录
        /// </summary>
        public void RemoveAllDependenceAssetBudleRecord()
        {
            foreach (var dependencesAssetBundleRecord in mAllDependencesAssetBundleRecordInfors.Values)
            {
                if (dependencesAssetBundleRecord == null) continue;
                dependencesAssetBundleRecord.NotifyRemoveBeReferenceByAssetBundleRecord(this);
            }
            mAllDependencesAssetBundleRecordInfors.Clear();
        }

        /// <summary>
        /// 通知 有从当前AssetBundle 中加载的资源依赖当前对象
        /// </summary>
        /// <param name="loadAssetBundleAssetRecord"></param>
        public void NotifyAddBeReferenceByLoadAsset(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null || loadAssetBundleAssetRecord.IsRecordEnable == false)
                return;
            mAllAssetBundleLoadAssetRecords[loadAssetBundleAssetRecord.mAssetFullUri] = loadAssetBundleAssetRecord;
        }

        /// <summary>
        /// 通知 取消从当前AssetBundle 中加载的资源依赖当前对象
        /// </summary>
        /// <param name="loadAssetBundleAssetRecord"></param>
        public void NotifyRemoveBeReferenceByLoadAsset(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null || loadAssetBundleAssetRecord.IsRecordEnable == false)
                return;
            mAllAssetBundleLoadAssetRecords.Remove(loadAssetBundleAssetRecord.mAssetFullUri);
        }


        /// <summary>
        /// 通知 增加自己被其他的AssetBundle 资源引用
        /// </summary>
        /// <param name="beReferenceAssetBundleRecordInfor"></param>
        public void NotifyAddBeReferenceByAssetBundleRecord(AssetBundleRecordInfor beReferenceAssetBundleRecordInfor)
        {
            if (beReferenceAssetBundleRecordInfor == null || beReferenceAssetBundleRecordInfor.IsAssetBundleEnable == false)
                return;

            mAllBeDependencesAssetBundleRecordInfors[beReferenceAssetBundleRecordInfor.mAssetBundleNameUri] = beReferenceAssetBundleRecordInfor;
        }

        /// 通知 去除自己被其他的AssetBundle 资源引用
        /// </summary>
        /// <param name="beReferenceAssetBundleRecordInfor"></param>
        public void NotifyRemoveBeReferenceByAssetBundleRecord(AssetBundleRecordInfor beReferenceAssetBundleRecordInfor)
        {
            if (beReferenceAssetBundleRecordInfor == null || beReferenceAssetBundleRecordInfor.IsAssetBundleEnable == false)
                return;

            mAllBeDependencesAssetBundleRecordInfors.Remove(beReferenceAssetBundleRecordInfor.mAssetBundleNameUri);
        }

        #endregion
    }
}