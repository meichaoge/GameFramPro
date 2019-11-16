using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.Upgrade;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>///
    ///  AssetBundle 资源管理器
    /// /// </summary>
    public static class AssetBundleManager
    {
        private static AssetBundleAssetTotalInfor s_ServerBundleAssetConfigInfor = null; //服务器上最新的AssetBundle 配置信息

        private static float s_LastFullCheckTime { get; set; } = 0;  //上一次全资源检测的时间 
        private static float s_FullCheckTimeInterval { get { return 300; } } //全资源空引用检测时间间隔


        //记录路径与AssetBundle的映射关系
        private static readonly Dictionary<string, AssetBundleRecordInfor> s_AllAssetBundleRecordInfors = new Dictionary<string, AssetBundleRecordInfor>();

        //key=需要加载的资源的详细路径 不能是资源名 所有加载的AssetBundle 包里面的资源
        private static readonly Dictionary<string, LoadAssetBundleAssetRecord> s_AllLoadedAssetBundleSubAssetRecord = new Dictionary<string, LoadAssetBundleAssetRecord>(50);


        #region 内部加载AssetBundle 方法，对 AssetBundle类加载方式的封装

        public static AssetBundle LoadAssetBundleFromFile(string assetBundleRelativePath)
        {
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath);
        }

        public static AssetBundle LoadAssetBundleFromMemory(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes);
        }

        public static AssetBundle LoadAssetBundleFromFile(string assetBundleRelativePath, uint crc)
        {
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFile(realPath, crc);
        }

        public static AssetBundle LoadAssetBundleFromMemory(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemory(assetBundelBytes, crc);
        }

        public static AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string assetBundleRelativePath)
        {
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath);
        }

        public static AssetBundleCreateRequest LoadAssetBundleFromMemoryAsync(byte[] assetBundelBytes)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes);
        }

        public static AssetBundleCreateRequest LoadAssetBundleFromFileAsync(string assetBundleRelativePath, uint crc)
        {
            string realPath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleRelativePath);
            return AssetBundle.LoadFromFileAsync(realPath, crc);
        }

        public static AssetBundleCreateRequest LoadAssetBundleFromMemoryAsync(byte[] assetBundelBytes, uint crc)
        {
            if (assetBundelBytes == null || assetBundelBytes.Length == 0)
                return null;

            return AssetBundle.LoadFromMemoryAsync(assetBundelBytes, crc);
        }

        #endregion


        /// <summary>/// 
        /// 在AssetBundle 流程更新完之后需要调用这个接口完成保存最新配置的功能
        /// /// </summary>
        public static void SaveAssetBundleTotalConfigInfor(AssetBundleAssetTotalInfor newConfig)
        {
            s_ServerBundleAssetConfigInfor = newConfig;
        }

        #region   同步 & 异步  加载AssetBundle 
        /// <summary>//
        /// / 加载缓存的 AssetBundleRecordInfor
        /// <param name="assetFullUri">资源相对于Resources的完整路径</param>
        private static AssetBundleRecordInfor LoadAssetBundleRecordInforFromCache(string assetBundleNameUri)
        {
            if (s_AllAssetBundleRecordInfors.TryGetValue(assetBundleNameUri, out var assetBundleRecordInfor))
            {
                if (assetBundleRecordInfor == null || assetBundleRecordInfor.IsAssetBundleEnable == false)
                {
                    s_AllLoadedAssetBundleSubAssetRecord.Remove(assetBundleNameUri);
                    if (assetBundleRecordInfor != null)
                        AssetBundleRecordInfor.ReleaseAssetBundleRecordInfor(assetBundleRecordInfor);
                    return null;
                }
                return assetBundleRecordInfor;
            }
            return null;
        }

        /// <summary>
        /// 同步 加载 AssetBundleRecordInfor 资源
        /// </summary>
        /// <param name="assetBundleNameUri">AssetBundle 文件名</param>
        /// <param name="assetBundleDirectoryUri">AssetBundle 相对目录 (可能是null)</param>
        /// <returns></returns>
        private static AssetBundleRecordInfor LoadAssetBundleRecordSync(string assetBundleNameUri, string assetBundleDirectoryUri)
        {
            if (string.IsNullOrEmpty(assetBundleNameUri))
            {
                Debug.LogError("LoadAssetBundleRecordSync Fail,Parameter is null");
                return null;
            }

            //缓存中取
            AssetBundleRecordInfor assetBundleRecordInfor = LoadAssetBundleRecordInforFromCache(assetBundleNameUri);
            if (assetBundleRecordInfor != null && assetBundleRecordInfor.IsAssetBundleEnable)
                return assetBundleRecordInfor;

            //依赖加载AssetBundle 并递归记录依赖关系
            Dictionary<string, string> dependenceBundlesPath = GetAssetBundleAllDependencies(assetBundleNameUri);
            //key = assetBundleName
            Dictionary<string, AssetBundleRecordInfor> allDependenceAssetBundleRecords = null;
            if (dependenceBundlesPath != null && dependenceBundlesPath.Count > 0)
            {
                allDependenceAssetBundleRecords = new Dictionary<string, AssetBundleRecordInfor>(dependenceBundlesPath.Count);
                foreach (var dependencePath in dependenceBundlesPath)
                {
                    var dependenceAssetBundleRecord = LoadAssetBundleRecordSync(dependencePath.Key, dependencePath.Value);
                    if (dependenceAssetBundleRecord == null)
                    {
                        Debug.LogError($"加载{dependencePath.Key} 的依赖资源{ dependencePath.Value} 失败");
                        return null;
                    }

                    allDependenceAssetBundleRecords[dependencePath.Key] = dependenceAssetBundleRecord;
                }
            } //递归加载依赖


            //加载参数指定的AssetBundle 
            AssetBundle assetBundle = null;
            if (string.IsNullOrEmpty(assetBundleDirectoryUri))
                assetBundle = LoadAssetBundleFromFile(assetBundleNameUri);
            else
                assetBundle = LoadAssetBundleFromFile(assetBundleDirectoryUri.CombinePathEx(assetBundleNameUri));

            return RecordAssetBundleDepdence(assetBundleNameUri, assetBundle, allDependenceAssetBundleRecords);
        }

        /// <summary>
        ///  异步加载 AssetBundleRecordInfor 资源
        /// </summary>
        /// <param name="assetBundleNameUri">AssetBundle 文件名</param>
        /// <param name="assetBundleDirectoryUri">AssetBundle 相对目录(可能是null)</param>
        /// <param name="completeCallback"></param>
        private static void LoadAssetBundleRecordAsync(string assetBundleNameUri, string assetBundleDirectoryUri, Action<AssetBundleRecordInfor> completeCallback)
        {
            if (string.IsNullOrEmpty(assetBundleNameUri))
            {
                Debug.LogError("LoadAssetBundleRecordAsync Fail,Parameter is null");
                completeCallback?.Invoke(null);
                return;
            }

            //缓存中取
            AssetBundleRecordInfor assetBundleRecordInfor = LoadAssetBundleRecordInforFromCache(assetBundleNameUri);
            if (assetBundleRecordInfor != null && assetBundleRecordInfor.IsAssetBundleEnable)
            {
                completeCallback?.Invoke(assetBundleRecordInfor);
                return;
            }

            // 依赖加载AssetBundle 并递归记录依赖关系
            Dictionary<string, string> dependenceBundlesPath = GetAssetBundleAllDependencies(assetBundleNameUri);
            //key = assetBundleName
            Dictionary<string, AssetBundleRecordInfor> allDependenceAssetBundleRecords = null;
            if (dependenceBundlesPath != null && dependenceBundlesPath.Count > 0)
            {
                allDependenceAssetBundleRecords = new Dictionary<string, AssetBundleRecordInfor>(dependenceBundlesPath.Count);
                int depdenceABundleCount = dependenceBundlesPath.Count;
                foreach (var dependencePath in dependenceBundlesPath)
                {
                    LoadAssetBundleRecordAsync(dependencePath.Key, dependencePath.Value, (dependenceRecord) =>
                    {
                        if (dependenceRecord == null)
                        {
                            Debug.LogError($"加载{dependencePath.Key} 的依赖资源{dependencePath.Value} 失败");
                            completeCallback?.Invoke(null);
                            return;
                        } //某个依赖加载失败

                        allDependenceAssetBundleRecords[dependencePath.Key] = dependenceRecord;
                        --depdenceABundleCount;
                        //全部以来加载完
                        if (depdenceABundleCount == 0)
                        {
                            AssetBundleCreateRequest requst = null;

                            if (string.IsNullOrEmpty(assetBundleDirectoryUri))
                                requst = LoadAssetBundleFromFileAsync(assetBundleNameUri);
                            else
                                requst = LoadAssetBundleFromFileAsync(assetBundleDirectoryUri.CombinePathEx(assetBundleNameUri));

                            if (requst == null)
                            {
                                Debug.LogError($"LoadAssetBundleRecordAsync Fail,AssetBundle NOT Exit {assetBundleNameUri}");
                                completeCallback?.Invoke(null);
                                return;
                            }

                            AsyncManager.StartAsyncOperation(requst, () => completeCallback?.Invoke(RecordAssetBundleDepdence(assetBundleNameUri, requst.assetBundle, allDependenceAssetBundleRecords)), null);
                        }
                    });
                }
            }
        }


        /// <summary>
        /// 生成AssetBundle 加载记录并记录其依赖的其他 AssetBundle
        /// </summary>
        /// <param name="assetBundleNameUri"></param>
        /// <param name="assetBundle"></param>
        /// <param name="allDependenceAssetBundleRecords"></param>
        /// <returns></returns>
        private static AssetBundleRecordInfor RecordAssetBundleDepdence(string assetBundleNameUri, AssetBundle assetBundle, Dictionary<string, AssetBundleRecordInfor> allDependenceAssetBundleRecords)
        {
            if (assetBundle == null)
            {
                Debug.LogError($"RecordAssetBundleDepdence Fail,AssetBundle NOT Exit {assetBundleNameUri}");
                return null;
            }

            AssetBundleRecordInfor assetBundleRecordInfor = AssetBundleRecordInfor.GetAssetBundleRecordInfor(assetBundleNameUri, assetBundle);
            if (allDependenceAssetBundleRecords != null)
            {
                //记录依赖这些AssetBundle
                foreach (var dependenceAssetBundleRecord in allDependenceAssetBundleRecords.Values)
                    assetBundleRecordInfor.AddDependenceAssetBudleRecord(dependenceAssetBundleRecord);
                allDependenceAssetBundleRecords.Clear();
            }

            s_AllAssetBundleRecordInfors[assetBundleNameUri] = assetBundleRecordInfor;
            return assetBundleRecordInfor;
        }


        #endregion



        #region 同步 & 异步 加载本地的 AssetBundle 中的资源

        /// <summary>/// 加载缓存的 子资源
        /// <param name="assetFullUri">资源相对于Resources的完整路径</param>
        private static LoadAssetBundleAssetRecord LoadAssetBundleSubAssetFromCache(string assetFullUri)
        {
            if (s_AllLoadedAssetBundleSubAssetRecord.TryGetValue(assetFullUri, out var record))
            {
                if (record == null || record.IsRecordEnable == false)
                {
                    s_AllLoadedAssetBundleSubAssetRecord.Remove(assetFullUri);
                    LoadAssetBundleAssetRecord.ReleaseAssetBundleRecordInfor(record);
                    return null;
                }

                return record;
            }
            return null;
        }


        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// /// <param name="assetFullUri">资源相对于Resouces 完整的路径</param>
        public static ILoadAssetRecord AssetBundleLoadAssetSync(string assetFullUri)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;

            GetBundleNameByAssetPath(assetFullUri, out var assetBundleNameUri, out var assetBundleDirectoryUri, out var assetRelativeUri);
            return AssetBundleLoadAssetSync(assetFullUri, assetBundleNameUri, assetBundleDirectoryUri, assetRelativeUri);
        }

        /// <summary>
        /// 同步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        /// <param name="assetFullUri">资源相对于Resouces 完整的路径</param>
        public static ILoadAssetRecord AssetBundleLoadAssetSync(string assetFullUri, string assetBundleNameUri, string assetBundleDirectoryUri, string assetRelativeUri)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
                return assetRecord;

            if (string.IsNullOrEmpty(assetRelativeUri))
            {
                //  Debug.LogError($"AssetBundleLoadAssetSync Fail,assetRelativeUri Is Null {assetFullUri} ");
                return null;
            }

            var assetBundleRecordInfor = LoadAssetBundleRecordSync(assetBundleNameUri, assetBundleDirectoryUri);
            if (assetBundleRecordInfor != null)
            {
                Object asset = assetBundleRecordInfor.LoadAsset<Object>(assetRelativeUri);
                if (asset != null)
                {
                    assetRecord = LoadAssetBundleAssetRecord.GetLoadAssetBundleAssetRecord(assetFullUri, assetBundleRecordInfor, assetRelativeUri, asset);
                    s_AllLoadedAssetBundleSubAssetRecord[assetFullUri] = assetRecord;
                    return assetRecord;
                }
            }

            return null;
        }


        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        public static void AssetBundleLoadAssetAsync(string assetFullUri, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            GetBundleNameByAssetPath(assetFullUri, out var assetBundleNameUri, out var assetBundleRelativeUri, out var assetRelativeUri);
            AssetBundleLoadAssetAsync(assetFullUri, assetBundleNameUri, assetBundleRelativeUri, assetRelativeUri, loadCallback);
        }

        /// <summary>
        /// 异步加载AssetBundle 方法（优先从缓存中读取）
        /// </summary>
        public static void AssetBundleLoadAssetAsync(string assetFullUri, string assetBundleNameUri, string assetBundleDirectoryUri, string assetRelativeUri, Action<ILoadAssetRecord> loadCallback)
        {
            LoadAssetBundleAssetRecord assetRecord = LoadAssetBundleSubAssetFromCache(assetFullUri);
            if (assetRecord != null && assetRecord.IsRecordEnable)
            {
                loadCallback?.Invoke(assetRecord);
                return;
            }

            if (string.IsNullOrEmpty(assetRelativeUri))
            {
                Debug.LogError($"AssetBundleLoadAssetAsync Fail, 加载的资源 {assetFullUri}  为null");
                loadCallback?.Invoke(null);
                return;
            }


            LoadAssetBundleRecordAsync(assetBundleNameUri, assetBundleDirectoryUri, (assetBundleInfor) =>
            {
                if (assetBundleInfor != null)
                {
                    Object asset = assetBundleInfor.LoadAsset<Object>(assetRelativeUri);
                    if (asset != null)
                    {
                        assetRecord = LoadAssetBundleAssetRecord.GetLoadAssetBundleAssetRecord(assetFullUri, assetBundleInfor, assetRelativeUri, asset);
                        s_AllLoadedAssetBundleSubAssetRecord[assetFullUri] = assetRecord;
                        loadCallback?.Invoke(assetRecord);
                        return;
                    }
                }

                Debug.LogError($"AssetBundleLoadAssetAsync Fail,assetFullUri={assetFullUri}  ");
                loadCallback?.Invoke(null);
            });
        }


        /// <summary>
        ///  根据指定的路径标识获取AssetBundle 和资源相对uri
        /// </summary>
        /// <param name="assetFullUri">需要加载的资源全路径 不带扩展名</param>
        /// <param name="assetBundleNameUri">当前资源所在的AssetBundle 资源名</param>
        /// <param name="assetBundleRelativeUri">当前资源所在的AssetBundle 资源相对路径</param>
        /// <param name="assetRelativeUri">当前资源所在的 AssetBundle 资源中加载改资源的名称</param>
        private static void GetBundleNameByAssetPath(string assetFullUri, out string assetBundleNameUri, out string assetBundleDirectoryUri, out string assetRelativeUri)
        {
            assetBundleDirectoryUri = assetBundleNameUri = assetRelativeUri = string.Empty;
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetBundleNameByAssetPath Fail,没有获取最新的AssetBundle 配置 ");
                return;
            }

            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Value.ContainAssetReUri.TryGetValue(assetFullUri, out assetRelativeUri))
                {
                    assetBundleNameUri = assetBundleInfor.Key;
                    assetBundleDirectoryUri = assetBundleInfor.Value.RelDirctory;
                    return;
                }
            }
