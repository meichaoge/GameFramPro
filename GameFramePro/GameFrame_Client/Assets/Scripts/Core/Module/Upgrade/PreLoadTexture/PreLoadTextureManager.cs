using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using GameFramePro.CacheEx;
using GameFramePro.NetWorkEx;
using UnityEngine.Networking;

namespace GameFramePro.Upgrade
{


    /// <summary>/// 用于在进入登录界面前提前加载CDN 上图片/// </summary>
    public class PreLoadTextureManager : Single<PreLoadTextureManager>, IUpgradeModule
    {
      
        #region 路径配置 

        private string mLocalStorePath = string.Empty;

        /// <summary>/// 活动图片资源本地存储的顶级目录 （也存储了本地的配置文件）/// </summary>
        public string LocalStorePath
        {
            get
            {
                if (string.IsNullOrEmpty(mLocalStorePath))
                    mLocalStorePath = $"{Application.persistentDataPath}/{ConstDefine.S_LocalStoreDirectoryName}/{ConstDefine.S_PreLoadTextureTopDirectoryName}/{AppUpgradeHelper.GetCurUpgradeLanguage()}/";
                return mLocalStorePath;
            }
        }

        private static string s_TextureCDNTopUrl = string.Empty;

        /// <summary>/// Texture CDN顶层目录url/// </summary>
        public static string S_TextureCDNTopUrl
        {
            get
            {
                if (string.IsNullOrEmpty(s_TextureCDNTopUrl))
                    s_TextureCDNTopUrl = ApplicationManager.S_TopCDNUrl.CombinePathEx(ConstDefine.S_PreLoadTextureTopDirectoryName);
                return s_TextureCDNTopUrl;
            }
        }

        private string mPreloadTextureServerUrl = null;

