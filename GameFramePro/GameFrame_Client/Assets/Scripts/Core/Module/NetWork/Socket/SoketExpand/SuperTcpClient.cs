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
    public class SuperTcpClient : IDisposable, INetworkClient
    {
        //需要发送的消息队列
        protected readonly ConcurrentQueue<byte[]> mAllNeedSendMessages = new ConcurrentQueue<byte[]>();
        protected TcpClient mTcpClient { get; set; } = null;

        protected SslStream mSslNetworkStream { get; set; } = null;  
    

        protected IPEndPoint mConnectEndPoint { get; set; } //连接的远程节点

        protected bool mIsInitialed { get; set; } = false; //表示是否初始化
        protected bool mIsSendAndReceiveTaskRunning { get; set; } = false; //是否在运行接收和发送消息任务


        protected const int s_ReceiveMessageThreadInterval = 50; //接收消息的线程 Sleep 时间间隔
        protected const int s_SendMessageThreadInterval = 50; //发送消息的线程 Sleep 时间间隔

        protected int mSslStreamWriteTimeOut { get; set; } = 30; //SslStream 写数据超时时间
        protected int S_BufferSize { get; set; } = 65536; //最大接收数组长度
        protected byte[] mBuffer { get; set; }  //接收数据的缓冲数组
        public int mMaxConnectTime { get; set; } = 2000; //连接超时 毫秒

        #region 接受和发送消息的取消令牌

        protected ManualResetEvent mTimeOutResetEvent = new ManualResetEvent(false);
        public CancellationTokenSource mReceiveTaskCancleToken { get; protected set; } = new CancellationTokenSource();

        public CancellationTokenSource mSendTaskCancleToken { get; protected set; } = new CancellationTokenSource();
        #endregion


        #region 构造函数

        public SuperTcpClient(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            mTcpClient = new TcpClient(addressFamily);
            mBuffer = new byte[S_BufferSize];
        }

        public SuperTcpClient(IPEndPoint localEP)
        {
            mTcpClient = new TcpClient(localEP);
            mBuffer = new byte[S_BufferSize];
        }

        public SuperTcpClient(string hostname, int port)
        {
            mTcpClient = new TcpClient(hostname, port);
            mBuffer = new byte[S_BufferSize];
        }

        #endregion


        #region  INetworkClient 接口实现
        public event NetWorkStateDelegate OnConectEvent;
        public event NetWorkStateDelegate OnDisConectEvent;
        public event NetWorkStateDelegate OnConectErrorEvent;
        public event NetWorkMessageDelegate OnReceiveMessageEvent;
        public event NetWorkMessageDelegate OnSendMessageEvent;

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
                IAsyncResult asyncResult = mTcpClient.BeginConnect(remoteEP.Address, remoteEP.Port, OnBeginConnectCallback, mTcpClient);
                asyncResult.AsyncWaitHandle.WaitOne(mMaxConnectTime, true);
                if (asyncResult.IsCompleted == false)
                {
                    OnConnectTimeOut($"连接超时");
                    mTcpClient.Close();
                }
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
            }
        }

        #endregion

        public void StopClient()
        {
            throw new NotImplementedException();
        }

        public void SendData(byte[] data)
        {
            throw new NotImplementedException();
        }
        #endregion

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
                    OnConnectError($"连接失败");
                else
                {
                    OnConnectSuccess(string.Empty);
                    SslStream sslStream = new SslStream(conncectTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    sslStream.WriteTimeout = mSslStreamWriteTimeOut;
                   sslStream.BeginAuthenticateAsClient(mConnectEndPoint.Address.ToString(), SslAuthenCallback, sslStream);

                   // sslStream.AuthenticateAsClientAsync(mConnectEndPoint.Address.ToString(),)


                }
            }
            catch (Exception e)
            {
                OnConnectError($"OnBeginConnectCallback 异常{e}");
            }
        }

        protected virtual void SslAuthenCallback(IAsyncResult asyncResult)
        {
            SslStream sslStream = asyncResult as SslStream;
            try
            {
                sslStream.EndAuthenticateAsClient(asyncResult);
                mSslNetworkStream = sslStream;

                BeginSendAndReceiveTask();
            }
            catch (AuthenticationException e)
            {
                OnAuthenticateException($"Ssl 验证异常!!  身份验证失败。 可以使用此对象尝试重新进行身份验证 {e}");
            }
            catch (ObjectDisposedException e)
            {
                OnAuthenticateException($"Ssl 验证异常!! 此对象已关闭 {e}");
            }
            catch (InvalidOperationException e)
            {
                OnAuthenticateException($"Ssl 验证异常!!  已经验证 {e}");
            }
            catch (Exception e)
            {
                OnAuthenticateException($"Ssl 验证异常{e}");
            }
        }
        //验证SslStream
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

                if (mAllNeedSendMessages.Count > 0)
                {
                    if (mAllNeedSendMessages.TryDequeue(out var data))
                    {
                        mSslNetworkStream.Write(data, 0, data.Length);
                        //if (dataLength != data.Length)
                        //    Debug.LogError($"发送的实际数据量{dataLength} 需要发送的数据量{data.Length}");

                        Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(data, mConnectEndPoint); });
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

                    if(mSslNetworkStream==null&& mSslNetworkStream.CanRead==false)
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
                      //  IsConnected = false;
                        // receiveDataOffset = streamDataLength = 0;
                        OnBeDisConnect($"接受到数据长度为0 断开连接{mConnectEndPoint.Address.ToString()}");
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

                        Debug.Log($"解析的协议id={protocolId}   长度={receiveByteArray.mDataRealLength} ");

                      //  var receiveMessage = BaseSocketReceiveMessage.GetSocketReceiveMessageData(protocolId, receiveByteArray, mClientSocket.RemoteEndPoint);
                   //     mBaseSocketMessageManager.SaveReceiveData(receiveMessage);
                     //   OnReceiveMessage(receiveMessage);

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
                OnReceiveMessageException($"TCP 接收数据异常{e}");
            }
        }

        #endregion

        #region 封装事件接口

        //protected override void RemoveAllEvents()
        //{
        //    base.RemoveAllEvents();
        //    OnBeginConnectEvent = null;
        //    OnConnectErrorEvent = null;
        //    OnConnectTimeOutEvent = null;
        //    OnBeDisConnectEvent = null;
        //    OnDisConnectEvent = null;
        //}


        protected virtual void OnBeginConnect(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.Log(message);
                else
                    Debug.Log($"开始连接服务器");
                //     OnBeginConnectEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual void OnConnectSuccess(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.Log(message);
                else
                    Debug.Log($"连接服务器成功");

                //if (mTcpHeartbeatManager == null)
                //    Debug.LogError($"心跳包没有启动");

                //mTcpHeartbeatManager?.StartHeartbeat(this, TimeSpan.FromSeconds(5));

                //     OnConnectSuccessEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual void OnConnectError(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                else
                    Debug.Log($"连接服务器异常");
                //       OnConnectErrorEvent?.Invoke(this, message);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual void OnConnectTimeOut(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                else
                    Debug.Log($"连接服务器超时");
                //    OnConnectTimeOutEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        protected virtual void OnBeDisConnect(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                else
                    Debug.Log($"服务器断开连接");
                //mTcpHeartbeatManager.StopHearbeat();
                //OnSocketDisConnected(false);
                //OnBeDisConnectEvent?.Invoke(this);
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
                else
                    Debug.Log($"连接服务器断开");
                //mTcpHeartbeatManager.StopHearbeat();
                //OnSocketDisConnected(true);
                //OnDisConnectEvent?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError($"主动断开连接异常{e}");
            }
        }

        /// <summary>
        /// SslStream 验证失败
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnAuthenticateException(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                else
                    Debug.Log($"SslStream 验证失败");
                //       OnConnectErrorEvent?.Invoke(this, message);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 接收数据异常
        /// </summary>
        /// <param name="message"></param>
        protected virtual void OnReceiveMessageException(string message)
        {
            try
            {
                if (string.IsNullOrEmpty(message) == false)
                    Debug.LogError(message);
                else
                    Debug.Log($"接收数据异常");
                //       OnConnectErrorEvent?.Invoke(this, message);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #endregion



        public void Dispose()
        {
            mSslNetworkStream?.Dispose();
            mTcpClient?.Dispose();
        }


    }
}