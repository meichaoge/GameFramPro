﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using GameFramePro.NetWorkEx;
using UnityEngine.Networking;
using System.Threading;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 负责AssetBundle 资源的更新逻辑
    /// </summary>
    public class AssetBundleUpgradeManager : Single<AssetBundleUpgradeManager>
    {

        #region 需要更新的AssetBundle 信息

        /// <summary>
        /// 标示更新Asset Bundle 资源时候的状态
        /// </summary>
        private enum AssetBundleAssetUpdateTagEnum
        {
            AddAsset,  //新增资源
            RemoveLocalAsset,  //本地资源已经无效 需要删除
            UpdateAsset, //本地资源需要更新
        }

        [System.Serializable]
        private class UpdateAssetBundleInfor
        {
            public AssetBundleAssetUpdateTagEnum mAssetBundleAssetUpdateTagEnum;
            public string mNeedUpdateAssetName = string.Empty;
            public long mAssetByteSize = 0;
            public uint mAssetCRC = 0;


            public UpdateAssetBundleInfor() { }
            public UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum tagEnum, string assetName, long byteSize, uint crc)
            {
                mAssetBundleAssetUpdateTagEnum = tagEnum;
                mNeedUpdateAssetName = assetName;
                mAssetByteSize = byteSize;
                mAssetCRC = crc;
            }
        }


        private HashSet<string> mAllNeedDownloadAssetBundleNameRecord = new HashSet<string>(); //所有需要下载更新的AssetBundle 信息
        private Dictionary<string, UpdateAssetBundleInfor> mAllNeedUpdateAssetBundleAssetInfor = new Dictionary<string, UpdateAssetBundleInfor>(); //所有需要更新的资源
        private const int S_MaxDownloadTimes = 3; //最大下载次数
        private int mCurDownloadTime = 0;
        #endregion


        #region 获取本地AssetBundle 资源以及对于的配置表                                               
        //     private AssetBundleAssetTotalInfor mLocalBundleAssetConfigInfor = null; //本地的AssetBundle 配置 ，可能是null
        //key=assetBundle 资源的路径 value=每个AssetBundle资源信息
        private Dictionary<string, AssetBundleInfor> mAllLocalAssetBundleAssetFileInfor = new Dictionary<string, AssetBundleInfor>();
        private float mAllLocalAssetBundleLoadProcess = 0f; //获取进度

        /// <summary>
        /// 首先需要判断本地文件有没有被修改过(或者被删除了部分资源)，如果有改动则需要读取每个文件的MD5,否则只需要根据本地配置文件修改即可
        /// </summary>
        /// <returns></returns>
        private bool CheckIsLocalAssetBundleAssetValib()
        {
            if (System.IO.Directory.Exists(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath) == false)
                return true;
            var allAssetBundleFiles = System.IO.Directory.GetFiles(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath, "*.*", System.IO.SearchOption.AllDirectories);
            if (allAssetBundleFiles.Length == 0)
                return true;
            Loom.S_Instance.RunAsync(() =>
            {
                for (int dex = 0; dex < allAssetBundleFiles.Length; dex++)
                {
                    var filePath = allAssetBundleFiles[dex];
                    AssetBundleInfor assetBundleInfor = new AssetBundleInfor();
                    assetBundleInfor.mBundleName = filePath.Substring(AssetBundleManager.S_LocalAssetBundleTopDirectoryPath.Length + 1);
                    assetBundleInfor.mBundleMD5Code = MD5Helper.GetFileMD5(filePath, ref assetBundleInfor.mBundleSize);
                    string assetBundleNameStr = assetBundleInfor.mBundleName.GetPathStringEx(); //去掉路径分隔符
                    mAllLocalAssetBundleAssetFileInfor[assetBundleNameStr] = assetBundleInfor; //记录本地AssetBundle 资源信息

                    mAllLocalAssetBundleLoadProcess = dex * 1f / allAssetBundleFiles.Length;  //进度
                }
                Loom.S_Instance.QueueOnMainThread(OnCompleteAllLocalAssetBundleInfor);
            });

            return false;
        }

        /// <summary>
        /// 完成获取本地资源的任务
        /// </summary>
        private void OnCompleteAllLocalAssetBundleInfor()
        {
            Debug.Log("OnCompleteAllLocalAssetBundleInfor------------->>"+Thread.CurrentThread.IsThreadPoolThread);
            mAllLocalAssetBundleLoadProcess = 1f;
            //   GetLocalAsetBundleContainAssetConfig();
            GetServerAssetBundleContainAssetConfig(GetAllNeedUpdateAssetBundleAssetInfor);
        }


        #endregion

        #region 下载服务器的AssetBundle 配置信息
        private AssetBundleAssetTotalInfor mServerBundleAssetConfigInfor = null; //从服务器获取的 AssetBundle 配置

        /// <summary>
        /// 下载服务器的配置信息
        /// </summary>
        private void GetServerAssetBundleContainAssetConfig(System.Action<bool> onCompleteDownloadConfig)
        {
            string assetBundleConfigFileUrl = AppUrlManager.S_AssetBundleCDNTopUrl.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
            DownloadManager.S_Instance.GetByteDataFromUrl(assetBundleConfigFileUrl, (webRequset, isSuccess, url) =>
            {
                OnCompleteGetServerAssetBundleConfig(webRequset, isSuccess, url, onCompleteDownloadConfig);
            }, UnityTaskPriorityEnum.Immediately);
        }


        private void OnCompleteGetServerAssetBundleConfig(UnityWebRequest webRequset, bool isSuccess, string url, System.Action<bool> onCompleteDownloadConfig)
        {
            if (isSuccess == false)
            {
                Debug.LogError("OnCompleteGetServerAssetBundleConfig Fail Error:" + webRequset.error);
                if (onCompleteDownloadConfig != null)
                    onCompleteDownloadConfig(false);
                return;
            }

            Debug.LogInfor("OnCompleteGetServerAssetBundleConfig Success!!");
            mServerBundleAssetConfigInfor = SerilazeManager.DeserializeObject<AssetBundleAssetTotalInfor>((webRequset.downloadHandler as DownloadHandlerBuffer).text);
            if (onCompleteDownloadConfig != null)
                onCompleteDownloadConfig(true);
        }
        #endregion


        #region 对比获取需要下载的资源 /下载资源
        /// <summary>
        /// 对比获取所有需要更新的 AssetBundle 资源资源
        /// </summary>
        /// <param name="isSuccessGetServerConfig"></param>
        /// <returns></returns>
        private void GetAllNeedUpdateAssetBundleAssetInfor(bool isSuccessGetServerConfig)
        {
            if (isSuccessGetServerConfig == false) return;
            //Key =AssetBundle Name, value{=true 标示需要更新 =fasle 标示需要删除资源}

            AssetBundleInfor localAssetBundleConfig = null;

            //对比服务器配置 获取哪些资源需要更新或者新增
            foreach (var assetBunleInfor in mServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                //if (mLocalBundleAssetConfigInfor == null)
                //{
                //    mAllNeedUpdateAssetBundleAssetInfor.Add(assetBunleInfor.mBundleName,new UpdateAssetBundleInfor(  AssetBundleAssetUpdateTagEnum.AddAsset, assetBunleInfor.mBundleName, assetBunleInfor.mBundleSize));
                //    continue;
                //}
                var assetBundleInfor = assetBunleInfor.Value;

                if (mAllLocalAssetBundleAssetFileInfor.TryGetValue(assetBunleInfor.Key, out localAssetBundleConfig))
                {

                    if (localAssetBundleConfig.mBundleMD5Code != assetBundleInfor.mBundleMD5Code)
                        mAllNeedUpdateAssetBundleAssetInfor.Add(assetBundleInfor.mBundleName, new UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum.UpdateAsset, assetBundleInfor.mBundleName, assetBundleInfor.mBundleSize, assetBundleInfor.mBundleCRC)); //需要更新
                    continue;
                }
                mAllNeedUpdateAssetBundleAssetInfor.Add(assetBunleInfor.Value.mBundleName, new UpdateAssetBundleInfor(AssetBundleAssetUpdateTagEnum.UpdateAsset, assetBundleInfor.mBundleName, assetBundleInfor.mBundleSize, assetBundleInfor.mBundleCRC)); //需要更新
            }


            HashSet<string> allNeedDeleteAssetBundleNameInfor = new HashSet<string>();
            //对比获取哪些资源需要删除
            //  if (mLocalBundleAssetConfigInfor == null)
            {
                foreach (var assetBunleInfor in mAllLocalAssetBundleAssetFileInfor)
                {
                    if (mServerBundleAssetConfigInfor.mTotalAssetBundleInfor.ContainsKey(assetBunleInfor.Key))
                        continue;
                    allNeedDeleteAssetBundleNameInfor.Add(assetBunleInfor.Value.mBundleName); //需要删除
                }
            }

            DeleteAllInvalidAssetBundelAsset(allNeedDeleteAssetBundleNameInfor);