        /// <summary>/// 活动图片资源配置信息地址/// </summary>
        public string PreloadTextureServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(mPreloadTextureServerUrl))
                    mPreloadTextureServerUrl = $"{S_TextureCDNTopUrl}/{AppUpgradeHelper.GetCurUpgradeLanguage()}/";
                return mPreloadTextureServerUrl;
            }
        }

        #endregion


        #region 需要预加载的图片信息

        //**Server
        private PreloadImgConfigInfor ServerPreloadImgConfigInfor = null; //服务器下载的需要预加载的资源配置

        ////**Local key=图片相对路径 value=MD5
        private Dictionary<string, string> LocalPreloadImgAssetMD5Infor = new Dictionary<string, string>(); //本地资源的MD5信息
        private Dictionary<string, byte[]> localPreloadImgData = new Dictionary<string, byte[]>(); //本地资源的数据 在计算MD5时候一起保存的

        //**下载
        HashSet<string> AllNeedDownloadTextureAssetRecord = new HashSet<string>(); //需要下载或者更新的资源
        HashSet<string> AllDownloadTextureFailRecord = new HashSet<string>(); //需要下载或者更新 失败的资源

        /// <summary>/// 总共需要下载的资源总数/// </summary>
        public uint TotalNeedDownloadAssetCount { get; private set; }

        private uint mCurDownloadCount = 0; //当前下载次数
        private const uint S_MaxDownloadTimes = 3; //最大下载次数

        #endregion


        #region IUpgradeModule 接口实现

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

            var preloadTextureSuperCoroutine = new SuperCoroutine(PreloadTextureUpgradeProcess(true));
            yield return preloadTextureSuperCoroutine.WaitDone(true);
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
            Debug.LogInfor("所有的 预加载图片 资源更新已经完成!!");


            OnUpgradeSuccessEvent?.Invoke();
        }

        public IEnumerator OnReBeginUpgrade()
        {
            mUpgradeState = UpgradeStateUsage.Begin;
            OnReBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("  准备重新下载 ", 0.2f);

            AllNeedDownloadTextureAssetRecord.Clear();
            foreach (var redownloadAsset in AllDownloadTextureFailRecord)
                AllNeedDownloadTextureAssetRecord.Add(redownloadAsset);

            var preloadTextureSuperCoroutine = new SuperCoroutine(PreloadTextureUpgradeProcess(false));
            yield return preloadTextureSuperCoroutine.WaitDone(true);
        }

        #endregion


        #region 内部实现

        /// <summary>/// 预加载图片 更新逻辑 。isFirstUpgrade=true 则表示第一次更新下载/// </summary>
        private IEnumerator PreloadTextureUpgradeProcess(bool isFirstUpgrade)
        {
            OnUpgradeProcess("获取服务器上 预加载图片 配置信息  ", 0);

            SuperCoroutine preloadTextureSuperCoroutine = null; //获取本地AssetBundle 信息
            if (isFirstUpgrade)
                preloadTextureSuperCoroutine = new SuperCoroutine(LoadAllLocalPreloadImgAssetInfor()); //获取本地 预加载图片 的信息

            if (ServerPreloadImgConfigInfor == null)
            {
                var getServerConfigureSuperCoroutine = new SuperCoroutine(LoadServerPreloadImgConfigure());
                yield return getServerConfigureSuperCoroutine.WaitDone(); //下载服务器的配置
                OnUpgradeProcess("获取服务器上 预加载图片 配置信息完成 ", 0.1f);
            }

            if (preloadTextureSuperCoroutine != null)
            {
                yield return preloadTextureSuperCoroutine.WaitDone(true); //获取本地 预加载图片 的信息
                OnUpgradeProcess("本地 预加载图片 资源配置获取完成 ", 0.15f);
            }

            //为了确保重试时候不用重新获取本地资源信息 这里要先处理本地资源
            if (ServerPreloadImgConfigInfor == null)
            {
                OnUpgradeFail("预加载图片配置下载失败，无法更新下载预加载资源");
                yield break;
            } //服务器文件下载失败

            if (isFirstUpgrade)
            {
                OnUpgradeProcess(" 开始获取需要 预加载图片 资源 ", 0.2f);
                var getNeedUpdateTexturesSuperCoroutine = new SuperCoroutine(GetAllNeedUpdateTexturesConfigure());
                yield return getNeedUpdateTexturesSuperCoroutine.WaitDone(); //获取需要下载的资源列表
                OnUpgradeProcess(" 获取需要 预加载图片 资源 完成", 0.3f);
            } //第一次需要判断哪些资源需要下载


            TotalNeedDownloadAssetCount = (uint) AllNeedDownloadTextureAssetRecord.Count; //需要下载的资源总数
            if (TotalNeedDownloadAssetCount != 0)
            {
                mCurDownloadCount = 1;
                OnUpgradeProcess(" 开始更新的 预加载图片 资源 ", 0.35f);
                var downloadTextureSuperCoroutine = new SuperCoroutine(BeginDownloadAllPreloadTextures());
                yield return downloadTextureSuperCoroutine.WaitDone(); //开始下载资源
                OnUpgradeProcess(" 更新的 预加载图片 资源 完成 ", 0.9f);
            }
            else
                OnUpgradeProcess(" 本地的 预加载图片 资源 是最新的 ", 0.9f);

            //下载流程结束
            if (AllNeedDownloadTextureAssetRecord.Count == 0 && AllDownloadTextureFailRecord.Count == 0)
                OnUpgradeSuccess();
            else
                OnUpgradeFail("部分资源多次下载均失败了 无法继续下载. 请检查网络后重试"); //有任务没有下载完成
        }


        /// <summary>/// 获取服务器版本信息/// </summary>
        private IEnumerator LoadServerPreloadImgConfigure()
        {
            if (ServerPreloadImgConfigInfor != null)
            {
                Debug.LogInfor("预加载图片 服务器配置文件已经下载了.. 不需要重新下载");
                yield break;
            }

            mCurDownloadCount = 1;
            while (ServerPreloadImgConfigInfor == null && mCurDownloadCount <= S_MaxDownloadTimes)
            {
                var url = PreloadTextureServerUrl.CombinePathEx(ConstDefine.S_PreloadImgConfiFileName);
                var downloadTask = DownloadManager.GetByteDataFromUrl(url,0,0,  null, TaskPriorityEnum.Immediately);
                if (downloadTask == null)
                {
                    Debug.LogError($"获取服务器配置的下载任务创建失败 第{mCurDownloadCount} 次 url={PreloadTextureServerUrl}");
                    yield return AsyncManager.WaitFor_Null; //等待被执行下载任务
                }
                else
                {
                    while (downloadTask.TaskState == TaskStateEum.Initialed)
                        yield return AsyncManager.WaitFor_Null; //等待被执行下载任务

                    if (downloadTask.TaskSuperCoroutinenfor != null)
                    {
                        yield return downloadTask.TaskSuperCoroutinenfor.WaitDone(true);

                        var downLoadCallback = downloadTask.DownloadTaskCallbackData;

                        if (downLoadCallback == null || downLoadCallback.isNetworkError || downLoadCallback.isDone == false || downLoadCallback.isHttpError)
                            Debug.LogError($"OnCompleteGetServerAssetBundleConfig Fail Error  下载参数为null   {downLoadCallback.url}");
                        else
                        {
                            string content = (downLoadCallback.downloadHandler as DownloadHandlerBuffer).text;
                            if (string.IsNullOrEmpty(content))
                            {
                                Debug.LogError($"下载的预加载图片配置失败 url={PreloadTextureServerUrl}");
                                content = SerializeManager.SerializeObject(new PreloadImgConfigInfor());
                                Debug.LogError("测试时 使用默认的字符串避免报错");
                            }

                            ServerPreloadImgConfigInfor = SerializeManager.DeserializeObject<PreloadImgConfigInfor>(content);
                            if (ServerPreloadImgConfigInfor != null)
                                yield break;
                        }
                    }
                }

                yield return AsyncManager.WaitFor_Null; //等待重试
                ++mCurDownloadCount;
            }
        }


        /// <summary>/// 加载并获取本地资源的MD5信息/// </summary>
        private IEnumerator LoadAllLocalPreloadImgAssetInfor()
        {
            LocalPreloadImgAssetMD5Infor.Clear();
            if (Directory.Exists(LocalStorePath) == false)
            {
                Directory.CreateDirectory(LocalStorePath); //方便后面的下载
                yield break;
            }

            string[] allFiles = Directory.GetFiles(LocalStorePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                string extensionName = Path.GetExtension(file);
                if (extensionName != ".png" && extensionName == ".jpg")
                    continue;

                string imgRelativetPath = file.Substring(LocalStorePath.Length);
                imgRelativetPath = imgRelativetPath.Replace("\\", @"\");
                var md5Code = MD5Helper.GetFileMD5OutData(file, out var fileData); //图片数据
                LocalPreloadImgAssetMD5Infor.Add(imgRelativetPath, md5Code);

#if UNITY_EDITOR

                Debug.LogEditorInfor($"--------------------->> Local Textures: {imgRelativetPath} : md5Code={md5Code} ");
#endif

                //****保存本地的图片数据 避免多次加载
                //      string fileRelativePath = imgAssetPath.Substring(LocalStorePath.Length); 
                if (localPreloadImgData.TryGetValue(imgRelativetPath, out var localData))
                {
                    localData = fileData;
                    Debug.LogError($"存在重复的资源 {imgRelativetPath} ");
                    continue;
                }

                localPreloadImgData.Add(imgRelativetPath, fileData);
            }
        }


        /// <summary>/// 获取所有需要更新的 预加载图片 信息/// </summary>
        private IEnumerator GetAllNeedUpdateTexturesConfigure()
        {
            if (ServerPreloadImgConfigInfor == null) yield break;


            //对比服务器配置 获取哪些资源需要更新或者新增
            foreach (var textureInfor in ServerPreloadImgConfigInfor.AllPreloadImgConfig.Values)
            {
                if (LocalPreloadImgAssetMD5Infor.TryGetValue(textureInfor.mTextureRelativePath, out var textureMd5))
                {
                    if (textureMd5.Equals(textureInfor.mAssetMD5, StringComparison.Ordinal))
                     //   if (textureMd5 != textureInfor.mAssetMD5)
                            AllNeedDownloadTextureAssetRecord.Add(textureInfor.mTextureRelativePath); //需要更新
                    continue;
                }
#if UNITY_EDITOR
                Debug.Log($"新增的图片{textureInfor.mTextureRelativePath} 配置");
#endif
                AllNeedDownloadTextureAssetRecord.Add(textureInfor.mTextureRelativePath); //需要更新
            }

            yield return AsyncManager.WaitFor_Null;

            HashSet<string> allNeedDeleteTexturesNameInfor = new HashSet<string>();
            //对比获取哪些资源需要删除
            foreach (var textureInfor in LocalPreloadImgAssetMD5Infor.Keys)
            {
                if (ServerPreloadImgConfigInfor.AllPreloadImgConfig.ContainsKey(textureInfor))
                    continue;
                allNeedDeleteTexturesNameInfor.Add(textureInfor); //需要删除
            }

            yield return AsyncManager.WaitFor_Null;
            DeleteAllInvalidPreloadTextureAsset(allNeedDeleteTexturesNameInfor);


#if UNITY_EDITOR
            ShowAllNeedUpdatePreloadTeturesByType(AllNeedDownloadTextureAssetRecord);
#endif
        }

        /// <summary>/// 开始下载 预加载图片 资源/// </summary>
        private IEnumerator BeginDownloadAllPreloadTextures()
        {
            if (AllNeedDownloadTextureAssetRecord == null || AllNeedDownloadTextureAssetRecord.Count == 0)
                yield break;

            AllDownloadTextureFailRecord.Clear();
            BeginDownloadTextures(AllNeedDownloadTextureAssetRecord); //开始下载

            while (mCurDownloadCount <= S_MaxDownloadTimes)
            {
                while (AllNeedDownloadTextureAssetRecord.Count != 0)
                    yield return AsyncManager.WaitFor_Null; //等待完成所有的下载

                if (AllDownloadTextureFailRecord.Count == 0)
                    yield break; //全部下载成功

                foreach (var reDownLoadTask in AllDownloadTextureFailRecord)
                    AllNeedDownloadTextureAssetRecord.Add(reDownLoadTask);

                yield return AsyncManager.WaitFor_Null;

                AllDownloadTextureFailRecord.Clear();
                mCurDownloadCount++;
                BeginDownloadTextures(AllNeedDownloadTextureAssetRecord); // 重新开始下载
            } //最多尝试 S_MaxDownloadTimes
        }

        #endregion

        #region 其他实现

        /// <summary>/// 根据获取的下载列表下载资源/// </summary>
        private void BeginDownloadTextures(HashSet<string> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
            {
                Debug.Log("所有的 预加载图片 资源都是最新的");
                return;
            }

            Debug.LogInfor($"BeginDownloadPreloadTextures ,一共有{dataSources.Count}个需要需要更新或者下载");

            foreach (var textureInfor in dataSources)
            {
                string updatePreloadTextureUrl = $"{PreloadTextureServerUrl}{textureInfor}";

#if UNITY_EDITOR
                Debug.LogEditorInfor($"开始下载 预加载图片  url={updatePreloadTextureUrl}");
#endif

                DownloadManager.GetTextureDataFromUrl(updatePreloadTextureUrl,  OnDownloadPreloadTextureCallback,TaskPriorityEnum.Immediately);
            }
        }

        /// <summary>/// 下载一个 预加载图片 资源回调/// </summary>
        private void OnDownloadPreloadTextureCallback(UnityWebRequest webRequest, bool isSuccess, string url)
        {
            string textureRelativePath = url.Substring(PreloadTextureServerUrl.Length);

            if (AllNeedDownloadTextureAssetRecord.Contains(textureRelativePath))
            {
                if (isSuccess)
                {
#if UNITY_EDITOR
                    Debug.LogEditorInfor($"OnDownloadPreloadTextureCallback Success  ---------------------->>> 图片相对路径= {textureRelativePath}    \t  url={url}");
#endif
                    DownloadHandlerTexture handle = webRequest.downloadHandler as DownloadHandlerTexture;
                    string savePath = $"{LocalStorePath}{textureRelativePath}";

                    IOUtility.CreateOrSetFileContent(savePath, handle.data, false);
                }
                else
                {
                    AllDownloadTextureFailRecord.Add(textureRelativePath); //记录下载失败的项
                    Debug.LogError($"OnDownloadAssetBundleCallback Fail,Error {webRequest.error}  \t url={webRequest.url}");
                }

                AllNeedDownloadTextureAssetRecord.Remove(textureRelativePath);
            }
            else
            {
                Debug.LogError($"OnDownloadPreloadTextureCallback Fail,没有记载的下载 预加载图片 记录  {textureRelativePath}  \t url={url}");
            }
        }


        /// <summary>/// 删除无效的 预加载图片 资源/// </summary>
        private void DeleteAllInvalidPreloadTextureAsset(HashSet<string> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
            {
                Debug.LogEditorInfor("没有需要删除的本地AssetBundle");
                return;
            }

            foreach (var textureInfor in dataSources)
            {
                IOUtility.DeleteFile(LocalStorePath.CombinePathEx(textureInfor));
#if UNITY_EDITOR
                Debug.LogEditorInfor("删除了无效的 预加载图片 资源  " + textureInfor);
#endif
            }

            Debug.Log("DeleteAllInvalidPreloadTextureAsset Complete");
        }

#if UNITY_EDITOR

        /// <summary>/// 根据类型分类需要更新的 预加载图片 资源/// </summary>
        private void ShowAllNeedUpdatePreloadTeturesByType(HashSet<string> dataSources)
        {
            string content = SerializeManager.SerializeObject(dataSources);
            string filePath = Application.dataPath.CombinePathEx(ConstDefine.S_EditorName).CombinePathEx("totalNeedUpdatePreloadTexture.txt");
            IOUtility.CreateOrSetFileContent(filePath, content);
            Debug.LogEditorInfor($"ShowAllNeedUpdatePreloadTeturesByType 成功，保存在目录 {filePath}");
        }
#endif

        #endregion
    }
}