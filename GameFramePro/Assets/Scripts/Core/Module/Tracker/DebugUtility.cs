using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text;

namespace GameFramePro.AnalysisEx
{
    /// <summary>/// 用于辅助 输出日志功能 /// </summary>
    public static class DebugUtility
    {
        /// <summary>/// 获取当前的调用堆栈信息/// </summary>
        public static string GetCurStackTraceInfor()
        {
            if (Application.isPlaying == false)
                return String.Empty;

            StackTrace stackTrace = new System.Diagnostics.StackTrace(true);
            StackFrame[] frames = stackTrace.GetFrames();
            if (frames == null || frames.Length == 0)
                return String.Empty;

            StringBuilder builder = StringUtility.GetStringBuilder();
            Dictionary<string, string> fileNamePathMap = new Dictionary<string, string>();
            for (int index = 0; index < frames.Length; index++)
            {
                if (index == 0) continue; //过滤掉 GetCurStackTraceInfor 当前这个接口的函数调用堆栈
                var targetFrame = frames[index];
                string filePath = targetFrame.GetFileName();

                if (fileNamePathMap.TryGetValue(filePath, out string scriptRelativePath) == false)
                {
                    scriptRelativePath = IOUtility.GetPathFromSpecialDirectoryName(filePath, "Assets"); // 脚本相对路径
                    fileNamePathMap[filePath] = scriptRelativePath;
                }


                builder.Append($"{scriptRelativePath} : {targetFrame.GetMethod().Name}()  (at Line{targetFrame.GetFileLineNumber()})  \r\n ");
            }

            string stackTraceInfor = builder.ToString();
            StringUtility.ReleaseStringBuilder(builder);

            return stackTraceInfor;
        }
    }
}
