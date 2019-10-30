using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramePro.NetWorkEx;
using UnityEngine.Networking;
using System.Threading;
using GameFramePro.ResourcesEx;
using System.Threading.Tasks;

namespace GameFramePro.Upgrade
{
    /// <summary> 
    /// 负责AssetBundle 资源的更新逻辑
    /// 1 获取本地的AssetBundle 版本信息和所有的本地资源信息 (如果本地资源配置文件被删除则取程序中的配置版本号)
    /// 2 获取服务器的 AssetBundle 配置
    /// 3 计算版本差异包的名称和下载地址{跨版本} 只需要下载一个指定的压缩包
    /// 4 支持断点续传的下载一个大的压缩包(或者多个，多个的话需要服务器配置)
    ///  5 解压覆盖本地版本
    /// </summary>
    public class AssetBundleUpgradeManager : Single<AssetBundleUpgradeManager>, IUpgradeModule
    {
        #region 路径配置
        private static string s_AssetBundleCDNTopUrl = string.Empty;
        /// <summary>/// AssetBundle CDN顶层目录url/// </summary>
        public static string S_AssetBundleCDNTopUrl
        {
            get
            {
                if (string.IsNullOrEmpty(s_AssetBundleCDNTopUrl))
                    s_AssetBundleCDNTopUrl = ApplicationManager.S_TopCDNUrl.CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                return s_AssetBundleCDNTopUrl;
            }
        }

        private string mServerAssetBundleConfigUrl = string.Empty;
        /// <summary>
        /// 服务器AssetBundle 配置文件Url
        /// </summary>
        public string ServerAssetBundleConfigUrl
        {
            get
            {
                if (string.IsNullOrEmpty(mServerAssetBundleConfigUrl))
                    mServerAssetBundleConfigUrl = ApplicationManager.S_TopCDNUrl.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
                return mServerAssetBundleConfigUrl;
            }
        }

        #endregion


        #region 需要更新的AssetBundle 信息

        //key =AssetBundleName
        private Dictionary<string, UpdateAssetBundleInfor> mAllNeedUpdateAssetBundleAssetInfor = new Dictionary<string, UpdateAssetBundleInfor>(); //所有需要更新的资源
        private Dictionary<string, UpdateAssetBundleInfor> mAllDownloadFailAssetBundleInfors = new Dictionary<string, UpdateAssetBundleInfor>(); //所有更新时候失败的资源

        //key=assetBundle 资源的路径 value=每个AssetBundle资源信息
        private Dictionary<string, UpdateAssetBundleInfor> mAllLocalAssetBundleAssetFileInfor = new Dictionary<string, UpdateAssetBundleInfor>();

        //本地无效的AssetBundle 需要删除的路径记录
        private HashSet<string> mAllNeedDeleteABundleFullUris = new HashSet<string>();

        /// <summary>/// 服务器上最新的AssetBundle 配置/// </summary>
        private AssetBundleAssetTotalInfor mServerBundleAssetConfigInfor = null; //从服务器获取的 AssetBundle 配置
        private string mServerBundleAssetConfigStr = string.Empty;  ////从服务器获取的 AssetBundle 配置

        /// <summary>/// 总共需要下载的资源总数/// </summary>
        public uint TotalNeedDownloadAssetCount { get; private set; }

        private uint mCurDownloadCount = 0; //当前下载次数
        private const uint S_MaxDownloadTimes = 3; //最大下载次数

        #endregion


        #region IUpgradeModule 接口

        public event System.Action OnBeginUpgradeEvent;
        public event System.Action<string, float> OnUpgradeProcessEvent;
        public event System.Action<string> OnUpgradeFailEvent;
        public event System.Action OnUpgradeSuccessEvent;
        public event System.Action OnReBeginUpgradeEvent;

        public float CurProcess { get; private set; } = 0f; //当前的下载进度
        public UpgradeStateUsage mUpgradeState { get; private set; } = UpgradeStateUsage.Initialed;

