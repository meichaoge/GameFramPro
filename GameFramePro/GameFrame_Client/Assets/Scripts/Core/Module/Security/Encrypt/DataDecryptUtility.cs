using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>
    /// 数据加解密
    /// </summary>
    public static class DataDecryptUtility
    {
        private static string sKEY = "ZTdkNTNmNDE2NTM3MWM0NDFhNTEzNzU1";
        private static string sIV = "4rZymEMfa/PpeJ89qY4gyA==";



        /// <summary>
        /// 获取 key 和IV 
        /// </summary>
        /// <param name="blockSize">取值范围 128/192/256</param>
        /// <param name="KeySize"></param>
        public static void GetKeyAndIV(int blockSize = 256, int KeySize = 128)
        {
            using (RijndaelManaged myRijndael = new RijndaelManaged())
            {
                myRijndael.BlockSize = blockSize;
                myRijndael.KeySize = KeySize;

                myRijndael.GenerateKey();
                myRijndael.GenerateIV();

                Debug.Log($"BlockSize={  myRijndael.BlockSize }  KeySize={  myRijndael.KeySize } ");

                Debug.Log("Key=" + Convert.ToBase64String(myRijndael.Key));
                Debug.Log("IV=" + Convert.ToBase64String(myRijndael.IV));
            }
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encString"></param>
        /// <returns></returns>
        public static string Decrypt(string encString)
        {
            try
            {
                RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    Padding = PaddingMode.Zeros,
                    Mode = CipherMode.CBC,
                    KeySize = 128,
                    BlockSize = 128
                };
                byte[] bytes = Encoding.UTF8.GetBytes(sKEY);
                byte[] rgbIV = Convert.FromBase64String(sIV);
                ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, rgbIV);
                byte[] array = Convert.FromBase64String(encString);
                byte[] array2 = new byte[array.Length];
                MemoryStream stream = new MemoryStream(array);
                CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                cryptoStream.Read(array2, 0, array2.Length);
                return Encoding.UTF8.GetString(array2).TrimEnd(new char[1]);
            }
            catch (Exception e)
            {
                Debug.LogError($"Decrypt Fail!  {e}");
            }

            return string.Empty;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="rawString"></param>
        /// <returns></returns>
        public static string Encrypt(string rawString)
        {
            try
            {
                RijndaelManaged rijndaelManaged = new RijndaelManaged
                {
                    Padding = PaddingMode.Zeros,
                    Mode = CipherMode.CBC,
                    KeySize = 128,
                    BlockSize = 128
                };
                byte[] bytes = Encoding.UTF8.GetBytes(sKEY);
                byte[] rgbIV = Convert.FromBase64String(sIV);
                ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, rgbIV);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
                byte[] bytes2 = Encoding.UTF8.GetBytes(rawString);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                byte[] inArray = memoryStream.ToArray();
                return Convert.ToBase64String(inArray);
            }
            catch (Exception e)
            {
                Debug.LogError($"Encrypt Fail!  {e}");
            }
            return string.Empty;
        }


    }
}