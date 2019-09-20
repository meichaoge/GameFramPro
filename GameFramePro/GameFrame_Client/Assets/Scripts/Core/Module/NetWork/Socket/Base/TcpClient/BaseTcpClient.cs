using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameFramePro;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// TCP 客户端基类/// </summary>
    public class BaseTcpClient : BaseSocketClient, IConnectSocketClient
    {
        #region 属性

        protected ManualResetEvent mTimeOutResetEvent = new ManualResetEvent(false);
        public IPEndPoint mTargetEndPoint { get; protected set; } //连接的远程端口

        public int mMillisecondsTimeout { get; protected set; } //超时时间

        public bool mIsTcpHeartbeatEnable { get; } //标识是否启用tcp 心跳包
        internal TcpHeartbeatManager mTcpHeartbeatManager { get; set; } //TCP心跳管理器

        #endregion

        #region IConnectSocketClient 接口实现

        /// <summary>/// Socket 是否已经连接/// </summary>
        protected bool IsConnected { get; set; } = false;

        /// <summary>/// Socket 上一次IO的连接状态/// </summary>
        public bool mIsConnected
        {
            get
            {
                if (IsConnected == false || mClientSocket == null) return false;
                return mClientSocket.Connected;
            }
        }

        public event System.Action<BaseSocketClient> OnBeginConnectEvent; //开始连接
        public event System.Action<BaseSocketClient> OnConnectSuccessEvent; //连接成功
        public event System.Action<BaseSocketClient, string> OnConnectErrorEvent; //连接失败
        public event System.Action<BaseSocketClient> OnConnectTimeOutEvent; //连接超时
        public event System.Action<BaseSocketClient> OnBeDisConnectEvent; //连接断开(被动)
        public event System.Action<BaseSocketClient> OnDisConnectEvent; //连接断开(主动断开)

        #endregion


        #region 构造函数

        public BaseTcpClient(string clientName, AddressFamily addressFamily = AddressFamily.InterNetwork, bool isHeatbeatEnable = true) : base(clientName, addressFamily, SocketType.Stream, ProtocolType.Tcp)
        {
            mSocketClientType = SocketClientUsage.TcpClient;
            mIsTcpHeartbeatEnable = isHeatbeatEnable;

            try
            {
                mClientSocket = new Socket(addressFamily, mSocketType, mProtocolType);
                mClientSocket.Bind(new IPEndPoint(mAddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0));
                mIsSocketInitialed = true;
                if (mIsTcpHeartbeatEnable)
                    mTcpHeartbeatManager = new TcpHeartbeatManager();
            }
            catch (Exception e)
            {
                OnInitialedClientFail($"初始化 Socket 客户端 异常{e}");
            }
        }

        #endregion


        #region 对外接口

        /// <summary>/// 重连/// </summary>
        public void ReConnectClient()
        {
            if (mIsSocketInitialed == false)
            {
                Debug.LogError($"tcp 客户端还没有启动");
                return;
            }

            if (mIsConnected)
            {
                Debug.LogError($"tcp 客户端已经连接");
                return;
            }

            try
            {
                Debug.Log($"重连接-***--");
                //     Connect(mTargetEndPoint, mMillisecondsTimeout);
                ReConnect();
            }
            catch (Exception e)
            {
                Debug.LogError($"重连接失败 {e}");
            }
        }

        public void DisConnect()
        {
            if (mIsSocketInitialed == false)
            {
                Debug.LogError($"tcp 客户端还没有启动");
                return;
            }

            if (mIsConnected == false)
            {
                Debug.LogError($"tcp 客户端没有连接");
                return;
            }

            try
            {
                Debug.Log($"开始断开连接---");
                IsConnected = false;
                mClientSocket.Shutdown(SocketShutdown.Both);
                mClientSocket.BeginDisconnect(false, (asyncResult) =>
                {
                    (asyncResult.AsyncState as Socket).EndDisconnect(asyncResult);
                    Debug.Log($"断开连接---{asyncResult}");
                    OnDisConnect(null);
                }, mClientSocket);
            }
            catch (Exception e)
            {
                OnDisConnect($"断开连接失败 {e}");
            }
        }

        public void Connect(IPAddress ipAddress, int port, int millisecondsTimeout)
        {
            Connect(new IPEndPoint(ipAddress, port), millisecondsTimeout);
        }

        public virtual void Connect(IPEndPoint remoteEP, int millisecondsTimeout)
        {
            if (remoteEP == null)
                throw new ArgumentNullException($"远程连接端口参数为null");

            if (IsConnected)
            {
                Debug.LogError($"当前已经连接了Socket");
                return;
            }


            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                IsConnected = false;
                mTargetEndPoint = remoteEP;
                mMillisecondsTimeout = millisecondsTimeout;

                OnBeginConnect($"开始连接到{remoteEP}");
                mTimeOutResetEvent.Reset();

                var asyncResult = mClientSocket.BeginConnect(remoteEP, new AsyncCallback((result) =>
                {
                    Debug.Log($"连接回调 连接状态{mClientSocket.Connected}");

                    mTimeOutResetEvent.Set();

                    if (mClientSocket.Connected)
                    {
                        IsConnected = true;
                        OnConnectSuccess($"连接成功");
                        StartReceiveAndSendThread();
                    }
                    else
                    {
                        OnConnectError($"连接失败{result}");
                    }
                }), null);
                mTimeOutResetEvent.WaitOne(millisecondsTimeout, true);
                if (asyncResult.IsCompleted == false)
                    OnConnectTimeOut($"连接超时");
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                OnConnectError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                OnConnectError($"TCP 连接Socket {remoteEP} 异常，无法连接服务器{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                OnConnectError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                OnConnectError($"TCP {remoteEP} 连接失败{e}");
                throw;
            }
        }

        protected virtual void ReConnect()
        {
            if (mIsSocketInitialed == false)
            {
                Debug.LogError($"tcp 客户端还没有启动");
                return;
            }

            if (IsConnected)
            {
                Debug.LogError($"当前已经连接了Socket");
                return;
            }


            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                OnBeginConnect($"开始连接到{mTargetEndPoint}");
                mTimeOutResetEvent.Reset();

                var asyncResult = mClientSocket.BeginConnect(mTargetEndPoint, new AsyncCallback((result) =>
                {
                    Debug.Log($"连接回调 连接状态{mClientSocket.Connected}");

                    mTimeOutResetEvent.Set();

                    if (mClientSocket.Connected)
                    {
                        IsConnected = true;
                        OnConnectSuccess($"连接成功");
                        StartReceiveAndSendThread();
                    }
                    else
                    {
                        OnConnectError($"连接失败{result}");
                    }
                }), null);
                mTimeOutResetEvent.WaitOne(mMillisecondsTimeout, true);
                if (asyncResult.IsCompleted == false)
                    OnConnectTimeOut($"连接超时");
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                OnConnectError($"TCP 连接 Socket {mTargetEndPoint} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                OnConnectError($"TCP 连接Socket {mTargetEndPoint} 异常，无法连接服务器{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                OnConnectError($"TCP 连接Socket  {mTargetEndPoint} 已经关闭{e}");
            }
            catch (Exception e)
            {
                OnConnectError($"TCP {mTargetEndPoint} 连接失败{e}");
                throw;
            }
        }


        public IAsyncResult BeginConnect(IPAddress address, int port, object state)
        {
            return BeginConnect(new IPEndPoint(address, port), state);
        }

        public virtual IAsyncResult BeginConnect(IPEndPoint remoteEP, object state)
        {
            if (IsConnected)
            {
                Debug.LogError($"当前已经连接了Socket");
                return null;
            }

            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                IsConnected = false;
                mTargetEndPoint = remoteEP;
                OnBeginConnect($"异步连接{remoteEP}");
                IAsyncResult asyncResult = mClientSocket.BeginConnect(remoteEP, OnBeginConnectCallback, state);
                asyncResult.AsyncWaitHandle.WaitOne(mMaxConnectTime, true);
                if (asyncResult.IsCompleted == false)
                {
                    OnConnectTimeOut($"连接超时");
                    mClientSocket.Close();
                }

                return asyncResult;
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                OnConnectError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                OnConnectError($"TCP 连接Socket {remoteEP} 不可用{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                OnConnectError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                OnConnectError($"TCP {remoteEP} 连接失败{e}");
            }

            return null;
        }

        /// <summary>/// 发送tcp 消息/// </summary>/// <param name="protocolId">x协议id</param>
        public virtual void Send(int protocolId, ByteArray data)
        {
            if (mIsConnected == false)
            {
                OnSocketException($"发送数据失败，Socket 没有连接上");
                return;
            }

//            Debug.Log($"Send-- {protocolId}  Length={data.mDataRealLength}  data={data.mBytes}");

            ByteArray sendByteArray = ByteArray.GetByteArray();
            sendByteArray.CloneFromByteArray(data); //克隆数据 避免污染源数据

            var sendMessage = BaseSocketSendMessage.GetSocketSendMessageData(protocolId, sendByteArray, mClientSocket.RemoteEndPoint, false);
            mBaseSocketMessageManager.CacheSocketSendData(sendMessage);
        }

        #endregion

        #region 基类重写

        protected override bool mIsSendDataEnable
        {
            get
            {
                if (mIsConnected == false || mClientSocket == null) return false;
                if (mBaseSocketMessageManager.mAllSendMessageQueue == null || mBaseSocketMessageManager.mAllSendMessageQueue.Count == 0) return false;
                return true;
            }
        }

        protected override bool mIsReceiveDataEnable
        {
            get
            {
                if (mIsConnected == false || mClientSocket == null) return false;
                return true;
            }
        }

        public override void Dispose()
        {
            mTimeOutResetEvent.Dispose();
            base.Dispose();
        }

        protected override void BeginSendMessageTask(object obj)
        {
            try
            {
                while (true)
                {
                    if (mIsSendDataEnable == false)
                    {
                        Thread.Sleep(S_SendMessageThreadInterval);
                        continue;
                    }

                    if (mBaseSocketMessageManager.GetWillSendSocketData(out var message))
                    {
                        int dataLength = mClientSocket.Send(message.mSendMessageByteArray.mBytes, 0, message.mSendMessageByteArray.mDataRealLength, SocketFlags.None);

#if UNITY_EDITOR
                        int length = SocketHead.GetPacketLength(message.mSendMessageByteArray.mBytes, 0);
                        Debug.LogEditorInfor($"要发送的数据长度{message.mSendMessageByteArray.mDataRealLength} 实际发送{dataLength}  byte={message.mSendMessageByteArray}  头部标识长度{length}");
#endif
                        OnSendMessage(message);
                        BaseSocketSendMessage.RecycleSocketMessageData(message); //回收数据
                    }

                    Thread.Sleep(S_SendMessageThreadInterval);
                }
            }
            catch (ThreadAbortException e)
            {
            } //线程被Abort() 调用时候抛出
            catch (Exception e)
            {
                OnSocketException($"发送消息异常{e}");
            }
        }

        protected override void BeginReceiveMessageTask(object obj)
        {
            try
            {
                int receiveDataOffset = 0; // mBuffer 中需要解析或者保存的数据偏移
                int packageLength = 0; //需要解析的包长度
                int totalReceiveDataLength = 0; //总共接收的数据总量 (待处理的数据)
                while (true)
                {
                  
                    if (mIsReceiveDataEnable == false)
                    {
                        Thread.Sleep(S_ReceiveMessageThreadInterval);
                        continue;
                    }

                    int receiveDataLength = mClientSocket.Receive(mBuffer, receiveDataOffset, S_BufferSize, SocketFlags.None);
                    totalReceiveDataLength += receiveDataLength;
                    Debug.Log($"本次 解析接收的包长度 {receiveDataLength}      {totalReceiveDataLength}");

                    if (totalReceiveDataLength > S_BufferSize)
                        Debug.LogError($"接受的数据太多{totalReceiveDataLength} 超过{S_BufferSize}");

                    if (receiveDataLength == 0)
                    {
                        IsConnected = false;
                        // receiveDataOffset = streamDataLength = 0;
                        OnBeDisConnect($"接受到数据长度为0 断开连接{mClientSocket}");
                        break;
                        //  Thread.Sleep(S_ReceiveMessageThreadInterval);
                    }

                    if (packageLength == 0)
                        packageLength = SocketHead.GetPacketLength(mBuffer, 0); //避免如果上一次有一个半包数据在缓存中解析错误

                    if (totalReceiveDataLength < packageLength)
                    {
                        Debug.LogInfor($"数据没有接受完 继续等待接受{totalReceiveDataLength}-- {receiveDataOffset}--- {packageLength}");
                        receiveDataOffset += receiveDataLength;
                        continue;
                    } //接收到了一部分数据(分包了)

                    //** 接收到了几个包的消息(粘包了)
                    receiveDataOffset = 0; //使得能够从上一次的 mBuffer 起始位置

                    while (packageLength + receiveDataOffset <= totalReceiveDataLength)
                    {
                        ByteArray receiveByteArray = ByteArray.GetByteArray();
                        receiveByteArray.CopyBytes(mBuffer, receiveDataOffset, packageLength, packageLength, 0);

                        int protocolId = SocketHead.GetPacketProtocolID(receiveByteArray.mBytes, 4);

                        UnityEngine.Debug.Log($"解析的协议id={protocolId}   长度={receiveByteArray.mDataRealLength} ");

                        var receiveMessage = BaseSocketReceiveMessage.GetSocketReceiveMessageData(protocolId, receiveByteArray, mClientSocket.RemoteEndPoint);
                        mBaseSocketMessageManager.SaveReceiveData(receiveMessage);
                        OnReceiveMessage(receiveMessage);

                        receiveDataOffset += packageLength;

                        if (receiveDataOffset == totalReceiveDataLength)
                            break; //刚好是几个整包
                        packageLength = SocketHead.GetPacketLength(mBuffer, receiveDataOffset); //继续处理下一个包
                    } //拆包

                    if (receiveDataOffset < totalReceiveDataLength)
                    {
                        System.Array.Copy(mBuffer, receiveDataOffset, mBuffer, 0, totalReceiveDataLength - receiveDataOffset);
                        receiveDataOffset = totalReceiveDataLength - receiveDataOffset;
                    } //说明mBuffer 后面还有一小部分下一个包的数据需要移动到buffer 开始位置
                    else if (receiveDataOffset == totalReceiveDataLength)
                    {
                        receiveDataOffset = 0;
                    } //所有的数据刚好是整包
                    else
                    {
                        Debug.LogError($"不可能出现的异常");
                    }

                    packageLength = 0;
                    totalReceiveDataLength = receiveDataOffset;
                }

                Thread.Sleep(S_ReceiveMessageThreadInterval);
            }
            catch (ThreadAbortException e)
            {
            } //线程被Abort() 调用时候抛出
            catch (Exception e)
            {
                OnSocketException($"TCP 接收数据异常{e}");
            }
        }

        public override void StopClient()
        {
            if (mIsTcpHeartbeatEnable)
                mTcpHeartbeatManager?.StopHearbeat();
            base.StopClient();
        }

        #endregion

        #region 内部实现

        protected virtual void OnBeginConnectCallback(IAsyncResult asyncResult)
        {
            mClientSocket.EndConnect(asyncResult);
            if (mClientSocket.Connected == false)
            {
                OnConnectError($"连接失败了");
            }
            else
            {
                IsConnected = true;
                OnConnectSuccess($"连接成功");
                StartReceiveAndSendThread();
            }
        }


        /// <summary>/// tcp socket 断开后必须重新创建Socket ，
        /// 不能直接复用,否则Soceket 异常（ystem.Net.Sockets.SocketException (0x80004005)在一个已经连接的套接字上做了一个连接请求）：/// </summary>
        protected virtual void OnSocketDisConnected(bool isNeedReconnected)
        {
            mClientSocket = new Socket(mAddressFamily, mSocketType, mProtocolType);
        }

        #endregion

        #region 封装事件接口

        protected override void RemoveAllEvents()
        {
            base.RemoveAllEvents();
            OnBeginConnectEvent = null;
            OnConnectErrorEvent = null;
            OnConnectTimeOutEvent = null;
            OnBeDisConnectEvent = null;
            OnDisConnectEvent = null;
        }


        protected virtual void OnBeginConnect(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.Log(message);
                OnBeginConnectEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        protected virtual void OnConnectSuccess(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.Log(message);


                if (mTcpHeartbeatManager == null)
                    Debug.LogError($"心跳包没有启动");

                mTcpHeartbeatManager?.StartHeartbeat(this, TimeSpan.FromSeconds(5));

                OnConnectSuccessEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        protected virtual void OnConnectError(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                OnConnectErrorEvent?.Invoke(this, message);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        protected virtual void OnConnectTimeOut(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                OnConnectTimeOutEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                global::Debug.LogError(e);
            }
        }

        protected virtual void OnBeDisConnect(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                mTcpHeartbeatManager.StopHearbeat();
                OnSocketDisConnected(false);
                OnBeDisConnectEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError($"被动断开连接异常{e}");
            }
        }

        //主动断开
        protected virtual void OnDisConnect(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                mTcpHeartbeatManager.StopHearbeat();
                OnSocketDisConnected(true);
                OnDisConnectEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError($"主动断开连接异常{e}");
            }
        }

        #endregion
    }
}