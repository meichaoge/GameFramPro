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


        public float MaxAliveTimeAfterNoReference { get { return 60; } } //最多存在1分钟

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="record"></param>
        public void NotifyAssetRelease(BaseLoadAssetRecord record)
        {
            if (record is AssetBundleSubAssetLoadRecord)
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(record.AssetUrl))
                    mAllLoadedAssetBundleSubAssetRecord.Remove(record.AssetUrl);
                else
                    Debug.LogError("NotifyAssetRelease Fail,没有找到这个AssetBundle 子资源的记录 {0} ", record.AssetUrl);
            }
            else if (record is AssetBundleAssetDepdenceRecord)
            {
                AssetBundleAssetDepdenceRecord assetBundleDepdence = record as AssetBundleAssetDepdenceRecord;
                if (mAllLoadAssetBundleCache.ContainsKey(assetBundleDepdence.AssetUrl))
                    mAllLoadAssetBundleCache.Remove(assetBundleDepdence.AssetUrl);
                else
                    Debug.LogError("NotifyAssetRelease Fail,没有找到这个AssetBundle的记录 {0} ", assetBundleDepdence.AssetUrl);

                ResourcesManager.UnLoadAssetBundle(assetBundleDepdence, true);
            }
            else
            {
                Debug.LogError("NotifyAssetReferenceChange Fail,无法处理的类型 " + record.GetType());
            }
        }




        public void NotifyAssetReferenceChange(BaseLoadAssetRecord record)
        {
            if (record == null) return;
            if (record is AssetBundleSubAssetLoadRecord)
            {
                AssetBundleSubAssetLoadRecord subAssetRecord = record as AssetBundleSubAssetLoadRecord;
                OnAssetBundleSubAssetReferenceChange(subAssetRecord);
                return;
            }

            if (record is AssetBundleAssetDepdenceRecord)
            {
                AssetBundleAssetDepdenceRecord depdenceAssetRecord = record as AssetBundleAssetDepdenceRecord;
                OnAssetBundleDepdenceReferenceChange(depdenceAssetRecord);
                return;
            }
            Debug.LogError("NotifyAssetReferenceChange Fail,无法处理的类型 " + record.GetType());
        }

        /// <summary>
        /// AssetBundle 加载的资源的引用关系改变
        /// </summary>
        /// <param name="subAssetRecord"></param>
        private void OnAssetBundleSubAssetReferenceChange(AssetBundleSubAssetLoadRecord subAssetRecord)
        {
            if (subAssetRecord.ReferenceCount == 0)
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(subAssetRecord.AssetUrl))
                {
                    // mAllLoadedAssetBundleSubAssetRecord.Remove(subAssetRecord.AssetUrl);  //这里不能释放需要等待资源真的被释放时候调用
                    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(subAssetRecord);
                }
                else
                {
                    Debug.LogError("NotifyAssetReferenceChange Error !!" + subAssetRecord.AssetUrl);
                }

                AssetBundleAssetDepdenceRecord assetBundleRecord = null;
                if (mAllLoadAssetBundleCache.TryGetValue(subAssetRecord.AssetBelongBundleName, out assetBundleRecord))
                {
                    assetBundleRecord.ReduceSubAssetReference(subAssetRecord);
                }

            } //资源没有引用时候 释放资源
            else
            {
                if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(subAssetRecord.AssetUrl) == false)
                {
                    mAllLoadedAssetBundleSubAssetRecord[subAssetRecord.AssetUrl] = subAssetRecord;
                }
                AssetBundleAssetDepdenceRecord assetBundleRecord = null;
                if (mAllLoadAssetBundleCache.TryGetValue(subAssetRecord.AssetBelongBundleName, out assetBundleRecord))
                {
                    assetBundleRecord.AddSubAssetReference(subAssetRecord);
                }
                else
                {
                    Debug.LogError("NotifyAssetReferenceChange Error !!" + subAssetRecord.AssetUrl);
                }
            }
        }

        /// <summary>
        /// AssetBundle 资源被引用次数改变
        /// </summary>
        /// <param name="subAssetRecord"></param>
        private void OnAssetBundleDepdenceReferenceChange(AssetBundleAssetDepdenceRecord depdenceAssetRecord)
        {
            if (depdenceAssetRecord.ReferenceCount == 0)
            {
                AssetBundleAssetDepdenceRecord assetBundleRecord = null;
                if (mAllLoadAssetBundleCache.TryGetValue(depdenceAssetRecord.AssetUrl, out assetBundleRecord))
                {
                    //**这里不能释放
                    //mAllLoadAssetBundleCache.Remove(depdenceAssetRecord.AssetUrl);
                    depdenceAssetRecord.ClearAllDepdence();  //释放依赖的其他AssetBundle  
                    AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(depdenceAssetRecord);
                    Debug.LogInfor("OnAssetBundleDepdenceReferenceChange AssetBundle 资源 {0} 已经没有任何资源引用了!!", depdenceAssetRecord.AssetUrl);
                }
                else
                {
                    Debug.LogError("OnAssetBundleDepdenceReferenceChange Error !!" + depdenceAssetRecord.AssetUrl);
                }
            } //资源没有引用时候 释放资源
              //    else
              //    {
              //        if (mAllLoadedAssetBundleSubAssetRecord.ContainsKey(subAssetRecord.AssetUrl) == false)
              //        {
              //            mAllLoadedAssetBundleSubAssetRecord[subAssetRecord.AssetUrl] = subAssetRecord;
              //        }
              //        AssetBundleAssetDepdenceRecord assetBundleRecord = null;
              //        if (mAllLoadAssetBundleCache.TryGetValue(subAssetRecord.AssetBelongBundleName, out assetBundleRecord))
              //        {
              //            assetBundleRecord.AddSubAssetReference(subAssetRecord);
              //        }
              //        else
              //        {
              //            Debug.LogError("NotifyAssetReferenceChange Error !!" + record.AssetUrl);
              //        }
              //    }
        }

        #endregion

        #region 资源加载

        /// <summary>
        /// 加载缓存的 子资源
        /// </summary>
        /// <param name="assetPath">资源相对于Resources的完整路径</param>
        /// <returns></returns>
        public AssetBundleSubAssetLoadRecord LoadSubAssetFromCache(string assetPath)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out record))
            {
                if (record.AssetBundleLoadSubAssetInfor.IsLoadAssetEnable)
                    return record;
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
        public BundleLoadUnityAssetInfor LoadAssetBundleSync(string assetBundlePath, out AssetBundleAssetDepdenceRecord assetBundleRecord)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetBundleSync Fail,Parameter is null");
            }

            assetBundleRecord = null;
            //缓存中取
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out assetBundleRecord))
            {
                if (assetBundleRecord != null && assetBundleRecord.AssetBundleLoadBundleInfor.IsLoadAssetEnable)
                {
                    return assetBundleRecord.AssetBundleLoadBundleInfor;
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

            BundleLoadUnityAssetInfor assetInfor = new BundleLoadUnityAssetInfor(assetBundlePath, assetBundle);

            assetBundleRecord.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetInfor, this);
            mAllLoadAssetBundleCache[assetBundlePath] = assetBundleRecord;
            #endregion
            return assetInfor;
        }

        /// <summary>
        /// 异步加载AssetBundle 资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void LoadAssetBundleAsync(string assetBundlePath, Action<AssetBundleAssetDepdenceRecord, BundleLoadUnityAssetInfor> assetBundleDepdence)
        {
            AssetBundleAssetDepdenceRecord record = null;
            // 缓存中取
            if (mAllLoadAssetBundleCache.TryGetValue(assetBundlePath, out record))
            {
                if (record != null)
                {
                    if (record.AssetBundleLoadBundleInfor.IsLoadAssetEnable)
                    {
                        if (assetBundleDepdence != null)
                            assetBundleDepdence(record, record.AssetBundleLoadBundleInfor);
                        return;
                    }
                    else
                    {
                        record.ClearAllDepdence();
                        record.ReduceReference(true);
                        mAssetBundleRecordPoolMgr.RecycleItemToPool(record);
                    } //释放资源
                }

                mAllLoadAssetBundleCache.Remove(assetBundlePath); //已经被销毁了 需要重新加载
            }

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

                BundleLoadUnityAssetInfor assetInfor = new BundleLoadUnityAssetInfor(assetBundlePath, result.assetBundle);
                record.Initial(assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetInfor, this);
                mAllLoadAssetBundleCache[assetBundlePath] = record;
                if (assetBundleDepdence != null)
                    assetBundleDepdence(record, assetInfor);
            }, null);
        }

        #endregion


        #region 同步 & 异步 加载本地的 AssetBundle 中的资源

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="loadCallback"></param>
        public void LoadAssetSync(string assetPath, Action<AssetBundleSubAssetLoadRecord> loadCallback)
        {
            AssetBundleSubAssetLoadRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }

            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetSync Fail,Not Exit asset " + assetPath);
                if (loadCallback != null)
                    loadCallback(null);
                return;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            LoadAssetSync(assetPath, assetBundlePath, assetName, loadCallback, false, false);
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认应该=true</param>
        /// <param name="isNeedCheckCaheRecord">标示是否检测缓存中是否有数据</param>
        public void LoadAssetSync(string assetPath, string assetBundlePath, string assetName, Action<AssetBundleSubAssetLoadRecord> loadCallback, bool isNeedTranslateAssetName, bool isNeedCheckCaheRecord)
        {
            AssetBundleSubAssetLoadRecord assetRecord = null;
            if (isNeedCheckCaheRecord)
            {
                assetRecord = LoadSubAssetFromCache(assetPath);
                if (assetRecord != null)
                {
                    if (loadCallback != null)
                        loadCallback(assetRecord);
                    return;
                }
            }

            if (isNeedTranslateAssetName)
                assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                if (loadCallback != null)
                    loadCallback(null);
                return;
            }


            AssetBundleAssetDepdenceRecord record = null;
            BundleLoadUnityAssetInfor assetBundle = LoadAssetBundleSync(assetBundlePath, out record);
            if (assetBundle != null)
            {
                BundleLoadSubUnityAssetInfor bundleSubAsset = assetBundle.LoadAssetBundleSubAsset(assetName);
                assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, bundleSubAsset);
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }

            Debug.LogError("LoadAssetSync Fail,assetPath={0}  ", assetPath);
            if (loadCallback != null)
                loadCallback(null);
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        /// <param name="isNeedCheckCaheRecord">标示是否检测缓存中是否有数据</param>
        /// <returns></returns>
        public AssetBundleSubAssetLoadRecord LoadAssetSync(string assetPath, string assetBundlePath, string assetName, bool isNeedTranslateAssetName, bool isNeedCheckCaheRecord)
        {
            AssetBundleSubAssetLoadRecord assetRecord = null;
            if (isNeedCheckCaheRecord)
            {
                assetRecord = LoadSubAssetFromCache(assetPath);
                if (assetRecord != null)
                    return assetRecord;
            }

            if (isNeedTranslateAssetName)
                assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                return null;
            }

            BundleLoadUnityAssetInfor assetBundle = LoadAssetBundleSync(assetBundlePath, out var record);
            if (assetBundle != null)
            {
                BundleLoadSubUnityAssetInfor assetInfor = assetBundle.LoadAssetBundleSubAsset(assetName);
                assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetInfor);
                return assetRecord;
            }
            return null;
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        /// <returns></returns>
        public AssetBundleSubAssetLoadRecord LoadAssetSync(string assetPath)
        {
            AssetBundleSubAssetLoadRecord assetRecord = LoadSubAssetFromCache(assetPath);
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
            return LoadAssetSync(assetPath, assetBundlePath, assetName, false, false);
        }


        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        public void LoadAssetAsync(string assetPath, Action<AssetBundleSubAssetLoadRecord> loadCallback)
        {
            AssetBundleSubAssetLoadRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }
            string assetBundlePath = AssetBundleUpgradeManager.S_Instance.GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetAsync Fail,Not Exit asset " + assetPath);
                if (loadCallback != null)
                    loadCallback(null);
                return;
            }
            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            LoadAssetAsync(assetPath, assetBundlePath, assetName, loadCallback, false, false);
        }

        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="loadCallback"></param>
        /// <param name="isNeedTranslateAssetName">标示是否需要处理这个参数，转化成没有扩展名的小写字符串！  默认=true</param>
        public void LoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<AssetBundleSubAssetLoadRecord> loadCallback, bool isNeedTranslateAssetName, bool isNeedCheckCaheRecord)
        {
            AssetBundleSubAssetLoadRecord assetRecord = LoadSubAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            LoadAssetBundleAsync(assetBundlePath, (record, assetBundleInfor) =>
            {
                if (assetBundleInfor != null)
                {
                    BundleLoadSubUnityAssetInfor assetInfor = assetBundleInfor.LoadAssetBundleSubAsset(assetName);
                    assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetInfor);
                    if (loadCallback != null)
                        loadCallback(assetRecord);
                    return;
                }
                Debug.LogError("LoadAssetAsync Fail,assetPath={0}  ", assetPath);

                if (loadCallback != null)
                    loadCallback(null);
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
            Debug.LogInfor("保存下载的AssetBundel 资源 " + fileSavePath);
        }


        /// <summary>
        /// 记录从AssetBundel 加载的资源
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <param name="parentAssetBundle"></param>
        private AssetBundleSubAssetLoadRecord RecordAssetBundleLoadSubAsset(string assetPath, string assetBundlePath, BundleLoadSubUnityAssetInfor assetInfor)
        {
            AssetBundleSubAssetLoadRecord record = null;
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out record))
                return record;

            record = AssetDelayDeleteManager.TryGetILoadAssetRecord(assetBundlePath) as AssetBundleSubAssetLoadRecord;
            if (record == null)
                record = mAssetBundleLoadSubAssetRecordPoolMgr.GetItemFromPool();
            record.Initial(assetPath, assetBundlePath, LoadedAssetTypeEnum.AssetBundle_UnKnown, assetInfor, this);

            mAllLoadedAssetBundleSubAssetRecord[assetPath] = record;
            return record;
        }

        #endregion


    }
}