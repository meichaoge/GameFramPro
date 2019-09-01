using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System;

namespace GameFramePro.NetWorkEx
{

    /// <summary>   /// 对内接受和缓存接受的数据 对外提供必要的访问接口  /// </summary>
    public class SocketManager : Single<SocketManager>
    {

        #region 提供Byte[] 数组
        private readonly ConcurrentQueue<byte[]> mSocketBytesQueue = new ConcurrentQueue<byte[]>(); //复用的Socket byte[] 数组
        public const int S_ByteLength = 2048; //表示每个 byte[] 数组的长度

        private static object S_GetByteLocker = new object();


        public byte[] GetBytes()
        {
            lock (S_GetByteLocker)
            {
                if (mSocketBytesQueue.Count == 0)
                    return new byte[S_ByteLength];
                if (mSocketBytesQueue.TryDequeue(out var byteData))
                {
                    System.Array.Clear(byteData, 0, S_ByteLength);
                    return byteData;
                }
                return new byte[S_ByteLength];
            }
        }

        public void RecycleBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length != S_ByteLength)
                throw new ArgumentException($"参数 为 null 或者长度不是 {S_ByteLength}");
            mSocketBytesQueue.Enqueue(bytes);
        }

        #endregion

        #region 提供 SocketHead
        private readonly ConcurrentQueue<SocketHead> mSocketHeadQueue = new ConcurrentQueue<SocketHead>(); //复用的Socket Head
        private static object S_GetSocketHeadLocker = new object();

        public SocketHead GetSocketHead()
        {
            lock (S_GetByteLocker)
            {
                if (mSocketBytesQueue.Count == 0)
                    return new SocketHead();
                if (mSocketHeadQueue.TryDequeue(out var socketHead))
                {
                    socketHead.Reset();
                    return socketHead;
                }
                return new SocketHead();
            }
        }

        public void RecycleSocketHead(SocketHead socketHead)
        {
            if (socketHead == null)
                throw new ArgumentException($"参数 为 null {socketHead}");
            mSocketHeadQueue.Enqueue(socketHead);
        }
        #endregion

    }

}