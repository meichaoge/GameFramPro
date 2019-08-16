//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.UI;
//using GameFramePro.CacheEx;
//
//namespace GameFramePro.Upgrade
//{
//    /// <summary>/// 用于在进入登录界面前提前加载CDN 上图片/// </summary>
//    public class PreLoadImgMgr2 : Single<PreLoadImgMgr2>
//    {
//        #region 路径配置 
//
//        /// <summary>/// CDN需要预加载的所有资源路径顶层文件夹名/// </summary>
//        private const string PreloadImgAssetServerTopDirectory = "PreloadTextures";
//
//        /// <summary>/// 本地和服务器的资源配置名称 (Unity 客户端使用相同的名称)/// </summary>
//        public const string PreloadImgConfiFileName = "PreloadImageAssetConfig.json";
//
//
//        private string mLocalStorePath = string.Empty;
//
//        /// <summary>/// 活动图片资源本地存储的顶级目录 （也存储了本地的配置文件）/// </summary>
//        public string LocalStorePath
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(mLocalStorePath))
//                    mLocalStorePath = $"{AppTextureCacheManager.S_TextureCacheTopDirectory}{PreloadImgAssetServerTopDirectory}/";
//                return mLocalStorePath;
//            }
//        }
//
//
//        private string mPreloadImgConfigServerUrl = null;
//
//        /// <summary>/// 活动图片资源配置信息地址/// </summary>
//        public string PreloadImgConfigServerUrl
//        {
//            get
//            {
//                if (string.IsNullOrEmpty(mPreloadImgConfigServerUrl))
//                    mPreloadImgConfigServerUrl = $"{AppUrlManager.S_TextureCDNTopUrl}/{PreloadImgAssetServerTopDirectory}/";
//
//                return mPreloadImgConfigServerUrl;
//            }
//        }
//
//        #endregion
//
//        #region 数据
//
//        //**Server
//        private PreloadImgConfigInfor ServerPreloadImgConfigInfor = null; //服务器下载的需要预加载的资源配置
//        private System.Action<bool> mOnCompleteDownLoadAct = null; //服务器配置文件下载事件
//
//        ////**Local
//        private bool IsLocalLocalImgAsset = false; //标示是否已经加载了本地的图片资源的信息
//        private Dictionary<string, string> LocalPreloadImgAssetMD5Infor = new Dictionary<string, string>(); //本地资源的MD5信息
//        private Dictionary<string, byte[]> localPreloadImgData = new Dictionary<string, byte[]>(); //本地资源的数据 在计算MD5时候一起保存的
//
//        //**下载
//        HashSet<string> AllNeedDownloadAssetRecord = new HashSet<string>(); //需要下载或者更新的资源
//
////        private Dictionary<string, DownloadThreadWWW> AllDowonloadimgLoader = new Dictionary<string, DownloadThreadWWW>(); //下载器
//        private static object lockObj = new object();
//        private int mTimerHashCode = 0;
//        private float mTimerTickerSpace = 0.3f; //检测时间间隔
//
//        private System.Action<bool> OnCompleteUpdatePreloadImagAct = null; //完成了所有的下载任务
//
//        //**对外接口
//        private Dictionary<string, Sprite> mAllPreloadImgSprites = new Dictionary<string, Sprite>(); //byte[] 转化成了sprite
//
//        #endregion
//
//
//        #region 服务器下载配置文件
//
//        /// <summary>/// 获取服务器版本信息/// </summary>
//        public void LoadServerPreloadImgConfig(System.Action<bool> completeCallback)
//        {
//            if (ServerPreloadImgConfigInfor != null)
//            {
//                Debug.Log("服务器配置文件已经下载了 不需要重复下载");
//                completeCallback?.Invoke(true);
//                return;
//            }
//
//            AsyncManager.S_Instance.StartCoroutine(DownLoadConfig(completeCallback));
//        }
//
//        /// <summary>/// 下载配置文件/// </summary>
//        private IEnumerator DownLoadConfig(System.Action<bool> completeCallback)
//        {
//            mOnCompleteDownLoadAct += completeCallback;
//            WWW ww = new WWW(PreloadImgConfigServerUrl);
//            yield return ww;
//            if (ww.isDone == false || string.IsNullOrEmpty(ww.error) == false)
//            {
//                Debug.LogError($"DownLoadConfig Fail, Error:{ww.error}  url={PreloadImgConfigServerUrl}");
//                CompleteDownLoadConfig(false);
//                yield break;
//            }
//
//            ServerPreloadImgConfigInfor = LitJson.JsonMapper.ToObject<PreloadImgConfigInfor>(ww.text);
//            CompleteDownLoadConfig(true);
//        }
//
//        /// <summary>/// 下载完成更新配置 和状态/// </summary>
//        private void CompleteDownLoadConfig(bool isSuccess)
//        {
//            Debug.LogInfor("CompleteDownLoadConfig isSuccess= " + isSuccess);
//
//            mOnCompleteDownLoadAct?.Invoke(isSuccess);
//        }
//
//        #endregion
//
//        #region 加载本地配置和资源
//
//        /// <summary>/// 加载并获取本地资源的MD5信息/// </summary>
//        private void LoadAllLocalPreloadImgAssetInfor(System.Action<bool> completeCallback)
//        {
//            IsLocalLocalImgAsset = false;
//            LocalPreloadImgAssetMD5Infor.Clear();
//            if (System.IO.Directory.Exists(LocalStorePath) == false)
//            {
//                System.IO.Directory.CreateDirectory(LocalStorePath); //方便后面的下载
//                completeCallback?.Invoke(true);
//                return;
//            }
//
//            string[] allFiles = System.IO.Directory.GetFiles(LocalStorePath, "*.*", SearchOption.AllDirectories);
//            foreach (var file in allFiles)
//            {
//                string extensionName = System.IO.Path.GetExtension(file);
//                if (extensionName == ".manifest" || extensionName == ".json")
//                {
//                    continue;
//                }
//
//                string imgAssetPath = file.Substring(LocalStorePath.Length); // file.Substring(file.IndexOf(LocalStorePath) + LocalStorePath.Length);
//                imgAssetPath = IOUtility.GetPathWithOutExtension(imgAssetPath); // System.IO.Path.GetFileNameWithoutExtension(file);
//                imgAssetPath = imgAssetPath.Replace("\\", "/");
//#if UNITY_EDITOR
//                Debug.LogEditorInfor("---------------------localPreloadimgAsset :" + imgAssetPath);
//#endif
//                string md5Code = string.Empty;
//                byte[] imgData = MD5Helper.GetFileMD5(file, out md5Code); //图片数据
//                LocalPreloadImgAssetMD5Infor.Add(imgAssetPath, md5Code);
//
//                //****保存本地的图片数据 避免多次加载
//                string fileName = System.IO.Path.GetFileNameWithoutExtension(imgAssetPath);
//                if (localPreloadImgData.ContainsKey(fileName))
//                {
//                    Debug.LogError("存在重复的资源 " + fileName);
//                    continue;
//                }
//
//                localPreloadImgData.Add(fileName, imgData);
//            }
//
//            IsLocalLocalImgAsset = true;
//
//            completeCallback?.Invoke(true);
//        }
//
//        #endregion
//
//        #region 对比本地数据和服务器的数据 获取需要下载的资源
//
//        /// <summary>/// 检车版本资源并开始活动预加载图片的加载流程/// </summary>
//        public void CheckAssetAndUpdate(System.Action<bool> completeCallback)
//        {
//            if (ServerPreloadImgConfigInfor == null)
//            {
//                LoadServerPreloadImgConfig((isSuccess) =>
//                {
//                    if (isSuccess == false)
//                    {
//                        completeCallback?.Invoke(false);
//                        return;
//                    }
//
//                    GetAllNeedUpdateOrDownloadImgAsset(completeCallback);
//                });
//                return;
//            } //服务器配置没有下载则先下载这个 然后比对
//
//            GetAllNeedUpdateOrDownloadImgAsset(completeCallback);
//        }
//
//
//        /// <summary>///  对比本地数据和服务器的数据 获取需要下载的资源/// </summary>
//        private void GetAllNeedUpdateOrDownloadImgAsset(System.Action<bool> completeCallback)
//        {
//            OnCompleteUpdatePreloadImagAct = completeCallback;
//            if (IsLocalLocalImgAsset == false)
//                LoadAllLocalPreloadImgAssetInfor(null); //确保本地资源被正确加载
//
//            Debug.Log($"开始比对服务器与本地资源版本信息，服务器资源版本:{ServerPreloadImgConfigInfor.Version} 包含{ServerPreloadImgConfigInfor.AllPreloadImgConfig.Count.ToString()}个数据");
//
//
//            Dictionary<string, bool> allNeedDeleteAssetRecord = new Dictionary<string, bool>(); //本地需要删除的资源 value 标示是否需要删除.json 文件
//
//            HashSet<string> allServerPreloadImgAssetTemp = new HashSet<string>(); //临时获取所有的服务器资源的key  用于判断哪些资源需要删除
//            foreach (var item in ServerPreloadImgConfigInfor.AllPreloadImgConfig)
//            {
//                //if (allServerPreloadImgAssetTemp.Contains(item.mImgName))
//                //{
//                //    CustomDebug.DebugLogError("重复的服务器资源 " + item.mImgName);
//                //}  //不需要 在生成配置时候已经判断了
//                allServerPreloadImgAssetTemp.Add(item.mImgName);
//                string localImgMD5 = string.Empty;
//                if (LocalPreloadImgAssetMD5Infor.TryGetValue(item.mImgName, out localImgMD5))
//                {
//                    if (localImgMD5 == item.mAssetMD5)
//                        continue; //已经是最新版本
//                    AllNeedDownloadAssetRecord.Add(item.mImgName); //需要删除本地资源 下载新的资源
//                    allNeedDeleteAssetRecord.Add(item.mImgName, false); //本地资源需要删除
//                    Debug.Log($"需要删除本地资源 下载新的资源:{item.mImgName}");
//                } //本地存在这个资源
//                else
//                {
//                    AllNeedDownloadAssetRecord.Add(item.mImgName); //需要下载新的资源
//                    Debug.Log($"需要下载新的资源:{item.mImgName}");
//                } //需要下载的新资源
//            }
//
//            ///获取需要删除的本地资源 (本地有资源 服务器配置里面没有的)
//            foreach (var item in LocalPreloadImgAssetMD5Infor.Keys)
//            {
//                if (allServerPreloadImgAssetTemp.Contains(item) == false)
//                {
//                    allNeedDeleteAssetRecord.Add(item, true); //需要下载新的资源 
//                    Debug.Log($"本地旧的资源:{item}");
//                }
//            }
//
//            if (allNeedDeleteAssetRecord.Count > 0)
//                DeleteLoadUnUseAsset(allNeedDeleteAssetRecord);
//
//            if (AllNeedDownloadAssetRecord.Count > 0)
//                DownloadImgAsset(AllNeedDownloadAssetRecord);
//            else
//                OnCompleteDownLoadImage(); //不需要下载
//        }
//
//
//        /// <summary>/// 删除本地无用或者过时的数据/// </summary>
//        private void DeleteLoadUnUseAsset(Dictionary<string, bool> allNeedDeleteAssetRecord)
//        {
//            try
//            {
//                foreach (var item in allNeedDeleteAssetRecord)
//                {
//                    string realPath = $"{LocalStorePath}{item.Key}.png";
//#if UNITY_EDITOR
//                    Debug.Log("需要删除的图片路径   " + realPath);
//#endif
//                    System.IO.File.Delete(realPath);
//                    if (item.Value)
//                    {
//                        realPath += ".json";
//                        if (System.IO.File.Exists(realPath))
//                            System.IO.File.Delete(realPath);
//                    } //删除.json 配置文件
//                }
//
//                //删除无效的缓存图片数据
//                foreach (var item in allNeedDeleteAssetRecord)
//                {
//                    if (localPreloadImgData.ContainsKey(item.Key))
//                    {
//                        Debug.Log("删除本地的缓存图片数据 " + item.Key);
//                        localPreloadImgData.Remove(item.Key);
//                    }
//                }
//            }
//            catch (System.Exception e)
//            {
//                Debug.LogError($"删除本地旧资源失败 {e.ToString()}  ");
//            }
//        }
//
//        #endregion
//
//        #region 下载新的资源
//
//        /// <summary>/// 下载需要的图片资源/// </summary>
//        private void DownloadImgAsset(HashSet<string> allNeedDownloadAssetRecord)
//        {
////            foreach (var item in allNeedDownloadAssetRecord)
////            {
////                string url = $"{PreloadImgAssetServerUrl}{item}.png";
////                string savepath = $"{LocalStorePath}{item}.png";
////                DownloadThreadWWW loader = new DownloadThreadWWW(url, savepath, 0);
////                loader.CompleteCallbackAct = OnDownloadImageLoaderCallback;
////                if (AllDowonloadimgLoader.ContainsKey(item))
////                {
////                    CustomDebug.DebugLogError("下载重复的资源 " + item);
////                    continue;
////                }
////
////                AllDowonloadimgLoader.Add(item, loader);
////            }
////
////            mTimerHashCode = TimeTickUtility.Instance.RegisterTimer(mTimerTickerSpace, CheckIfCompleteAllDownload);
//        }
//
////        /// <summary>/// 下载回调/// </summary>
////        private void OnDownloadImageLoaderCallback(DownloadThreadWWW loader)
////        {
////            lock (lockObj)
////            {
////                string loaderKey = System.IO.Path.GetFileNameWithoutExtension(loader.tUrl);
////                AllDowonloadimgLoader.Remove(loaderKey);
////                if (loader.IsSuccess)
////                {
////                    CustomDebug.DebugLog(string.Format("下载预加载图片资源成功： {0} 还有{1}个资源需要下载", loaderKey, AllDowonloadimgLoader.Count));
////                }
////                else
////                {
////                    CustomDebug.DebugLogError(string.Format("多次下载预加载图片资源失败 {0}", loaderKey));
////                }
////            }
////        }
//
//        /// <summary>/// 检测是否完成了所有的下载任务 可能有些资源下载失败  /// </summary>
//        private void CheckIfCompleteAllDownload(float time, int hashCode)
//        {
////            if (mTimerHashCode == hashCode)
////            {
////                if (AllDowonloadimgLoader.Count == 0)
////                {
////                    OnCompleteDownLoadImage();
////                    TimeTickUtility.S_Instance.UnRegisterTimer_Delay(mTimerHashCode);
////                }
////            }
//        }
//
//        private void OnCompleteDownLoadImage()
//        {
//            //CustomDebug.DebugLog("-------------------------------完成了预加载图片资源的更新任务-------------");
//            RecordNewDownloadAssetInfor();
//            AllNeedDownloadAssetRecord.Clear(); //清理数据
//
//            if (OnCompleteUpdatePreloadImagAct != null)
//            {
//                OnCompleteUpdatePreloadImagAct(true);
//            }
//        }
//
//
//        /// <summary>/// 重新记录新下载的资源信息/// </summary>
//        private void RecordNewDownloadAssetInfor()
//        {
//            if (System.IO.Directory.Exists(LocalStorePath) == false)
//            {
//                System.IO.Directory.CreateDirectory(LocalStorePath); //方便后面的下载
//                return;
//            }
//
//            string[] allFiles = System.IO.Directory.GetFiles(LocalStorePath, "*.*", SearchOption.AllDirectories);
//            foreach (var file in allFiles)
//            {
//                string extensionName = System.IO.Path.GetExtension(file);
//                if (extensionName == ".manifest" || extensionName == ".json")
//                {
//                    continue;
//                }
//
//                //***这里避免判断之前已经预读取的资源信息
//                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
//                if (AllNeedDownloadAssetRecord.Contains(fileName) == false)
//                    continue;
//
//
//                string imgAssetPath = string.Empty;
//                imgAssetPath = file.Substring(LocalStorePath.Length); // file.Substring(file.IndexOf(LocalStorePath) + LocalStorePath.Length);
//                imgAssetPath = IOUtility.GetPathWithOutExtension(imgAssetPath); // System.IO.Path.GetFileNameWithoutExtension(file);
//                imgAssetPath = imgAssetPath.Replace("\\", "/");
//#if UNITY_EDITOR
//                Debug.Log("---------------------localPreloadimgAsset :" + imgAssetPath);
//#endif
//                string md5Code = string.Empty;
//                byte[] imgData = MD5Helper.GetFileMD5(file, out md5Code); //图片数据
//                LocalPreloadImgAssetMD5Infor[imgAssetPath] = md5Code; //这里可能是更新旧的资源
//
//
//                //****保存本地的图片数据 避免多次加载
//                if (localPreloadImgData.ContainsKey(fileName))
//                {
//                    Debug.LogError("存在重复的资源 " + fileName);
//                    continue;
//                }
//
//                localPreloadImgData.Add(fileName, imgData);
//            }
//
//            Debug.Log("--------------完成下载后重新读取下载资源的信息-----------");
//        }
//
//        #endregion
//
//
//        #region 对外接口
//
//        /// <summary>/// 检测指定图片资源是否存在(byte[] 或者sprite形式，二者之一)/// </summary>
//        public bool CheckIfExitImage(string iamgeName)
//        {
//            bool isExitByte = localPreloadImgData.ContainsKey(iamgeName);
//            if (isExitByte)
//                return true; //byte 形式
//            bool isExitSprite = mAllPreloadImgSprites.ContainsKey(iamgeName);
//            if (isExitSprite)
//                return true; //已经转化成了sprite
//            return false;
//        }
//
//        /// <summary>/// 获取指定的图片资源/// </summary>
//        /// <param name="isShowErrorLog">是否显示错误日志</param>
//        public bool GetPreloadImgeSprite(UnityEngine.UI.Image target, string imageName, bool isSetNativeSize = true, bool isShowErrorLog = true)
//        {
//            if (string.IsNullOrEmpty(imageName))
//            {
//                Debug.LogError("imageName is Null");
//                return false;
//            }
//
//            if (target == null)
//            {
//                Debug.LogError("target is Null");
//                return false;
//            }
//
//            if (mAllPreloadImgSprites.TryGetValue(imageName, out var result))
//            {
//                target.sprite = result;
//                if (isSetNativeSize)
//                    target.SetNativeSize();
//                return true;
//            }
//
//            if (localPreloadImgData.TryGetValue(imageName, out var data))
//            {
//                Texture2D texture = new Texture2D(100, 100, TextureFormat.RGBA32, false, true);
//                texture.LoadImage(data);
//
//                result = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
//                target.sprite = result;
//                if (isSetNativeSize)
//                    target.SetNativeSize();
//
//                //****保存创建的精灵资源
//                localPreloadImgData.Remove(imageName);
//                mAllPreloadImgSprites.Add(imageName, result);
//                return true;
//            }
//
//            if (isShowErrorLog)
//                Debug.LogError("不存在这个图片资源  " + imageName);
//            return false;
//        }
//
//        /// <summary>/// 获取所有活动图片资源 有些需要临时下载/// </summary>
//        /// <param name="isSetNativeSize">标示是否设置原始尺寸</param>
//        public void GetActivityImgeSprite(Image target, string imageName, bool isSetNativeSize = true)
//        {
//            if (string.IsNullOrEmpty(imageName))
//            {
//                Debug.LogError("imageName is Null");
//                return;
//            }
//
//            if (target == null)
//            {
//                Debug.LogError("target is Null");
//                return;
//            }
//
//            if (GetPreloadImgeSprite(target, imageName, isSetNativeSize, false))
//                return;
//
//            //  target.DOFade(0, 0.3f);
//            //    string url = string.Format("{0}{1}.png", ActivityImgAssetTopUrl, imageName);
//
////            ImageDownloadMgrEx.instance.loadImageByUrl(target, url, delegate(bool issucc)
////            {
////                if (issucc)
////                {
////                    target.DOFade(1, 0.3f);
////                }
////            });
//
//
//            if (isSetNativeSize)
//                target.SetNativeSize();
//        }
//
//        #endregion
//    }
//}