        public IEnumerator OnBeginUpgrade()
        {
            mUpgradeState = UpgradeStateUsage.Begin;
            OnBeginUpgradeEvent?.Invoke();

            var assetBundleUpgradeSuperCoroutine = new SuperCoroutine(AssetBundleIUpgradeProcess(true));
            yield return assetBundleUpgradeSuperCoroutine.WaitDone(true);
        }

        public void OnUpgradeProcess(string message, float process)
        {
            mUpgradeState = UpgradeStateUsage.Upgrading;
            CurProcess = process;

#if UNITY_EDITOR
            Debug.LogEditorInfor($"OnUpgradeProcess  Now={DateTime.Now.ToString()}  {message}");
#endif
            OnUpgradeProcessEvent?.Invoke(message, process);
        }

        public void OnUpgradeFail(string message)
        {
            mUpgradeState = UpgradeStateUsage.UpgradeFail;
            Debug.LogError(message);
            OnUpgradeFailEvent?.Invoke(message);
        }

        public void OnUpgradeSuccess()
        {
            mUpgradeState = UpgradeStateUsage.UpgradeSuccess;
            Debug.LogInfor("所有的AssetBundle 资源更新已经完成!!");

            string filePath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.GetFilePathParentDirectory(1).CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
            IOUtility.CreateOrSetFileContent(filePath, mServerBundleAssetConfigStr);
            AssetBundleManager.S_Instance.SaveAssetBundleTotalConfigInfor(mServerBundleAssetConfigInfor); //保存得到的配置

            OnUpgradeSuccessEvent?.Invoke();
            mServerBundleAssetConfigStr = string.Empty;
        }

        public IEnumerator OnReBeginUpgrade()
        {
            mUpgradeState = UpgradeStateUsage.Begin;
            OnReBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("  准备重新下载 ", 0.2f);

            mAllNeedUpdateAssetBundleAssetInfor.Clear();
            foreach (var reDownloadFailAssetBundleInfor in mAllDownloadFailAssetBundleInfors)
                mAllNeedUpdateAssetBundleAssetInfor.Add(reDownloadFailAssetBundleInfor.Key, reDownloadFailAssetBundleInfor.Value);

            var assetBundleUpgradeSuperCoroutine = new SuperCoroutine(AssetBundleIUpgradeProcess(false));
            yield return assetBundleUpgradeSuperCoroutine.WaitDone(true); //重新下载逻辑


            if (mServerBundleAssetConfigInfor == null)
            {
                OnUpgradeProcess("获取服务器上AssetBundle 配置信息 ", 0.25f);
                var getServerAssetBundleConfig = new SuperCoroutine(GetServerAssetBundleServerAssetConfig());
                yield return getServerAssetBundleConfig.WaitDone(); //下载服务器的配置
            }
        }

        #endregion


        #region 获取本地AssetBundle 资源以及对于的配置表                                               