#if UNITY_EDITOR
            ShowAllNeedUpdateAssetBundleByType(mAllNeedUpdateAssetBundleAssetInfor);
#endif
            BeginDownloadAssetBundle(mAllNeedUpdateAssetBundleAssetInfor);  //开始下载
            return;
        }

        /// <summary>
        /// 删除无效的AsssetBundle 资源
        /// </summary>
        private void DeleteAllInvalidAssetBundelAsset(HashSet<string> dataSources)
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
            Debug.Log("DeleteAllInvalidAssetBundelAsset Complete");
        }



        /// <summary>
        /// 根据获取的下载列表下载资源
        /// </summary>
        /// <param name="dataSources"></param>
        private void BeginDownloadAssetBundle(Dictionary<string, UpdateAssetBundleInfor> dataSources)
        {
            if (dataSources == null || dataSources.Count == 0)
            {
                Debug.Log("所有的AssetBundle 资源都是最新的");
                OnAllLocalAssetBundleIsValib();
                return;
            }
            Debug.LogInfor(string.Format("BeginDownloadAssetBundle ,一共有{0}个需要需要更新或者下载", dataSources.Count));
            //mAllNeedDownloadAssetBundleNameRecord = new HashSet<string>();

            foreach (var assetBundleRecord in dataSources.Values)
            {
                if (assetBundleRecord.mAssetBundleAssetUpdateTagEnum == AssetBundleAssetUpdateTagEnum.RemoveLocalAsset)
                {
                    Debug.LogError("BeginDownloadAssetBundle Error " + assetBundleRecord.mNeedUpdateAssetName);
                    continue;
                }
                string updateAssetBundleUrl = string.Format("{0}/{1}", AppUrlManager.S_AssetBundleCDNTopUrl, assetBundleRecord.mNeedUpdateAssetName);
                mAllNeedDownloadAssetBundleNameRecord.Add(assetBundleRecord.mNeedUpdateAssetName); //记录那些资源需要下载
                DownloadManager.S_Instance.GetByteDataFromUrl(updateAssetBundleUrl,OnDownloadAssetBundleCallback, UnityTaskPriorityEnum.Immediately);
            }

            //if (mAllNeedDownloadAssetBundleNameRecord.Count == 0)
            //{
            //    Debug.LogError("没有需要下载的AssetBundle 资源,只需要删除旧的资源");
            //    return;
            //}
            Debug.Log(string.Format("BeginDownloadAssetBundle 一共有{0} 个下载AssetBundle 需求", mAllNeedDownloadAssetBundleNameRecord.Count));
        }


        /// <summary>
        /// 下载一个Asetbundle 资源回调
        /// </summary>
        /// <param name="assetBundle"></param>
        /// <param name="url"></param>
        private void OnDownloadAssetBundleCallback(UnityWebRequest webRequest, bool isSuccess, string url)
        {
            if (isSuccess == false)
            {
                Debug.LogError("OnDownloadAssetBundleCallback Fail,Error " + webRequest.error + "   url=" + webRequest.url);
                return;
            }

            string assetBundleName = url.Substring(AppUrlManager.S_AssetBundleCDNTopUrl.Length + 1);
            if (mAllNeedDownloadAssetBundleNameRecord.Contains(assetBundleName) == false)
            {
                Debug.LogError("OnDownloadAssetBundleCallback Fail,没有记载的下载AssetBundle 记录 " + assetBundleName + " \t Url=" + url);
                return;
            }

            DownloadHandlerBuffer handle = webRequest.downloadHandler as DownloadHandlerBuffer;

            Debug.Log("OnDownloadAssetBundleCallback-->>>   \t " + url);
            mAllNeedDownloadAssetBundleNameRecord.Remove(assetBundleName);

            mAllNeedUpdateAssetBundleAssetInfor.Remove(assetBundleName);
            AssetBundleManager.S_Instance.SaveAssetBundleFromDownload(handle, assetBundleName);

            if (mAllNeedDownloadAssetBundleNameRecord.Count == 0)
                OnCompleteDownloadAssetBundelAsset();
        }

    

        ///// <summary>
        ///// 检测是否完成下载任务
        ///// </summary>
        ///// <returns></returns>
        //private bool CheckIfCompleteAllDownloadAssetBundle()
        //{

        //        return false;
        //    if (mAllNeedUpdateAssetBundleAssetInfor.Count != 0)
        //    {
        //        Debug.LogError("CheckIfCompleteAllDownloadAssetBundle !!! 有部分资源下载失败 " + mAllNeedUpdateAssetBundleAssetInfor.Count);
        //        return false;
        //    }

        //    return true;
        //}

        private void OnAllLocalAssetBundleIsValib()
        {
            Debug.LogInfor("OnAllLocalAssetBundleIsValib 本地资源是最新的不需要处理");
        }

        /// <summary>
        /// 完成 了资源的下载 
        /// </summary>
        private void OnCompleteDownloadAssetBundelAsset()
        {
            if (mAllNeedUpdateAssetBundleAssetInfor.Count != 0)
            {
                Debug.LogError("CheckIfCompleteAllDownloadAssetBundle !!! 有部分资源下载失败 " + mAllNeedUpdateAssetBundleAssetInfor.Count);
                if (mCurDownloadTime < S_MaxDownloadTimes)
                    BeginDownloadAssetBundle(mAllNeedUpdateAssetBundleAssetInfor); //重新下载
                Debug.LogError("多次下载失败");
                return;
            }

            Debug.LogInfor("完成了所有的任务");
        }


