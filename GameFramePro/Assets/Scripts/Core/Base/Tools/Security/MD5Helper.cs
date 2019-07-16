using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 获取文件MD5码的帮助类
    /// </summary>
    public static class MD5Helper
    {
        /// <summary>
        /// 获取指定绝对路径下文件的MD5 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileMD5(string filePath)
        {
            FileStream fileStream = null;
            StringBuilder builder = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileStream);
                builder = StringUtility.GetStringBuilder();
                foreach (byte b in result)
                    builder.Append(Convert.ToString(b, 16));

                return builder.ToString();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return string.Empty;
            }

            finally
            {
                if (builder != null)
                    StringUtility.ReleaseStringBuilder(builder);
                if (fileStream != null)
                    fileStream.Close();
            }

        }

        /// <summary>
        /// 获取指定绝对路径下文件的MD5 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileSize">文件大小</param>
        /// <returns></returns>
        public static string GetFileMD5(string filePath,ref long fileSize)
        {
            FileStream fileStream = null;
            StringBuilder builder = null;
            fileSize = 0;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileSize = fileStream.Length;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileStream);
                builder = StringUtility.GetStringBuilder();
                foreach (byte b in result)
                    builder.Append(Convert.ToString(b, 16));

                return builder.ToString();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return string.Empty;
            }

            finally
            {
                if (builder != null)
                    StringUtility.ReleaseStringBuilder(builder);
                if (fileStream != null)
                    fileStream.Close();
            }

        }


    }
}
