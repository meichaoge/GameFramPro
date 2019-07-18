using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.Networking;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// AssetBundle 资源管理器
    /// </summary>
    public class AssetBundleManager : Single<AssetBundleManager>, IAssetManager
    {

        private static string s_LocalAssetBundleTopDirectoryPath = string.Empty;
        public static string S_LocalAssetBundleTopDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_LocalAssetBundleTopDirectoryPath))
                {
                    s_LocalAssetBundleTopDirectoryPath = Application.persistentDataPath.CombinePathEx(ConstDefine.S_LocalStoreDirectoryName).
                        CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                }
                return s_LocalAssetBundleTopDirectoryPath;
            }
        } //本地AssetBundle 顶层存储的路径

        #region Date
        private Dictionary<string, AssetBundleAssetDepdenceRecord> mAllLoadAssetBundleCache = new Dictionary<string, AssetBundleAssetDepdenceRecord>(50); //所有加载的AssetBundle 资源
        private NativeObjectPool<AssetBundleAssetDepdenceRecord> mAssetBundleRecordPoolMgr;


        private Dictionary<string, AssetBundleSubAssetLoadRecord> mAllLoadedAssetBundleSubAssetRecord = new Dictionary<string, AssetBundleSubAssetLoadRecord>(50); //所有加载的AssetBundle 包里面的资源
        private NativeObjectPool<AssetBundleSubAssetLoadRecord> mAssetBundleLoadSubAssetRecordPoolMgr;

        private HashSet<int> mAllAssetBundleLoadAssetInstanceIds = new HashSet<int>();

#if UNITY_EDITOR
        public Dictionary<string, AssetBundleAssetDepdenceRecord> Debug_mAllLoadAssetBundleCache { get { return mAllLoadAssetBundleCache; } }
        public Dictionary<string, AssetBundleSubAssetLoadRecord> Debug_mAllLoadedAssetBundleSubAssetRecord { get { return mAllLoadedAssetBundleSubAssetRecord; } }

