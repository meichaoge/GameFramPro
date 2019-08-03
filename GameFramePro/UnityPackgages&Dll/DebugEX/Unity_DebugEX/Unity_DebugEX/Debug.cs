using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Class containing methods to ease debugging while developing a game.
/// 重载了Untiy引擎自带的Debug
/// 2017年8月24日10:04:13
/// </summary>
///  新增日志级别控制    public static LogLevel S_LogLevel = LogLevel.Log  以及 Infor 级别的日志(LogInfor)     注：当S_LogLevel=S_LogLevel.None时候无日志输出

public static class Debug
{
    /// <summary>
    /// 日志输出级别与DebugVersionEnum 配合使用
    /// </summary>
    public enum LogLevel
    {
        Warning = -1,  //兼容Unity状态 不建议使用
        Assert = 0,
        EditorLog = 1,
        Log = 2,
        Infor = 3,
        Exception = 4,
        Error = 10,
    }

    /// <summary>
    /// 日志输出控制 (Release 版本时候很多Debug 不会输出)
    /// </summary>
    public enum DebugVersionEnum
    {
        Debug,  //测试版本
        Release, //正式版本
    }
    [System.Serializable]
    public class LogColorDefine
    {
        /// <summary>
        /// Infor 级别的日志颜色
        /// </summary>
        public string mInforLevelColor = "#5EAB15FF";
        /// <summary>
        /// 只在UnityEditor下的 Infor 日志颜色
        /// </summary>
        public string mEditorInforLevelColor = "#37D1D8F8";
        /// <summary>
        ///  只在 UnityEditor下的 Error 日志颜色
        /// </summary>
        public string mEditorErroColor = "#FF2D00";
    }

    public static LogLevel S_LogLevel = LogLevel.EditorLog;  //当前使用的日记级别
    public static DebugVersionEnum S_DebugVersion = DebugVersionEnum.Debug;  //当前日志输出级别
    public static LogColorDefine S_LogColorDefine = new LogColorDefine(); //日志颜色配置


    //****下面四个方式只在编辑器下有输出日志
    /// <summary>
    /// 编辑器下输出日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    public static void LogEditorInfor(object message, params object[] args)
    {
#if UNITY_EDITOR
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);

        message = string.Format("[EditorOnly] <color={0}> {1}</color>", S_LogColorDefine.mEditorInforLevelColor, message);
        UnityEngine.Debug.Log(message);
#endif
    }
    /// <summary>
    /// 编辑器下输出日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context"></param>
    public static void LogEditorInfor(object message, UnityEngine.Object context, params object[] args)
    {
#if UNITY_EDITOR
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);

        message = string.Format("[EditorOnly] <color={0}> {1}</color>", S_LogColorDefine.mEditorErroColor, message);
        UnityEngine.Debug.Log(message, context);
#endif
    }


    /// <summary>
    /// 编辑器下输出错误日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    public static void LogEditorError(object message, params object[] args)
    {
#if UNITY_EDITOR
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);

        message = string.Format("[EditorOnly] <color={0}> {1}</color>", S_LogColorDefine.mEditorErroColor, message);
        UnityEngine.Debug.LogError(message);
#endif
    }

    /// <summary>
    /// 编辑器下输出错误日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context"></param>
    public static void LogEditorError(object message, UnityEngine.Object context, params object[] args)
    {
#if UNITY_EDITOR
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);

        message = string.Format("[EditorOnly] <color={0}> {1}</color>", S_LogColorDefine.mEditorErroColor, message);
        UnityEngine.Debug.LogError(message, context);
