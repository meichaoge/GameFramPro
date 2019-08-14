using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramePro.NetWorkEx;
using UnityEngine.Networking;
using System.Threading;
using GameFramePro.ResourcesEx;


namespace GameFramePro.Upgrade
{
    /// <summary>/// 负责AssetBundle 资源的更新逻辑/// </summary>
    public class AssetBundleUpgradeManager : Single<AssetBundleUpgradeManager>, IUpgradeModule
    {
        #region 需要更新的AssetBundle 信息

        //key =AssetBundleName
        private Dictionary<string, UpdateAssetBundleInfor> mAllNeedUpdateAssetBundleAssetInfor = new Dictionary<string, UpdateAssetBundleInfor>(); //所有需要更新的资源
        private Dictionary<string, UpdateAssetBundleInfor> mAllDownloadFailAssetBundleInfors = new Dictionary<string, UpdateAssetBundleInfor>(); //所有更新时候失败的资源

        //key=assetBundle 资源的路径 value=每个AssetBundle资源信息
        private Dictionary<string, AssetBundleInfor> mAllLocalAssetBundleAssetFileInfor = new Dictionary<string, AssetBundleInfor>();

        /// <summary>/// 服务器上最新的AssetBundle 配置/// </summary>
        private AssetBundleAssetTotalInfor mServerBundleAssetConfigInfor = null; //从服务器获取的 AssetBundle 配置

        /// <summary>/// 总共需要下载的资源总数/// </summary>
        public int TotalNeedDownloadAssetCount { get; private set; }

        private const int S_MaxDownloadTimes = 3; //最大下载次数

        #endregion


        #region IUpgradeModule 接口

        public event OnBeginUpgradeDelegate OnBeginUpgradeEvent;
        public event OnUpgradeProcessDelegate OnUpgradeProcessEvent;
        public event OnUpgradeFailDelegate OnUpgradeFailEvent;
        public event OnUpgradeSuccessDelegate OnUpgradeSuccessEvent;
        public event OnReBeginUpgradeDelegate OnReBeginUpgradeEvent;

        public float CurProcess { get; private set; } = 0f; //当前的下载进度

        public IEnumerator OnBeginUpgrade()
        {
            OnBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("开始检测 本地AssetBundle 资源状态 ", 0);
            var getServerAssetBundleConfig = new SuperCoroutine(GetServerAssetBundleContainAssetConfig());
            getServerAssetBundleConfig.StartCoroutine(); //下载服务器的配置


            var localAssetBundleCoroutineEx = new SuperCoroutine(GetAllLocalAssetBundleInformation());
            yield return localAssetBundleCoroutineEx.WaitDone(true); //获取本地AssetBundle 的信息
            OnUpgradeProcess("本地AssetBundle 资源状态获取完成 ", 0.1f);

            yield return getServerAssetBundleConfig.WaitDone(); //等待完成获取服务器配置


            OnUpgradeProcess(" 开始获取需要更新的 AssetBundle 资源 ", 0.2f);
            var getNeedUpdateAssetBundleCoroutineEx = new SuperCoroutine(GetAllNeedUpdateAssetBundleConfig());
            yield return getNeedUpdateAssetBundleCoroutineEx.WaitDone(); //获取需要下载的资源列表
            OnUpgradeProcess(" 获取需要更新的 AssetBundle 资源 完成", 0.3f);


            TotalNeedDownloadAssetCount = mAllNeedUpdateAssetBundleAssetInfor.Count; //需要下载的资源总数
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
                OnUpgradeFail(); //有任务没有下载完成
        }

        public void OnUpgradeProcess(string message, float process)
        {
            CurProcess = process;

#if UNITY_EDITOR
            Debug.LogEditorInfor($"OnUpgradeProcess  Now={DateTime.Now.ToString()}  {message}");
#endif
            OnUpgradeProcessEvent?.Invoke(message, process);
        }

        public void OnUpgradeFail()
        {
            Debug.LogError("有部分资源多次下载均失败了 无法继续下载！！！ 请检查网络后重试");
            OnUpgradeFailEvent?.Invoke("有部分资源多次下载均失败了 无法继续下载！！！ 请检查网络后重试");
        }

        public void OnUpgradeSuccess()
        {
            Debug.LogInfor("所有的AssetBundle 资源更新已经完成!!");

            AssetBundleManager.S_Instance.SaveAssetBundleTotalConfigInfor(mServerBundleAssetConfigInfor); //保存得到的配置

            OnUpgradeSuccessEvent?.Invoke();
        }

