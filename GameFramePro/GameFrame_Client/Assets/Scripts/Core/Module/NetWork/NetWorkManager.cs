using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using GameFramePro.NetWorkEx;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text;


namespace GameFramePro
{
    /// <summary>/// 负责对内网络状态的控制对外提供网络回调/// </summary>
    public class NetWorkManager : Single<NetWorkManager>, IUpdateCountTick
    {
        #region 数据

        private List<BaseSocketClient> mAllSocketClients = new List<BaseSocketClient>(5);

        #endregion

        #region 监听Socket 客户端的创建

        public void RegisterSocketClient(BaseSocketClient socketClient)
        {
            lock (mAllSocketClients)
            {
                mAllSocketClients.Add(socketClient);
            }

            socketClient.OnInitialedFailEvent += OnSocketInitialFailCallback;
            socketClient.OnSocketErrorEvent += OnSocketErrorCallback;
            socketClient.OnSendMessageEvent += OnSendMessageCallback;
            socketClient.OnReceiveMessageEvent += OnReceiveMessageCallback;


            switch (socketClient.mSocketClientType)
            {
                case SocketClientUsage.TcpClient:
                    var tcpClient = socketClient as BaseTcpClient;

                    tcpClient.OnBeginConnectEvent += OnBeginConnectCallback;
                    tcpClient.OnConnectSuccessEvent += OnConnectSuccessCallback;
                    tcpClient.OnConnectErrorEvent += OnConnectErrorCallback;
                    tcpClient.OnConnectTimeOutEvent += OnConnectTimeOutCallback;
                    tcpClient.OnDisConnectEvent += OnDisConnectEvent;

                    break;
                case SocketClientUsage.UdpClient:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void UnRegisterSocketClient(BaseSocketClient socketClient)
        {
            lock (mAllSocketClients)
            {
                mAllSocketClients.Remove(socketClient);
            }
        }

        #endregion


        #region IUpdateCountTick 接口

        public uint TickPerUpdateCount { get; protected set; } = 1;

        public void UpdateTick(float currentTime)
        {
            try
            {
                BaseSocketReceiveMessage messageData = null;
                foreach (var socketClient in mAllSocketClients)
                {
                    if (socketClient == null || socketClient.mBaseSocketMessageManager == null) continue;
                    while (socketClient.mBaseSocketMessageManager.GetSocketReceiveData(out messageData))
                    {
                        string messageStr = Encoding.UTF8.GetString(messageData.mReceiveMessageByteArray.mBytes, SocketHead.S_HeadLength, messageData.mReceiveMessageByteArray.mDataRealLength);
                        Debug.Log($"接收到{messageData.mEndPoint}  消息id={messageData.mProtocolID}  内容{messageStr}");

                        BaseSocketReceiveMessage.RecycleSocketMessageData(messageData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"读取服务器数据异常{e}");
            }
        }

        #endregion


        #region 注册事件

        #region BaseSocketClient 事件

        private void OnSocketInitialFailCallback(BaseSocketClient socketClient)
        {
        }

        private void OnSocketErrorCallback(BaseSocketClient socketClient, string message)
        {
        }

        private void OnSendMessageCallback(BaseSocketClient socketClient, BaseSocketSendMessage messageData)
        {
        }

        private void OnReceiveMessageCallback(BaseSocketClient socketClient, BaseSocketReceiveMessage messageData)
        {
        }

        #endregion

        #region TCP 客户端

        private void OnBeginConnectCallback(BaseSocketClient tcpSocketClient)
        {
        }

        private void OnConnectSuccessCallback(BaseSocketClient tcpSocketClient)
        {
        }

        private void OnConnectErrorCallback(BaseSocketClient tcpSocketClient, string errorMessage)
        {
        }

        private void OnConnectTimeOutCallback(BaseSocketClient tcpSocketClient)
        {
        }


        private void OnDisConnectEvent(BaseSocketClient tcpSocketClient)
        {
        }

        #endregion

        #endregion
    }
}