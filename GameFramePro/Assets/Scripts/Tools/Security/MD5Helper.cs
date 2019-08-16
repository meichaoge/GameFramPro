﻿using System;
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
                if (builder != null)
                    builder.Clear();
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>/// 获取指定绝对路径下文件的MD5 /// </summary>
        public static string GetFileMD5(string filePath, ref long fileSize)
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
                if (builder != null)
                    builder.Clear();
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        /// <summary>/// 获取指定绝对路径下文件的MD5 /// </summary>
        public static byte[] GetFileMD5(string filePath, out string md5Code)
        {
            FileStream fileStream = null;
            StringBuilder builder = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                var data = new byte [fileStream.Length];
                fileStream.Read(data, 0, data.Length);


                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(fileStream);
                builder = new StringBuilder();
                foreach (byte b in result)
                    builder.Append(Convert.ToString(b, 16));

                md5Code = builder.ToString();
                return data;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                md5Code = string.Empty;
                return new byte[0];
            }

            finally
            {
                builder?.Clear();
                fileStream?.Close();
            }
        }


        /// <summary>/// 获取指定Byte数组资源的MD5/// </summary>
        public static string GetFileMD5(byte[] data)
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
    }
}