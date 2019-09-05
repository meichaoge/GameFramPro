using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>    /// Socket 消息的消息头部/// </summary>
    internal class SocketHead
    {
        #region 头部信息 如果有新增的也需要修改对应的 S_HeadLength

        public int mMessageLength; //消息实际数据长度 （放在首位方便取）
        public int mProtocolId; //协议ID
        public int mVersion; //版本号

        public const int S_HeadLength = 3 * sizeof(int); //头部字节长度

        #endregion

        #region SocketHead 缓存池

        private static readonly ConcurrentQueue<SocketHead> mSocketHeadQueue = new ConcurrentQueue<SocketHead>(); //复用的Socket Head

        //获取一个 SocketHead 对象
        public static SocketHead GetSocketHead(int commandId, int messageLength, int version)
        {
            if (mSocketHeadQueue.Count == 0)
                return new SocketHead(commandId, messageLength, version);
            if (mSocketHeadQueue.TryDequeue(out var socketHead))
            {
                socketHead.ResetSocketHead();
                return socketHead;
            }

            return new SocketHead(commandId, messageLength, version);
        }

        //回收Socket Head
        public static void RecycleSocketHead(SocketHead socketHead)
        {
            if (socketHead == null)
                throw new ArgumentNullException($"参数 为 null ");
            mSocketHeadQueue.Enqueue(socketHead);
        }

        #endregion


        #region 构造函数

        private SocketHead(int protocolId, int messageLength, int version)
        {
            mProtocolId = protocolId;
            mMessageLength = messageLength;
            mVersion = version;
        }

        #endregion


        #region 静态辅助接口

        private static byte[] LengthBuffer = new byte[4];

        /// <summary>     /// 获取数据包的真实长度    /// </summary> 
        public static int GetPacketLength(byte[] buffer, int startIndex)
        {
            if (buffer == null || buffer.Length < 4)
                throw new ArgumentNullException($"数据包长度异常 参数为null  或者小于4");
            if (startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException($"参数 startIndex 的值 超过了数据包的实际长度 ");
            Array.Copy(buffer, startIndex, LengthBuffer, 0, 4);
            return BitConverter.ToInt32(LengthBuffer, 0);
        }


        private static byte[] CommandIDBuffer = new byte[4];

        /// <summary>     /// 获取数据包的协议id    /// </summary> 
        public static int GetPacketProtocolID(byte[] buffer, int startIndex)
        {
            if (buffer == null || buffer.Length < 4)
                throw new ArgumentNullException($"数据协议id异常 参数为null  或者小于4");
            if (startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException($"参数 startIndex 的值 超过了数据包的实际长度 ");
            Array.Copy(buffer, startIndex, CommandIDBuffer, 0, 4);
            return BitConverter.ToInt32(CommandIDBuffer, 0);
        }

        #endregion

        #region 接口

        public void ResetSocketHead()
        {
            mProtocolId = 0;
            mMessageLength = 0;
            mVersion = 0;
        }

        /// <summary>    /// 附加Socket 头部信息 /// </summary>
        public void AppendMessageHead(ByteArray sourceData)
        {
            sourceData.CopyBytes(BitConverter.GetBytes(mMessageLength), 0, 4, sourceData.mDataRealLength + 4, 0);
            sourceData.CopyBytes(BitConverter.GetBytes(mProtocolId), 0, 4, sourceData.mDataRealLength + 4, 4);
            sourceData.CopyBytes(BitConverter.GetBytes(mVersion), 0, 4, sourceData.mDataRealLength + 4, 8);
        }

        #endregion
    }
}