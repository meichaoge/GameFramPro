using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace GameFramePro
{

    /// <summary>
    /// 长度为 2048 的全局共享复用Byte[] 数组
    /// </summary>
    [System.Serializable]
    public class ByteArray
    {
        public const int S_ByteLength = 2048; //表示每个 byte[] 数组的长度

        /// <summary>  
        /// 不要重定向这个数组，只能在里面读写数据   
        /// </summary>
        public byte[] mBytes { get; private set; }

        public int mDataRealLength { get; set; } //数据的真实长度

#if UNITY_EDITOR
        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int dex = 0; dex < mDataRealLength; dex++)
            {
                builder.Append(mBytes[dex]);
                builder.Append("-");
            }

            return builder.ToString();
        }
#endif

        #region ByteArray 对象池 接口

        private static readonly ConcurrentQueue<ByteArray> mByteArrayQueue = new ConcurrentQueue<ByteArray>(); //复用的Socket byte[] 数组


        private static object S_GetByteLocker = new object();

        /// <summary>      /// 获取一个 ByteArray 对象   /// </summary>
        public static ByteArray GetByteArray()
        {
            lock (S_GetByteLocker)
            {
                if (mByteArrayQueue.Count == 0)
                    return new ByteArray();

                if (mByteArrayQueue.TryDequeue(out var byteArray))
                {
                    byteArray.ClearByteArray();
                    return byteArray;
                }

                return new ByteArray();
            }
        }

        /// <summary>       /// 回收  ByteArray 对象      /// </summary>
        public static void RecycleByteArray(ByteArray byteArray)
        {
            if (byteArray == null || byteArray.mBytes == null)
                throw new ArgumentNullException($"参数 byteArray或者 byteArray.mByte 为null ");

            if (byteArray.mBytes.Length != S_ByteLength)
            {
                Debug.LogError($"请不要修改 byteArray.mBytes 数组的长度");
                return;
            }

            mByteArrayQueue.Enqueue(byteArray);
        }

        #endregion

        #region 构造函数

        public ByteArray()
        {
            mBytes = new byte[ByteArray.S_ByteLength];
            mDataRealLength = 0;
        }

        #endregion

        #region 获取需要设置的数据源

        /// <summary>/// 从同类型数据上克隆数据/// </summary>
        public void CloneFromByteArray(ByteArray sourcesByteArray)
        {
            if (sourcesByteArray == null)
            {
                global::Debug.LogError($"参数sourcesArrary 为null");
                return;
            }

            System.Array.Copy(sourcesByteArray.mBytes, 0, mBytes, 0, sourcesByteArray.mDataRealLength);
            mDataRealLength = sourcesByteArray.mDataRealLength;
        }


        /// <summary>
        /// 从其他的Byte数组中获取指定的数据源   
        /// </summary>
        /// <param name="sourcesArrary">源数据</param>
        /// <param name="sourcesStartIndex">开始复制的起点</param>
        /// <param name="sourcesCopyLength">复制的数据长度</param>
        /// <param name="destinationStartIndex">目标保存的起点</param>
        public void CopyBytes(System.Array sourcesArrary, int sourcesStartIndex, int sourcesCopyLength, int destinationStartIndex)
        {
            if (sourcesArrary == null)
            {
                Debug.LogError($"参数sourcesArrary 为null");
                return;
            }

            if (sourcesCopyLength >= S_ByteLength)
            {
                Debug.LogError($"最多允许{S_ByteLength} 长度，实际参数值为{sourcesCopyLength}");
                return;
            }

            if (sourcesStartIndex < 0 || sourcesStartIndex >= sourcesArrary.Length || sourcesCopyLength > sourcesArrary.Length)
            {
                Debug.LogError($"参数越界{sourcesArrary.Length} sourcesStartIndex= {sourcesStartIndex} sourcesCopyLength={sourcesCopyLength}");
            }

            Array.Copy(sourcesArrary, sourcesStartIndex, mBytes, destinationStartIndex, sourcesCopyLength);
            mDataRealLength = sourcesCopyLength;
        }

        /// <summary>/// 
        /// 根据指定内容 按照指定的编码格式编码得到结果
        /// // </summary>
        public void EncodingGetBytes(string message, Encoding encoding, int byteStartIndex = 0)
        {
            if (message == null)
                message = string.Empty;
            encoding.GetBytes(message.ToCharArray(), 0, message.Length, mBytes, byteStartIndex);
            mDataRealLength = message.Length;
        }

        /// <summary>
        /// 根据序列化的结果自动填充
        /// </summary>
        public void SerilizeGetBytes(object objectMessage, int byteStartIndex = 0)
        {
            if (objectMessage == null)
            {
                Debug.LogError($"序列化的对象为null");
                return;
            }


            string message = SerializeManager.SerializeObject(objectMessage);
            EncodingGetBytes(message, Encoding.UTF8, byteStartIndex);
        }

        #endregion


        public void ClearByteArray()
        {
            Array.Clear(mBytes, 0, mBytes.Length);
            mDataRealLength = 0;
        }
    }
}