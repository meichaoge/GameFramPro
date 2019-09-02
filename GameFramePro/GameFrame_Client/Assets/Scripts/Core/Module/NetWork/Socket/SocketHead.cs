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
        public int mMessageID;  //消息ID
        public int mMessageLength;  //消息实际数据长度
        public int mVersion; //版本号

        public const int S_HeadLength = 3 * sizeof(int); //头部字节长度
        #endregion

         public SocketHead() : this(0, 0, 0) { }


        public SocketHead(int messageId, int messageLength, int version)
        {
            mMessageID = messageId;
            mMessageLength = messageLength;
            mVersion = version;
        }


        public void Reset()
        {
            mMessageID = 0;
            mMessageLength = 0;
            mVersion = 0;
        }

        /// <summary>    /// 附加Socket 头部信息 /// </summary>
        public void GetMessageHead(ref byte[] sourceData)
        {
            Array.Copy(BitConverter.GetBytes(mMessageID), 0, sourceData, 0, 4);
            Array.Copy(BitConverter.GetBytes(mMessageLength), 0, sourceData, 4, 4);
            Array.Copy(BitConverter.GetBytes(mVersion), 0, sourceData, 4 * 2, 4);
        }


    }
}