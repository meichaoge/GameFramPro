using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>   ///  作为Socket Tcp和Udp 客户端的一部分，提供统一的收发消息安全的队列管理  /// </summary>
    internal class BaseSocketMessageManager
    {
        internal readonly ConcurrentQueue<BaseSocketReceiveMessage> mAllReceiveMessageQueue = new ConcurrentQueue<BaseSocketReceiveMessage>(); //接受到的所有来自服务器的数据
        internal readonly ConcurrentQueue<BaseSocketSendMessage> mAllSendMessageQueue = new ConcurrentQueue<BaseSocketSendMessage>(); //需要发送的数据


        public void ClearData()
        {
            while (mAllReceiveMessageQueue.Count > 0)
                mAllReceiveMessageQueue.TryDequeue(out var messageData);

            while (mAllSendMessageQueue.Count > 0)
                mAllSendMessageQueue.TryDequeue(out var messageData);
        }


        #region 接收的消息 （缓存+被提取处理）

        /// <summary>  /// 缓存接受到的数据  (被Socket 网络接受线程调用)  /// </summary>
        public void SaveReceiveData(BaseSocketReceiveMessage receiveData)
        {
            if (receiveData == null)
                throw new ArgumentNullException($"保存接受到的数据异常，数据为null");
            mAllReceiveMessageQueue.Enqueue(receiveData);
        }


        /// <summary>      /// 主线程中获取缓存的网络数据    /// </summary>
        public BaseSocketReceiveMessage GetSocketReceiveData()
        {
            if (mAllReceiveMessageQueue.Count == 0)
                return null;

            if (mAllReceiveMessageQueue.TryDequeue(out var receiveMessage))
                return receiveMessage;
            return null;
        }

        public bool GetSocketReceiveData(out BaseSocketReceiveMessage data)
        {
            data = null;
            if (mAllReceiveMessageQueue.Count == 0)
                return false;

            return mAllReceiveMessageQueue.TryDequeue(out data);
        }

        #endregion

        #region 发送消息

        /// <summary>  /// 缓存要发送数据  (被主线程调用)  /// </summary><param name="protocalID">协议ID(整型)</param>
        public void CacheSocketSendData(BaseSocketSendMessage socketSendMessage)
        {
            if (socketSendMessage == null)
                throw new ArgumentNullException($"缓存要发送的数据异常，数据为null ");
            
            Debug.Log($"缓存要发送的数据 {socketSendMessage.mProtocolID}  长度={socketSendMessage.mSendMessageByteArray.mDataRealLength} ：{socketSendMessage.mSendMessageByteArray.mBytes}");

            SocketHead head = SocketHead.GetSocketHead(socketSendMessage.mProtocolID, socketSendMessage.mSendMessageByteArray.mDataRealLength, 0);
            
       //     Debug.Log($"2222缓存要发送的数据  长度={socketSendMessage.mSendMessageByteArray.mDataRealLength} ：{socketSendMessage.mSendMessageByteArray.mBytes}");

            
            head.AppendMessageHead(socketSendMessage.mSendMessageByteArray); //增加头部信息
            SocketHead.RecycleSocketHead(head);


            mAllSendMessageQueue.Enqueue(socketSendMessage);
        }

        /// <summary>      ///Socket 发送消息线程  /// </summary>
        public BaseSocketSendMessage GetWillSendSocketData()
        {
            if (mAllSendMessageQueue.Count == 0)
                return null;

            if (mAllSendMessageQueue.TryDequeue(out var sendMessage))
                return sendMessage;
            return null;
        }

        public bool GetWillSendSocketData(out BaseSocketSendMessage data)
        {
            data = null;
            if (mAllSendMessageQueue.Count == 0)
                return false;

            return mAllSendMessageQueue.TryDequeue(out data);
        }

        #endregion
    }
}