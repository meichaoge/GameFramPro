using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.Upgrade;
using Object = UnityEngine.Object;
using UnityEngine.Networking;

namespace GameFramePro.ResourcesEx
{
    /// <summary>/// AssetBundle 资源管理器/// </summary>
    public sealed class AssetBundleManager : Single<AssetBundleManager>
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
        private static AssetBundleAssetTotalInfor s_ServerBundleAssetConfigInfor = null; //服务器上最新的AssetBundle 配置信息
        //记录路径与AssetBundle的映射关系
        private static readonly Dictionary<string, AssetBundleRecordInfor> mAllAssetBundleRecords = new Dictionary<string, AssetBundleRecordInfor>(); 

        //key=需要加载的资源的详细路径 不能是资源名 所有加载的AssetBundle 包里面的资源
        private static readonly Dictionary<string, LoadAssetBundleAssetRecord> mAllLoadedAssetBundleSubAssetRecord = new Dictionary<string, LoadAssetBundleAssetRecord>(50);

        #endregion



        #region 对外接口

        /// <summary>/// 在AssetBundle 流程更新完之后需要调用这个接口完成保存最新配置的功能/// </summary>
        public void SaveAssetBundleTotalConfigInfor(AssetBundleAssetTotalInfor newConfig)
        {
            s_ServerBundleAssetConfigInfor = newConfig;
        }


        /// <summary>/// 根据一个资源名称获取所在的AssetBundle name/// </summary>
        public string GetBundleNameByAssetPath(string assetPath)
        {
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetBundleNameByAssetPath Fail,没有获取最新的AssetBundle 配置 ");
                return null;
            }

            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Value.mContainAssetPathInfor.Contains(assetPath))
                    return assetBundleInfor.Value.mBundleName;
            }
#if UNITY_EDITOR
            Debug.LogEditorInfor("GetBundleNameByAssetPath Fail,没有找到资源 " + assetPath);