#if UNITY_EDITOR

        /// <summary>
        /// 根据类型分类需要更新的AssetBundle 资源
        /// </summary>
        /// <param name="dataSources"></param>
        private void ShowAllNeedUpdateAssetBundleByType(Dictionary<string, UpdateAssetBundleInfor> dataSources)
        {
            var dataList = dataSources.GroupBy((assetBundleItem) => assetBundleItem.Value.mAssetBundleAssetUpdateTagEnum).ToList();
            string content = SerilazeManager.SerializeObject(dataList);
            string filePath = Application.dataPath.CombinePathEx(ConstDefine.S_EditorName).CombinePathEx("totalNeedUpdateAssetBundle.txt");
            Debug.Log("ShowAllNeedUpdateAssetBundleByType-->>> " + filePath);
            IOUtility.CreateOrSetFileContent(filePath, content);
            Debug.LogEditorInfor("ShowAllNeedUpdateAssetBundleByType 成功，保存在目录" + filePath);
        }
#endif


        #endregion


        #region 接口
        /// <summary>
        /// 检测AssetBundle 资源的状态是否需要更新下载
        /// </summary>
        public void BeginUpdateAssetBundle()
        {
            bool isLoalAssetBundleValib = CheckIsLocalAssetBundleAssetValib();
            if (isLoalAssetBundleValib)
            {
                Debug.LogInfor("BeginUpdateAssetBundle 本地没有资源 下载所有的资源");
                mAllLocalAssetBundleLoadProcess = 1f;
                GetServerAssetBundleContainAssetConfig(GetAllNeedUpdateAssetBundleAssetInfor);
            }
        }


        /// <summary>
        /// 根据一个资源名称获取所在的AssetBundle name
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public string GetBundleNameByAssetPath(string assetPath)
        {
            if (mServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetBundleNameByAssetPath Fail,没有获取最新的AssetBundle 配置 ");
                return null;
            }
            foreach (var assetBundleInfor in mServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Value.mContainAssetPathInfor.Contains(assetPath))
                    return assetBundleInfor.Value.mBundleName;
            }

            Debug.LogError("GetBundleNameByAssetPath Fail,没有找到资源 " + assetPath);
            return string.Empty;
        }

        /// <summary>
        /// 获取指定参数的AssetBundle 依赖的信息 (内部会对路径处理)
        /// </summary>
        /// <param name="assetBundlePath"></param>
        /// <returns></returns>
        public string[] GetAllDependencies(string assetBundlePath)
        {
            if (mServerBundleAssetConfigInfor == null)
            {
                Debug.LogError("GetAllDependencies Fail,没有初始化获取AssetBundle 配置信息 " );
                return new string[0];
            }
            assetBundlePath = assetBundlePath.GetPathStringEx();

            foreach (var assetBundleInfor in mServerBundleAssetConfigInfor.mTotalAssetBundleInfor)
            {
                if (assetBundleInfor.Key == assetBundlePath)
                    return assetBundleInfor.Value.mDepdenceAssetBundleInfor;
            }

            Debug.LogError("GetAllDependencies Fail,没有找到以来关系 " + assetBundlePath);
            return new string[0];
        }

        #endregion

    }
}