        /// <summary>/// AssetBundel 更新逻辑 。isFirstUpgrade=true 则表示第一次更新下载/// </summary>
        private IEnumerator AssetBundleIUpgradeProcess(bool isFirstUpgrade)
        {
            OnUpgradeProcess("获取服务器上AssetBundle 配置信息 ", 0);

            SuperCoroutine localAssetBundleSuperCoroutine = null; //获取本地AssetBundle 信息
            if (isFirstUpgrade)
                localAssetBundleSuperCoroutine = new SuperCoroutine(GetAllLocalAssetBundleInformation());

            if (mServerBundleAssetConfigInfor == null)
            {
                var getServerAssetBundleConfig = new SuperCoroutine(GetServerAssetBundleServerAssetConfig());
                yield return getServerAssetBundleConfig.WaitDone(); //等待完成获取服务器配置
            }

            if (localAssetBundleSuperCoroutine != null)
            {
                yield return localAssetBundleSuperCoroutine.WaitDone(true); //获取本地AssetBundle 的信息
                OnUpgradeProcess("本地AssetBundle 资源状态获取完成 ", 0.1f);
            } //后判断避免阻塞服务器配置下载


            //为了确保重试时候不用重新获取本地资源信息 这里要先处理本地资源
            if (mServerBundleAssetConfigInfor == null)
            {
                OnUpgradeFail("服务器最新的AssetBundle配置下载失败,请检查网络后重试!!!");
                yield break;
            }


            if (isFirstUpgrade)
            {
                OnUpgradeProcess(" 开始获取需要更新的 AssetBundle 资源 ", 0.2f);
                var getNeedUpdateAssetBundleCoroutineEx = new SuperCoroutine(GetAllNeedUpdateAssetBundleConfig());
                yield return getNeedUpdateAssetBundleCoroutineEx.WaitDone(); //获取需要下载的资源列表
                OnUpgradeProcess(" 获取需要更新的 AssetBundle 资源 完成", 0.3f);
            } //第一次需要判断哪些资源需要下载


            TotalNeedDownloadAssetCount = (uint)mAllNeedUpdateAssetBundleAssetInfor.Count; //需要下载的资源总数
            if (TotalNeedDownloadAssetCount != 0)
            {
                OnUpgradeProcess(" 开始更新的 AssetBundle 资源 ", 0.35f);
                var downloadAssetBundleCoroutineEx = new SuperCoroutine(BeginDownloadAllAssetBundle());
                yield return downloadAssetBundleCoroutineEx.WaitDone(); //开始下载资源
                OnUpgradeProcess(" 更新的 AssetBundle 资源 完成 ", 0.9f);
            }
            else
                OnUpgradeProcess(" 本地的 AssetBundle 资源 是最新的 ", 0.9f);

            //下载流程结束
            if (mAllNeedUpdateAssetBundleAssetInfor.Count == 0 && mAllDownloadFailAssetBundleInfors.Count == 0)
                OnUpgradeSuccess();
            else
                OnUpgradeFail("部分资源多次下载均失败了 无法继续下载. 请检查网络后重试"); //有任务没有下载完成
        }


        private float mAllLocalAssetBundleLoadProcess = 0f; //获取进度

        /// <summary>/// 首先需要判断本地文件有没有被修改过(或者被删除了部分资源)，如果有改动则需要读取每个文件的MD5,否则只需要根据本地配置文件修改即可/// </summary>
        private IEnumerator GetAllLocalAssetBundleInformation()
        {
            bool isLocalAssetBundleExit = false;

            // 检测本地的AssetBundle 资源是否存在

            isLocalAssetBundleExit = System.IO.Directory.Exists(ConstDefine.S_LocalAssetBundleTopDirectoryPath);
            string[] allAssetBundleFiles = null;
            if (isLocalAssetBundleExit)
            {
                allAssetBundleFiles = System.IO.Directory.GetFiles(ConstDefine.S_LocalAssetBundleTopDirectoryPath, "*.*", System.IO.SearchOption.AllDirectories);
                if (allAssetBundleFiles.Length == 0)
                    isLocalAssetBundleExit = false;
            }

            // 如果本地资源存在则获取本地资源的详细信息 并录入

            #region 使用Loom

            //            if (isLocalAssetBundleExit)
            //            {
            //                ///利用多线程计算本地资源的信息
            //                Loom.S_Instance.RunAsync(() =>
            //                {
            //                    for (int dex = 0; dex < allAssetBundleFiles.Length; dex++)
            //                    {
            //                        var filePath = allAssetBundleFiles[dex];
            //                        AssetBundleInfor assetBundleInfor = new AssetBundleInfor();
            //                        assetBundleInfor.mBundleName = filePath.Substring(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath.Length + 1);
            //                        assetBundleInfor.mBundleMD5Code = MD5Helper.GetFileMD5(filePath, ref assetBundleInfor.Size);
            //                        string assetBundleNameStr = assetBundleInfor.mBundleName.GetPathStringEx(); //去掉路径分隔符
            //                        mAllLocalAssetBundleAssetFileInfor[assetBundleNameStr] = assetBundleInfor; //记录本地AssetBundle 资源信息
            //                        mAllLocalAssetBundleLoadProcess = dex * 1f / allAssetBundleFiles.Length; //进度
            //                    }
            //
            //                    isCompleteGetLocalFileInfor = true;
            //                    //   Loom.S_Instance.QueueOnMainThread(OnCompleteAllLocalAssetBundleInfor);
            //                });
            //            } //获取本地的所有的AssetBundle 信息 (文件MD5 大小等)
            //            else
            //            {
            //                Debug.LogInfor("BeginUpdateAssetBundle 本地没有资源 下载所有的资源");
            //                mAllLocalAssetBundleLoadProcess = 1f;
            //                isCompleteGetLocalFileInfor = true;
            //            }
            //
            ////等待所有的本地资源信息的录入
            //            while (isCompleteGetLocalFileInfor == false)
            //                yield return AsyncManager.WaitFor_Null;

            #endregion

            if (isLocalAssetBundleExit)
            {
                yield return AsyncManager.JumpToBackground; //到子线程
                int dex = 0;
                foreach (var loadAssetBundlePath in allAssetBundleFiles)
                {
                    ++dex;
                    ////***文件名称校验 必须符合指定的规则
                    string assetUri = loadAssetBundlePath.Substring(ConstDefine.S_LocalAssetBundleTopDirectoryPath.Length + 1);
                    string fileName = IOUtility.GetFileNameWithoutExtensionEx(assetUri);
                    UpdateAssetBundleInfor assetBundleInfor = null;

                    if (AsserBundleDetail.GetAsserBundleDetail(fileName, out AsserBundleDetail asserBundleDetail))
                    {
#if UNITY_EDITOR
                        Debug.LogEditorInfor($"File={assetUri}    \t Size={asserBundleDetail.Size}    \t Md5={asserBundleDetail.MD5}");
#endif
                        assetBundleInfor = new UpdateAssetBundleInfor(assetUri, asserBundleDetail.Size, asserBundleDetail.MD5, loadAssetBundlePath);
                    }
                    else
                    {
                        assetBundleInfor = new UpdateAssetBundleInfor(string.Empty, 0, string.Empty, loadAssetBundlePath);
                    }

                    mAllLocalAssetBundleAssetFileInfor[fileName] = assetBundleInfor; //记录本地AssetBundle 资源信息
                    mAllLocalAssetBundleLoadProcess = dex * 1f / allAssetBundleFiles.Length; //进度

                }

                yield return AsyncManager.JumpToUnity; //回到主线程
            }
            else
            {
                Debug.LogInfor("BeginUpdateAssetBundle 本地没有资源 下载所有的资源");
                mAllLocalAssetBundleLoadProcess = 1f;
            }
        }

