using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using GameFramePro.NetWorkEx;

namespace GameFramePro.CacheEx
{


    /// <summary>/// 图片缓存管理器/// </summary>
    public partial class AppTextureCacheManager : Single<AppTextureCacheManager>, ICacheManager
    {
        private static string s_TextureCacheTopDirectory = String.Empty;

        /// <summary>/// 缓存图片的顶层目录/// </summary>
        public static string S_TextureCacheTopDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(s_TextureCacheTopDirectory))
                    s_TextureCacheTopDirectory = $"{Application.persistentDataPath}/{ConstDefine.S_LocalStoreDirectoryName}/TextureCache/";
                return s_TextureCacheTopDirectory;
            }
        }


        internal static Dictionary<string, CacheTextureInfor> AllCacheTexturesInfor = new Dictionary<string, CacheTextureInfor>(10);


        #region ICacheManager 接口实现

        public bool TryCache(byte[] data, string cacheRelativePath, uint maxCacheTimeSecond = 0)
        {
            if (string.IsNullOrEmpty(cacheRelativePath))
            {
                Debug.LogError("缓存图片失败，指定参数cacheRelativePath 为null");
                return false;
            }

            if (data == null || data.Length == 0)
            {
                Debug.LogError("缓存图片失败，图片数据为null");
                return false;
            }


            SaveCacheTexture(data, cacheRelativePath, maxCacheTimeSecond);
            return true;
        }

        public byte[] GetDataFromCache(string textureRelativePath)
        {
            if (string.IsNullOrEmpty(textureRelativePath))
            {
                Debug.LogError("获取缓存图片失败，指定参数cacheRelativePath 为null");
                return null;
            }

            if (AllCacheTexturesInfor.TryGetValue(textureRelativePath, out CacheTextureInfor record))
            {
                if (record.IsAvailable)
                    return record.TextureData;
                else
                {
                    DeleteCacheTexture(record.mTextureRelativePath);
                    return null;
                }
            }

            string fileRealPath = $"{S_TextureCacheTopDirectory}{textureRelativePath}";
            CacheTextureInfor localData = GetLocalTextureInfor(fileRealPath);

            if (localData == null) return null;

            if (localData.IsAvailable)
            {
                AddOrUpdateCacheTextureRecord(localData);
                return localData.TextureData;
            }
            else
            {
                DeleteCacheTexture(localData.mTextureRelativePath);
                return null;
            }
        }

        #endregion


        #region 内部实现

        /// <summary>/// 删除缓存的图片/// </summary>
        private static void DeleteCacheTexture(string textureReaPath)
        {
            if (string.IsNullOrEmpty(textureReaPath))
            {
                Debug.LogError("DeleteCacheTexture Fail,Parameter is null");
                return;
            }

            RemoveCacheTextureRecord(textureReaPath);

            IOUtility.DeleteFile(textureReaPath);
        }

        /// <summary>/// 保存缓存的图片/// </summary>
        private static void SaveCacheTexture(byte[] data, string cacheRelativePath, uint maxCacheTimeSecond = 0)
        {
            CacheTextureInfor textureInfor = new CacheTextureInfor(data, cacheRelativePath, maxCacheTimeSecond);
            if (textureInfor == null)
            {
                Debug.LogError("SaveCacheTexture Fail,Parameter is null");
                return;
            }

            if (AddOrUpdateCacheTextureRecord(textureInfor))
            {
                if (System.IO.File.Exists(textureInfor.TextureRealPath))
                    DeleteCacheTexture(textureInfor.mTextureRelativePath);

                IOUtility.CreateOrSetFileContent(textureInfor.TextureRealPath, textureInfor.TextureData, false);
            }
        }


        /// <summary>/// 增加图片缓存记录/// </summary>
        private static bool AddOrUpdateCacheTextureRecord(CacheTextureInfor textureInfor)
        {
            if (AllCacheTexturesInfor.TryGetValue(textureInfor.mTextureRelativePath, out CacheTextureInfor record))
            {
                if (record == null || record.mTextureMd5Code != textureInfor.mTextureMd5Code)
                {
                    record = textureInfor;
                    return true;
                }
                return false;
            }

            AllCacheTexturesInfor[textureInfor.mTextureRelativePath] = textureInfor;
            return true;
        }

        /// <summary>/// 移除图片缓存记录/// </summary>
        private static void RemoveCacheTextureRecord(string cacheRelativePath)
        {
            if (AllCacheTexturesInfor.TryGetValue(cacheRelativePath, out CacheTextureInfor record))
            {
                AllCacheTexturesInfor.Remove(cacheRelativePath);
                return;
            }
        }


        /// <summary>/// 获取本地指定路径上图片资源的信息/// </summary>
        private static CacheTextureInfor GetLocalTextureInfor(string fileRealPath)
        {
            if (string.IsNullOrEmpty(fileRealPath))
            {
                Debug.LogError("GetLocalTextureInfor，指定参数cacheRelativePath 为null");
                return null;
            }
            if (System.IO.File.Exists(fileRealPath) == false)
                return null;

            CacheTextureInfor textureInfor = new CacheTextureInfor(fileRealPath);

            return textureInfor;
        }

        #endregion
    }


    public partial class AppTextureCacheManager
    {
        /// <summary>/// 缓存的图片信息/// </summary>
        internal class CacheTextureInfor
        {
            public string mTextureRelativePath; //图片相对路径

            /// <summary>/// 真实的存储路径/// </summary>
            public string TextureRealPath => $"{S_TextureCacheTopDirectory}{mTextureRelativePath}";

            public byte[] TextureData { get; private set; }



            public uint mTextureAliveTimeSecond = 0;
            public DateTime LastModifyTime;

            public int mTextureSize
            {
                get { return TextureData == null ? 0 : TextureData.Length; }
            }

            public string mTextureMd5Code { get; private set; }

            /// <summary>/// 判断缓存资源是否过期/// </summary>
            public bool IsAvailable
            {
                get
                {
                    if (TextureData == null || TextureData.Length == 0) return false;
                    if (mTextureAliveTimeSecond == 0) return true;
                    return (DateTime.UtcNow - LastModifyTime).TotalSeconds < mTextureAliveTimeSecond;
                }
            }

            public CacheTextureInfor()
            {
                LastModifyTime = DateTime.UtcNow;
            }

            public CacheTextureInfor(string fileRealPath) : this()
            {
                mTextureRelativePath = fileRealPath;

                string md5Code = MD5Helper.GetFileMD5OutData(fileRealPath, out var fileData);
                TextureData = fileData;
                mTextureAliveTimeSecond = 0;
            }

            public CacheTextureInfor(byte[] data, string cacheRelativePath, uint maxCacheTimeSecond = 0) : this()
            {
                TextureData = data;
                mTextureRelativePath = cacheRelativePath;
                mTextureAliveTimeSecond = maxCacheTimeSecond;
                mTextureMd5Code = MD5Helper.GetByteDataMD5(data);
            }
        }
    }



}
