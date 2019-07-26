using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace GameFramePro
{
    /// <summary>
    /// 辅助String 操作
    /// </summary>
    public static class StringUtility
    {
        #region StringBuilder 对象池 避免不断的创建和销毁

        private static NativeObjectPool<StringBuilder> s_StringBuilderPool = new NativeObjectPool<StringBuilder>(5, builder => { builder.Clear(); }, null);
        /// <summary>
        /// 获取一个 StringBuilder 对象
        /// </summary>
        /// <returns></returns>
        public static StringBuilder GetStringBuilder()
        {
            return s_StringBuilderPool.GetItemFromPool();
        }
        /// <summary>
        /// 回收获取的 StringBuilder
        /// </summary>
        /// <param name="builder"></param>
        public static void ReleaseStringBuilder(StringBuilder builder)
        {
            s_StringBuilderPool.RecycleItemToPool(builder);
        }


        private static NativeObjectPool<List<StringBuilder>> s_StringBuilderListPool = new NativeObjectPool<List<StringBuilder>>(5, null,null);
        /// <summary>
        /// 获取一个 StringBuilder 对象
        /// </summary>
        /// <returns></returns>
        public static List<StringBuilder> GetStringBuilderList()
        {
            return s_StringBuilderListPool.GetItemFromPool();
        }
        /// <summary>
        /// 回收获取的 StringBuilder
        /// </summary>
        /// <param name="builder"></param>
        public static void ReleaseStringBuilderList(List<StringBuilder> builder)
        {
            foreach (var item in builder)
                ReleaseStringBuilder(item);
            builder.Clear();

            s_StringBuilderListPool.RecycleItemToPool(builder);
        }


        #endregion

        #region List<String> 对象池
        private static NativeObjectPool<List<string>> s_ListStringPool = new NativeObjectPool<List<string>>(5, stringList =>
        {
            if (stringList.Count != 0)
                stringList.Clear();
        }, null);


        #endregion


        /// <summary>
        /// 将参数字符串按照 split拆成指定的分数，并且当拆分后的数据不足时候自己
        /// </summary>
        /// <param name="targetStr"></param>
        /// <param name="split"></param>
        /// <param name="exceptCount">当这个值=0时候isAutoInitialed 无效，这个接口等同于string.Split()</param>
        /// <param name="isAutoInitialed"></param>
        /// <returns></returns>
        public static string[] SplitString(this string targetStr,char split,uint exceptCount=0,bool isAutoInitialed=false)
        {
            if (exceptCount == 0)
                return targetStr.Split(split);

            string[] result = null;
            if (isAutoInitialed)
            {
               result = new string[exceptCount];
                var temp = targetStr.Split(split);
                if (targetStr.Length != 0)
                {
                    int copyLength = Mathf.Min((int)exceptCount, temp.Length);
                    System.Array.Copy(temp, 0, result, 0, copyLength);
                }
            }
            else
            {
                result= targetStr.Split(split);
            }
            return result;
        }

    }
}