        /// <summary>/// 获取服务器的AssetBundle 配置/// </summary>
        private IEnumerator GetServerAssetBundleServerAssetConfig()
        {
            if (mServerBundleAssetConfigInfor != null)
            {
                Debug.LogInfor($"AssetBundle 服务器配置已经下载完成 不需要重新下载");
                yield break;
            }


            mCurDownloadCount = 1;

            while (mServerBundleAssetConfigInfor == null && mCurDownloadCount <= S_MaxDownloadTimes)
            {
                var downloadTask = DownloadManager.S_Instance.GetByteDataFromUrl(ServerAssetBundleConfigUrl, TaskPriorityEnum.Immediately, null);
                if (downloadTask == null)
                {
                    Debug.LogError("获取服务器配置的下载任务创建失败");
                }
                else
                {
                    while (downloadTask.TaskState == TaskStateEum.Initialed)
                        yield return AsyncManager.WaitFor_Null;


                    if (downloadTask.TaskSuperCoroutinenfor != null)
                    {
                        yield return downloadTask.TaskSuperCoroutinenfor.WaitDone(true);

                        var downLoadCallback = downloadTask.DownloadTaskCallbackData;

                        if (downLoadCallback == null || downLoadCallback.isNetworkError || downLoadCallback.isDone == false || downLoadCallback.isHttpError)
                            Debug.LogError($"OnCompleteGetServerAssetBundleConfig Fail Error  下载参数为null   {downLoadCallback?.url}");
                        else
                        {
                            mServerBundleAssetConfigStr = (downLoadCallback.downloadHandler as DownloadHandlerBuffer).text;
                            mServerBundleAssetConfigInfor = SerializeManager.DeserializeObject<AssetBundleAssetTotalInfor>(mServerBundleAssetConfigStr);
                            if (mServerBundleAssetConfigInfor != null)
                                yield break;
                        }
                    }

                    yield return AsyncManager.WaitFor_Null;
                    ++mCurDownloadCount; //下载失败继续下载
                }
            }
        }

