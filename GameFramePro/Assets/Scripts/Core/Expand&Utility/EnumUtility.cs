using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 扩展常用的Enum 类型
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// 获取一个枚举所有定义的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="targettEnum"></param>
        /// <returns></returns>
        public static List<T> GetEnumValue<T>() where T : Enum
        {
            var values = System.Enum.GetValues(typeof(T));
            List<T> enumValues = new List<T>(values.Length);
            foreach (var item in values)
                enumValues.Add((T)(Enum.Parse(typeof(T), item.ToString())));
            return enumValues;
        }


        //public List<T>GetFlagEnum<T>(this T targetEnum,T flagEnum) where T : Enum
        //{
        //    List<T> enumValues = GetEnumValue<T>(targetEnum);
        //    List<T> result = new List<T>();
        //    for (int dex = 0; dex < enumValues.Count; dex++)
        //    {
        //        if (((Enum)flagEnum & (Enum)enumValues[dex]) == enumValues[dex])
        //            result.Add(enumValues[dex]);
        //    }
        //}



    }
}