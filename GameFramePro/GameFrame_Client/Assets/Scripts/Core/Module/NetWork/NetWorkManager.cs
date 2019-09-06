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

        private Dictionary<string, BaseSocketClient> mAllSocketClients = new Dictionary<string, BaseSocketClient>(5);

        /// <summary>/// 所有注册的网络消息处理回调/// </summary>
        private Dictionary<int, HashSet<System.Action<object>>> mAllRegisterResponseCallback = new Dictionary<int, HashSet<Action<object>>>(100);

        #endregion


        #region IUpdateCountTick 接口

        public uint TickPerUpdateCount { get; private set; } = 1;

        /// <summary>/// 主线程更新分发收到的数据/// </summary>
        public void UpdateTick(float currentTime)
        {
            try
            {
                BaseSocketReceiveMessage messageData = null;
                foreach (var socketClient in mAllSocketClients.Values)
                {
                    if (socketClient == null || socketClient.mBaseSocketMessageManager == null) continue;
                    while (socketClient.mBaseSocketMessageManager.GetSocketReceiveData(out messageData))
                    {
                        string messageStr = Encoding.UTF8.GetString(messageData.mReceiveMessageByteArray.mBytes, SocketHead.S_HeadLength, messageData.mReceiveMessageByteArray.mDataRealLength);
                        Debug.Log($"接收到{messageData.mEndPoint}  消息id={messageData.mProtocolID}  内容{messageStr}");

                        var messageObj = SerializeManager.DeserializeObject(messageStr);
                        var callbacks = GetAllNetWorkCallback(messageData.mProtocolID);
                        if (callbacks != null)
                        {
                            foreach (var netWorkProcess in callbacks)
                                netWorkProcess?.Invoke(messageObj);
                        }

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


        #region 网络消息处理监听

        /// <summary>/// 获取指定消息对应的处理回调/// </summary>
        private HashSet<System.Action<object>> GetAllNetWorkCallback(int protocolId)
        {
            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netWorkCallback))
                return netWorkCallback;
            return null;
        }

        /// <summary>/// 注册对应回调id 的处理回调/// </summary>
        public void RegisterNetWorkCallback(int protocolId, System.Action<object> netWorkResponseCallback)
        {
            if (netWorkResponseCallback == null)
            {
                Debug.LogError($"注册网络回调失败 {protocolId},处理函数参数为null");
                return;
            }

            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netWorkCallback) == false)
            {
                netWorkCallback = new HashSet<Action<object>>();
                netWorkCallback.Add(netWorkResponseCallback);
            }
            else
            {
                if (netWorkCallback.Contains(netWorkResponseCallback))
                    Debug.LogError($"处理{protocolId} 重复注册处理回调{netWorkCallback}");
                else
                    netWorkCallback.Add(netWorkResponseCallback);
            }
        }

        /// <summary>/// 取消注册对应回调id 的处理回调/// </summary>
        public void UnRegisterNetWorkCallback(int protocolId, System.Action<object> netWorkResponseCallback)
        {
            if (netWorkResponseCallback == null)
            {
                Debug.LogError($"注册网络回调失败 {protocolId},处理函数参数为null");
                return;
            }

            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netWorkCallback) == false)
                netWorkCallback.Remove(netWorkResponseCallback);
        }

        /// <summary>/// 取消注册对应回调id 的所有处理回调/// </summary>
        public void UnRegisterAllNetWorkCallback(int protocolId)
        {
            mAllRegisterResponseCallback.Remove(protocolId);
        }

        #endregion


        #region Socket 客户端创建等 对应的事件

        #region 监听Socket 客户端的创建

        public T GetSocketMessage<T>(string clientName) where T : BaseSocketClient
        {
            if (mAllSocketClients.TryGetValue(clientName, out var socketClient))
                return socketClient as T;
            Debug.LogError($"指定名称{clientName} 的Socket 没有创建");
            return null;
        }


//        public T CreateSocketMessage<T>(string clientName) where T : BaseSocketClient
//        {
//            if (mAllSocketClients.TryGetValue(clientName, out var socketClient))
//            {
//                if (socketClient.GetType() == typeof(T))
//                    return socketClient as T;
//                Debug.LogError($"创建名称{clientName} 失败，已经有一个类型为{socketClient.GetType()}");
//                return null;
//            }
//            
//            
//
//            Debug.LogError($"指定名称{clientName} 的Socket 没有创建");
//            return null;
//        }

        public void RegisterSocketClient(BaseSocketClient socketClient)
        {
            lock (mAllSocketClients)
            {
                if (mAllSocketClients.ContainsKey(socketClient.mClientName))
                {
                    Debug.LogError($"生成相同名称{socketClient.mClientName}的两个客户端 :{socketClient.mSocketClientType} :{mAllSocketClients[socketClient.mClientName].mSocketClientType}");
                    return;
                }

                mAllSocketClients.Add(socketClient.mClientName, socketClient);
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
                mAllSocketClients.Remove(socketClient.mClientName);
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

        #endregion
    }

    /// <summary>/// 定义和获取指定名称的Socket 客户端/// </summary>
    public static class SocketClientHelper
    {
        public static readonly string BaseTcpClientName = "TcpLoginClient"; //连接登录服的tcp 客户端 
        private static BaseTcpClient mBaseLoginTcpClient = null;

        /// <summary>/// 登录服/// </summary>
        public static BaseTcpClient BaseLoginTcpClient
        {
            get
            {
                if (mBaseLoginTcpClient != null) return mBaseLoginTcpClient;
                mBaseLoginTcpClient = NetWorkManager.S_Instance.GetSocketMessage<BaseTcpClient>(BaseTcpClientName);
                if (mBaseLoginTcpClient != null) return mBaseLoginTcpClient;
                mBaseLoginTcpClient = new BaseTcpClient(BaseTcpClientName, AddressFamily.InterNetwork);
                return mBaseLoginTcpClient;
            }
        }
    }
}