        /// <summary>/// 获取所有需要更新的 AssetBundle 信息/// </summary>
        private IEnumerator GetAllNeedUpdateAssetBundleConfig()
        {
            if (mServerBundleAssetConfigInfor == null) yield break;

            //Key =AssetBundle Name, value{=true 标示需要更新 =fasle 标示需要删除资源}
            //对比服务器配置 获取哪些资源需要更新或者新增
            foreach (var assetBunleInfor in mServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (mAllLocalAssetBundleAssetFileInfor.TryGetValue(assetBunleInfor.Key, out var localAssetBundleConfig)) //标识为需要更新
                {
                    mAllLocalAssetBundleAssetFileInfor.Remove(assetBunleInfor.Key);//处理完成这个记录
                    if (localAssetBundleConfig.mMd5Code == assetBunleInfor.Value.GetABundleDetail(assetBunleInfor.Key).MD5)
                        continue;
                }

                if (localAssetBundleConfig == null)
                    localAssetBundleConfig = new UpdateAssetBundleInfor(assetBunleInfor.Key, assetBunleInfor.Value, string.Empty);
                if (string.IsNullOrEmpty(assetBunleInfor.Value.RelDirctory))
                    mAllNeedUpdateAssetBundleAssetInfor.Add(assetBunleInfor.Key, localAssetBundleConfig); //需要更新
                else
                    mAllNeedUpdateAssetBundleAssetInfor.Add(assetBunleInfor.Value.RelDirctory.CombinePathEx(assetBunleInfor.Key), localAssetBundleConfig); //需要更新

            }

            yield return AsyncManager.WaitFor_Null;


            //对比获取哪些资源需要删除
            foreach (var assetBunleInfor in mAllLocalAssetBundleAssetFileInfor)
            {
                if (mServerBundleAssetConfigInfor.IsContainABundle(assetBunleInfor.Key))
                    continue;
                mAllNeedDeleteABundleFullUris.Add(assetBunleInfor.Value.mAssetAbsFullUri); //需要删除
            }

            yield return AsyncManager.WaitFor_Null;
            DeleteAllInvalidAssetBundleAsset(mAllNeedDeleteABundleFullUris);

#if UNITY_EDITOR
            ShowAllNeedUpdateAssetBundleByType(mAllNeedUpdateAssetBundleAssetInfor);
#endif
        }

        /// <summary>/// 开始下载AssetBundle 资源/// </summary>
        private IEnumerator BeginDownloadAllAssetBundle()
        {
            if (mAllNeedUpdateAssetBundleAssetInfor == null || mAllNeedUpdateAssetBundleAssetInfor.Count == 0)
                yield break;

            mCurDownloadCount = 1;
            mAllDownloadFailAssetBundleInfors.Clear();
            BeginDownloadAssetBundle(mAllNeedUpdateAssetBundleAssetInfor); //开始下载

            while (mCurDownloadCount <= S_MaxDownloadTimes)
            {
                while (mAllNeedUpdateAssetBundleAssetInfor.Count != 0)
                    yield return AsyncManager.WaitFor_Null; //等待完成所有的下载

                if (mAllDownloadFailAssetBundleInfors.Count == 0)
                    yield break; //全部下载成功

                foreach (var reDownLoadTask in mAllDownloadFailAssetBundleInfors)
                    mAllNeedUpdateAssetBundleAssetInfor.Add(reDownLoadTask.Key, reDownLoadTask.Value);

                yield return AsyncManager.WaitFor_Null;

                mAllDownloadFailAssetBundleInfors.Clear();
                mCurDownloadCount++;
                BeginDownloadAssetBundle(mAllNeedUpdateAssetBundleAssetInfor); // 重新开始下载
            } //最多尝试 S_MaxDownloadTimes
        }

        #region 其他实现

