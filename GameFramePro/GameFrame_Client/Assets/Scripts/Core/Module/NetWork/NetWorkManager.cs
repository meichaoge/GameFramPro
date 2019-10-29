using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using GameFramePro.NetWorkEx;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text;
using LitJson;
using Newtonsoft.Json;
using JsonReader = Newtonsoft.Json.JsonReader;


namespace GameFramePro
{
    /// <summary>/// 负责对内网络状态的控制对外提供网络回调/// </summary>
    public class NetWorkManager : Single<NetWorkManager>, IUpdateCountTick
    {
        /// <summary>/// 监听的事件回调信息/// </summary>
        protected class NetResponseCallbackInfor
        {
            public int mProtocolId; //监听的协议id 
            public Type mResponseType; //协议返回对象类型
            public HashSet<Action<object>> mResponseCallback = new HashSet<Action<object>>(); //回调

            #region 构造函数

            private NetResponseCallbackInfor(int protocold)
            {
                mProtocolId = protocold;
            }

            #endregion

            #region 对象池

            private static Queue<NetResponseCallbackInfor> s_AllNetWorkCallbackInforPool = new Queue<NetResponseCallbackInfor>();

            //获取一个指定类型的对象
            public static NetResponseCallbackInfor GetNetWorkCallbackInfor(int protocolId)
            {
                if (s_AllNetWorkCallbackInforPool.Count > 0)
                    return s_AllNetWorkCallbackInforPool.Dequeue();

                return new NetResponseCallbackInfor(protocolId);
            }

            //回收一个指定的对象
            public static void RecycleNetWorkCallbackInfor(NetResponseCallbackInfor netResponseCallbackInfor)
            {
                if (netResponseCallbackInfor == null)
                {
                    Debug.LogError($"回收失败，对象null");
                    return;
                }

                netResponseCallbackInfor.mResponseCallback.Clear();
                netResponseCallbackInfor.mProtocolId = 0;
                netResponseCallbackInfor.mResponseType = null;

                s_AllNetWorkCallbackInforPool.Enqueue(netResponseCallbackInfor);
            }

            #endregion


            #region 接口

            //注册的回调总数
            public int AllCallbackCount
            {
                get { return mResponseCallback.Count; }
            }


            /// <summary>/// 监听某个指定类型和id 的网络回调/// </summary>
            public void AddCallback(Type type, int protocold, System.Action<object> callback)
            {
                if (callback == null || type == null)
                {
                    Debug.LogError($"监听回调为null{mProtocolId}  {type}  ");
                    return;
                }

                if (protocold != mProtocolId || (mResponseType != null && mResponseType != type))
                {
                    Debug.LogError($"监听类型不一致当前{mProtocolId}  {mResponseType}   ：注册{protocold}  {type}");
                    return;
                }

                if (mResponseType == null)
                    mResponseType = type;

                if (mResponseCallback.Contains(callback))
                {
                    Debug.LogError($"重复监听{mProtocolId}  {type}  {callback}");
                    return;
                }


                mResponseCallback.Add(callback);
            }


            /// <summary>/// 监听某个指定类型和id 的网络回调/// </summary>
            public void RemoveCallback(int protocold, System.Action<object> callback)
            {
                if (callback == null)
                {
                    Debug.LogError($"监听回调为null{mProtocolId}    ");
                    return;
                }

                if (protocold != mProtocolId)
                {
                    Debug.LogError($"监听类型不一致当前{mProtocolId}  {mResponseType}   ：取消注册{protocold}  ");
                    return;
                }

                mResponseCallback.Remove(callback);
                if (mResponseCallback.Count == 0)
                    mResponseCallback = null;
            }

            /// <summary>/// 移除所有的监听/// </summary>
            public void RemoveAllCallback(int protocold)
            {
                if (protocold != mProtocolId)
                {
                    Debug.LogError($"监听类型不一致当前{mProtocolId}  {mResponseType}   ：取消注册{protocold}  ");
                    return;
                }

                mResponseType = null;
                mResponseCallback.Clear();
            }

            /// <summary>/// 触发指定消息id 的网络回调事件/// </summary>
            public void DispatchMessage(int protocolID, string message)
            {
                if (mProtocolId != protocolID)
                {
                    Debug.LogError($"触发消息失败 {protocolID}  ：{mProtocolId}");
                    return;
                }

                if (mResponseCallback.Count == 0)
                {
                    Debug.LogInfor($"没有监听{protocolID} 网络回调的函数，不处理");
                    return;
                }

                object response = SerializeManager.DeserializeObject(message, mResponseType);
                HashSet<Action<object>> responseCallbacks = new HashSet<Action<object>>(mResponseCallback);

                foreach (var callback in responseCallbacks)
                    callback?.Invoke(response);
            }

            #endregion
        }


        #region 数据

        private Dictionary<string, BaseSocketClient> mAllSocketClients = new Dictionary<string, BaseSocketClient>(5);

        /// <summary>/// 所有注册的网络消息处理回调/// </summary>
        private Dictionary<int, NetResponseCallbackInfor> mAllRegisterResponseCallback = new Dictionary<int, NetResponseCallbackInfor>(100);

        #endregion


        public void InitialedNetWork()
        {
            NetWorkCertigicate.InitialedNetWorkCertigicate();
        }

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
                        Debug.Log($"{messageData.mProtocolID}   {messageData.mReceiveMessageByteArray.mDataRealLength}   {messageData.mReceiveMessageByteArray}");


