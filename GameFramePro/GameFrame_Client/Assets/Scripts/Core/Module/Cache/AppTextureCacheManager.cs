using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


        internal Dictionary<string, CacheTextureInfor> AllCacheTexturesInfor = new Dictionary<string, CacheTextureInfor>(10);


        #region ICacheManager 接口实现

        public bool TryCache(byte[] data, string cacheRelativePath, uint maxCacheTime = 0)
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

            CacheTextureInfor textureInfor = new CacheTextureInfor(data, cacheRelativePath, maxCacheTime);
            SaveCacheTexture(textureInfor);
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
                    DeleteCacheTexture(record);
                    return null;
                }
            }

            string fileRealPath = $"{S_TextureCacheTopDirectory}{textureRelativePath}";
            CacheTextureInfor localData = GetLocalTextureInfor(fileRealPath);
            if (localData.IsAvailable)
            {
                AddOrUpdateCacheTextureRecord(localData);
                return localData.TextureData;
            }
            else
            {
                DeleteCacheTexture(localData);
                return null;
            }
        }

        #endregion


        #region 内部实现

        /// <summary>/// 删除缓存的图片/// </summary>
        private void DeleteCacheTexture(CacheTextureInfor textureInfor)
        {
            if (textureInfor == null)
            {
                Debug.LogError("DeleteCacheTexture Fail,Parameter is null");
                return;
            }

            RemoveCacheTextureRecord(textureInfor.TextureRealPath);

            IOUtility.DeleteFile(textureInfor.TextureRealPath);
        }

        /// <summary>/// 保存缓存的图片/// </summary>
        private void SaveCacheTexture(CacheTextureInfor textureInfor)
        {
            if (textureInfor == null)
            {
                Debug.LogError("SaveCacheTexture Fail,Parameter is null");
                return;
            }

            AddOrUpdateCacheTextureRecord(textureInfor);

            //  string fileRealPath = $"{S_TextureCacheTopDirectory}{textureInfor.mTextureRelativePath}";
            if (System.IO.File.Exists(textureInfor.TextureRealPath))
            {
                var localTextureInfor = GetLocalTextureInfor(textureInfor.TextureRealPath);
                if (localTextureInfor.mTextureMd5Code != textureInfor.mTextureMd5Code)
                {
                    DeleteCacheTexture(localTextureInfor);
                    IOUtility.CreateOrSetFileContent(textureInfor.TextureRealPath, textureInfor.TextureData, false);
                }
            }
            else
            {
                IOUtility.CreateOrSetFileContent(textureInfor.TextureRealPath, textureInfor.TextureData, false);
            }
        }


        /// <summary>/// 增加图片缓存记录/// </summary>
        private void AddOrUpdateCacheTextureRecord(CacheTextureInfor textureInfor)
        {
            if (AllCacheTexturesInfor.TryGetValue(textureInfor.mTextureRelativePath, out CacheTextureInfor record))
            {
                record = textureInfor;
                return;
            }

            AllCacheTexturesInfor[textureInfor.mTextureRelativePath] = textureInfor;
        }

        /// <summary>/// 移除图片缓存记录/// </summary>
        private void RemoveCacheTextureRecord(string cacheRelativePath)
        {
            if (AllCacheTexturesInfor.TryGetValue(cacheRelativePath, out CacheTextureInfor record))
            {
                AllCacheTexturesInfor.Remove(cacheRelativePath);
                return;
            }
        }


        /// <summary>/// 获取本地指定路径上图片资源的信息/// </summary>
        private CacheTextureInfor GetLocalTextureInfor(string fileRealPath)
        {
            if (string.IsNullOrEmpty(fileRealPath))
            {
                Debug.LogError("GetLocalTextureInfor，指定参数cacheRelativePath 为null");
                return null;
            }

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
            public uint mTextureAliveTime = 0;
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
                    if (mTextureAliveTime == 0) return true;
                    return (DateTime.UtcNow - LastModifyTime).TotalSeconds < mTextureAliveTime;
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
                mTextureAliveTime = 0;
            }

            public CacheTextureInfor(byte[] data, string cacheRelativePath, uint maxCacheTime = 0) : this()
            {
                TextureData = data;
                mTextureRelativePath = cacheRelativePath;
                mTextureAliveTime = maxCacheTime;
                mTextureMd5Code = MD5Helper.GetByteDataMD5(data);
            }
        }
    }
}
