using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameFramePro;
using UnityEngine;

public class TcpServer : IDisposable
{
    protected class TcpSocketMessage
    {
        public byte[] Message;

        public Socket mRemoteSocket;
        //     public bool mIsIpv4;
    }

    protected class TcpSocketClient
    {
        public Socket mTcpClient;
        public byte[] mReceiveBuffer;
        public bool mIsConnect = true;

        public TcpSocketClient(Socket client)
        {
            mTcpClient = client;
            mReceiveBuffer = new byte[S_BufferSize];
        }

    }

    protected ConcurrentDictionary<Socket, TcpSocketClient> mAllConnectTcpClients = new ConcurrentDictionary<Socket, TcpSocketClient>();

    #region 属性

    protected Socket mIpv4Socket = null; //服务器需要同时支持IPV4 和IPV6
    //   protected Socket mIpv6Socket = null;

    public int EndPort { get; protected set; } //端口号
    public bool mIsTcpServerStart { get; protected set; } //标示是否已经启动了UDP 服务器


    protected const int S_BufferSize = 65536;
    protected readonly byte[] mBuffer = new byte[S_BufferSize];
    protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mReceiveMessageThread = null; //接收消息的后台线程
    public SocketMessageDelagate OnReceiveMessageEvent;

    protected ConcurrentQueue<TcpSocketMessage> mAllWillSendoutDatas; //线程安全的队列
    protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mSendMessageThread = null; //发送消息的后台线程
    public SocketMessageDelagate OnSendMessageEvent;


    protected int mMaxTcpClient = 100;

    #endregion


    #region 构造函数

    public TcpServer(int port)
    {
        EndPort = port;
    }

    #endregion

    #region 接口