                        string messageStr = Encoding.UTF8.GetString(messageData.mReceiveMessageByteArray.mBytes, SocketHead.S_HeadLength, messageData.mReceiveMessageByteArray.mDataRealLength - SocketHead.S_HeadLength);

                        //Debug.Log("-->>"+messageStr);

                        if (ProtocolCommand.ResponseLogin == messageData.mProtocolID)
                        {
                            Debug.Log($"登录回包");
                            // Debug.Log($"接收到{messageData.mEndPoint}  消息id={messageData.mProtocolID}  内容{messageStr}");
                            //    Debug.Log($"接收到  消息id={messageData.mProtocolID}  内容{messageStr}");
                        }

                        if (ProtocolCommand.ResponseHearBeat == messageData.mProtocolID)
                        {
                            Debug.Log($"心跳回包 ");
                        }

                        if (mAllRegisterResponseCallback.TryGetValue(messageData.mProtocolID, out var netResponseCallbackInfor))
                        {
                            netResponseCallbackInfor.DispatchMessage(messageData.mProtocolID, messageStr);
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

        /// <summary>/// 注册对应回调id 的处理回调/// </summary>
        public void RegisterNetWorkCallback<T>(int protocolId, System.Action<object> netWorkResponseCallback)
        {
            if (netWorkResponseCallback == null)
            {
                Debug.LogError($"注册网络回调失败 {protocolId},处理函数参数为null");
                return;
            }


            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netResponseCallback) == false)
            {
                netResponseCallback = NetResponseCallbackInfor.GetNetWorkCallbackInfor(protocolId);
                netResponseCallback.AddCallback(typeof(T), protocolId, netWorkResponseCallback);

                mAllRegisterResponseCallback[protocolId] = netResponseCallback;
            }
            else
            {
                netResponseCallback.AddCallback(typeof(T), protocolId, netWorkResponseCallback);
            }
        }

        /// <summary>/// 取消注册对应回调id 的处理回调/// </summary>
        public void UnRegisterNetWorkCallback(int protocolId, System.Action<object> netWorkResponseCallback)
        {
            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netResponseCallback) == false)
            {
                netResponseCallback.RemoveCallback(protocolId, netWorkResponseCallback);
                if (netResponseCallback.AllCallbackCount == 0)
                {
                    NetResponseCallbackInfor.RecycleNetWorkCallbackInfor(netResponseCallback);
                    mAllRegisterResponseCallback.Remove(protocolId);
                }
            }
        }

        /// <summary>/// 取消注册对应回调id 的所有处理回调/// </summary>
        public void UnRegisterAllNetWorkCallback(int protocolId)
        {
            if (mAllRegisterResponseCallback.TryGetValue(protocolId, out var netResponseCallback) == false)
            {
                netResponseCallback.RemoveAllCallback(protocolId);
                NetResponseCallbackInfor.RecycleNetWorkCallbackInfor(netResponseCallback);
                mAllRegisterResponseCallback.Remove(protocolId);
            }
        }

        #endregion


        #region Socket 客户端创建等 对应的事件

        /// <summary>/// 关闭所有的客户端/// </summary>
        public void CloseAllSocketClient()
        {
            Dictionary<string, BaseSocketClient> temp = new Dictionary<string, BaseSocketClient>(mAllSocketClients);

            foreach (var socketClient in temp.Values)
            {
                if (socketClient == null) continue;
                socketClient.StopClient();
            }

            mAllSocketClients.Clear();
        }

        public void DisConnectSocket(string socketName)
        {
            Dictionary<string, BaseSocketClient> temp = new Dictionary<string, BaseSocketClient>(mAllSocketClients);

            foreach (var socketClient in temp.Values)
            {
                if (socketClient == null) continue;
                if (socketClient is BaseTcpClient == false) continue;
                if (socketClient.mClientName == socketName)
                {
                    (socketClient as BaseTcpClient).DisConnect();
                }
            }
        }

        public void ReConnectSocket(string socketName)
        {
            Dictionary<string, BaseSocketClient> temp = new Dictionary<string, BaseSocketClient>(mAllSocketClients);

            foreach (var socketClient in temp.Values)
            {
                if (socketClient == null) continue;
                if (socketClient is BaseTcpClient == false) continue;
                if (socketClient.mClientName == socketName)
                {
                    (socketClient as BaseTcpClient).ReConnectClient();;
                }
            }
        }


        #region 监听Socket 客户端的创建

        public T GetSocketClient<T>(string clientName) where T : BaseSocketClient
        {
            if (mAllSocketClients.TryGetValue(clientName, out var socketClient))
                return socketClient as T;
            Debug.LogError($"指定名称{clientName} 的Socket 没有创建");
            return null;
        }


//        public T GetSocketClient<T>(string clientName) where T : BaseSocketClient
//        {
//            if (mAllSocketClients.TryGetValue(clientName, out var socketClient))
//            {
//                if (socketClient.GetType() == typeof(T))
//                    return socketClient as T;
//                Debug.LogError($"创建名称{clientName} 失败，已经有一个类型为{socketClient.GetType()}");
//                return null;
//            }
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
                if (mBaseLoginTcpClient != null)
                    return mBaseLoginTcpClient;
                mBaseLoginTcpClient = NetWorkManager.S_Instance.GetSocketClient<BaseTcpClient>(BaseTcpClientName);
                if (mBaseLoginTcpClient != null)
                    return mBaseLoginTcpClient;
                mBaseLoginTcpClient = new BaseTcpClient(BaseTcpClientName, AddressFamily.InterNetwork);
                return mBaseLoginTcpClient;
            }
        }
    }
}