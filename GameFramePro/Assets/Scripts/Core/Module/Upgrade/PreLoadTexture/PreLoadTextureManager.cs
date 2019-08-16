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

        /// <summary>/// CDN需要预加载的所有资源路径顶层文件夹名/// </summary>
        private const string s_PreloadImgAssetServerTopDirectory = "PreloadTextures";

        /// <summary>/// 本地和服务器的资源配置名称 (Unity 客户端使用相同的名称)/// </summary>
        public const string s_PreloadImgConfiFileName = "PreloadImageAssetConfig.json";


        private string mLocalStorePath = string.Empty;

        /// <summary>/// 活动图片资源本地存储的顶级目录 （也存储了本地的配置文件）/// </summary>
        public string LocalStorePath
        {
            get
            {
                if (string.IsNullOrEmpty(mLocalStorePath))
                    mLocalStorePath = $"{AppTextureCacheManager.S_TextureCacheTopDirectory}{s_PreloadImgAssetServerTopDirectory}/";
                return mLocalStorePath;
            }
        }


        private string mPreloadImgConfigServerUrl = null;

        /// <summary>/// 活动图片资源配置信息地址/// </summary>
        public string PreloadImgConfigServerUrl
        {
            get
            {
                if (string.IsNullOrEmpty(mPreloadImgConfigServerUrl))
                    mPreloadImgConfigServerUrl = $"{AppUrlManager.S_TextureCDNTopUrl}/{s_PreloadImgAssetServerTopDirectory}/";

                return mPreloadImgConfigServerUrl;
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
        public int TotalNeedDownloadAssetCount { get; private set; }

        private const int S_MaxDownloadTimes = 3; //最大下载次数


        //**对外接口
        private Dictionary<string, Sprite> mAllPreloadImgSprites = new Dictionary<string, Sprite>(); //byte[] 转化成了sprite

        #endregion


        #region IUpgradeModule 接口实现

        public event OnBeginUpgradeDelegate OnBeginUpgradeEvent;
        public event OnUpgradeProcessDelegate OnUpgradeProcessEvent;
        public event OnUpgradeFailDelegate OnUpgradeFailEvent;
        public event OnUpgradeSuccessDelegate OnUpgradeSuccessEvent;
        public event OnReBeginUpgradeDelegate OnReBeginUpgradeEvent;
        public float CurProcess { get; private set; } = 0f; //当前的下载进度

        public IEnumerator OnBeginUpgrade()
        {
            OnBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("获取服务器上 预加载图片 配置信息  ", 0);
            var getServerConfigureSuperCoroutine = new SuperCoroutine(LoadServerPreloadImgConfigure());
            getServerConfigureSuperCoroutine.StartCoroutine(); //下载服务器的配置


            var localPreLoadTextureSuperCoroutine = new SuperCoroutine(LoadAllLocalPreloadImgAssetInfor());
            yield return localPreLoadTextureSuperCoroutine.WaitDone(true); //获取本地 预加载图片 的信息
            OnUpgradeProcess("本地 预加载图片 资源状态获取完成 ", 0.1f);

            yield return getServerConfigureSuperCoroutine.WaitDone(); //等待完成获取服务器配置


            OnUpgradeProcess(" 开始获取需要 预加载图片 资源 ", 0.2f);
            var getNeedUpdateTexturesSuperCoroutine = new SuperCoroutine(GetAllNeedUpdateTexturesConfigure());
            yield return getNeedUpdateTexturesSuperCoroutine.WaitDone(); //获取需要下载的资源列表
            OnUpgradeProcess(" 获取需要 预加载图片 资源 完成", 0.3f);


            TotalNeedDownloadAssetCount = AllNeedDownloadTextureAssetRecord.Count; //需要下载的资源总数
            if (TotalNeedDownloadAssetCount != 0)
            {
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
            Debug.LogInfor("所有的 预加载图片 资源更新已经完成!!");


            OnUpgradeSuccessEvent?.Invoke();
        }

        public IEnumerator OnReBeginUpgrade()
        {
            OnReBeginUpgradeEvent?.Invoke();

            OnUpgradeProcess("  准备重新下载 ", 0.2f);

            AllNeedDownloadTextureAssetRecord.Clear();
            foreach (var redownloadAsset in AllDownloadTextureFailRecord)
                AllNeedDownloadTextureAssetRecord.Add(redownloadAsset);


            OnUpgradeProcess("  重新下载失败的 预加载图片 资源 ", 0.35f);
            var reDownloadTextureSuperCoroutine = new SuperCoroutine(BeginDownloadAllPreloadTextures());
            yield return reDownloadTextureSuperCoroutine.WaitDone(); //开始下载资源
            OnUpgradeProcess(" 重新下载失败的 预加载图片 资源 完成 ", 0.9f);

            //下载流程结束
            if (AllDownloadTextureFailRecord.Count == 0)
                OnUpgradeSuccess();
            else
                OnUpgradeFail(); //有任务没有下载完成
        }

        #endregion


        #region 内部实现

        /// <summary>/// 获取服务器版本信息/// </summary>
        public IEnumerator LoadServerPreloadImgConfigure()
        {
            if (ServerPreloadImgConfigInfor != null)
            {
                Debug.LogInfor("预加载图片 服务器配置文件已经下载了.. 不需要重新下载");
                yield break;
            }

            var downloadTask = DownloadManager.S_Instance.GetByteDataFromUrl(PreloadImgConfigServerUrl, TaskPriorityEnum.Immediately, null);
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
                    ServerPreloadImgConfigInfor = SerilazeManager.DeserializeObject<PreloadImgConfigInfor>((downloadTask.DownloadTaskCallbackData.downloadHandler as DownloadHandlerBuffer).text);
            }
        }


        /// <summary>/// 加载并获取本地资源的MD5信息/// </summary>
        private IEnumerator LoadAllLocalPreloadImgAssetInfor()
        {
            LocalPreloadImgAssetMD5Infor.Clear();
            if (System.IO.Directory.Exists(LocalStorePath) == false)
            {
                System.IO.Directory.CreateDirectory(LocalStorePath); //方便后面的下载
                yield break;
            }

            string[] allFiles = System.IO.Directory.GetFiles(LocalStorePath, "*.*", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                string extensionName = System.IO.Path.GetExtension(file);
                if (extensionName == ".manifest" || extensionName == ".json")
                    continue;

                string imgAssetPath = file.Substring(LocalStorePath.Length); // file.Substring(file.IndexOf(LocalStorePath) + LocalStorePath.Length);
                imgAssetPath = IOUtility.GetPathWithOutExtension(imgAssetPath); // System.IO.Path.GetFileNameWithoutExtension(file);
                imgAssetPath = imgAssetPath.Replace("\\", "/");
#if UNITY_EDITOR
                Debug.LogEditorInfor($"---------------------Load Local Textures {imgAssetPath} :");
#endif
                string md5Code = string.Empty;
                byte[] imgData = MD5Helper.GetFileMD5(file, out md5Code); //图片数据
                LocalPreloadImgAssetMD5Infor.Add(imgAssetPath, md5Code);

                //****保存本地的图片数据 避免多次加载
                string fileName = System.IO.Path.GetFileNameWithoutExtension(imgAssetPath);
                if (localPreloadImgData.ContainsKey(fileName))
                {
                    Debug.LogError("存在重复的资源 " + fileName);
                    continue;
                }

                localPreloadImgData.Add(fileName, imgData);
            }

            yield break;
        }


        /// <summary>/// 获取所有需要更新的 预加载图片 信息/// </summary>
        private IEnumerator GetAllNeedUpdateTexturesConfigure()
        {
            if (ServerPreloadImgConfigInfor == null) yield break;

            //Key =AssetBundle Name, value{=true 标示需要更新 =fasle 标示需要删除资源}

            //对比服务器配置 获取哪些资源需要更新或者新增
            foreach (var textureInfor in ServerPreloadImgConfigInfor.AllPreloadImgConfig.Values)
            {
                if (LocalPreloadImgAssetMD5Infor.TryGetValue(textureInfor.mTextureRelativePath, out var textureMd5))
                {
                    if (textureMd5 != textureInfor.mAssetMD5)
                        AllNeedDownloadTextureAssetRecord.Add(textureInfor.mTextureRelativePath); //需要更新
                    continue;
                }

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

            int curDownloadTime = 1;
            AllDownloadTextureFailRecord.Clear();
            BeginDownloadTextures(AllNeedDownloadTextureAssetRecord); //开始下载

            while (true)
            {
                if (curDownloadTime > S_MaxDownloadTimes)
                    yield break; //尝试多次仍然失败

                while (AllNeedDownloadTextureAssetRecord.Count != 0)
                    yield return AsyncManager.WaitFor_Null; //等待完成所有的下载

                if (AllDownloadTextureFailRecord.Count == 0)
                    yield break; //全部下载成功

                foreach (var reDownLoadTask in AllDownloadTextureFailRecord)
                    AllNeedDownloadTextureAssetRecord.Add(reDownLoadTask);

                AllDownloadTextureFailRecord.Clear();
                curDownloadTime++;
                BeginDownloadTextures(AllNeedDownloadTextureAssetRecord); // 重新开始下载
            } //最多尝试 S_MaxDownloadTimes
        }


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
                string updatePreloadTextureUrl = $"{PreloadImgConfigServerUrl}{textureInfor}";

#if UNITY_EDITOR
                Debug.LogEditorInfor($"开始下载 预加载图片  url={updatePreloadTextureUrl}");
#endif

                DownloadManager.S_Instance.GetByteDataFromUrl(updatePreloadTextureUrl, TaskPriorityEnum.Immediately, OnDownloadPreloadTextureCallback);
            }
        }

        /// <summary>/// 下载一个 预加载图片 资源回调/// </summary>
        private void OnDownloadPreloadTextureCallback(UnityWebRequest webRequest, bool isSuccess, string url)
        {
            string textureRelativePath = url.Substring(LocalStorePath.Length + 1);

            if (AllNeedDownloadTextureAssetRecord.Contains(textureRelativePath))
            {
                if (isSuccess)
                {
#if UNITY_EDITOR
                    Debug.LogEditorInfor($"OnDownloadPreloadTextureCallback Success  ---------------------->>> 图片相对路径= {textureRelativePath}    \t  url={url}");
#endif
                    DownloadHandlerBuffer handle = webRequest.downloadHandler as DownloadHandlerBuffer;
                    string savePath = $"{LocalStorePath}{textureRelativePath}";

                    IOUtility.CreateOrSetFileContent(savePath, handle.data, false);
                }
                else
                {
                    AllDownloadTextureFailRecord.Add(textureRelativePath); //记录下载失败的项
                    Debug.LogError($"OnDownloadAssetBundleCallback Fail,Error {webRequest.error}  \t url={webRequest.url}");
                }

                AllDownloadTextureFailRecord.Remove(textureRelativePath);
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
            string content = SerilazeManager.SerializeObject(dataSources);
            string filePath = Application.dataPath.CombinePathEx(ConstDefine.S_EditorName).CombinePathEx("totalNeedUpdatePreloadTexture.txt");
            IOUtility.CreateOrSetFileContent(filePath, content);
            Debug.LogEditorInfor($"ShowAllNeedUpdatePreloadTeturesByType 成功，保存在目录 {filePath}");
        }
#endif

        #endregion
    }
}
