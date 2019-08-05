using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.Networking;

namespace GameFramePro.ResourcesEx
{
    /// <summary>/// AssetBundle 资源管理器/// </summary>
    public sealed class AssetBundleManager : Single<AssetBundleManager>, IAssetManager
    {
        private static string s_LocalAssetBundleTopDirectoryPath = string.Empty;

        public static string S_LocalAssetBundleTopDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(s_LocalAssetBundleTopDirectoryPath))
                {
                    s_LocalAssetBundleTopDirectoryPath = Application.persistentDataPath.CombinePathEx(ConstDefine.S_LocalStoreDirectoryName).CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                }

                return s_LocalAssetBundleTopDirectoryPath;
            }
        } //本地AssetBundle 顶层存储的路径

        #region Date

        private readonly Dictionary<string, LoadAssetBundleAssetRecord> mAllLoadAssetBundleCache = new Dictionary<string, LoadAssetBundleAssetRecord>(50); //所有加载的AssetBundle 资源
        private NativeObjectPool<LoadAssetBundleAssetRecord> mAssetBundleRecordPoolMgr;

        //key=需要加载的资源的详细路径 不能是资源名
        private readonly Dictionary<string, LoadAssetBundleSubAssetRecord> mAllLoadedAssetBundleSubAssetRecord = new Dictionary<string, LoadAssetBundleSubAssetRecord>(50); //所有加载的AssetBundle 包里面的资源
        private NativeObjectPool<LoadAssetBundleSubAssetRecord> mAssetBundleLoadSubAssetRecordPoolMgr;

#if UNITY_EDITOR
        public Dictionary<string, LoadAssetBundleAssetRecord> Debug_mAllLoadAssetBundleCache
        {
            get { return mAllLoadAssetBundleCache; }
        }

        public Dictionary<string, LoadAssetBundleSubAssetRecord> Debug_mAllLoadedAssetBundleSubAssetRecord
        {
            get { return mAllLoadedAssetBundleSubAssetRecord; }
        }