#if UNITY_EDITOR
            Debug.LogEditorInfor($"GetBundleNameByAssetPath Fail,没有找到资源 {assetRelativeUri}");
#endif
        }

        #endregion

        #region 资源释放

 

        /// <summary>
        /// 移除所有没有被引用的AssetBundle 记录
        /// </summary>
        public static void RemoveAllUnReferenceAssetBundleRecord(HashSet<int> assetBundleInstanceIDs)
        {
            if (s_LastFullCheckTime == 0)
                s_LastFullCheckTime = Time.realtimeSinceStartup;

            if (assetBundleInstanceIDs != null && assetBundleInstanceIDs.Count > 0 || (Time.realtimeSinceStartup - s_LastFullCheckTime) >= s_FullCheckTimeInterval)
            {
                s_LastFullCheckTime = Time.realtimeSinceStartup;

                Dictionary<string, AssetBundleRecordInfor> tempAssetBundleRecordInfors = new Dictionary<string, AssetBundleRecordInfor>(s_AllAssetBundleRecordInfors);

                foreach (var assetBundleRecordInfor in tempAssetBundleRecordInfors)
                {
                    if (assetBundleRecordInfor.Value == null)
                    {
                        s_AllAssetBundleRecordInfors.Remove(assetBundleRecordInfor.Key);
                        continue;
                    }

                    if (assetBundleRecordInfor.Value.mTotalReferenceCount == 0 || assetBundleRecordInfor.Value.IsAssetBundleEnable == false)
                    {
                        s_AllAssetBundleRecordInfors.Remove(assetBundleRecordInfor.Key);
                        AssetBundleRecordInfor.ReleaseAssetBundleRecordInfor(assetBundleRecordInfor.Value);
                        continue;
                    }
                    if (assetBundleInstanceIDs == null || assetBundleInstanceIDs.Count == 0)
                        continue;


                    int assetBundleInstanceID = assetBundleRecordInfor.Value.mAssetBundleInstanceID;
                    if (assetBundleInstanceIDs.Contains(assetBundleInstanceID))
                    {
#if UNITY_EDITOR
                        Debug.Log($"释放AssetBundle 资源： {assetBundleRecordInfor.Value.mAssetBundleNameUri}");
#endif
                        s_AllAssetBundleRecordInfors.Remove(assetBundleRecordInfor.Value.mAssetBundleNameUri);
                        AssetBundleRecordInfor.ReleaseAssetBundleRecordInfor(assetBundleRecordInfor.Value);
                    }

                }
            }
        }

        #endregion



        /// <summary>
        /// 获取指定参数的AssetBundle 依赖的信息
        /// </summary>
        /// <param name="assetBundleName"> AssetBundle 文件名</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetAssetBundleAllDependencies(string assetBundleName)
        {
            if (s_ServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetAssetBundleAllDependencies Fail,没有初始化获取AssetBundle 配置信息 ");
                return null;
            }
            //   assetBundleName = assetBundleName.GetPathStringEx();
            foreach (var assetBundleInfor in s_ServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Key == assetBundleName)
                    return assetBundleInfor.Value.DepABundleInfor;
            }

            Debug.LogError($"GetAssetBundleAllDependencies Fail,没有找到依赖关系 {assetBundleName}");
            return null;
        }


    }
}