        /// <summary>/// 删除无效的AssetBundle 资源/// </summary>
        private void DeleteAllInvalidAssetBundleAsset(HashSet<string> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
            {
                Debug.LogEditorInfor("没有需要删除的本地AssetBundle");
                return;
            }

            foreach (var assetBundlePath in dataSources)
            {
                IOUtility.DeleteFile(ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundlePath));
#if UNITY_EDITOR
                Debug.LogEditorInfor("删除了无效的AssetBundle资源  " + assetBundlePath);
#endif
            }

            Debug.Log("DeleteAllInvalidAssetBundleAsset Complete");
        }
#if UNITY_EDITOR

        /// <summary>/// 根据类型分类需要更新的AssetBundle 资源/// </summary>
        private void ShowAllNeedUpdateAssetBundleByType(Dictionary<string, UpdateAssetBundleInfor> dataSources)
        {
            var dataList = dataSources.Values.ToList();
            string content = SerializeManager.SerializeObject(dataList);
            string filePath = Application.dataPath.CombinePathEx(ConstDefine.S_EditorName).CombinePathEx("totalNeedUpdateAssetBundle.txt");
            IOUtility.CreateOrSetFileContent(filePath, content);
            Debug.LogEditorInfor($"ShowAllNeedUpdateAssetBundleByType 成功，保存在目录 {filePath}");
        }
#endif

        /// <summary>/// 根据获取的下载列表下载资源/// </summary>
        private void BeginDownloadAssetBundle(Dictionary<string, UpdateAssetBundleInfor> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
            {
                Debug.Log("所有的AssetBundle 资源都是最新的");
                return;
            }

            Debug.LogInfor($"BeginDownloadAssetBundle ,一共有{dataSources.Count}个需要需要更新或者下载");

            foreach (var assetBundleRecord in dataSources)
            {
                string updateAssetBundleUrl = $"{S_AssetBundleCDNTopUrl}/{assetBundleRecord.Key}";
#if UNITY_EDITOR
                Debug.LogEditorInfor($"开始下载AssetBundle  url={updateAssetBundleUrl}");
#endif

                DownloadManager.S_Instance.GetByteDataFromUrl(updateAssetBundleUrl, TaskPriorityEnum.Immediately, OnDownloadAssetBundleCallback);
            }
        }

        /// <summary>/// 下载一个 AssetBundle 资源回调/// </summary>
        private void OnDownloadAssetBundleCallback(UnityWebRequest webRequest, bool isSuccess, string url)
        {
            string assetBundleName = url.Substring(S_AssetBundleCDNTopUrl.Length + 1);

            if (mAllNeedUpdateAssetBundleAssetInfor.TryGetValue(assetBundleName, out var bundleInfor))
            {
                if (isSuccess)
                {
#if UNITY_EDITOR
                    Debug.LogEditorInfor($"OnDownloadAssetBundleCallback Success  ---------------------->>> AssetBundleName= {assetBundleName}    \t  url={url}");
#endif
                    DownloadHandlerBuffer handle = webRequest.downloadHandler as DownloadHandlerBuffer;
                    SaveAssetBundleFromDownload(handle, assetBundleName);
                }
                else
                {
                    mAllDownloadFailAssetBundleInfors[assetBundleName] = bundleInfor; //记录下载失败的项
                    Debug.LogError($"OnDownloadAssetBundleCallback Fail,Error {webRequest.error}  \t url={webRequest.url}");
                }

                mAllNeedUpdateAssetBundleAssetInfor.Remove(assetBundleName);
            }
            else
            {
                Debug.LogError($"OnDownloadAssetBundleCallback Fail,没有记载的下载AssetBundle 记录 AssetBundleName= {assetBundleName}  \t url={url}");
            }
        }

        /// <summary>
        ///保存下载的AssetBundel 资源
        /// </summary>
        private void SaveAssetBundleFromDownload(DownloadHandlerBuffer handle, string assetBundleName)
        {
            if (handle == null)
                return;
                string fileSavePath = ConstDefine.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundleName);
            IOUtility.CreateOrSetFileContent(fileSavePath, handle.data, false);
            Debug.LogInfor($"保存下载的AssetBundle 资源{assetBundleName} ");
        }

        #endregion

        #endregion





    }
}