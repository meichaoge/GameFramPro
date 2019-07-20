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

        //key=需要加载的资源的详细路径 不能是资源名
        private Dictionary<string, AssetBundleSubAssetLoadRecord> mAllLoadedAssetBundleSubAssetRecord = new Dictionary<string, AssetBundleSubAssetLoadRecord>(50); //所有加载的AssetBundle 包里面的资源
        private NativeObjectPool<AssetBundleSubAssetLoadRecord> mAssetBundleLoadSubAssetRecordPoolMgr;

        //value  为上面的key
        private Dictionary<int,string > mAllAssetBundleLoadAssetInstanceIds = new Dictionary<int, string>();

#if UNITY_EDITOR
        public Dictionary<string, AssetBundleAssetDepdenceRecord> Debug_mAllLoadAssetBundleCache { get { return mAllLoadAssetBundleCache; } }
        public Dictionary<string, AssetBundleSubAssetLoadRecord> Debug_mAllLoadedAssetBundleSubAssetRecord { get { return mAllLoadedAssetBundleSubAssetRecord; } }

#endif

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

        #region 资源加载


        #region 加载缓存的资源

        public Object LoadAssetFromCache(string assetPath) { return LoadAssetFromCache<Object>(assetPath); }

        /// <summary>
        /// 加载缓存的AssetBundle
        /// </summary>
        /// <param name="assetPath">资源相对于Resources的完整路径</param>
        /// <returns></returns>
        public T LoadAssetFromCache<T>(string assetPath) where T : UnityEngine.Object
        {
            AssetBundleSubAssetLoadRecord record = null;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out record))
            {
                if (record.TargetAsset != null)
                {
                    record.AddReference();
                    return record.TargetAsset as T;
                }
            }
            return null;
        }

        #endregion

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
                AssetBundleAssetDepdenceRecord depdenceRecord = null;
                LoadAssetBundleSync(depdence, out depdenceRecord);
                assetBundleRecord.AddDepdence(depdenceRecord);
            }

            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle == null)
            {
                Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                assetBundleRecord.ClearAllDepdence();
                mAssetBundleRecordPoolMgr.RecycleItemToPool(assetBundleRecord);
                return null;
            }//当前AssetBundle 不存在则需要清理申请的资源

            assetBundleRecord.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundle, this);
            mAllLoadAssetBundleCache[assetBundlePath] = assetBundleRecord;
            #endregion
            return assetBundle;
        }

        /// <summary>
        /// 异步加载AssetBundle 资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void LoadAssetBundleAsync(string assetBundlePath, Action<AssetBundleAssetDepdenceRecord, AssetBundle> assetBundleDepdence)
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
                    if (record != null)
                    {
                        record.ClearAllDepdence();
                        record.ReduceReference(true);
                        mAssetBundleRecordPoolMgr.RecycleItemToPool(record);
                    }//释放资源
                    mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
                }
            }
            #endregion

            #region 依赖加载AssetBundle 并递归记录依赖关系

            record = mAssetBundleRecordPoolMgr.GetItemFromPool();

            string[] depdenceAssetBundle = AssetBundleUpgradeManager.S_Instance.GetAllDependencies(assetBundlePath);
            foreach (var depdence in depdenceAssetBundle)
            {
                LoadAssetBundleAsync(depdence, (depdenceRecord, bundle) =>
                 {
                     record.AddDepdence(depdenceRecord);
                 });
            }

            #endregion

            AssetBundleCreateRequest requst = LoadAssetBundleAsync(assetBundlePath);
            AsyncManager.S_Instance.StartAsyncOperation(requst, (asyncOperation) =>
            {
                var result = asyncOperation as AssetBundleCreateRequest;
                if (result.assetBundle == null)
                {
                    record.ClearAllDepdence();
                    record.ReduceReference(true);
                    mAssetBundleRecordPoolMgr.RecycleItemToPool(record);
                    Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);

                    if (assetBundleDepdence != null)
                        assetBundleDepdence(null, null);
                    return;
                }

                record.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, result.assetBundle, this);
                mAllLoadAssetBundleCache[assetBundlePath] = record;

                if (assetBundleDepdence != null)
                    assetBundleDepdence(record, result.assetBundle);

            }, null);
        }

        #endregion


        #region 同步加载本地的 AssetBundle

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <typeparam name="T">资源相对于Resouces 完整的路径</typeparam>
        /// <param name="assetPath"></param>
        /// <param name="loadCallback"></param>
        public void LoadAssetSync<T>(string assetPath, Action<T> loadCallback) where T : UnityEngine.Object
        {
            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetSync Fail,Not Exit asset " + assetPath);
                if (loadCallback != null)
                    loadCallback(null);
                return;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            LoadAssetSync<T>(assetPath, assetBundlePath, assetName, loadCallback, false);
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        public void LoadAssetSync<T>(string assetPath, string assetBundlePath, string assetName, Action<T> loadCallback,bool isNeedTranslateAssetName=true) where T : UnityEngine.Object
        {
            T assetObject = LoadAssetFromCache<T>(assetPath);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }
            if (isNeedTranslateAssetName)
                assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                if (loadCallback != null) loadCallback(null);
                return;
            }


            AssetBundleAssetDepdenceRecord record = null;
            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath, out record);
            if (assetBundle != null)
            {
                T asset = assetBundle.LoadAsset<T>(assetName);
                RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetName, asset, assetBundle);
                if (loadCallback != null)
                    loadCallback(asset as T);
                return;
            }
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        /// <returns></returns>
        public T LoadAssetSync<T>(string assetPath, string assetBundlePath, string assetName, bool isNeedTranslateAssetName = true) where T: UnityEngine.Object
        {
            T assetObject = LoadAssetFromCache<T>(assetPath);
            if (assetObject != null)
                return assetObject;

            if(isNeedTranslateAssetName)
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                return null;
            }

            AssetBundleAssetDepdenceRecord record = null;
            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath, out record);
            if (assetBundle != null)
            {
                assetObject = assetBundle.LoadAsset<T>(assetName);
                RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetName, assetObject, assetBundle);
                return assetObject;
            }
            return null;
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        /// <returns></returns>
        public T LoadAssetSync<T>(string assetPath) where T : UnityEngine.Object
        {
            T assetObject = LoadAssetFromCache<T>(assetPath);
            if (assetObject != null)
                return assetObject;

            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetSync Fail,Not Exit asset " + assetPath);
                return null;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            return LoadAssetSync<T>(assetPath, assetBundlePath, assetName,false);
        }
        #endregion

        #region 异步
        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAssetAsync<T>(string assetPath, Action<T> loadCallback) where T : UnityEngine.Object
        {
            T assetObject = LoadAssetFromCache<T>(assetPath);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }
            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetAsync Fail,Not Exit asset " + assetPath);
                if (loadCallback != null)
                    loadCallback(null);
                return ;
            }
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            LoadAssetAsync<T>(assetPath, assetBundlePath, assetName, loadCallback,false);
        }



        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        public void LoadAssetAsync<T>(string assetPath, string assetBundlePath, string assetName, Action<T> loadCallback, bool isNeedTranslateAssetName = true) where T: UnityEngine.Object
        {
            T assetObject = LoadAssetFromCache<T>(assetPath);
            if (assetObject != null)
            {
                if (loadCallback != null)
                    loadCallback(assetObject);
                return;
            }
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            LoadAssetBundleAsync(assetBundlePath, (record, assetBundle) =>
            {
                if (assetBundle != null)
                {
                    T asset = assetBundle.LoadAsset<T>(assetName);
                    RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetName, asset, assetBundle);
                    if (loadCallback != null)
                        loadCallback(asset);
                    return;
                }
            });
        }

        #endregion

        #region 非泛型的同步、异步方法
        public void LoadAssetSync(string assetPath, string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            LoadAssetSync<UnityEngine.Object>(assetPath, assetBundlePath, assetName, loadCallback);
        }

        public UnityEngine.Object LoadAssetSync(string assetPath, string assetBundlePath, string assetName)
        {
            return LoadAssetSync<UnityEngine.Object>(assetPath, assetBundlePath, assetName);
        }

        public void LoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<UnityEngine.Object> loadCallback)
        {
            LoadAssetAsync<UnityEngine.Object>(assetPath, assetBundlePath, assetName, loadCallback);
        }
        #endregion

        #endregion

        #region 资源释放
        /// <summary>
        /// 较少某个加载资源的引用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asset"></param>
        /// <returns>如果成功则返回true</returns>
        public bool ReleaseReference<T>(T asset) where T : UnityEngine.Object
        {
            if (asset == null) return false;
            int instanceID = asset.GetInstanceID();
            string recordKey = string.Empty;
            if (mAllAssetBundleLoadAssetInstanceIds.TryGetValue(instanceID, out recordKey))
            {
                AssetBundleSubAssetLoadRecord record = null;
                if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(recordKey, out record))
                {
                    record.ReduceReference();
                    return true;
                }
                Debug.LogError("数据记录不一致 " + recordKey);
                return false;
            }
            return false;
        }
        #endregion

        #region 辅助

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
                if (assetBundleRecord != null && assetBundleRecord.TargetAsset != null)
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
        private void RecordAssetBundleLoadSubAsset(string assetPath, string assetBundlePath, string assetName, Object asset, AssetBundle parentAssetBundle)
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

            mAllLoadedAssetBundleSubAssetRecord[assetPath] = record;

            mAllAssetBundleLoadAssetInstanceIds[record.InstanceID] = assetPath;
        }

        ////记录加载的AssetBundle
        //private void RecordAssetBundleLoad(string assetBundlePath, AssetBundle targetAssetBundle)
        //{
        //    AssetBundleAssetDepdenceRecord record = null;
        //    if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out record))
        //    {
        //        record.AddReference();
        //        return;
        //    }

        //    record = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetBundlePath) as AssetBundleAssetDepdenceRecord;
        //    if (record == null)
        //        record = mAssetBundleRecordPoolMgr.GetItemFromPool();
        //    record.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, targetAssetBundle, this);

        //    mAllLoadAssetBundleCache[assetBundlePath] = record;
        //}
        #endregion


    }
}