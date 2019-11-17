using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 
    /// Socket 发送消息基类
    /// // </summary>
    public class BaseSocketSendMessage
    {
        public int mProtocolID { get; set; } //协议id
        public ByteArray mSendMessageByteArray { get; private set; }

        public EndPoint mEndPoint { get; set; }
        public bool mIsBrocast { get; set; } = false; //是否是广播消息



        #region 对象池

        internal static readonly ConcurrentQueue<BaseSocketSendMessage> mAllSendMessageDataPool = new ConcurrentQueue<BaseSocketSendMessage>(); //发送Socket 消息池


        /// <summary>
        /// 获取一个指定的对象
        /// </summary>
        /// <returns></returns>
        public static BaseSocketSendMessage GetSocketSendMessageData()
        {
            if (mAllSendMessageDataPool.TryDequeue(out var messageData) == false)
                messageData = new BaseSocketSendMessage();
            else
                messageData.ClearSocketMessage();

            return messageData;
        }

        public static BaseSocketSendMessage GetSocketSendMessageData(int protocolId, ByteArray message, EndPoint endPoint, bool isBrocast)
        {
            if (mAllSendMessageDataPool.TryDequeue(out var messageData) == false)
                messageData = new BaseSocketSendMessage();

            messageData.InitialedSocketSendMessage(protocolId, message, endPoint, isBrocast);
            return messageData;
        }


        /// <summary>///
        /// 回收数据/// </summary>
        public static void RecycleSocketMessageData(BaseSocketSendMessage socketSendMessage)
        {
            if (socketSendMessage == null)
            {
                Debug.LogError($"待回收的对象为null");
                return;
            }

            socketSendMessage.ClearSocketMessage();
            mAllSendMessageDataPool.Enqueue(socketSendMessage);
        }

        #endregion


        #region 构造函数

        protected BaseSocketSendMessage()
        {
        }

        //public BaseSocketSendMessage(int protocolId, ByteArray message, EndPoint endPoint, bool isBrocast)
        //{
        //    InitialedSocketSendMessage(protocolId, message, endPoint, isBrocast);
        //}

        #endregion

        protected void InitialedSocketSendMessage(int protocolId, ByteArray message, EndPoint endPoint, bool isBrocast)
        {
            mSendMessageByteArray = message;
            mProtocolID = protocolId;
            mSendMessageByteArray = message;
            mIsBrocast = isBrocast;

            if (message == null)
                throw new ArgumentNullException($"参数message null");
        }

        public virtual void ClearSocketMessage()
        {
            mProtocolID = 0;
            if (mSendMessageByteArray != null)
            {
                ByteArray.RecycleByteArray(mSendMessageByteArray);
                mSendMessageByteArray = null;
            }
        }
    }
}