﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// Socket 客户端类型/// </summary>
    public enum SocketClientUsage
    {
        TcpClient,
        UdpClient,
    }


    /// <summary>  /// 各种Socket 客户端基类 /// </summary>
    public abstract class BaseSocketClient: ISocketClient, IDisposable 
    {
        #region Socket 属性

        public SocketClientUsage mSocketClientType { get; protected set; } //客户端类型

        public Socket mClientSocket { get; protected set; } = null; //可能是IPv4 或者 ipv6
        public AddressFamily mAddressFamily { get; protected set; }
        public SocketType mSocketType { get; protected set; }
        public ProtocolType mProtocolType { get; protected set; }
        

        #endregion

        #region 状态

        /// <summary>/// 表示Socket 客户端是否 完成初始化绑定指定端口/// </summary>
        public bool mIsSocketInitialed { get; protected set; } = false;

        /// <summary>/// 标识是否有数据可以发送/// </summary>
        protected virtual bool mIsSendDataEnable
        {
            get
            {
                if (mIsConnected == false || mClientSocket == null) return false;
                if (mBaseSocketMessageManager.mAllSendMessageQueue == null || mBaseSocketMessageManager.mAllSendMessageQueue.Count == 0) return false;
                return true;
            }
        }

        /// <summary>/// 标识是否可以接收数据/// </summary>
        protected virtual bool mIsReceiveDataEnable
        {
            get
            {
                if (mIsConnected == false || mClientSocket == null) return false;
                return true;
            }
        }


        /// <summary>/// Socket 是否已经断开连接/// </summary>
        protected bool IsDisConnect = false;

        /// <summary>/// Socket 上一次IO的连接状态/// </summary>
        public bool mIsConnected
        {
            get
            {
                if (IsDisConnect) return false;
                return mClientSocket == null ? false : mClientSocket.Connected;
            }
        }

        #endregion


        #region 属性

        public string mClientName { get;  } //用于标识是哪个Sicket 客户端

        internal BaseSocketMessageManager mBaseSocketMessageManager { get; set; } //消息缓冲管理

        public int mMaxConnectTime = 2000; //连接超时 毫秒

        protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
        protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔


        protected int S_BufferSize = 65536;
        protected readonly byte[] mBuffer;
        protected Thread mReceiveMessageThread = null;

        protected Thread mSendMessageThread = null;

        #endregion


        #region INetWorkEventCallback 接口实现

        public event System.Action<BaseSocketClient> OnInitialedFailEvent; //创建Socket 失败
        public event System.Action<BaseSocketClient, string> OnSocketErrorEvent; //Socket异常败

        public event System.Action<BaseSocketClient, BaseSocketSendMessage> OnSendMessageEvent; //发送消息
        public event System.Action<BaseSocketClient, BaseSocketReceiveMessage> OnReceiveMessageEvent; //接受到消息

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
            mClientSocket?.Dispose();
        }

        #endregion

        #region 构造函数

        public BaseSocketClient(string clientName,AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            mClientName = clientName;
            mAddressFamily = addressFamily;
            mSocketType = socketType;
            mProtocolType = protocolType;

            mBaseSocketMessageManager = new BaseSocketMessageManager();
            mBuffer = new byte[S_BufferSize];

            NetWorkManager.S_Instance.RegisterSocketClient(this);
        }

        #endregion


        #region 接口

        public virtual void StopClient()
        {
            try
            {
                Debug.Log($"关闭客户端{mClientSocket.LocalEndPoint}");
                
                RemoveAllEvents();
                NetWorkManager.S_Instance.UnRegisterSocketClient(this);

                mReceiveMessageThread.Abort();
                mSendMessageThread.Abort();

                mClientSocket?.Shutdown(SocketShutdown.Both);
                mClientSocket?.Close();
                mBaseSocketMessageManager?.ClearData();
            }
            catch (Exception e)
            {
                OnSocketException($"Socket 关闭异常{e}");
            }
        }

        #endregion

        #region 内部实现

        /// <summary>   /// 启动收发消息的线程   /// </summary>
        protected virtual void StartReceiveAndSendThread()
        {
            Debug.LogInfor($"启动Socket 客户端接收和发送数据线程");

            IsDisConnect = false;

            mReceiveMessageThread = new Thread(BeginReceiveMessageThread);
            mReceiveMessageThread.IsBackground = true;
            mReceiveMessageThread.Start();

            mSendMessageThread = new Thread(BeginSendMessageThread);
            mSendMessageThread.IsBackground = true;
            mSendMessageThread.Start();
        }

        /// <summary>/// 接收消息的线程/// </summary>
        protected abstract void BeginReceiveMessageThread(object obj);


        /// <summary>/// 发送消息的线程/// </summary>
        protected abstract void BeginSendMessageThread(object obj);

        #endregion

        #region 事件触发封装

        /// <summary>/// 清理事件/// </summary>
        protected virtual void RemoveAllEvents()
        {
            OnInitialedFailEvent = null;
            OnSocketErrorEvent = null;
            OnSendMessageEvent = null;
            OnReceiveMessageEvent = null;
        }


        /// <summary>/// 初始化Socket 客户端异常/// </summary>
        protected virtual void OnInitialedClientFail(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                mIsSocketInitialed = false;
                OnInitialedFailEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }


        protected virtual void OnSocketException(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                OnSocketErrorEvent?.Invoke(this, message);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        /// <summary>/// 发送消息事件 /// </summary>
        protected virtual void OnSendMessage(BaseSocketSendMessage messageData)
        {
            try
            {
#if UNITY_EDITOR
          //     Debug.Log($"[{DateTime.Now}]给{mClientSocket.RemoteEndPoint}发送消息{Encoding.UTF8.GetString(messageData.mSendMessageByteArray.mBytes)}");
#endif

                OnSendMessageEvent?.Invoke(this, messageData);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        /// <summary>/// 发送消息事件 /// </summary>
        protected virtual void OnReceiveMessage(BaseSocketReceiveMessage messageData)
        {
            try
            {
#if UNITY_EDITOR
             //   Debug.Log($"[{DateTime.Now}]接收到消息{Encoding.UTF8.GetString(messageData.mReceiveMessageByteArray.mBytes)}");
#endif

                OnReceiveMessageEvent?.Invoke(this, messageData);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        #endregion
    }
}