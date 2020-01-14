using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// DateTime 扩展方法
/// </summary>
public static class DateTime_Ex
{
    public static readonly DateTime TimestampBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc); //基准的格林威治时间
    public static readonly DateTime MaxTimestampBaseTime = TimestampBaseTime.AddSeconds(int.MaxValue); //基准的格林威治时间 最大标示时间

    public static readonly long TimestampBaseTimeTicks = TimestampBaseTime.Ticks; //基准的格林威治时间tickes (1秒=1000*10000 ticks)



    #region 时间戳处理
    /// <summary>
    /// 时间戳(秒)转成 UTC DateTime
    /// </summary>
    /// <param name="timestampSecond">时间戳(单位秒)</param>
    /// <returns></returns>
    public static DateTime TimestampToDateTime(this int timestampSecond)
    {
        DateTime baseTime = new DateTime(TimestampBaseTimeTicks);
        return baseTime.AddSeconds(timestampSecond);
    }

    /// <summary>
    ///  UTC DateTime 转成 时间戳
    /// </summary>
    /// <param name="timeStamp">时间戳(单位秒)</param>
    /// <returns></returns>
    public static int ToTimestamp_Second(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            Debug.LogInfor("DateTimeToTimestamp_Second 传入的时间不是UTC 时间，已经自动转换成UTC时间计算");
            utcDateTime = utcDateTime.ToUniversalTime();
        }

        if (utcDateTime > MaxTimestampBaseTime || utcDateTime < TimestampBaseTime)
        {
            Debug.LogError(string.Format("格林威治时间只能处理从 {0}  到 {1} 的UTC时间", TimestampBaseTime.ToString(), MaxTimestampBaseTime.ToString()));
            return 0;
        }

        TimeSpan span = utcDateTime - TimestampBaseTime;
        return (int)(span.TotalSeconds);
    }

    /// <summary>
    /// 时间戳(毫秒)转成 UTC DateTime
    /// </summary>
    /// <param name="timestampSecond">时间戳(单位毫秒)</param>
    /// <returns></returns>
    public static DateTime TimestampToDateTime(this long timestampMillisecond)
    {
        DateTime baseTime = new DateTime(TimestampBaseTimeTicks);
        return baseTime.AddMilliseconds(timestampMillisecond);
    }

    /// <summary>
    ///  UTC DateTime 转成 时间戳
    /// </summary>
    /// <param name="timeStamp">时间戳(单位毫秒)</param>
    /// <returns></returns>
    public static long ToTimestamp_Millisecond(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
        {
            Debug.LogInfor("DateTimeToTimestamp_Millisecond 传入的时间不是UTC 时间，已经自动转换成UTC时间计算");
            utcDateTime = utcDateTime.ToUniversalTime();
        }

        if (utcDateTime > MaxTimestampBaseTime || utcDateTime < TimestampBaseTime)
        {
            Debug.LogError(string.Format("格林威治时间只能处理从 {0}  到 {1} 的UTC时间", TimestampBaseTime.ToString(), MaxTimestampBaseTime.ToString()));
            return 0;
        }

        TimeSpan span = utcDateTime - TimestampBaseTime;
        return (long)(span.TotalMilliseconds);
    }

    /// <summary>
    ///    本地 UTC DateTime 转成 时间戳
    /// </summary>
    /// <param name="timeStamp">时间戳(单位毫秒)</param>
    /// <returns></returns>
    public static long LocalToTimestamp_Millisecond(this DateTime localDateTime)
    {

        if (localDateTime > MaxTimestampBaseTime || localDateTime < TimestampBaseTime)
        {
            Debug.LogError(string.Format("格林威治时间只能处理从 {0}  到 {1} 的UTC时间", TimestampBaseTime.ToString(), MaxTimestampBaseTime.ToString()));
            return 0;
        }

        TimeSpan span = localDateTime - TimestampBaseTime;
        return (long)(span.TotalMilliseconds);
    }
    #endregion



    #region 格式转换
    /// <summary>
    /// 参数DataTime 裁剪成指定格式(忽略秒以后的数据部分数据)
    /// </summary>
    /// <param name="targetDataTime"></param>
    /// <returns></returns>
    public static DateTime TruncatedDataTime_Second(this DateTime targetDataTime)
    {
        return new DateTime(targetDataTime.Year, targetDataTime.Month, targetDataTime.Day, targetDataTime.Hour, targetDataTime.Minute, targetDataTime.Second, targetDataTime.Kind);
    }

    /// <summary>
    /// 参数DataTime 裁剪成指定格式(忽略分钟以后的数据部分数据)
    /// </summary>
    /// <param name="targetDataTime"></param>
    /// <returns></returns>
    public static DateTime TruncatedDataTime_Minute(this DateTime targetDataTime)
    {
        return new DateTime(targetDataTime.Year, targetDataTime.Month, targetDataTime.Day, targetDataTime.Hour, targetDataTime.Minute, 0, targetDataTime.Kind);
    }

    /// <summary>
    /// 参数DataTime 裁剪成指定格式(忽略分钟以后的数据部分数据)
    /// </summary>
    /// <param name="targetDataTime"></param>
    /// <returns></returns>
    public static DateTime TruncatedDataTime_Hour(this DateTime targetDataTime)
    {
        return new DateTime(targetDataTime.Year, targetDataTime.Month, targetDataTime.Day, targetDataTime.Hour, 0, 0, targetDataTime.Kind);
    }

    /// <summary>
    /// 参数DataTime 裁剪成指定格式(忽略天以后的数据部分数据)
    /// </summary>
    /// <param name="targetDataTime"></param>
    /// <returns></returns>
    public static DateTime TruncatedDataTime_Day(this DateTime targetDataTime)
    {
        return new DateTime(targetDataTime.Year, targetDataTime.Month, targetDataTime.Day, 0, 0, 0, targetDataTime.Kind);
    }
    #endregion

}
