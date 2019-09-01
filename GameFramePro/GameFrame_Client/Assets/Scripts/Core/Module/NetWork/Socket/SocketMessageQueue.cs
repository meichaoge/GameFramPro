using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    public class SocketMessageData
    {
        public int mProtocolID; //协议id
        public byte[] mMessageData; //消息
        public EndPoint mEndPoint;
        public bool mIsBrocast = false; //是否是广播消息


        public SocketMessageData(int protocolId, byte[] message, EndPoint endPoint, bool isBrocase)
        {
            mProtocolID = protocolId;
            mMessageData = message;
            mEndPoint = endPoint;
            mIsBrocast = isBrocase;
        }
    }

    /// <summary>   ///  作为Socket Tcp和Udp 客户端的一部分，提供统一的收发消息安全的队列管理  /// </summary>
    internal class SocketMessageQueue
    {
        internal readonly ConcurrentQueue<SocketMessageData> mAllReceiveDataQueue = new ConcurrentQueue<SocketMessageData>(); //接受到的所有来自服务器的数据
        internal readonly ConcurrentQueue<SocketMessageData> mAllSendDataQueue = new ConcurrentQueue<SocketMessageData>(); //需要发送的数据


        public void ClearData()
        {
            while (mAllReceiveDataQueue.Count > 0)
                mAllReceiveDataQueue.TryDequeue(out var messageData);

            while (mAllSendDataQueue.Count > 0)
                mAllSendDataQueue.TryDequeue(out var messageData);
        }


        #region 接收的消息 （缓存+被提取处理）
        /// <summary>  /// 缓存接受到的数据  (被Socket 网络接受线程调用)  /// </summary>
        /// <param name="protocalID">协议ID(整型)</param>
        public void SaveReceiveData(int protocolId, byte[] message, EndPoint endPoint, bool isBrocast)
        {
            var receiveData = new SocketMessageData(protocolId, message, endPoint, isBrocast);
            mAllReceiveDataQueue.Enqueue(receiveData);
        }
        /// <summary>      /// 主线程中获取缓存的网络数据    /// </summary>
        public SocketMessageData GetSocketReceiveData()
        {
            if (mAllReceiveDataQueue.Count == 0)
                return null;

            if (mAllReceiveDataQueue.TryDequeue(out var receiveMessage))
                return receiveMessage;
            return null;
        }
        public bool GetSocketReceiveData(out SocketMessageData data)
        {
            data = null;
            if (mAllReceiveDataQueue.Count == 0)
                return false;

            return mAllReceiveDataQueue.TryDequeue(out data);
        }
        #endregion

        #region 发送消息
        /// <summary>  /// 缓存要发送数据  (被主线程调用)  /// </summary>
        /// <param name="protocalID">协议ID(整型)</param>
        public void SaveSendData(int protocolId, byte[] message, EndPoint endPoint, bool isBrocast)
        {
            byte[] sendMessage = SocketManager.S_Instance.GetBytes();
            System.Array.Copy(message, 0, sendMessage, SocketHead.S_HeadLength, message.Length);

            SocketHead head = new SocketHead(protocolId, message.Length,0);
            head.GetMessageHead(ref sendMessage);

            var sendData = new SocketMessageData(protocolId, message, endPoint, isBrocast);
            mAllSendDataQueue.Enqueue(sendData);
        }
        /// <summary>      ///Socket 发送消息线程  /// </summary>
        public SocketMessageData GetWillSendSocketData()
        {
            if (mAllSendDataQueue.Count == 0)
                return null;

            if (mAllSendDataQueue.TryDequeue(out var sendMessage))
                return sendMessage;
            return null;
        }

        public bool GetWillSendSocketData(out SocketMessageData data)
        {
            data = null;
            if (mAllSendDataQueue.Count == 0)
                return false;

            return mAllSendDataQueue.TryDequeue(out data);
        }

        #endregion

    }
}