#endif
    }



    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition)
    {
        if (S_LogLevel > LogLevel.Assert) return;
        UnityEngine.Debug.Assert(condition);
    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, string message)
    {
        if (S_LogLevel > LogLevel.Assert) return;
        UnityEngine.Debug.Assert(condition, message);
    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message)
    {
        if (S_LogLevel > LogLevel.Assert) return;
        UnityEngine.Debug.Assert(condition, message);
    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void Assert(bool condition, object message, UnityEngine.Object context)
    {
        if (S_LogLevel > LogLevel.Assert) return;
        UnityEngine.Debug.Assert(condition, message, context);
    }
    [Conditional("UNITY_ASSERTIONS")]
    [Obsolete("Assert(bool, string, params object[]) is obsolete. Use AssertFormat(bool, string, params object[]) (UnityUpgradable) -> AssertFormat(*)", true)]
    public static void Assert(bool condition, string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Assert) return;
        UnityEngine.Debug.Assert(condition, format, args);
    }



    /// <summary>
    /// 如果第三个参数不为空则会自动调用String.Formate 格式化
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void Log(object message, params object[] args)
    {
        if (S_LogLevel > LogLevel.Log) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        UnityEngine.Debug.Log(message);
    }

    /// <summary>
    /// 如果第三个参数不为空则会自动调用String.Formate 格式化
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context"></param>
    /// <param name="args"></param>
    public static void Log(object message, UnityEngine.Object context, params object[] args)
    {
        if (S_LogLevel > LogLevel.Log) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        UnityEngine.Debug.Log(message, context);
    }

    /// <summary>
    /// 新增 高于Debug级别的日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogInfor(object message, params object[] args)
    {
        if (S_LogLevel > LogLevel.Infor) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        message = string.Format("<color={0}> {1}</color>", S_LogColorDefine.mEditorInforLevelColor, message);
        UnityEngine.Debug.Log(message);
    }
    /// <summary>
    /// 新增 高于Debug级别的日志 (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context"></param>
    /// <param name="args"></param>
    public static void LogInfor(object message, UnityEngine.Object context, params object[] args)
    {
        if (S_LogLevel > LogLevel.Infor) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        message = string.Format("<color={0}> {1}</color>", S_LogColorDefine.mEditorInforLevelColor, message);
        UnityEngine.Debug.Log(message, context);
    }




    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogAssertion(message);
    }
    [Conditional("UNITY_ASSERTIONS")]
    public static void LogAssertion(object message, UnityEngine.Object context)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogAssertion(message, context);
    }
    [Conditional("UNITY_ASSERTIONS")]
    [Obsolete("使用这个格式化日志会导致堆栈信息无法点击跳转,请使用  其他的接口 ")]
    public static void LogAssertionFormat(string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogAssertionFormat(format, args);
    }
    [Conditional("UNITY_ASSERTIONS")]
    [Obsolete("使用这个格式化日志会导致堆栈信息无法点击跳转,请使用  其他的接口 ")]
    public static void LogAssertionFormat(UnityEngine.Object context, string format)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogAssertionFormat(context, format);
    }

    /// <summary>
    /// 错误日志  (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public static void LogError(object message, params object[] args)
    {
        if (S_LogLevel > LogLevel.Error) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(UnityEngine.Object message)
    {
        if (S_LogLevel > LogLevel.Error) return;
        UnityEngine.Debug.LogError(message);
    }

    /// <summary>
    /// 错误日志  (如果第三个参数不为空则会自动调用String.Formate 格式化)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="context"></param>
    /// <param name="args"></param>
    public static void LogError(object message, UnityEngine.Object context, params object[] args)
    {
        if (S_LogLevel > LogLevel.Error) return;
        if (args != null && message != null)
            message = string.Format(message.ToString(), args);
        UnityEngine.Debug.LogError(message, context);
    }

    [Obsolete("使用这个格式化日志会导致堆栈信息无法点击跳转,请使用  LogError ")]
    public static void LogErrorFormat(string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Error) return;
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    [Obsolete("使用这个格式化日志会导致堆栈信息无法点击跳转,请使用  LogError ")]
    public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Error) return;
        UnityEngine.Debug.LogErrorFormat(context, format, args);
    }

    public static void LogException(Exception exception)
    {
        if (S_LogLevel > LogLevel.Exception) return;
        UnityEngine.Debug.LogException(exception);
    }

    public static void LogException(Exception exception, UnityEngine.Object context)
    {
        if (S_LogLevel > LogLevel.Exception) return;
        UnityEngine.Debug.LogException(exception, context);
    }


    public static void LogFormat(string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogFormat(format, args);
    }

    public static void LogFormat(UnityEngine.Object context, string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Log) return;
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    public static void LogWarning(object message)
    {
        if (S_LogLevel > LogLevel.Warning) return;
        UnityEngine.Debug.LogWarning(message);
    }

    public static void LogWarning(object message, UnityEngine.Object context)
    {
        if (S_LogLevel > LogLevel.Warning) return;
        UnityEngine.Debug.LogWarning(message, context);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Warning) return;
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args)
    {
        if (S_LogLevel > LogLevel.Warning) return;
        UnityEngine.Debug.LogWarningFormat(context, format, args);
    }

    #region Unity 编辑器下的绘制
#if UNITY_EDITOR
    public static void DrawLine(Vector3 start, Vector3 end, Color color)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawLine(start, end, color);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawLine(start, end, color, duration);
#endif
    }


    public static void DrawLine(Vector3 start, Vector3 end)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawLine(start, end);
#endif
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0f, bool depthTest = true)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
#endif
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration = 0f, bool depthTest = true)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
#endif
    }
    public static void DrawRay(Vector3 start, Vector3 dir)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawRay(start, dir);
#endif
    }
    public static void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawRay(start, dir, color);
#endif
    }

    public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.DrawRay(start, dir, color, duration);
#endif
    }
#endif

    #endregion


}