#endif

        //private AssetBundleManifest mAssetBundleManifest = null;
        //public AssetBundleManifest AssetBundleManifestInfor
        //{
        //    get
        //    {
        //        if (mAssetBundleManifest == null)
        //        {
        //            string AssetBundleManifestPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName) + ConstDefine.S_AssetBundleManifestExtension;
        //            AssetBundle mainAssetBundle = LoadAssetBundleSync(AssetBundleManifestPath);
        //            if (mainAssetBundle == null)
        //                return null;
        //            mAssetBundleManifest = mainAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        //        }
        //        return mAssetBundleManifest;
        //    }
        //}

        #endregion


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedAssetBundleLoad();
        }

        #region IAssetManager 接口实现

        private void InitialedAssetBundleLoad()
        {
            mAssetBundleRecordPoolMgr = new NativeObjectPool<AssetBundleAssetDepdenceRecord>(50, OnBeforGetAssetBundleLoadRecord, OnBeforRecycleAsssetBundleLoadRecord);
            mAssetBundleLoadSubAssetRecordPoolMgr = new NativeObjectPool<AssetBundleSubAssetLoadRecord>(50, OnBeforGetAssetBundleLoadSubAssetRecord, OnBeforRecycleAsssetBundleLoadSubAssetRecord);
        }

        private void OnBeforGetAssetBundleLoadRecord(AssetBundleAssetDepdenceRecord record)
        {

        }

        private void OnBeforRecycleAsssetBundleLoadRecord(AssetBundleAssetDepdenceRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        private void OnBeforGetAssetBundleLoadSubAssetRecord(AssetBundleSubAssetLoadRecord record)
        {

        }

        private void OnBeforRecycleAsssetBundleLoadSubAssetRecord(AssetBundleSubAssetLoadRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        public float MaxAliveTimeAfterNoReference { get { return 600; } } //最多存在10分钟

        public void NotifyAssetNoReference(ILoadAssetRecord infor)
        {
            if (infor == null) return;
            if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(infor.AssetPath))
            {
                mAllLoadedAssetBundleSubAssetRecord.Remove(infor.AssetPath);
                AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(infor);
            }
            else
            {
                Debug.LogError("NotifyAssetNoReference Error !!" + infor.AssetPath);
            }
        }

        public void MarkTargetAssetNull(ILoadAssetRecord record)
        {
            if (record == null) return;
            AssetBundleSubAssetLoadRecord recordInfor = null;
            string key = record.AssetPath;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(key, out recordInfor))
            {
                mAssetBundleLoadSubAssetRecordPoolMgr.RecycleItemToPool(recordInfor);
                mAllLoadedAssetBundleSubAssetRecord.Remove(key);
            }
        }
        #endregion

        /// <summary>
        /// 加载缓存的AssetBundle
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        private Object LoadAssetFromCache(string assetName)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetName, out record))
            {
                if (record.TargetAsset != null)
                {
                    record.AddReference();
                    return record.TargetAsset;
                }
            }
            return null;
        }



        #region 加载AssetBundle 以及依赖资源 并记录
        /// <summary>
        /// 加载AssetBundle 资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public AssetBundle LoadAssetBundleSync(string assetBundlePath, out AssetBundleAssetDepdenceRecord assetBundleRecord)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetBundleSync Fail,Parameter is null");
            }


             assetBundleRecord = null;
            #region 缓存中取

            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out assetBundleRecord))
            {
                if (assetBundleRecord != null && assetBundleRecord.TargetAsset != null)
                {
                    assetBundleRecord.AddReference();
                    return assetBundleRecord.TargetAsset as AssetBundle;
                }
                else
                {
                    mAssetBundleRecordPoolMgr.RecycleItemToPool(assetBundleRecord);
                    mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
                }
            }
            #endregion

            #region 依赖加载AssetBundle 并递归记录依赖关系

            assetBundleRecord = mAssetBundleRecordPoolMgr.GetItemFromPool();

            string[] depdenceAssetBundle = AssetBundleUpgradeManager.S_Instance.GetAllDependencies(assetBundlePath);
            foreach (var depdence in depdenceAssetBundle)
            {
                AssetBundleAssetDepdenceRecord depdenceRecord =null;
                LoadAssetBundleSync(depdence, out depdenceRecord);
                assetBundleRecord.AddDepdence(depdenceRecord);
            }

            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle == null)
            {
                Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                assetBundleRecord.ReduceReference();
                return null;
            }//TODO 这里需要处理下  TODO

            assetBundleRecord.Initial(assetBundlePath,LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundle,this);
            mAllLoadAssetBundleCache[assetBundlePath] = assetBundleRecord;
            #endregion
            return assetBundle;
        }

        /// <summary>
        /// 加载AssetBundle 资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void  LoadAssetBundleAsync(string assetBundlePath, Action<AssetBundleAssetDepdenceRecord,AssetBundle>  assetBundleDepdence)
        {
            AssetBundleAssetDepdenceRecord record = null;
            #region 缓存中取

            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out record))
            {
                if (record != null && record.TargetAsset != null)
                {
                    record.AddReference();
                    if (assetBundleDepdence != null)
                        assetBundleDepdence(record, record.TargetAsset as AssetBundle);
                    return;
                }
                else
                {
                    mAssetBundleRecordPoolMgr.RecycleItemToPool(record);
                    mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
                }
            }
            #endregion

            #region 依赖加载AssetBundle 并递归记录依赖关系

            record = mAssetBundleRecordPoolMgr.GetItemFromPool();

            string[] depdenceAssetBundle = AssetBundleUpgradeManager.S_Instance.GetAllDependencies(assetBundlePath);
            foreach (var depdence in depdenceAssetBundle)
            {
                LoadAssetBundleAsync(depdence, (depdenceRecord,bundle)=> { record.AddDepdence(depdenceRecord); });
            }

            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle == null)
            {
                Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                record.ReduceReference();
                if (assetBundleDepdence != null)
                    assetBundleDepdence(record,null);
                return ;
            }//TODO 这里需要处理下  TODO

            record.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundle, this);
            mAllLoadAssetBundleCache[assetBundlePath] = record;
            #endregion
            if (assetBundleDepdence != null)
                assetBundleDepdence(record, assetBundle);
        }



        #endregion



        #region 同步加载本地的 AssetBundle

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAssetSync(string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            Object assetObject = LoadAssetFromCache(assetName);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }

            AssetBundleAssetDepdenceRecord record = null;
            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath, out record);
            if (assetBundle != null)
            {
                Object asset = assetBundle.LoadAsset(assetName);
                RecordAssetBundleLoadSubAsset(assetBundlePath, assetName, asset, assetBundle);
                if (loadCallback != null)
                    loadCallback(asset);
                return;
            }
        }
        public UnityEngine.Object LoadAssetSync(string assetBundlePath, string assetName)
        {
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            Object assetObject = LoadAssetFromCache(assetName);
            if (assetObject != null)
                return assetObject;

            AssetBundleAssetDepdenceRecord record = null;
            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath, out record);
            if (assetBundle != null)
            {
                assetObject = assetBundle.LoadAsset(assetName);
                RecordAssetBundleLoadSubAsset(assetBundlePath, assetName, assetObject, assetBundle);
                return assetObject;
            }
            return null;
        }


        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAssetAsync(string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            Object assetObject = LoadAssetFromCache(assetName);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }

            LoadAssetBundleAsync(assetBundlePath, (record, assetBundle) =>
            {
                if (assetBundle != null)
                {
                    Object asset = assetBundle.LoadAsset(assetName);
                    RecordAssetBundleLoadSubAsset(assetBundlePath, assetName, asset, assetBundle);
                    if (loadCallback != null)
                        loadCallback(asset);
                    return;
                }
            });

        }

        #endregion





        #region 内部加载AssetBundle 方法，对 AssetBundle类加载方式的封装

        private AssetBundle LoadAssetBundleSync(string assetBundlePath)
        {
            return AssetBundle.LoadFromFile(assetBundlePath);
        }
        private AssetBundle LoadAssetBundleSync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes);
        }
        private AssetBundle LoadAssetBundleSync(string assetBundlePath, uint crc)
        {
            return AssetBundle.LoadFromFile(assetBundlePath, crc);
        }
        private AssetBundle LoadAssetBundleSync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes, crc);
        }


        private AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundlePath)
        {
            return AssetBundle.LoadFromFileAsync(assetBundlePath);
        }
        private AssetBundleCreateRequest LoadAssetBundleAsync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes);
        }
        private AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundlePath, uint crc)
        {
            return AssetBundle.LoadFromFileAsync(assetBundlePath, crc);
        }
        private AssetBundleCreateRequest LoadAssetBundleAsync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes, crc);
        }


        #endregion



        #region 辅助
        /// <summary>
        /// 保存下载的AssetBundel 资源
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="assetBundleName"></param>
        public void SaveAssetBundleFromDownload(DownloadHandlerBuffer handle, string assetBundleName)
        {
            if (handle == null)
                return;
            string fileSavePath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleName);
            IOUtility.CreateOrSetFileContent(fileSavePath, handle.data, false);
            Debug.LogInfor("保存下载的AssetBundel 资源 " + fileSavePath);
        }

        /// <summary>
        /// 根据路劲获取已经加载的 AssetBundle 可能得到空结果
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        private AssetBundle GetAssetBundleByPath(string assetBundlePath)
        {
            AssetBundleAssetDepdenceRecord assetBundleRecord = null;
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out assetBundleRecord))
            {
                if (assetBundleRecord != null && assetBundleRecord.TargetAsset!=null)
                {
                    assetBundleRecord.AddReference();
                    return assetBundleRecord.TargetAsset as AssetBundle;
                }
                Debug.LogInfor(string.Format("GetAssetBundleByPath Fail，AssetBundle={0} 资源已经被卸载了", assetBundlePath));
                mAllLoadAssetBundleCache.Remove(assetBundlePath);
            }
            return null;
        }

     

        /// <summary>
        /// 记录从AssetBundel 加载的资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <param name="parentAssetBundle"></param>
        private void RecordAssetBundleLoadSubAsset(string assetBundlePath, string assetName, Object asset, AssetBundle parentAssetBundle)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetName, out record))
            {
                record.AddReference();
                return;
            }

            record = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetBundlePath) as AssetBundleSubAssetLoadRecord;
            if (record == null)
                record = mAssetBundleLoadSubAssetRecordPoolMgr.GetItemFromPool();
            record.Initial(assetBundlePath, assetName, LoadedAssetTypeEnum.AssetBundle_UnKnown, asset, this, assetBundlePath);

            mAllLoadedAssetBundleSubAssetRecord[assetName] = record;
        }

        //记录加载的AssetBundle
        private void RecordAssetBundleLoad(string assetBundlePath,  AssetBundle targetAssetBundle )
        {
            AssetBundleAssetDepdenceRecord record = null;
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out record))
            {
                record.AddReference();
                return;
            }

            record = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetBundlePath) as AssetBundleAssetDepdenceRecord;
            if (record == null)
                record = mAssetBundleRecordPoolMgr.GetItemFromPool();
            record.Initial(assetBundlePath,  LoadedAssetTypeEnum.AssetBundle_UnKnown, targetAssetBundle, this);

            mAllLoadAssetBundleCache[assetBundlePath] = record;
        }
        #endregion


    }
}