#endif
            return string.Empty;
        }

        /// <summary>/// 获取指定参数的AssetBundle 依赖的信息 (内部会对路径处理)/// </summary>
        public string[] GetAllDependencies(string assetBundlePath)
        {
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetAllDependencies Fail,没有初始化获取AssetBundle 配置信息 ");
                return new string[0];
            }

            assetBundlePath = assetBundlePath.GetPathStringEx();

            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Key == assetBundlePath)
                    return assetBundleInfor.Value.mDepdenceAssetBundleInfor;
            }

            Debug.LogError("GetAllDependencies Fail,没有找到依赖关系 " + assetBundlePath);
            return new string[0];
        }

        #endregion

        #region 同步 & 异步 加载本地的 AssetBundle 中的资源


        /// <summary>
        /// 同步 加载AssetBundle 资源
        ///  </summary>
        public AssetBundle LoadAssetBundleSync(string assetBundlePath)
        {
            if (string.IsNullOrEmpty(assetBundlePath))
            {
                Debug.LogError("LoadAssetBundleSync Fail,Parameter is null");
                return null;
            }

            //缓存中取
            if (mAllAssetBundleRecords.TryGetValue(assetBundlePath, out var assetBundleRecord))
            {
                if (assetBundleRecord != null && assetBundleRecord.IsAssetBundleEnable)
                    return assetBundleRecord.mAssetBundle;
            }

            #region 依赖加载AssetBundle 并递归记录依赖关系
            string[] dependenceBundlesPath = GetAllDependencies(assetBundlePath);
            Dictionary<string, AssetBundle> depdenceAssetBundles = new Dictionary<string, AssetBundle>(dependenceBundlesPath.Length);
            foreach (var dependence in dependenceBundlesPath)
            {
                var depdenceAssetBundle = LoadAssetBundleSync(dependence);
                depdenceAssetBundles[dependence] = depdenceAssetBundle;
                if (mAllAssetBundleRecords.ContainsKey(dependence) == false)
                {
                    var assetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(dependence, depdenceAssetBundle);
                    mAllAssetBundleRecords[dependence] = assetBundleRecordInfor;
                }//记录加载的 AssetBundle资源
            }

            AssetBundle assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle != null)
            {
                foreach (var depdenceAssetBundle in depdenceAssetBundles)
                {
                    if (mAllAssetBundleRecords.TryGetValue(depdenceAssetBundle.Key, out var depdenceAssetBundleRecordInfor))
                        depdenceAssetBundleRecordInfor.AddBeDepdenceAssetBudle(assetBundlePath);
                }
                assetBundleRecord = AssetBundleRecordInfor.GetAssetBundleRecordInfor(assetBundlePath, assetBundle);
                mAllAssetBundleRecords[assetBundlePath] = assetBundleRecord;
            }
            else
            {
                Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                return null;
            } //当前AssetBundle 不存在则需要清理申请的资源

            #endregion

            return assetBundle;
        }

        /// <summary>
        /// 异步加载AssetBundle 资源
        /// </summary>
        public void LoadAssetBundleAsync(string assetBundlePath, Action<AssetBundle> completeCallback)
        {
            // 缓存中取
            if (mAllAssetBundleRecords.TryGetValue(assetBundlePath, out var assetBundleRecordInfor))
            {
                if (assetBundleRecordInfor != null && assetBundleRecordInfor.IsAssetBundleEnable)
                {
                    completeCallback?.Invoke(assetBundleRecordInfor.mAssetBundle);
                    return;
                }
            }


            // 依赖加载AssetBundle 并递归记录依赖关系
            string[] dependenceAssetBundlePath = GetAllDependencies(assetBundlePath);
            Dictionary<string, AssetBundle> depdenceAssetBundles = new Dictionary<string, AssetBundle>(dependenceAssetBundlePath.Length);
            foreach (var dependence in dependenceAssetBundlePath)
            {
                LoadAssetBundleAsync(dependence, (dependenceRecord) =>
                {
                    depdenceAssetBundles[dependence] = dependenceRecord;
                    if (mAllAssetBundleRecords.ContainsKey(dependence) == false)
                    {
                        var depdenceAssetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(dependence, dependenceRecord);
                        mAllAssetBundleRecords[dependence] = depdenceAssetBundleRecordInfor;
                    }//记录加载的 依赖 AssetBundle资源
                });
            }


            AssetBundleCreateRequest requst = LoadAssetBundleFromFileAsync(assetBundlePath);
            AsyncManager.StartAsyncOperation(requst, () =>
            {
                if (requst.assetBundle == null)
                {
                    Debug.LogError("LoadAssetBundleSync Fail,AssetBundle NOT Exit " + assetBundlePath);
                    completeCallback?.Invoke(null);
                    return;
                }

                foreach (var dependenceAssetBundle in depdenceAssetBundles)
                {
                    if (mAllAssetBundleRecords.TryGetValue(dependenceAssetBundle.Key, out var depdenceAssetBundleRecordInfor))
                        depdenceAssetBundleRecordInfor.AddBeDepdenceAssetBudle(assetBundlePath);
                }
                assetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(assetBundlePath, requst.assetBundle);
                mAllAssetBundleRecords[assetBundlePath] = assetBundleRecordInfor;

                completeCallback?.Invoke(requst.assetBundle);
            }, null);
        }




        /// <summary>/// 加载缓存的 子资源
        /// <param name="assetPath">资源相对于Resources的完整路径</param>
        private LoadAssetBundleAssetRecord LoadAssetBundleSubAssetFromCache(string assetPath)
        {
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetPath, out var record))
                return record;
            return null;
        }

        /// <summary>/// 同步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        public ILoadAssetRecord AssetBundleLoadAssetSync(string assetPath, string assetBundlePath, string assetName)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;
            assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();

            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError($"LoadAssetSync Fail,assetName Is Null {assetPath} "  );
                return null;
            }

            assetRecord = ReLoadAssetSync(assetPath, assetBundlePath, assetName);
            if (assetRecord != null)
                return assetRecord;
            return null;
        }

        /// <summary>/// 真实的 重新加载一个AssetBundle 中的资源 不会检测参数是否合法/// </summary>
        private LoadAssetBundleAssetRecord ReLoadAssetSync(string assetPath, string assetBundlePath, string assetName)
        {
            var assetBundle = LoadAssetBundleSync(assetBundlePath);
            if (assetBundle != null)
            {
                Object bundleAsset = assetBundle.LoadAsset(assetName);
                var assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetName, bundleAsset);
                return assetRecord;
            }

            return null;
        }

        /// <summary>/// 同步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        /// /// <param name="assetPath">资源相对于Resouces 完整的路径</param>
        public ILoadAssetRecord AssetBundleLoadAssetSync(string assetPath)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;

            string assetBundlePath = GetBundleNameByAssetPath(assetPath);
            if (string.IsNullOrEmpty(assetBundlePath))
            {
#if UNITY_EDITOR
                Debug.LogEditorError("LoadAssetSync Fail,Not Exit asset " + assetPath);
#endif
                return null;
            }

            string assetName = System.IO.Path.GetFileNameWithoutExtension(assetPath).ToLower();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("LoadAssetSync Fail,assetName Is Null " + assetPath);
                return null;
            }

            assetRecord = ReLoadAssetSync(assetPath, assetBundlePath, assetName);

            if (assetRecord != null)
                return assetRecord;
            return null;
        }


        /// <summary>/// 异步加载AssetBundle 方法（优先从缓存中读取）/// </summary>
        public void AssetBundleLoadAssetAsync(string assetPath, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            string assetBundlePath = GetBundleNameByAssetPath(assetPath);
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
        public void AssetBundleLoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetPath);
            if (assetRecord != null && assetRecord.IsRecordEnable)
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
        private void ReLoadAssetAsync(string assetPath, string assetBundlePath, string assetName, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAsync(assetBundlePath, (assetBundleInfor) =>
            {
                if (assetBundleInfor != null)
                {
                    Object asset = assetBundleInfor.LoadAsset(assetName);
                    var assetRecord = RecordAssetBundleLoadSubAsset(assetPath, assetBundlePath, assetName, asset);
                    loadCallback?.Invoke(assetRecord);
                    return;
                }

                Debug.LogError("LoadAssetAsync Fail,assetPath={0}  ", assetPath);
                loadCallback?.Invoke(null);
            });
        }

        #endregion

        #region 资源释放
        public void RemoveLoadAssetBundleAssetRecord(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null) return;
            mAllLoadedAssetBundleSubAssetRecord.Remove(loadAssetBundleAssetRecord.mAssetFullUri);
            //TODO 处理AssetBundle 引用

            LoadAssetBundleAssetRecord.ReleaseAssetBundleRecordInfor(loadAssetBundleAssetRecord);

        }
        #endregion


        #region 辅助

        #region 内部加载AssetBundle 方法，对 AssetBundle类加载方式的封装

        private AssetBundle LoadAssetBundleFromFile(string assetBundleRelativePath)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath);
        }

        private AssetBundle LoadAssetBundleFromMemory(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes);
        }

        private AssetBundle LoadAssetBundleFromFile(string assetBundleRelativePath, uint crc)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath, crc);
        }

        private AssetBundle LoadAssetBundleFromMemory(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes, crc);
        }

        private AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string assetBundleRelativePath)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath);
        }

        private AssetBundleCreateRequest LoadAssetBundleFromMemoryAsync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes);
        }

        private AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string assetBundleRelativePath, uint crc)
        {
            string realPath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath, crc);
        }

        private AssetBundleCreateRequest LoadAssetBundleFromMemoryAsync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes, crc);
        }

        #endregion


        /// <summary>/// 保存下载的AssetBundel 资源/// </summary>
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
        private LoadAssetBundleAssetRecord RecordAssetBundleLoadSubAsset(string assetFullUri, string assetBundleUri, string assetRelativeUri, Object assetInfor)
        {
            if (mAllLoadedAssetBundleSubAssetRecord.TryGetValue(assetFullUri, out var record))
                return record;

            record = LoadAssetBundleAssetRecord.GetLoadAssetBundleAssetRecord(assetFullUri, assetBundleUri, assetRelativeUri, assetInfor);

            mAllLoadedAssetBundleSubAssetRecord[assetFullUri] = record;
            return record;
        }

        #endregion
    }
}