#endif

        #endregion

        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            InitialedAssetBundleLoad();
        }

        #region 对象池接口

        private void InitialedAssetBundleLoad()
        {
            mAssetBundleRecordPoolMgr = new NativeObjectPool<LoadAssetBundleAssetRecord>(50, OnBeforeGetAssetBundleLoadRecord, OnBeforeRecycleAssetBundleLoadRecord);
            mAssetBundleLoadSubAssetRecordPoolMgr = new NativeObjectPool<LoadAssetBundleSubAssetRecord>(50, OnBeforeGetAssetBundleLoadSubAssetRecord, OnBeforeRecycleAssetBundleLoadSubAssetRecord);
        }

        private void OnBeforeGetAssetBundleLoadRecord(LoadAssetBundleAssetRecord record)
        {
        }

        private void OnBeforeRecycleAssetBundleLoadRecord(LoadAssetBundleAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }


        private void OnBeforeGetAssetBundleLoadSubAssetRecord(LoadAssetBundleSubAssetRecord record)
        {
        }

        private void OnBeforeRecycleAssetBundleLoadSubAssetRecord(LoadAssetBundleSubAssetRecord record)
        {
            record.NotifyReleaseRecord(); //回收时候销毁引用
        }

        #endregion


        #region IAssetManager 接口实现

        public float MaxAliveTimeAfterNoReference
        {
            get { return 60; }
        } //最多存在1分钟

        /// <summary>/// 释放资源/// </summary>
        public void NotifyAssetRelease(LoadAssetBaseRecord record)
        {
            if (record is LoadAssetBundleSubAssetRecord)
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(record.AssetUrl))
                    mAllLoadedAssetBundleSubAssetRecord.Remove(record.AssetUrl);
                else
                    Debug.LogError("NotifyAssetRelease Fail,没有找到这个AssetBundle 子资源的记录 {0} ", record.AssetUrl);
            }
            else if (record is LoadAssetBundleAssetRecord)
            {
                var assetBundleDependence = record as LoadAssetBundleAssetRecord;
                if (mAllLoadAssetBundleCache.ContainsKey(assetBundleDependence.AssetUrl))
                    mAllLoadAssetBundleCache.Remove(assetBundleDependence.AssetUrl);
                else
                    Debug.LogError("NotifyAssetRelease Fail,没有找到这个AssetBundle的记录 {0} ", assetBundleDependence.AssetUrl);

                ResourcesManager.UnLoadAssetBundle(assetBundleDependence, true);
            }
            else
            {
                Debug.LogError("NotifyAssetReferenceChange Fail,无法处理的类型 " + record.GetType());
            }
        }


        public void NotifyAssetReferenceChange(LoadAssetBaseRecord record)
        {
            if (record == null) return;
            if (record is LoadAssetBundleSubAssetRecord)
            {
                var subAssetRecord = record as LoadAssetBundleSubAssetRecord;
                OnAssetBundleSubAssetReferenceChange(subAssetRecord);
                return;
            }

            if (record is LoadAssetBundleAssetRecord)
            {
                var dependenceAssetRecord = record as LoadAssetBundleAssetRecord;
                OnAssetBundleDependenceReferenceChange(dependenceAssetRecord);
                return;
            }

            Debug.LogError("NotifyAssetReferenceChange Fail,无法处理的类型 " + record.GetType());
        }

        /// <summary>/// AssetBundle 加载的资源的引用关系改变/// </summary>
        private void OnAssetBundleSubAssetReferenceChange(LoadAssetBundleSubAssetRecord subAssetRecord)
        {
            if (subAssetRecord.ReferenceCount == 0)
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(subAssetRecord.AssetUrl))
                {
                    // mAllLoadedAssetBundleSubAssetRecord.Remove(subAssetRecord.AssetUrl);  //这里不能释放需要等待资源真的被释放时候调用
                    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(subAssetRecord);
                }
                else
                    Debug.LogError("NotifyAssetReferenceChange Error !!" + subAssetRecord.AssetUrl);

                if (mAllLoadAssetBundleCache.TryGetValue(subAssetRecord.AssetBelongBundleName, out var assetBundleRecord))
                    assetBundleRecord.ReduceSubAssetReference(subAssetRecord);
            } //资源没有引用时候 释放资源
            else
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(subAssetRecord.AssetUrl) == false)
                    mAllLoadedAssetBundleSubAssetRecord[subAssetRecord.AssetUrl] = subAssetRecord;

                if (mAllLoadAssetBundleCache.TryGetValue(subAssetRecord.AssetBelongBundleName, out var assetBundleRecord))
                    assetBundleRecord.AddSubAssetReference(subAssetRecord);
                else
                    Debug.LogError("NotifyAssetReferenceChange Error !!" + subAssetRecord.AssetUrl);
            }
        }

        /// <summary>/// AssetBundle 资源被引用次数改变/// </summary>
        private void OnAssetBundleDependenceReferenceChange(LoadAssetBundleAssetRecord depdenceAssetRecord)
        {
            if (depdenceAssetRecord.ReferenceCount == 0)
            {
                LoadAssetBundleAssetRecord assetBundleRecord = null;
                if (mAllLoadAssetBundleCache.TryGetValue(depdenceAssetRecord.AssetUrl, out assetBundleRecord))
                {
                    //**这里不能释放
                    //mAllLoadAssetBundleCache.Remove(depdenceAssetRecord.AssetUrl);
                    depdenceAssetRecord.ClearAllDependence(); //释放依赖的其他AssetBundle  
                    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(depdenceAssetRecord);
                    Debug.LogInfor("OnAssetBundleDependenceReferenceChange AssetBundle 资源 {0} 已经没有任何资源引用了!!", depdenceAssetRecord.AssetUrl);
                }
                else
                {
                    Debug.LogError("OnAssetBundleDependenceReferenceChange Error !!" + depdenceAssetRecord.AssetUrl);
                }
            } //资源没有引用时候 释放资源
        }

        #endregion

        #region 资源加载

        /// <summary>/// 加载缓存的 子资源
        /// <param name="assetPath">资源相对于Resources的完整路径</param>
        public LoadAssetBundleSubAssetRecord LoadSubAssetFromCache(string assetPath)
        {
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out var record))
            {
                if (record.IsReferenceEnable)
                    return record;
            }

            return null;
        }

        /// <summary>/// 同步 加载AssetBundle 资源/// </summary>
        public LoadAssetBundleInfor LoadAssetBundleSync(string assetBundlePath, out LoadAssetBundleAssetRecord assetBundleRecord)
        {
            assetBundleRecord = null;
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetBundleSync Fail,Parameter is null");
                return null;
            }

            //缓存中取
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out assetBundleRecord))
            {
                if (assetBundleRecord != null && assetBundleRecord.IsReferenceEnable)
                {
                    return assetBundleRecord.LoadAssetBundleInformation;
                }
                else
                {
                    mAssetBundleRecordPoolMgr.RecycleItemToPool(assetBundleRecord);
                    mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
                }
            }

            #region 依赖加载AssetBundle 并递归记录依赖关系

            assetBundleRecord = mAssetBundleRecordPoolMgr.GetItemFromPool();

            string[] depdenceAssetBundle = AssetBundleUpgradeManager.S_Instance.GetAllDependencies(assetBundlePath);
            foreach (var depdence in depdenceAssetBundle)
            {
                LoadAssetBundleSync(depdence, out var depdenceRecord);
                assetBundleRecord.AddDependence(depdenceRecord);
            }

            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle == null)
            {
                Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                assetBundleRecord.ClearAllDependence();
                mAssetBundleRecordPoolMgr.RecycleItemToPool(assetBundleRecord);
                return null;
            } //当前AssetBundle 不存在则需要清理申请的资源

            var assetBundleInfor = new LoadAssetBundleInfor(assetBundlePath, assetBundle);

            assetBundleRecord.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundleInfor, this);
            mAllLoadAssetBundleCache[assetBundlePath] = assetBundleRecord;

            #endregion

            return assetBundleInfor;
        }

        /// <summary>/// 异步加载AssetBundle 资源/// </summary>
        public void LoadAssetBundleAsync(string assetBundlePath, Action<LoadAssetBundleAssetRecord, LoadAssetBundleInfor> assetBundleDependence)
        {
            // 缓存中取
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out var record) && record != null && record.IsReferenceEnable)
            {
                assetBundleDependence?.Invoke(record, record.LoadAssetBundleInformation);
                return;
            }

            if (record != null)
            {
                if (record.IsReferenceEnable)
                {
                    record.ClearAllDependence();
                    record.ReduceReference(true);
                } //释放资源

                mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
            }


            #region 依赖加载AssetBundle 并递归记录依赖关系

            record = mAssetBundleRecordPoolMgr.GetItemFromPool();

            string[] dependenceAssetBundle = AssetBundleUpgradeManager.S_Instance.GetAllDependencies(assetBundlePath);
            foreach (var dependence in dependenceAssetBundle)
            {
                LoadAssetBundleAsync(dependence, (dependenceRecord, bundle) => { record.AddDependence(dependenceRecord); });
            }

            #endregion

            AssetBundleCreateRequest requst = LoadAssetBundleAsync(assetBundlePath);
            AsyncManager.S_Instance.StartAsyncOperation(requst, (asyncOperation) =>
            {
                var result = asyncOperation as AssetBundleCreateRequest;
                if (result.assetBundle == null)
                {
                    record.ClearAllDependence();
                    record.ReduceReference(true);
                    mAssetBundleRecordPoolMgr.RecycleItemToPool(record);
                    Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);

                    assetBundleDependence?.Invoke(null, null);
                    return;
                }

                var assetBundleInfor = new LoadAssetBundleInfor(assetBundlePath, result.assetBundle);

                record.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetBundleInfor, this);
                mAllLoadAssetBundleCache[assetBundlePath] = record;
                assetBundleDependence?.Invoke(record, assetBundleInfor);
            }, null);
        }


        #region 同步 & 异步 加载本地的 AssetBundle 中的资源

        /// <summary>/// 同步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        public LoadAssetBundleSubAssetRecord LoadAssetSync(string assetPath, string assetBundlePath, string assetName)
        {
            LoadAssetBundleSubAssetRecord assetRecord = null;
            assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
                return assetRecord;

            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                return null;
            }

            return ReLoadAssetSync(assetPath, assetBundlePath, assetName);
        }

        /// <summary>/// 真实的 重新加载一个AssetBundle 中的资源 不会检测参数是否合法/// </summary>
        private LoadAssetBundleSubAssetRecord ReLoadAssetSync(string assetPath, string assetBundlePath, string assetName)
        {
            var assetBundle = LoadAssetBundleSync(assetBundlePath, out var record);
            if (assetBundle != null)
            {
                var assetInfor = assetBundle.LoadAssetBundleSubAsset(assetName);
                var assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetInfor);
                return assetRecord;
            }

            return null;
        }

        /// <summary>/// 同步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        /// /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        public LoadAssetBundleSubAssetRecord LoadAssetSync(string assetPath)
        {
            LoadAssetBundleSubAssetRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
                return assetRecord;

            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
#if UNITY_EDITOR
                Debug.LogInfor("LoadAssetSync Fail,Not Exit asset " + assetPath);
#endif
                return null;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                return null;
            }

            return ReLoadAssetSync(assetPath, assetBundlePath, assetName);
        }


        /// <summary>/// 异步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        public void LoadAssetAsync(string assetPath, Action<LoadAssetBundleSubAssetRecord> loadCallback)
        {
            LoadAssetBundleSubAssetRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetAsync Fail,Not Exit asset " + assetPath);
                loadCallback?.Invoke(null);
                return;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError($"LoadAssetAsync Fail, 加载的资源 {assetPath}  名 为null");
                loadCallback?.Invoke(null);
                return;
            }

            ReLoadAssetAsync(assetPath, assetBundlePath, assetName, loadCallback);
        }

        /// <summary>/// 异步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        public void LoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<LoadAssetBundleSubAssetRecord> loadCallback)
        {
            LoadAssetBundleSubAssetRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError($"LoadAssetAsync Fail, 加载的资源 {assetPath}  名 为null");
                loadCallback?.Invoke(null);
                return;
            }

            ReLoadAssetAsync(assetPath, assetBundlePath, assetName, loadCallback);
        }

        /// <summary>/// 真实的 异步加载 AssetBundle 中的资源  不会检测参数是否合法/// </summary>
        private void ReLoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<LoadAssetBundleSubAssetRecord> loadCallback)
        {
            LoadAssetBundleAsync(assetBundlePath, (record, assetBundleInfor) =>
            {
                if (assetBundleInfor != null)
                {
                    var assetInfor = assetBundleInfor.LoadAssetBundleSubAsset(assetName);
                    var assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetInfor);
                    loadCallback?.Invoke(assetRecord);
                    return;
                }

                Debug.LogError("LoadAssetAsync Fail,assetPath={0}  ", assetPath);

                loadCallback?.Invoke(null);
            });
        }

        #endregion

        #endregion

        #region 辅助

        #region 内部加载AssetBundle 方法，对 AssetBundle类加载方式的封装

        private AssetBundle LoadAssetBundleSync(string assetBundleRelativePath)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath);
        }

        private AssetBundle LoadAssetBundleSync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes);
        }

        private AssetBundle LoadAssetBundleSync(string assetBundleRelativePath, uint crc)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath, crc);
        }

        private AssetBundle LoadAssetBundleSync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes, crc);
        }


        private AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundleRelativePath)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath);
        }

        private AssetBundleCreateRequest LoadAssetBundleAsync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes);
        }

        private AssetBundleCreateRequest LoadAssetBundleAsync(string assetBundleRelativePath, uint crc)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath, crc);
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
            Debug.LogInfor($"保存下载的AssetBundle 资源{fileSavePath} ");
        }


        /// <summary>
        /// 记录从AssetBundel 加载的资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <param name="parentAssetBundle"></param>
        private LoadAssetBundleSubAssetRecord RecordAssetBundleLoadSubAsset(string assetPath, string assetBundlePath, LoadBasicAssetInfor assetInfor)
        {
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out var record))
                return record;

            record = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetBundlePath) as LoadAssetBundleSubAssetRecord;
            if (record == null)
                record = mAssetBundleLoadSubAssetRecordPoolMgr.GetItemFromPool();
            record.Initial(assetPath, assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetInfor, this);

            mAllLoadedAssetBundleSubAssetRecord[assetPath] = record;
            return record;
        }

        #endregion
    }
}
