using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 集合类型扩展
    /// </summary>
    public static class CollectionExpand
    {
        /// <summary>
        /// 从字典中删除所有满足条件的项
        /// </summary>
        /// <param name="targetDictionary"></param>
        /// <param name="match"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        public static Dictionary<T, V> RemoveAll<T, V>(this Dictionary<T, V> targetDictionary, Predicate<KeyValuePair<T, V>> match)
        {
            if (targetDictionary == null || targetDictionary.Count == 0 || match == null)
                return null;
            Dictionary<T, V> tempDictionary = new Dictionary<T, V>(targetDictionary);
            Dictionary<T, V> resultDictionary = new Dictionary<T, V>(targetDictionary.Count);
            foreach (var item in tempDictionary)
            {
                if (match.Invoke(item))
                {
                    resultDictionary[item.Key] = item.Value;
                    targetDictionary.Remove(item.Key);
                }
            }

            return resultDictionary;
        }
    }
}