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
using GameFramePro.NetWorkEx;


namespace System.Net.Sockets
{
    /// <summary>/// Tcp 客户端/// </summary>
    public class SimpleTcpClient : IDisposable
    {
        #region 属性

        public Socket mClientSocket { get; protected set; } = null; //可能是IPv4 或者 ipv6
        public AddressFamily mAddressFamily { get; protected set; }

        public int mMaxConnectTime = 2000; //连接超时 毫秒

        protected ManualResetEvent mTimeOutResetEvent = new ManualResetEvent(false);

        #endregion


        #region 状态+数据

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


        protected ConcurrentQueue<byte[]> mByteMessages; //线程安全的队列

        protected const int S_BufferSize = 65536;
        protected readonly byte[] mBuffer = new byte[S_BufferSize];
        protected Thread mReceiveMessageThread = null;

        public SocketMessageDelagate OnReceiveMessageEvent;
        //    protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔

        //     protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
        protected Thread mSendMessageThread = null;
        public SocketMessageDelagate OnSendMessageEvent;

        #endregion


        #region 构造函数

        public SimpleTcpClient(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            mAddressFamily = addressFamily;
            mByteMessages = new ConcurrentQueue<byte[]>();
            try
            {
                mClientSocket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                mClientSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
            }
            catch (Exception e)
            {
                Debug.LogError($"初始化 SimpleTcpClient 异常{e}");
                throw;
            }
        }

        #endregion

        #region 对外接口

        public void Connect(IPAddress ipAddress, int port, int timeOut)
        {
            Connect(new IPEndPoint(ipAddress, port), timeOut);
        }

        public void Connect(IPEndPoint remoteEP, int timeOut)
        {
            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");


                var asyncResult = mClientSocket.BeginConnect(remoteEP, new AsyncCallback((result) =>
                {
                    Debug.Log($"连接回调 连接状态{mClientSocket.Connected}");

                    mTimeOutResetEvent.Set();

                    if (mClientSocket.Connected)
                    {
                        Debug.LogError($"连接成功");
                        StartReceiveAndSendThread();
                    }
                    else
                    {
                        //  System.Net.Sockets.SocketAsyncResult res = result as System.Net.Sockets.SocketAsyncResult;
                        Debug.LogError($"连接失败{result}");
                        
                        Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(Encoding.UTF8.GetBytes($"连接失败{result}"), null); });
                    }
                }), null);
                mTimeOutResetEvent.WaitOne(timeOut, true);
                if (asyncResult.IsCompleted == false)
                {
                    Debug.LogError($"连接超时");
                }
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.LogError($"TCP 连接Socket {remoteEP} 异常，无法连接服务器{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                Debug.LogError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TCP {remoteEP} 连接失败{e}");
                throw;
            }
        }


        public IAsyncResult BeginConnect(IPAddress address, int port, object state)
        {
            return BeginConnect(new IPEndPoint(address, port), state);
        }

        public IAsyncResult BeginConnect(IPEndPoint remoteEP, object state)
        {
            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                IAsyncResult asyncResult = mClientSocket.BeginConnect(remoteEP, OnBeginConnectCallback, state);
                asyncResult.AsyncWaitHandle.WaitOne(mMaxConnectTime, true);
                if (asyncResult.IsCompleted == false)
                {
                    mClientSocket.Close();
                    Debug.LogError($"连接超时");
                }

                return asyncResult;
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.LogError($"TCP 连接Socket {remoteEP} 不可用{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                Debug.LogError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TCP {remoteEP} 连接失败{e}");
                throw;
            }

            return null;
        }

        public void Send(byte[] data)
        {
            if (mIsConnected == false)
            {
                Debug.LogError($"发送数据失败，Socket 没有连接上");
                return;
            }

            mByteMessages.Enqueue(data);
        }

        public void StopClient()
        {
            if (mIsConnected)
                mClientSocket?.Disconnect(false);
            mClientSocket?.Close();
            mByteMessages = null;
        }

        #endregion


        #region 内部实现

        private void StartReceiveAndSendThread()
        {
            IsDisConnect = false;

            mReceiveMessageThread = new Thread(BeginReceiveMessageThread);
            mReceiveMessageThread.IsBackground = true;
            mReceiveMessageThread.Start();

            mSendMessageThread = new Thread(BeginSendMessageThread);
            mSendMessageThread.IsBackground = true;
            mSendMessageThread.Start();
        }


        private void OnBeginConnectCallback(IAsyncResult asyncResult)
        {
            mClientSocket.EndConnect(asyncResult);
            if (mClientSocket.Connected == false)
                Debug.LogError($"连接失败了");

            StartReceiveAndSendThread();
        }

        private void BeginSendMessageThread(object obj)
        {
            try
            {
                while (true)
                {
                    if (mIsConnected == false)
                        continue;

                    if (mClientSocket == null || mByteMessages == null || mByteMessages.Count == 0)
                        continue;

                    if (mByteMessages.TryDequeue(out var message))
                    {
                        Debug.Log($"给{mClientSocket.RemoteEndPoint}发送消息{Encoding.UTF8.GetString(message)}");
                        int dataLength = mClientSocket.Send(message);

                        Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(message, mClientSocket.RemoteEndPoint); });
                    }
                }
            }
            catch (ThreadAbortException e)
            {
            } //线程被Abort() 调用时候抛出
            catch (Exception e)
            {
                Debug.LogError($"发送消息异常{e}");
                throw;
            }
        }


        private void BeginReceiveMessageThread(object obj)
        {
            try
            {
                while (true)
                {
                    if (mIsConnected == false || mClientSocket == null)
                        continue;

                    //                if (mClientSocket.Poll(-1, SelectMode.SelectError))
                    //                {
                    //                    Debug.LogError($"连接异常");
                    //                }

                    if (mClientSocket.Poll(100, SelectMode.SelectRead))
                    {
                        int receiveData = mClientSocket.Receive(mBuffer, 0, S_BufferSize, SocketFlags.None);

                        if (receiveData > S_BufferSize)
                            Debug.LogError($"接受的数据太多{receiveData} 超过{S_BufferSize}");

                        if (receiveData == 0)
                        {
                            Debug.LogError($"接受到数据长度为0 断开连接{mClientSocket}");
                            IsDisConnect = true;
                            //Thread.Sleep(100);
                            continue;
                        }

                        string message = Encoding.UTF8.GetString(mBuffer, 0, receiveData);
                        Debug.Log($"接受来自{mClientSocket.RemoteEndPoint} {message}");
                        Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(mBuffer, mClientSocket.RemoteEndPoint); });
                    }
                    //else
                    //{
                    //    Debug.LogError($"Socket 连接异常");
                    //}

                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException e)
            {
            } //线程被Abort() 调用时候抛出
            catch (Exception e)
            {
                Debug.LogError($"TCP 接受数据异常{e}");
                throw;
            }
        }

        #endregion


        public void Dispose()
        {
            mClientSocket?.Dispose();
        }
    }
}