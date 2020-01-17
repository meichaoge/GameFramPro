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
    public const uint DaySecondCount = 86400;  //每一天的秒数

    #region 时间戳处理
    /// <summary>
    /// 时间戳(秒)转成  DateTime
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
    ///  本地 DateTime 转成 时间戳
    /// </summary>
    /// <param name="timeStamp">时间戳(单位秒)</param>
    /// <returns></returns>
    public static int ToTimestampLocal_Second(this DateTime dateTime)
    {
        //if (utcDateTime.Kind != DateTimeKind.Utc)
        //{
        //    Debug.LogInfor("DateTimeToTimestamp_Second 传入的时间不是UTC 时间，已经自动转换成UTC时间计算");
        //    utcDateTime = utcDateTime.ToUniversalTime();
        //}

        if (dateTime > MaxTimestampBaseTime || dateTime < TimestampBaseTime)
        {
            Debug.LogError(string.Format("格林威治时间只能处理从 {0}  到 {1} 的UTC时间", TimestampBaseTime.ToString(), MaxTimestampBaseTime.ToString()));
            return 0;
        }

        TimeSpan span = dateTime - TimestampBaseTime;
        return (int)(span.TotalSeconds);
    }
    /// <summary>
    /// 现在本地时间转成时间戳
    /// </summary>
    /// <returns></returns>
    public static int NowToTimestampLocal_Second()
    {
        return ToTimestampLocal_Second(DateTime.Now);
    }
    /// <summary>
    /// 时间戳(毫秒)转成  DateTime
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
    ///    本地  DateTime 转成 时间戳
    /// </summary>
    /// <param name="timeStamp">时间戳(单位毫秒)</param>
    /// <returns></returns>
    public static long ToTimestampLocal_Millisecond(this DateTime utcDateTime)
    {

        if (utcDateTime > MaxTimestampBaseTime || utcDateTime < TimestampBaseTime)
        {
            Debug.LogError(string.Format("格林威治时间只能处理从 {0}  到 {1} 的UTC时间", TimestampBaseTime.ToString(), MaxTimestampBaseTime.ToString()));
            return 0;
        }

        TimeSpan span = utcDateTime - TimestampBaseTime;
        return (long)(span.TotalMilliseconds);
    }
    #endregion


    #region DateTime 格式转换
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

    #region 其他类型时间处理
    /// <summary>
    /// 时间秒数转换成分钟和秒
    /// </summary>
    /// <param name="isForeceTwo">是否强制显示两位</param>
    /// <returns></returns>
    public static string SecondFormat_MinusSecond(uint secondCount, bool isForeceTwo = false, string minuteSendSplit = ":")
    {
        if (secondCount >= 3600)
            secondCount = secondCount % 3600; //避免超过1小时的时间

        uint minute = secondCount / 60;
        uint second = secondCount % 60;

        if (isForeceTwo)
            return string.Format("{0:D2}{1}{2:D2}", minute, minuteSendSplit, second);
        return string.Format("{0}{1}{2}", minute, minuteSendSplit, second);
    }

    /// <summary>
    /// 时间秒数转换成分钟和秒
    /// </summary>
    /// <param name="isForeceTwo">是否强制显示两位</param>
    /// <returns></returns>
    public static string SecondFormat_HourMinusSecond(uint secondCount, bool isAutoHideHour = false, bool isForeceTwo = false, string minuteSendSplit = ":")
    {
        if (secondCount >= DaySecondCount)
            secondCount = secondCount % DaySecondCount; //避免超过1天的时间

        uint hour = secondCount / 3600;
        secondCount = secondCount % 3600;

        uint minute = secondCount / 60;
        uint second = secondCount % 60;

        if (isAutoHideHour)
        {
            if (hour == 0)
            {
                if (isForeceTwo)
                    return string.Format("{0:D2}{1}{2:D2}", minute, minuteSendSplit, second);
                return string.Format("{0}{1}{2}", minute, minuteSendSplit, second);
            }
        }

        if (isForeceTwo)
            return string.Format("{0:D2} {1:D2}{2}{3:D2}", hour, minute, minuteSendSplit, second);
        return string.Format("{0} {1}{2}{3}", hour, minute, minuteSendSplit, second);
    }
    #endregion


}