        public IEnumerator OnReBeginUpgrade()
        {
            OnReBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("  准备重新下载 ", 0.2f);

            mAllNeedUpdateAssetBundleAssetInfor.Clear();
            foreach (var reDownloadFailAssetBundleInfor in mAllDownloadFailAssetBundleInfors)
                mAllNeedUpdateAssetBundleAssetInfor.Add(reDownloadFailAssetBundleInfor.Key, reDownloadFailAssetBundleInfor.Value);


            OnUpgradeProcess("  重新下载失败的 AssetBundle 资源 ", 0.35f);
            var downloadAssetBundleCoroutineEx = new SuperCoroutine(BeginDownloadAllAssetBundle());
            yield return downloadAssetBundleCoroutineEx.WaitDone(); //开始下载资源
            OnUpgradeProcess(" 重新下载失败的 AssetBundle 资源 完成 ", 0.9f);

            //下载流程结束
            if (mAllDownloadFailAssetBundleInfors.Count == 0)
                OnUpgradeSuccess();
            else
                OnUpgradeFail(); //有任务没有下载完成
        }

        #endregion


        #region AssetBundle 更新细节实现

        #region 获取本地AssetBundle 资源以及对于的配置表                                               

        private float mAllLocalAssetBundleLoadProcess = 0f; //获取进度

        /// <summary>/// 首先需要判断本地文件有没有被修改过(或者被删除了部分资源)，如果有改动则需要读取每个文件的MD5,否则只需要根据本地配置文件修改即可/// </summary>
        private IEnumerator GetAllLocalAssetBundleInformation()
        {
            bool isLocalAssetBundleExit = false;

            // 检测本地的AssetBundle 资源是否存在

            isLocalAssetBundleExit = System.IO.Directory.Exists(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath);
            string[] allAssetBundleFiles = null;
            if (isLocalAssetBundleExit)
            {
                allAssetBundleFiles = System.IO.Directory.GetFiles(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath, "*.*", System.IO.SearchOption.AllDirectories);
                if (allAssetBundleFiles.Length == 0)
                    isLocalAssetBundleExit = false;
            }

            bool isCompleteGetLocalFileInfor = false; //标示是否完成了所有的本地资源信息的录入

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
//                        assetBundleInfor.mBundleMD5Code = MD5Helper.GetFileMD5(filePath, ref assetBundleInfor.mBundleSize);
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
                for (int dex = 0; dex < allAssetBundleFiles.Length; dex++)
                {
                    var filePath = allAssetBundleFiles[dex];
                    AssetBundleInfor assetBundleInfor = new AssetBundleInfor();
                    assetBundleInfor.mBundleName = filePath.Substring(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath.Length + 1);
                    assetBundleInfor.mBundleMD5Code = MD5Helper.GetFileMD5(filePath, ref assetBundleInfor.mBundleSize);
                    string assetBundleNameStr = assetBundleInfor.mBundleName.GetPathStringEx(); //去掉路径分隔符
                    mAllLocalAssetBundleAssetFileInfor[assetBundleNameStr] = assetBundleInfor; //记录本地AssetBundle 资源信息
                    mAllLocalAssetBundleLoadProcess = dex * 1f / allAssetBundleFiles.Length; //进度
                }

                yield return AsyncManager.JumpToUnity; //回到主线程
                isCompleteGetLocalFileInfor = true;
            }
            else
            {
                Debug.LogInfor("BeginUpdateAssetBundle 本地没有资源 下载所有的资源");
                mAllLocalAssetBundleLoadProcess = 1f;
                isCompleteGetLocalFileInfor = true;
            }
        }

        /// <summary>/// 获取服务器的AssetBundle 配置/// </summary>
        private IEnumerator GetServerAssetBundleContainAssetConfig()
        {
            string assetBundleConfigFileUrl = AppUrlManager.S_AssetBundleCDNTopUrl.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
            UnityWebRequestDownloadTask downloadTask = DownloadManager.S_Instance.GetByteDataFromUrl(assetBundleConfigFileUrl, TaskPriorityEnum.Immediately, null);
            if (downloadTask == null)
            {
                Debug.LogError("获取服务器配置的下载任务创建失败");
                yield break;
            }

            while (downloadTask.TaskState == TaskStateEum.Initialed)
                yield return AsyncManager.WaitFor_Null;


            if (downloadTask.TaskSuperCoroutinenfor != null)
            {
                yield return downloadTask.TaskSuperCoroutinenfor.WaitDone(true);

                if (downloadTask == null || downloadTask.DownloadTaskCallbackData == null || downloadTask.DownloadTaskCallbackData.isNetworkError || downloadTask.DownloadTaskCallbackData.isDone == false)
                    Debug.LogError("OnCompleteGetServerAssetBundleConfig Fail Error  下载参数为null");
                else
                    mServerBundleAssetConfigInfor = SerilazeManager.DeserializeObject<AssetBundleAssetTotalInfor>((downloadTask.DownloadTaskCallbackData.downloadHandler as DownloadHandlerBuffer).text);
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
                var assetBundleInfor = assetBunleInfor.Value;

                if (mAllLocalAssetBundleAssetFileInfor.TryGetValue(assetBunleInfor.Key, out var localAssetBundleConfig))
                {
                    if (localAssetBundleConfig.mBundleMD5Code != assetBundleInfor.mBundleMD5Code)
                        mAllNeedUpdateAssetBundleAssetInfor.Add(assetBundleInfor.mBundleName, new UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum.UpdateAsset, assetBundleInfor)); //需要更新
                    continue;
                }

                mAllNeedUpdateAssetBundleAssetInfor.Add(assetBunleInfor.Value.mBundleName, new UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum.UpdateAsset, assetBundleInfor)); //需要更新
            }

            yield return AsyncManager.WaitFor_Null;

            HashSet<string> allNeedDeleteAssetBundleNameInfor = new HashSet<string>();
            //对比获取哪些资源需要删除
            foreach (var assetBunleInfor in mAllLocalAssetBundleAssetFileInfor)
            {
                if (mServerBundleAssetConfigInfor.mTotalAssetBundleInfor.ContainsKey(assetBunleInfor.Key))
                    continue;
                allNeedDeleteAssetBundleNameInfor.Add(assetBunleInfor.Value.mBundleName); //需要删除
            }

            yield return AsyncManager.WaitFor_Null;
            DeleteAllInvalidAssetBundleAsset(allNeedDeleteAssetBundleNameInfor);


