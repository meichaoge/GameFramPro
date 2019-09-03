using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>    /// Socket 消息的消息头部/// </summary>
    public class SocketHead
    {
        #region 头部信息 如果有新增的也需要修改对应的 S_HeadLength

        public int mMessageLength; //消息实际数据长度 （放在首位方便取）
        public int mMessageID; //消息ID
        public int mVersion; //版本号

        public const int S_HeadLength = 3 * sizeof(int); //头部字节长度

        #endregion

        #region 构造函数

        public SocketHead() : this(0, 0, 0)
        {
        }

        public SocketHead(int messageId, int messageLength, int version)
        {
            mMessageID = messageId;
            mMessageLength = messageLength;
            mVersion = version;
        }

        #endregion


        #region 静态接口

        private static byte[] LengthBuffer = new byte[4];
        
        /// <summary>     /// 获取数据包的真实长度    /// </summary>  /// <returns></returns>
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

        /// <summary>     /// 获取数据包的协议id    /// </summary>  /// <returns></returns>
        public static int GetPacketCommandID(byte[] buffer, int startIndex)
        {
            if (buffer == null || buffer.Length < 4)
                throw new ArgumentNullException($"数据协议id异常 参数为null  或者小于4");
            if (startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException($"参数 startIndex 的值 超过了数据包的实际长度 ");
            Array.Copy(buffer, startIndex, CommandIDBuffer, 0, 4);
            return BitConverter.ToInt32(CommandIDBuffer, 0);
        }

        #endregion

        public void Reset()
        {
            mMessageID = 0;
            mMessageLength = 0;
            mVersion = 0;
        }

        /// <summary>    /// 附加Socket 头部信息 /// </summary>
        public void GetMessageHead(ByteArray sourceData)
        {
            sourceData.CopyBytes(BitConverter.GetBytes(mMessageLength), 0, 4, sourceData.mDataRealLength + 4, 0);
            sourceData.CopyBytes(BitConverter.GetBytes(mMessageID), 0, 4, sourceData.mDataRealLength + 4, 4);
            sourceData.CopyBytes(BitConverter.GetBytes(mVersion), 0, 4, sourceData.mDataRealLength + 4, 8);
        }
    }
}