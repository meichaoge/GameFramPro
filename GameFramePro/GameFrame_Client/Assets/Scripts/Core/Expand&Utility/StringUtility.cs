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
  
        /// <summary>
        /// 将参数字符串按照 split拆成指定的分数，并且当拆分后的数据不足时候自己
        /// </summary>
        /// <param name="targetStr"></param>
        /// <param name="split"></param>
        /// <param name="exceptCount">当这个值=0时候isAutoInitialed 无效，这个接口等同于string.Split()</param>
        /// <param name="isAutoInitialed"></param>
        /// <returns></returns>
        public static string[] SplitString(this string targetStr, char split, uint exceptCount = 0, bool isAutoInitialed = false)
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
                    int copyLength = Mathf.Min((int) exceptCount, temp.Length);
                    System.Array.Copy(temp, 0, result, 0, copyLength);
                }
            }
            else
            {
                result = targetStr.Split(split);
            }

            return result;
        }

        /// <summary>
        /// 将字符串按照换行符切割
        /// </summary>
        /// <param name="isRemoveAllEmptyLine">标识是否自动过滤空行</param>
        /// <returns></returns>
        public static string[] SplitStringsToLines(this string targetStr, bool isRemoveAllEmptyLine = false)
        {
            if (string.IsNullOrEmpty(targetStr))
            {
                Debug.LogError($"给定的字符串为null");
                return new string[0];
            }

            //得到的行偶数行是分隔符空行需要过滤
            string[] result = targetStr.Split(System.Environment.NewLine.ToCharArray());

            List<string> allLines = new List<string>(result.Length);
            int index = 0;
            foreach (var line in result)
            {
                ++index;
                if (index % 2 == 0)
                    continue;

                if (isRemoveAllEmptyLine && String.IsNullOrEmpty(line))
                    continue;

                allLines.Add(line);
            }

            return allLines.ToArray();
        }
    }
}