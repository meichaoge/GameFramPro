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
    public static int DateTimeToTimestamp_Second(this DateTime utcDateTime)
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
    public static long DateTimeToTimestamp_Millisecond(this DateTime utcDateTime)
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
    #endregion


}