#if UNITY_EDITOR
            ShowAllNeedUpdateAssetBundleByType(mAllNeedUpdateAssetBundleAssetInfor);
#endif
        }

        /// <summary>/// 开始下载AssetBundle 资源/// </summary>
        private IEnumerator BeginDownloadAllAssetBundle()
        {
            if (mAllNeedUpdateAssetBundleAssetInfor == null || mAllNeedUpdateAssetBundleAssetInfor.Count == 0)
                yield break;

            int curDownloadTime = 1;
            mAllDownloadFailAssetBundleInfors.Clear();
            BeginDownloadAssetBundle(mAllNeedUpdateAssetBundleAssetInfor); //开始下载

            while (true)
            {
                if (curDownloadTime > S_MaxDownloadTimes)
                    yield break; //尝试多次仍然失败

                while (mAllNeedUpdateAssetBundleAssetInfor.Count != 0)
                    yield return AsyncManager.WaitFor_Null; //等待完成所有的下载

                if (mAllDownloadFailAssetBundleInfors.Count == 0)
                    yield break; //全部下载成功

                foreach (var reDownLoadTask in mAllDownloadFailAssetBundleInfors)
                    mAllNeedUpdateAssetBundleAssetInfor.Add(reDownLoadTask.Key, reDownLoadTask.Value);

                mAllDownloadFailAssetBundleInfors.Clear();
                curDownloadTime++;
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
                IOUtility.DeleteFile(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath.CombinePathEx(assetBundlePath));
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
            var dataList = dataSources.GroupBy((assetBundleItem) => assetBundleItem.Value.mAssetBundleAssetUpdateTagEnum).ToList();
            string content = SerilazeManager.SerializeObject(dataList);
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

            foreach (var assetBundleRecord in dataSources.Values)
            {
                if (assetBundleRecord.mAssetBundleAssetUpdateTagEnum == AssetBundleAssetUpdateTagEnum.RemoveLocalAsset)
                {
                    Debug.LogError("BeginDownloadAssetBundle Error " + assetBundleRecord.mNeedUpdateAssetName);
                    continue;
                }

                string updateAssetBundleUrl = $"{AppUrlManager.S_AssetBundleCDNTopUrl}/{assetBundleRecord.mNeedUpdateAssetName}";

#if UNITY_EDITOR
                Debug.LogEditorInfor($"开始下载AssetBundle  url={updateAssetBundleUrl}");
#endif

                DownloadManager.S_Instance.GetByteDataFromUrl(updateAssetBundleUrl, TaskPriorityEnum.Immediately, OnDownloadAssetBundleCallback);
            }
        }

        /// <summary>/// 下载一个 AssetBundle 资源回调/// </summary>
        private void OnDownloadAssetBundleCallback(UnityWebRequest webRequest, bool isSuccess, string url)
        {
            string assetBundleName = url.Substring(AppUrlManager.S_AssetBundleCDNTopUrl.Length + 1);

            if (mAllNeedUpdateAssetBundleAssetInfor.TryGetValue(assetBundleName, out var bundleInfor))
            {
                if (isSuccess)
                {
#if UNITY_EDITOR
                    Debug.LogEditorInfor($"OnDownloadAssetBundleCallback Success  ---------------------->>> AssetBundleName= {assetBundleName}    \t  url={url}");
#endif
                    DownloadHandlerBuffer handle = webRequest.downloadHandler as DownloadHandlerBuffer;
                    AssetBundleManager.S_Instance.SaveAssetBundleFromDownload(handle, assetBundleName);
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

        #endregion

        #endregion

        #endregion
    }
}
