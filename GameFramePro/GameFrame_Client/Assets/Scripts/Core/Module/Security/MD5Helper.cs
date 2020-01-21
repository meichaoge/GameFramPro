using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 获取文件MD5码的帮助类/// </summary>
    public static class MD5Helper
    {
        /// <summary>/// 获取指定绝对路径下文件的MD5 /// </summary>
        public static string GetFileMD5(string filePath)
        {
            if (System.IO.File.Exists(filePath) == false)
                return string.Empty;
            FileStream fileStream = null;
            StringBuilder builder = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileStream);
                builder = new StringBuilder();
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
                builder?.Clear();
                fileStream?.Close();
            }
        }

        /// <summary>/// 获取指定绝对路径下文件的MD5 /// </summary>
        public static string GetFileMD5OutLength(string filePath, out long fileSize)
        {
            fileSize = 0;
            if (System.IO.File.Exists(filePath) == false)
                return string.Empty;
            FileStream fileStream = null;
            StringBuilder builder = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileSize = fileStream.Length;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileStream);
                builder = new StringBuilder();
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
                builder?.Clear();
                fileStream?.Close();
            }
        }

        /// <summary>/// 获取指定绝对路径下文件的MD5 /// </summary>
        public static string GetFileMD5OutData(string filePath, out byte[] fileData)
        {
            fileData = null;
            if (System.IO.File.Exists(filePath) == false)
                return string.Empty;
            FileStream fileStream = null;
            StringBuilder builder = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileData = new byte [fileStream.Length];
                fileStream.Read(fileData, 0, fileData.Length);

                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileData); //这里如果直接读取流文件获取的MD5 不一致TODO
                builder = new StringBuilder();
                foreach (byte b in result)
                    builder.Append(Convert.ToString(b, 16));


                return builder.ToString();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return String.Empty;
            }

            finally
            {
                builder?.Clear();
                fileStream?.Close();
            }
        }


        /// <summary>/// 获取指定Byte数组资源的MD5/// </summary>
        public static string GetByteDataMD5(byte[] data)
        {
            if (data == null || data.Length == 0)
                return string.Empty;

            MD5 md5 = new MD5CryptoServiceProvider();
            var builder = new StringBuilder();
            byte[] result = md5.ComputeHash(data);
            foreach (byte b in result)
                builder.Append(Convert.ToString(b, 16));

            return builder.ToString();
        }


        /// <summary>
        /// 获取指定文本的MD5
        /// </summary>
        /// <param name="data"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetStringContentMD5(string data,Encoding encoding)
        {
            return GetByteDataMD5(encoding.GetBytes(data));
        }
    }
}
