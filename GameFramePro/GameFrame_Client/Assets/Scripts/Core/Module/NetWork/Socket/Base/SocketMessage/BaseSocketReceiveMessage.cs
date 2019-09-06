using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// Socket 接收消息基类/// </summary>
    public class BaseSocketReceiveMessage
    {
        public int mProtocolID { get; set; } //协议id
        public ByteArray mReceiveMessageByteArray { get; private set; }

        public EndPoint mEndPoint { get; set; }

        #region 接收消息对象池

        internal static readonly ConcurrentQueue<BaseSocketReceiveMessage> mAllReceiveMessageDataPool = new ConcurrentQueue<BaseSocketReceiveMessage>(); //接收Socket 消息池

        public static BaseSocketReceiveMessage GetSocketReceiveMessageData(int protocolId, ByteArray message, EndPoint endPoint)
        {
            if (mAllReceiveMessageDataPool.TryDequeue(out var messageData) == false)
                messageData = new BaseSocketReceiveMessage(protocolId, message, endPoint);
            else
                messageData.InitialedSocketReceiveMessage(protocolId, message, endPoint);

            return messageData;
        }

        /// <summary>/// 回收数据/// </summary>
        public static void RecycleSocketMessageData(BaseSocketReceiveMessage socketReceiveMessage)
        {
            if (socketReceiveMessage == null)
            {
                Debug.LogError($"待回收的对象为null");
                return;
            }

            socketReceiveMessage.ClearSocketMessage();
            mAllReceiveMessageDataPool.Enqueue(socketReceiveMessage);
        }

        #endregion

        #region 构造函数

        private BaseSocketReceiveMessage()
        {
        }

        public BaseSocketReceiveMessage(int protocolId, ByteArray message, EndPoint endPoint)
        {
            InitialedSocketReceiveMessage(protocolId, message, endPoint);
        }

        #endregion

        public virtual void InitialedSocketReceiveMessage(int protocolId, ByteArray message, EndPoint endPoint)
        {
            mReceiveMessageByteArray = message;
            mProtocolID = protocolId;
            mEndPoint = endPoint;

            if (message == null)
                throw new ArgumentNullException($"参数message null");
        }

        public virtual void ClearSocketMessage()
        {
            mProtocolID = 0;
            if (mReceiveMessageByteArray != null)
            {
                ByteArray.RecycleByteArray(mReceiveMessageByteArray);
                mReceiveMessageByteArray = null;
            }
        }
    }
}