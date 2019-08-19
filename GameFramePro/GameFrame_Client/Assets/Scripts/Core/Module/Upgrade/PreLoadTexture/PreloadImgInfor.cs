using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace GameFramePro.Upgrade
{
    /// <summary>/// 包含需要提前加载的资源信息/// </summary>
    [System.Serializable]
    public class PreloadImgInfor
    {
        public string mTextureRelativePath;
        public string mAssetMD5;

        /// <summary>///资源大小 (byte)/// </summary>
        public int mAssetSize;

        public PreloadImgInfor()
        {
        }
    }


    /// <summary>/// 所有需要预加载的活动图片配置/// </summary>
    [System.Serializable]
    public class PreloadImgConfigInfor
    {
        /// <summary>/// 版本号/// </summary>
        public string Version = string.Empty;
        public int TotalSize; //资源总大小
        public UpgradeLanguage mLanguage;
        public Dictionary<string, PreloadImgInfor> AllPreloadImgConfig = new Dictionary<string, PreloadImgInfor>();

        public PreloadImgConfigInfor()
        {
        }

        public void GetTotalSize()
        {
            TotalSize = 0;
            foreach (var item in AllPreloadImgConfig.Values)
                TotalSize += item.mAssetSize;
        }

        /// <summary>/// 显示总大小/// </summary>
        public string GetTotalSizeForShow()
        {
            GetTotalSize();
            return IOUtility.ByteConversionOthers(TotalSize);
        }
    }
}
