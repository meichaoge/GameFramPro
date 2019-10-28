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
        private static AssetBundleAssetTotalInfor s_ServerBundleAssetConfigInfor = null; //服务器上最新的AssetBundle 配置信息

        //记录路径与AssetBundle的映射关系
        private static readonly Dictionary<string, AssetBundleRecordInfor> s_AllAssetBundleRecordInfors = new Dictionary<string, AssetBundleRecordInfor>();

        //key=需要加载的资源的详细路径 不能是资源名 所有加载的AssetBundle 包里面的资源
        private static readonly Dictionary<string, LoadAssetBundleAssetRecord> s_AllLoadedAssetBundleSubAssetRecord = new Dictionary<string, LoadAssetBundleAssetRecord>(50);


        #region 内部加载AssetBundle 方法，对 AssetBundle类加载方式的封装

        private AssetBundle LoadAssetBundleFromFile(string assetBundleRelativePath)
        {
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
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
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
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
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
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
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath, crc);
        }

        private AssetBundleCreateRequest LoadAssetBundleFromMemoryAsync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes, crc);
        }

        #endregion

        #region 对外接口

        /// <summary>/// 在AssetBundle 流程更新完之后需要调用这个接口完成保存最新配置的功能/// </summary>
        public void SaveAssetBundleTotalConfigInfor(AssetBundleAssetTotalInfor newConfig)
        {
            s_ServerBundleAssetConfigInfor = newConfig;
        }


        /// <summary>
        /// 根据指定的路径标识获取AssetBundle 和资源相对uri
        /// </summary>
        /// <param name="assetFullUri"></param>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetRelativeUri"></param>
        private void GetBundleNameByAssetPath(string assetFullUri, out string assetBundleUri, out string assetRelativeUri)
        {
            assetBundleUri = assetRelativeUri = string.Empty;
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetBundleNameByAssetPath Fail,没有获取最新的AssetBundle 配置 ");
                return;
            }

            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor.Values)
            {
                if (assetBundleInfor.mContainAssetPathInfor.Contains(assetFullUri))
                {
                    assetBundleUri = assetBundleInfor.mAssetBundleUri;
                    assetRelativeUri = assetBundleInfor.mAssetRelativeAssetBundleUri;
                    return;
                }
            }
#if UNITY_EDITOR
            Debug.LogEditorInfor($"GetBundleNameByAssetPath Fail,没有找到资源 {assetRelativeUri}");
