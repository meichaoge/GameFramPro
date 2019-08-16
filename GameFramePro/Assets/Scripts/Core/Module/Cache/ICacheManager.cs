using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.CacheEx
{
    public interface ICacheManager
    {
        /// <summary>/// 缓存一个对象 /// </summary>
        /// <param name="maxCacheTime">最长缓存时间，超过时间自动清理(单位秒)=0 标示不会过期</param>
        bool TryCache(byte[] data, string cacheRelativePath, uint maxCacheTime = 0);

        /// <summary>/// 取出一个缓存对象/// </summary>
        byte[] GetDataFromCache(string cacheRelativePath);
    }
}
