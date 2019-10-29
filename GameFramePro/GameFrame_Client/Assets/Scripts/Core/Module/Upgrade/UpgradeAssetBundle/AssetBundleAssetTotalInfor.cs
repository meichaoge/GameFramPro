using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.Upgrade
{


    /// <summary>/// 单个AssetBundle 信息(上传服务器的配置)/// </summary>
    [System.Serializable]
    public class AssetBundleInfor
    {
    //    public string mABundleUri; //对应 AssetBundle 包路径
        public string RelDirctory = string.Empty; //当前资源保存和下载的相对目录 ，与 mABundleUri 组合才是得到的相对路径

        /// <summary>
        /// AssetBundle 的详细信息(大小+MD5+CRC等) 生成配置时候为null,需要时候从 mABundleUri 解析
        /// </summary>
        private AsserBundleDetail _ABundleDetail = null;

        public AsserBundleDetail GetABundleDetail(string assetBundleFileName)
        {
            if (_ABundleDetail == null)
            {
                if (AsserBundleDetail.GetAsserBundleDetail(assetBundleFileName, out _ABundleDetail) == false)
                    _ABundleDetail = new AsserBundleDetail();
            }
            return _ABundleDetail;
        }

        /// <summary>
        /// AssetBundle 中包含的资源相对于Resources 路径 (key 原始的相对路径 Value Bundle资源中的名字)
        /// </summary>
        public Dictionary<string, string> ContainAssetReUri = new Dictionary<string, string>(); //包含的资源路径

        /// <summary>
        /// 依赖的其他AssetBundle 资源 key =AssetBundle Name   Value=AssetBundle 相对路径
        /// </summary>
        public Dictionary<string,string> DepABundleInfor=new Dictionary<string, string>(); //依赖的其他AssetBundle 资源
    }

    [System.Serializable]
    public class AsserBundleDetail
    {
        public long Size; // 对应 EditorAssetBundleInfor.mPackageSize
        /// CRC 码
        public uint CRC;
        public string MD5; //资源MD5

        /// <summary>
        /// 根据AssetBundle 的文件名解析
        /// </summary>
        /// <param name="assetBundleUri"></param>
        /// <returns></returns>
        public static bool GetAsserBundleDetail(string assetBundleFileNameUri, out AsserBundleDetail asserBundleDetail)
        {
            asserBundleDetail = null;
            if (string.IsNullOrEmpty(assetBundleFileNameUri))
            {
                Debug.LogError($"选择的参数为null");
                return false;
            }

            #region 解析组合字符串


            int dexOfFistSparator = assetBundleFileNameUri.IndexOf(ConstDefine.S_AssetBundleAssetNameSeparatorChar);
            if (dexOfFistSparator == -1)
            {
#if UNITY_EDITOR
                Debug.LogError($"无法解析的文件名{assetBundleFileNameUri}");
#endif
                return false;
            }
            string fileLengthStr = assetBundleFileNameUri.Substring(0, dexOfFistSparator);
            string fileCRC_Md5Str = assetBundleFileNameUri.Substring(dexOfFistSparator + 1);
            if (long.TryParse(fileLengthStr, out long fileSize) == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"无法解析的文件名 无法处理长度信息{assetBundleFileNameUri}");
#endif
                return false;
            }

            int dexOfSecondSparator = fileCRC_Md5Str.IndexOf(ConstDefine.S_AssetBundleAssetNameSeparatorChar);
            if (dexOfSecondSparator == -1)
            {
#if UNITY_EDITOR
                Debug.LogError($"无法解析的文件名{assetBundleFileNameUri}");
#endif
                return false;
            }
            string CRCStr = fileCRC_Md5Str.Substring(0, dexOfSecondSparator);
            string fileMd5Str = fileCRC_Md5Str.Substring(dexOfSecondSparator + 1);
            if (uint.TryParse(CRCStr, out uint crc) == false)
            {
#if UNITY_EDITOR
                Debug.LogError($"无法解析的文件名 无法处理CRC 信息{assetBundleFileNameUri}");
#endif
                return false;
            }

            #endregion


            asserBundleDetail = new AsserBundleDetail();
            asserBundleDetail.Size = fileSize;
            asserBundleDetail.CRC = crc;
            asserBundleDetail.MD5 = fileMd5Str;

            return true;
        }

    }


    /// <summContainAssetRelativeUriInformary>/// 记录了从服务器获取的所有的AssetBundle 对于的信息/// </summary>
    [System.Serializable]
    public class AssetBundleAssetTotalInfor
    {
        public string mVersion = "1.0.0.1"; //对应 EditorTotalAssetBundleInfor. mVersion 资源版本
        public long mConfigBuildTime = 0L; //对应 EditorTotalAssetBundleInfor.mConfigBuildTime  创建这个配置文件的时间(utc);
        public long mTotalSize; //对应 EditorTotalAssetBundleInfor.mTotalSize  总大小

        //key AssetBundle 修改后的名称 Value 当前AssetBundle 的信息
        public Dictionary<string,AssetBundleInfor> mTotalAssetBundleInfor = new Dictionary<string, AssetBundleInfor>(); //总的AssetBundle 信息

        /// <summary>
        /// 判断是否包含指定的AssetBundle
        /// </summary>
        /// <param name="abundleUri"></param>
        /// <returns></returns>
        public bool IsContainABundle(string abundleUri)
        {
            return mTotalAssetBundleInfor.ContainsKey(abundleUri);
        }

    }
}