    public void StartServer()
    {
        if (mIsTcpServerStart)
        {
            Debug.LogError("UDP 服务器已经启动");
            return;
        }

        try
        {
            //IPv4 
            if (Socket.OSSupportsIPv4)
            {
                mIpv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mIpv4Socket?.Bind(new IPEndPoint(IPAddress.Any, EndPort));

                //   Thread.Sleep(TimeSpan.FromSeconds(15));
                mIpv4Socket?.Listen(mMaxTcpClient);
                Debug.Log($"服务器启动支持IPv4 的Socket,绑定端口{EndPort}");
            }
            else
            {
                Debug.LogError($"当前不支持IPV4");
            }

            //            //IPv6 
            //            if (Socket.OSSupportsIPv6)
            //            {
            //                mIpv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            //                mIpv6Socket?.Bind(new IPEndPoint(IPAddress.IPv6Any, EndPort));
            //                Debug.Log($"服务器启动支持IPv6 的Socket,绑定端口{EndPort}");
            //            }


            mAllWillSendoutDatas = new ConcurrentQueue<TcpSocketMessage>();

            mReceiveMessageThread = new Thread(BeginReceiveMessage) { IsBackground = true };
            mReceiveMessageThread.IsBackground = true;
            mReceiveMessageThread.Start();

            mSendMessageThread = new Thread(BeginSendMessage) { IsBackground = true };
            mSendMessageThread.IsBackground = true;
            mSendMessageThread.Start();

            mIsTcpServerStart = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"启动UDP服务器异常 {e}");
            throw;
        }
    }

    public void StopServer()
    {
        if (mIsTcpServerStart == false)
            return;

        try
        {
            mAllWillSendoutDatas = null;
            OnReceiveMessageEvent = OnSendMessageEvent = null;

            mReceiveMessageThread.Abort();
            mSendMessageThread.Abort();

            mIpv4Socket?.Close();
            // mIpv6Socket?.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"结束UDP服务器异常 {e}");
            throw;
        }
        finally
        {
            mIsTcpServerStart = false;
        }
    }


    public void SendMessage(string message, Socket remoteSocket, bool isIPv4 = true)
    {
        SendMessage(Encoding.UTF8.GetBytes(message), remoteSocket, isIPv4);
    }

    public void SendMessage(byte[] message, Socket remoteSocket, bool isIPv4 = true)
    {
        TcpSocketMessage messageDat = new TcpSocketMessage()
        {
            Message = message,
            mRemoteSocket = remoteSocket,
            //     mIsIpv4 = isIPv4,
        };

        mAllWillSendoutDatas.Enqueue(messageDat);
    }

    #endregion


    #region 内部实现

    /// <summary>/// 发送消息的线程/// </summary>
    private void BeginSendMessage(object obj)
    {
        try
        {
            while (true)
            {
                if (mAllWillSendoutDatas.Count == 0)
                    continue;

                if (mAllWillSendoutDatas.TryDequeue(out var messageData))
                {
                    int sendDataLength = 0;


                    Socket targetSocket = mIpv4Socket; //messageData.mIsIpv4 ? mIpv4Socket : mIpv6Socket;

                    if (targetSocket == null)
                    {
                        Debug.LogError($"当前的Socket 对象创建失败 是否是IPv4:{true}");
                        continue;
                    }

                    sendDataLength = targetSocket.Send(messageData.Message, 0, messageData.Message.Length, SocketFlags.None);


                    if (sendDataLength != messageData.Message.Length)
                        Debug.LogError($"数据发送异常，发送的数据量不一致{sendDataLength}  {messageData.Message.Length}");

                    Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(Encoding.UTF8.GetString(messageData.Message), messageData.mRemoteSocket.LocalEndPoint); });

                    Debug.Log($"发送{messageData.mRemoteSocket.LocalEndPoint} 消息 {Encoding.UTF8.GetString(messageData.Message)}");
                }
                Thread.Sleep(S_SendMessageThreadInterval);
            }
        }
        catch (ThreadAbortException e)
        {

        }//线程结束时候回抛出异常
        catch (Exception e)
        {
            Debug.LogError($"发现消息异常{e.ToString()}");
            throw;
        }
    }

    /// <summary>/// 接受消息的线程/// </summary>
    private void BeginReceiveMessage(object obj)
    {
        try
        {
            while (true)
            {
                if (mIpv4Socket == null || mIsTcpServerStart == false)
                {
                    Thread.Sleep(S_ReceiveMessageThreadInterval);
                    continue;
                }

                foreach (var socket in mAllConnectTcpClients.Values)
                {
                    if (socket.mTcpClient == null) continue;
                    if (socket.mIsConnect == false) continue;
                    ConnectSocketBeginReceive(socket.mTcpClient);
                }
                mIpv4Socket.BeginAccept(OnBeginAccept, mIpv4Socket);
                Thread.Sleep(50);
            }
        }
        catch (ThreadAbortException e)
        {
        }//线程结束时候回抛出异常
        catch (Exception e)
        {
            Debug.LogError($"接受数据异常 {e}");
            throw;
        }
    }

    protected void OnBeginAccept(IAsyncResult asyncResult)
    {
        Socket clientSocket = (asyncResult.AsyncState as Socket).EndAccept(asyncResult);
        mAllConnectTcpClients.TryAdd(clientSocket, new TcpSocketClient(clientSocket));

        Debug.Log($"客户端{clientSocket.RemoteEndPoint} 连接服务器");
    }

    protected void ConnectSocketBeginReceive(Socket socket)
    {
        if (mAllConnectTcpClients.TryGetValue(socket, out var socketClient))
        {
            socket.BeginReceive(socketClient.mReceiveBuffer, 0, S_BufferSize, SocketFlags.None, OnReceiveCallback, socketClient);
        }

    }

    protected void OnReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            TcpSocketClient tcpSocketClient = asyncResult.AsyncState as TcpSocketClient;
            int dataLength = tcpSocketClient.mTcpClient.EndReceive(asyncResult);//            socket.Receive(mBuffer, 0, S_BufferSize, SocketFlags.None);
            if (dataLength > 0)
            {
                if (dataLength > S_BufferSize)
                    Debug.LogError($"接收的数据{dataLength}超过最大的接受上限{S_BufferSize}");

                string message = Encoding.UTF8.GetString(tcpSocketClient.mReceiveBuffer, 0, dataLength);
                Debug.Log($"接受到来自{tcpSocketClient.mTcpClient.RemoteEndPoint} 的消息{message}");

                Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(message, tcpSocketClient.mTcpClient.LocalEndPoint); });
            }
            else
            {
                Debug.LogError($"接受到{tcpSocketClient.mTcpClient.RemoteEndPoint} 的数据长度为0是否已经断开连接");
                tcpSocketClient.mIsConnect = false;
                mAllConnectTcpClients.TryRemove(tcpSocketClient.mTcpClient, out var client);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"数据接受异常{e}");
            throw;
        }

    }


    #endregion

    public void Dispose()
    {
        mIpv4Socket?.Dispose();
        //     mIpv6Socket?.Dispose();
    }
}