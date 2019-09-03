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
        public int mMessageLength;  //消息实际数据长度 （放在首位方便取）
        public int mMessageID;  //消息ID
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
        public void GetMessageHead(ByteArray sourceData)
        {
            sourceData.CopyBytes(BitConverter.GetBytes(mMessageLength), 0, 4, sourceData.mDataRealLength+4, 0);
            sourceData.CopyBytes(BitConverter.GetBytes(mMessageID), 0, 4, sourceData.mDataRealLength + 4,4);
            sourceData.CopyBytes(BitConverter.GetBytes(mVersion), 0, 4, sourceData.mDataRealLength + 4, 8);
        }


    }

}