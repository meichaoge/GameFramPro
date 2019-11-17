using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 封装 TcpClient 的 TCP客户端
    /// </summary>
    public class SuperTcpClient : IDisposable, IConnectedSocketClient
    {
        #region TCP客户端配置
        protected TcpClient mTcpClient { get; set; } = null;

        protected SslStream mSslNetworkStream { get; set; } = null;

        protected IPEndPoint mConnectEndPoint { get; set; } //连接的远程节点

        protected bool mIsSendAndReceiveTaskRunning { get; set; } = false; //是否在运行接收和发送消息任务

        #endregion

        #region 数据收发配置

        protected const int s_ReceiveMessageThreadInterval = 50; //接收消息的线程 Sleep 时间间隔
        protected const int s_SendMessageThreadInterval = 50; //发送消息的线程 Sleep 时间间隔

        protected int mSslStreamWriteTimeOut { get; set; } = 30; //SslStream 写数据超时时间
        protected int S_BufferSize { get; set; } = 65536; //最大接收数组长度
        protected byte[] mBuffer { get; set; }  //接收数据的缓冲数组
        public int mMaxConnectTime { get; set; } = 2000; //连接超时 毫秒

        /// <summary>
        /// 消息缓冲管理
        /// </summary>
        internal BaseSocketMessageManager mBaseSocketMessageManager { get; set; } = new BaseSocketMessageManager();

        //接受和发送消息的取消令牌
        //    protected ManualResetEvent mTimeOutResetEvent = new ManualResetEvent(false);
        public CancellationTokenSource mReceiveTaskCancleToken { get; protected set; } = new CancellationTokenSource();

        public CancellationTokenSource mSendTaskCancleToken { get; protected set; } = new CancellationTokenSource();

        #endregion


        #region 构造函数

        public SuperTcpClient(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            mTcpClient = new TcpClient(addressFamily);
            mBuffer = new byte[S_BufferSize];
            ClientState = NetWorkStateUsage.Running;
        }

        public SuperTcpClient(IPEndPoint localEP)
        {
            mTcpClient = new TcpClient(localEP);
            mBuffer = new byte[S_BufferSize];
            ClientState = NetWorkStateUsage.Running;
        }

        public SuperTcpClient(string hostname, int port)
        {
            mTcpClient = new TcpClient(hostname, port);
            mBuffer = new byte[S_BufferSize];
            ClientState = NetWorkStateUsage.Running;
        }

        #endregion

        #region IConnectedSocketClient
        public NetWorkStateUsage ClientState { get; protected set; } = NetWorkStateUsage.None;

        protected bool mIsConnected { get; set; } = false;

        public bool IsStopClient { get; protected set; } = false;

        /// <summary>
        /// 表示是否连接成功
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (mSslNetworkStream == null || mSslNetworkStream.IsAuthenticated == false)
                    return false;
                return mTcpClient.Connected && mIsConnected;
            }
        }

        public event System.Action<ISocketClient2, NetWorkStateUsage, string> OnSocketStateChangeEvent; //Socket State 改变
        public event System.Action<ISocketClient2, BaseSocketSendMessage> OnSendMessageEvent; //发送消息
        public event System.Action<ISocketClient2, BaseSocketReceiveMessage> OnReceiveMessageEvent; //接受到消息 

        #endregion

        #region 链接远程服务器


        public virtual void ConnectSync(IPEndPoint remoteEP) { }

        /// <summary>
        /// 异步链接远程服务器
        /// </summary>
        /// <param name="remoteEP"></param>
        public virtual void ConnectAsync(IPEndPoint remoteEP)
        {
            if (remoteEP == null)
            {
                Debug.LogError($"Tcp 链接异常，参数不合法 ");
                return;
            }

            try
            {
                mIsConnected = false;
                IsStopClient = false;

                ClientState = NetWorkStateUsage.Connecting;
                mConnectEndPoint = remoteEP;
                IAsyncResult asyncResult = mTcpClient.BeginConnect(remoteEP.Address, remoteEP.Port, OnBeginConnectCallback, mTcpClient);
                asyncResult.AsyncWaitHandle.WaitOne(mMaxConnectTime, true);
                if (asyncResult.IsCompleted == false)
                {
                    //  OnConnectTimeOut($"连接超时");
                    mTcpClient.Close();
                    ClientState = NetWorkStateUsage.ConnectedTimeOut;
                    OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接超时");
                }
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                ClientState = NetWorkStateUsage.Error;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                ClientState = NetWorkStateUsage.Error;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接Socket {remoteEP} 异常，无法连接服务器{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                ClientState = NetWorkStateUsage.Error;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                ClientState = NetWorkStateUsage.Error;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP {remoteEP} 连接失败{e}");
            }
        }

        #endregion

        public void StopClient()
        {
            try
            {
                mIsConnected = false;
                IsStopClient = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"StopClient 异常{e}");
            }
        }

        public void SendData(int protocolID,byte[] data)
        {
            if (data == null || data.Length == 0)
                return;

            ByteArray byteArray = ByteArray.GetByteArray();
            byteArray.CopyBytes(data, 0, data.Length, 0);

            var socketSendMessage =  BaseSocketSendMessage.GetSocketSendMessageData(protocolID, byteArray, mConnectEndPoint,false);
            mBaseSocketMessageManager.CacheSocketSendData(socketSendMessage);
        }

        #region 内部实现
        /// <summary>
        /// 连接服务器回调
        /// </summary>
        /// <param name="ar"></param>
        protected virtual void OnBeginConnectCallback(IAsyncResult ar)
        {
            try
            {
                TcpClient conncectTcpClient = ar.AsyncState as TcpClient;

                conncectTcpClient.EndConnect(ar);
                if (conncectTcpClient.Connected == false)
                {
                    ClientState = NetWorkStateUsage.ConnectedFail;
                    OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接失败");
                }
                else
                {
                    mIsConnected = true;
                    ClientState = NetWorkStateUsage.Conected;
                    OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 连接成功");

                    SslStream sslStream = new SslStream(conncectTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    sslStream.WriteTimeout = mSslStreamWriteTimeOut;
                    sslStream.BeginAuthenticateAsClient(mConnectEndPoint.Address.ToString(), SslAuthenCallback, sslStream);
                    // sslStream.AuthenticateAsClientAsync(mConnectEndPoint.Address.ToString(),)
                }
            }
            catch (Exception e)
            {
                ClientState = NetWorkStateUsage.Error;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"OnBeginConnectCallback 异常{e}");
            }
        }

        protected virtual void SslAuthenCallback(IAsyncResult asyncResult)
        {
            SslStream sslStream = asyncResult as SslStream;
            try
            {
                sslStream.EndAuthenticateAsClient(asyncResult);
                mSslNetworkStream = sslStream;

                ClientState = NetWorkStateUsage.AuthenSuccess;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"TCP 认证成功");

                BeginSendAndReceiveTask();
            }
            catch (AuthenticationException e)
            {
                ClientState = NetWorkStateUsage.AuthenException;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"Ssl 验证异常!!  身份验证失败。 可以使用此对象尝试重新进行身份验证 {e}");
            }
            catch (ObjectDisposedException e)
            {
                ClientState = NetWorkStateUsage.AuthenException;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"Ssl 验证异常!! 此对象已关闭 {e}");
            }
            catch (InvalidOperationException e)
            {
                ClientState = NetWorkStateUsage.AuthenException;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"Ssl 验证异常!!  已经验证 {e}");
            }
            catch (Exception e)
            {
                ClientState = NetWorkStateUsage.AuthenException;
                OnSocketStateChangeEvent?.Invoke(this, ClientState, $"Ssl 验证异常{e}");
            }
        }

        /// <summary>
        /// 验证SslStream
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        protected virtual void BeginSendAndReceiveTask()
        {
            if (mIsSendAndReceiveTaskRunning)
                return;
            Task.Factory.StartNew(BeginReceiveMessageTask, mReceiveTaskCancleToken, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(BeginSendMessageTask, mSendTaskCancleToken, TaskCreationOptions.LongRunning);
            mIsSendAndReceiveTaskRunning = true;
        }

        protected void BeginSendMessageTask(object obj)
        {
            while (true)
            {
                if (mSslNetworkStream == null && mSslNetworkStream.CanWrite == false)
                {
                    Thread.Sleep(s_SendMessageThreadInterval);
                    continue;
                }
                if (IsStopClient)
                {
                    CloseClient();
                    break;
                }

                if (mBaseSocketMessageManager != null && mBaseSocketMessageManager.mAllSendMessageQueue.Count > 0)
                {
                    var sendData = mBaseSocketMessageManager.GetWillSendSocketData();
                    if (sendData != null)
                    {
                        mSslNetworkStream.Write(sendData.mSendMessageByteArray.mBytes, 0, sendData.mSendMessageByteArray.mDataRealLength);
                        Debug.Log($"发送的实际数据量{sendData.mSendMessageByteArray.mDataRealLength} ");
                        OnSendMessageEvent?.Invoke(this, sendData);
                    }
                }
                Thread.Sleep(s_SendMessageThreadInterval);
            }
        }

        protected void BeginReceiveMessageTask(object obj)
        {
            try
            {
                int receiveDataOffset = 0; // mBuffer 中需要解析或者保存的数据偏移
                int packageLength = 0; //需要解析的包长度
                int totalReceiveDataLength = 0; //总共接收的数据总量 (待处理的数据)
                while (true)
                {
                    if (mSslNetworkStream == null || mSslNetworkStream.CanRead == false)
                    {
                        Thread.Sleep(s_ReceiveMessageThreadInterval);
                        continue;
                    }
                    //if (mIsReceiveDataEnable == false)
                    //{
                    //    Thread.Sleep(s_ReceiveMessageThreadInterval);
                    //    continue;
                    //}

                    int receiveDataLength = mSslNetworkStream.Read(mBuffer, receiveDataOffset, S_BufferSize);
                    totalReceiveDataLength += receiveDataLength;
                    Debug.Log($"本次 解析接收的包长度 {receiveDataLength}      {totalReceiveDataLength}");

                    if (totalReceiveDataLength > S_BufferSize)
                        Debug.LogError($"接受的数据太多{totalReceiveDataLength} 超过{S_BufferSize}");

                    if (receiveDataLength == 0)
                    {
                        // receiveDataOffset = streamDataLength = 0;
                        mIsConnected = false;
                        ClientState = NetWorkStateUsage.DisConnect;
                        OnSocketStateChangeEvent?.Invoke(this, ClientState, $"接受到数据长度为0 断开连接{mConnectEndPoint.Address.ToString()}");
                        break;
                    }
                    if (mBuffer.Length < 4)
                        continue; //确保能够获取到数据长度

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
                        receiveByteArray.CopyBytes(mBuffer, receiveDataOffset, packageLength,  0);
                        if (receiveByteArray.mBytes.Length < 8)
                            break;     //数据不够解析出协议id

                        int protocolId = SocketHead.GetPacketProtocolID(receiveByteArray.mBytes, 4);
                        Debug.Log($"解析的协议id={protocolId}   长度={receiveByteArray.mDataRealLength} ");

                        BaseSocketReceiveMessage receiveMessage = BaseSocketReceiveMessage.GetSocketReceiveMessageData(protocolId, receiveByteArray, null);
                        mBaseSocketMessageManager.SaveReceiveData(receiveMessage);

                        OnReceiveMessageEvent?.Invoke(this, receiveMessage);
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

                Thread.Sleep(s_ReceiveMessageThreadInterval);
            }
            catch (ThreadAbortException e)
            {
            } //线程被Abort() 调用时候抛出
            catch (Exception e)
            {
                Debug.LogError($"TCP 接收数据异常{e}");
            }
        }

        #endregion


       protected virtual void CloseClient()
        {
            try
            {
                ClientState = NetWorkStateUsage.None;
                mSslNetworkStream?.Flush();
                mTcpClient?.Close();
                mIsConnected = false;
                mReceiveTaskCancleToken.Cancel();
                mSendTaskCancleToken.Cancel();

                OnSocketStateChangeEvent?.Invoke(this, ClientState, string.Empty);
            }
            catch (Exception e)
            {
                Debug.LogError($"结束客户端异常{e}");
            }
        }


        public void Dispose()
        {
            mSslNetworkStream?.Dispose();
            mTcpClient?.Dispose();
        }


    }
}