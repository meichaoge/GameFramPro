using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 负责AssetBundle 资源的更新逻辑
    /// </summary>
    public  class AssetBundleUpgradeManager:Single<AssetBundleUpgradeManager>
    {

        private static string s_LocalAssetBundleTopDirectoryPath = string.Empty;
        public static string S_LocalAssetBundleTopDirectoryPath
        {
            get
            {
                if (s_LocalAssetBundleTopDirectoryPath == null)
                {
                    s_LocalAssetBundleTopDirectoryPath = Application.persistentDataPath.CombinePathEx(ConstDefine.S_LocalStoreDirectoryName).
                        CombinePathEx(ConstDefine.S_AssetBundleDirectoryName);
                }
                return s_LocalAssetBundleTopDirectoryPath;
            }
        } //本地AssetBundle 顶层存储的路径


        #region 获取本地AssetBundle 资源以及对于的配置表                                               
        private LocalAssetBundleAssetTotalInfor mLocalBundleAssetConfigInfor = null; //本地的AssetBundle 配置 ，可能是null

        private void GetLocalAsetBundleContainAssetConfig()
        {
            mLocalBundleAssetConfigInfor = null;
            string localAssetBundleTotalConfigFilePath = S_LocalAssetBundleTopDirectoryPath.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName); //本地配置文件的路径
            string assetBundleConfigContent = string.Empty;
            if(IOUtility.GetFileContent(localAssetBundleTotalConfigFilePath,out assetBundleConfigContent) == false)
            {
                Debug.LogInfor(string.Format("GetLocalAsetBundleContainAssetConfig Fail,Local Path ({0}) AssetBundle Config Not Exit!", localAssetBundleTotalConfigFilePath));
                return;
            }

            mLocalBundleAssetConfigInfor = SerilazeManager.DeserializeObject<LocalAssetBundleAssetTotalInfor>(assetBundleConfigContent);
        }

        #endregion


        #region 下载服务器的AssetBundle 配置信息
        /// <summary>
        /// 下载服务器的配置信息
        /// </summary>
        private void GetServerAssetBundleContainAssetConfig()
        {
            string assetBundleConfigFileUrl = AppUrlManager.S_AssetBundleCDNTopUrl.CombinePathEx(ConstDefine.S_AssetBundleConfigFileName);
            //DownloadManager.S_Instance.GetStringDataFromUrl(assetBundleConfigFileUrl, OnCompleteGetServerAssetBundleConfig, UnityTaskPriorityEnum.Immediately);
        }


        private void OnCompleteGetServerAssetBundleConfig(string assetBundleConfig,string url)
        {
            Debug.LogInfor("----  " + assetBundleConfig);
        }
        #endregion


        #region 接口
        /// <summary>
        /// 检测AssetBundle 资源的状态是否需要更新下载
        /// </summary>
        public void BeginCheckAssetBundleAssetState()
        {
            GetServerAssetBundleContainAssetConfig();
        }




        public AssetBundle GetAssetBundleByPath(string assetBundleRelativePath)
        {
            return null;
        }
        #endregion

    }
}