#endif
        }

        /// <summary>/
        ///获取指定参数的AssetBundle 依赖的信息 (内部会对路径处理)
        ///  </summary>
        private string[] GetAssetBundleAllDependencies(string assetBundleUri)
        {
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetAssetBundleAllDependencies Fail,没有初始化获取AssetBundle 配置信息 ");
                return new string[0];
            }

            assetBundleUri = assetBundleUri.GetPathStringEx();

            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Key == assetBundleUri)
                    return assetBundleInfor.Value.mDependenceAssetBundleInfor;
            }

            Debug.LogError($"GetAssetBundleAllDependencies Fail,没有找到依赖关系 {assetBundleUri}");
            return new string[0];
        }

        #endregion

        #region 同步 & 异步 加载本地的 AssetBundle 中的资源

        /// <summary>
        /// 同步 加载 AssetBundleRecordInfor 资源
        ///  </summary>
        private AssetBundleRecordInfor LoadAssetBundleRecordSync(string assetBundleUri)
        {
            if (string.IsNullOrEmpty(assetBundleUri))
            {
                Debug.LogError("LoadAssetBundleRecordSync Fail,Parameter is null");
                return null;
            }

            //缓存中取
            if (s_AllAssetBundleRecordInfors.TryGetValue(assetBundleUri, out var assetBundleRecordInfor) && assetBundleRecordInfor != null && assetBundleRecordInfor.IsAssetBundleEnable)
                return assetBundleRecordInfor;

            //依赖加载AssetBundle 并递归记录依赖关系
            string[] dependenceBundlesPath = GetAssetBundleAllDependencies(assetBundleUri);
            Dictionary<string, AssetBundleRecordInfor> allDependenceAssetBundleRecords = null;
            if (dependenceBundlesPath != null && dependenceBundlesPath.Length > 0)
            {
                allDependenceAssetBundleRecords = new Dictionary<string, AssetBundleRecordInfor>(dependenceBundlesPath.Length);
                foreach (var dependencePath in dependenceBundlesPath)
                {
                    var dependenceAssetBundleRecord = LoadAssetBundleRecordSync(dependencePath);
                    if (dependenceAssetBundleRecord == null)
                    {
                        Debug.LogError($"加载{assetBundleUri} 的依赖资源{dependencePath} 失败");
                        return null;
                    }

                    allDependenceAssetBundleRecords[dependencePath] = dependenceAssetBundleRecord;
                }
            } //递归加载依赖


            //加载参数指定的AssetBundle 
            AssetBundle assetBundle = LoadAssetBundleFromFile(assetBundleUri);
            if (assetBundle != null)
            {
                assetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(assetBundleUri, assetBundle);
                if (allDependenceAssetBundleRecords != null)
                {
                    foreach (var dependenceAssetBundleRecord in allDependenceAssetBundleRecords.Values)
                        assetBundleRecordInfor.AddDependenceAssetBudleRecord(dependenceAssetBundleRecord);
                    allDependenceAssetBundleRecords.Clear();
                }

                s_AllAssetBundleRecordInfors[assetBundleUri] = assetBundleRecordInfor;
                return assetBundleRecordInfor;
            }

            Debug.LogError($"LoadAssetBundleRecordSync Fail,AssetBundle NOT Exit {assetBundleUri}");
            return null;
        }


        /// <summary>
        /// 异步加载 AssetBundleRecordInfor 资源
        /// </summary>
        private void LoadAssetBundleRecordAsync(string assetBundleUri, Action<AssetBundleRecordInfor> completeCallback)
        {
            if (string.IsNullOrEmpty(assetBundleUri))
            {
                Debug.LogError("LoadAssetBundleRecordAsync Fail,Parameter is null");
                completeCallback?.Invoke(null);
                return;
            }

            // 缓存中取
            if (s_AllAssetBundleRecordInfors.TryGetValue(assetBundleUri, out var assetBundleRecordInfor) && assetBundleRecordInfor != null && assetBundleRecordInfor.IsAssetBundleEnable)
            {
                completeCallback?.Invoke(assetBundleRecordInfor);
                return;
            }


            // 依赖加载AssetBundle 并递归记录依赖关系
            string[] dependenceAssetBundlePath = GetAssetBundleAllDependencies(assetBundleUri);
            Dictionary<string, AssetBundleRecordInfor> allDependenceAssetBundleRecords = null;
            if (dependenceAssetBundlePath != null && dependenceAssetBundlePath.Length > 0)
            {
                allDependenceAssetBundleRecords = new Dictionary<string, AssetBundleRecordInfor>(dependenceAssetBundlePath.Length);
                foreach (var dependencePath in dependenceAssetBundlePath)
                {
                    LoadAssetBundleRecordAsync(dependencePath, (dependenceRecord) =>
                    {
                        if (dependenceRecord == null)
                        {
                            Debug.LogError($"加载{assetBundleUri} 的依赖资源{dependencePath} 失败");
                            completeCallback?.Invoke(null);
                            return;
                        } //某个依赖加载失败

                        allDependenceAssetBundleRecords[dependencePath] = dependenceRecord;
                    });
                }
            }


            AssetBundleCreateRequest requst = LoadAssetBundleFromFileAsync(assetBundleUri);
            AsyncManager.StartAsyncOperation(requst, () =>
            {
                if (requst.assetBundle == null)
                {
                    Debug.LogError($"LoadAssetBundleRecordAsync Fail,AssetBundle NOT Exit {assetBundleUri}");
                    completeCallback?.Invoke(null);
                    return;
                }

                assetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(assetBundleUri, requst.assetBundle);
                if (allDependenceAssetBundleRecords != null)
                {
                    //记录依赖这些AssetBundle
                    foreach (var dependenceAssetBundleRecord in allDependenceAssetBundleRecords.Values)
                        assetBundleRecordInfor.AddDependenceAssetBudleRecord(dependenceAssetBundleRecord);
                    allDependenceAssetBundleRecords.Clear();
                }

                s_AllAssetBundleRecordInfors[assetBundleUri] = assetBundleRecordInfor;
                completeCallback?.Invoke(assetBundleRecordInfor);
            }, null);
        }


        /// <summary>/// 加载缓存的 子资源
        /// <param name="assetFullUri">资源相对于Resources的完整路径</param>
        private LoadAssetBundleAssetRecord LoadAssetBundleSubAssetFromCache(string assetFullUri)
        {
            if (s_AllLoadedAssetBundleSubAssetRecord.TryGetValue(assetFullUri, out var record))
                return record;
            return null;
        }


        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// /// <param name="assetFullUri">资源相对于Resouces 完整的路径</param>
        public ILoadAssetRecord AssetBundleLoadAssetSync(string assetFullUri)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;

            GetBundleNameByAssetPath(assetFullUri, out var assetBundleUri, out var assetRelativeUri);
            return AssetBundleLoadAssetSync(assetFullUri, assetBundleUri, assetRelativeUri);
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetFullUri">资源相对于Resouces 完整的路径</param>
        public ILoadAssetRecord AssetBundleLoadAssetSync(string assetFullUri, string assetBundleUri, string assetRelativeUri)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;

            if (string.IsNullOrEmpty(assetRelativeUri))
            {
                Debug.LogError($"AssetBundleLoadAssetSync Fail,assetRelativeUri Is Null {assetFullUri} ");
                return null;
            }

            //  assetRelativeUri = assetRelativeUri.GetPathWithOutExtension().ToLower(); //这里可能带有相对路径
            assetRelativeUri = assetRelativeUri.ToLower();

            var assetBundleRecordInfor = LoadAssetBundleRecordSync(assetBundleUri);
            if (assetBundleRecordInfor != null)
            {
                Object asset = assetBundleRecordInfor.LoadAsset<Object>(assetRelativeUri);
                if (asset != null)
                {
                    assetRecord = RecordAssetBundleLoadSubAsset(assetFullUri, assetBundleRecordInfor, assetRelativeUri, asset);
                    return assetRecord;
                }
            }

            return null;
        }


        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        public void AssetBundleLoadAssetAsync(string assetFullUri, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            GetBundleNameByAssetPath(assetFullUri, out var assetBundleUri, out var assetRelativeUri);
            AssetBundleLoadAssetAsync(assetFullUri, assetBundleUri, assetRelativeUri, loadCallback);
        }

        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        public void AssetBundleLoadAssetAsync(string assetFullUri, string assetBundleUri, string assetRelativeUri, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            if (string.IsNullOrEmpty(assetRelativeUri))
            {
                Debug.LogError($"AssetBundleLoadAssetAsync Fail, 加载的资源 {assetFullUri}  名 为null");
                loadCallback?.Invoke(null);
                return;
            }

            //    assetName = System.IO.Path.GetFileNameWithoutExtension(assetName).ToLower();
            assetRelativeUri = assetRelativeUri.ToLower();

            LoadAssetBundleRecordAsync(assetBundleUri, (assetBundleInfor) =>
            {
                if (assetBundleInfor != null)
                {
                    Object asset = assetBundleInfor.LoadAsset<Object>(assetRelativeUri);
                    if (asset != null)
                    {
                        assetRecord = RecordAssetBundleLoadSubAsset(assetFullUri, assetBundleInfor, assetRelativeUri, asset);
                        loadCallback?.Invoke(assetRecord);
                        return;
                    }
                }

                Debug.LogError($"AssetBundleLoadAssetAsync Fail,assetFullUri={assetFullUri}  ");
                loadCallback?.Invoke(null);
            });
        }

        #endregion

        #region 资源释放

        /// <summary>
        /// 删除从AssetBundle 中加载某个资源的记录
        /// </summary>
        /// <param name="loadAssetBundleAssetRecord"></param>
        public void RemoveLoadAssetBundleAssetRecord(LoadAssetBundleAssetRecord loadAssetBundleAssetRecord)
        {
            if (loadAssetBundleAssetRecord == null) return;
            s_AllLoadedAssetBundleSubAssetRecord.Remove(loadAssetBundleAssetRecord.mAssetFullUri);
            AssetBundleRecordInfor assetBundleRecordInfor = loadAssetBundleAssetRecord.mDependenceAssetBundleRecord;
            if (assetBundleRecordInfor != null)
            {
                assetBundleRecordInfor.NotifyRemoveBeReferenceByLoadAsset(loadAssetBundleAssetRecord);
                if (assetBundleRecordInfor.mTotalReferenceCount == 0)
                {
                    assetBundleRecordInfor.RemoveAllDependenceAssetBudleRecord();
                    //   AssetBundleRecordInfor.ReleaseAssetBundleRecordInfor(assetBundleRecordInfor);
                } //没有引用
            }
        }

        /// <summary>
        /// 移除所有没有被引用的AssetBundle 记录
        /// </summary>
        public void RemoveAllUnReferenceAssetBundleRecord(ref HashSet<int> assetBundleInstanceIDs)
        {
            if (assetBundleInstanceIDs == null)
                assetBundleInstanceIDs = new HashSet<int>();

            Dictionary<string, AssetBundleRecordInfor> tempAssetBundleRecordInfors = new Dictionary<string, AssetBundleRecordInfor>(s_AllAssetBundleRecordInfors);

            foreach (var assetBundleRecordInfor in tempAssetBundleRecordInfors.Values)
            {
                if (assetBundleRecordInfor == null) continue;
                if (assetBundleRecordInfor.mTotalReferenceCount != 0) continue;


                int assetBundleInstanceID = assetBundleRecordInfor.mAssetBundleInstanceID;
                if (assetBundleInstanceID != -1 && assetBundleInstanceIDs.Contains(assetBundleInstanceID) == false)
                    assetBundleInstanceIDs.Add(assetBundleInstanceID);

                s_AllAssetBundleRecordInfors.Remove(assetBundleRecordInfor.mAssetBundleUri);
                AssetBundleRecordInfor.ReleaseAssetBundleRecordInfor(assetBundleRecordInfor);
            }
        }

        #endregion


        #region 辅助

        /// <summary>
        ///保存下载的AssetBundel 资源
        /// </summary>
        public void SaveAssetBundleFromDownload(DownloadHandlerBuffer handle, string assetBundleName)
        {
            if (handle == null)
                return;
            string fileSavePath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleName);
            IOUtility.CreateOrSetFileContent(fileSavePath, handle.data, false);
            Debug.LogInfor($"保存下载的AssetBundle 资源{fileSavePath} ");
        }


        /// <summary>
        /// 记录从AssetBundel 加载的资源
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        /// <param name="parentAssetBundle"></param>
        private LoadAssetBundleAssetRecord RecordAssetBundleLoadSubAsset(string assetFullUri, AssetBundleRecordInfor referenceAssetBundle, string assetRelativeUri, Object assetInfor)
        {
            if (s_AllLoadedAssetBundleSubAssetRecord.TryGetValue(assetFullUri, out var record))
                return record;

            record = LoadAssetBundleAssetRecord.GetLoadAssetBundleAssetRecord(assetFullUri, referenceAssetBundle, assetRelativeUri, assetInfor);

            s_AllLoadedAssetBundleSubAssetRecord[assetFullUri] = record;
            return record;
        }

        #endregion
    }
}