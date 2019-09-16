using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameFramePro;
using GameFramePro.NetWorkEx;
using UnityEngine;

public class TcpServer : IDisposable
{
    protected class TcpSocketMessage
    {
        //   public byte[] Message;
        public ByteArray mSendMesage;

        public Socket mRemoteSocket;
    }

    protected class TcpSocketClient
    {
        public Socket mTcpClient;
        public byte[] mReceiveBuffer;
        public int mDataCount = 0; //真实的数据量
        public bool mIsConnect = true;
        public bool mIsReceivIngData { get; set; } = false; //标识是否正在接收数据中

        public TcpSocketClient(Socket client)
        {
            mTcpClient = client;
            mReceiveBuffer = new byte[S_BufferSize];
        }
    }

    protected ConcurrentDictionary<Socket, TcpSocketClient> mAllConnectTcpClients = new ConcurrentDictionary<Socket, TcpSocketClient>();


    protected bool mIsSendEaneble { get; set; } = true;
    protected bool mIsReceiveEaneble { get; set; } = true;


    #region 属性

    protected Socket mIpv4Socket = null; //服务器需要同时支持IPV4 和IPV6
    //   protected Socket mIpv6Socket = null;

    public int EndPort { get; protected set; } //端口号
    public bool mIsTcpServerStart { get; protected set; } //标示是否已经启动了UDP 服务器


    protected const int S_BufferSize = 65536;


    protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mReceiveMessageThread = null; //接收消息的后台线程

    protected ConcurrentQueue<TcpSocketMessage> mAllWillSendoutDatas; //线程安全的队列
    protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mSendMessageThread = null; //发送消息的后台线程
    public SocketMessageDelagate OnSendMessageEvent;


    #region Task 

    private CancellationTokenSource mReceiveCancleToken;
    private Task mReceiveMessageTask;


    private CancellationTokenSource mSendCancleToken;
    private Task mSendMessageTask;

    #endregion


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

//            mReceiveMessageThread = new Thread(BeginReceiveMessage) {IsBackground = true};
//            mReceiveMessageThread.IsBackground = true;
//            mReceiveMessageThread.Start();

            mReceiveCancleToken = new CancellationTokenSource();
            mReceiveMessageTask = Task.Factory.StartNew(BeginReceiveMessage, mSendCancleToken, TaskCreationOptions.LongRunning);


//            mSendMessageThread = new Thread(BeginSendMessage) {IsBackground = true};
//            mSendMessageThread.IsBackground = true;
//            mSendMessageThread.Start();


            mSendCancleToken = new CancellationTokenSource();
            mSendMessageTask = Task.Factory.StartNew(BeginSendMessage, mSendCancleToken, TaskCreationOptions.LongRunning);

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

        mIsSendEaneble = mIsReceiveEaneble = false;
        try
        {
            mAllWillSendoutDatas = null;
            OnSendMessageEvent = null;

            //    mIpv4Socket.Shutdown(SocketShutdown.Receive);
            mIpv4Socket?.Close();

            foreach (var connectTcpClient in mAllConnectTcpClients)
            {
                if (connectTcpClient.Key == null || connectTcpClient.Value == null)
                    continue;
                connectTcpClient.Key.Disconnect(false);
                //   connectTcpClient.Key.Shutdown(SocketShutdown.Both);
                connectTcpClient.Key.Close();
            }

            mAllConnectTcpClients.Clear();


            // mReceiveMessageThread.Abort();
            //     mSendMessageThread.Abort();
            mReceiveCancleToken.Cancel();
            mSendCancleToken.Cancel();
            // mIpv6Socket?.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"结束Tcp服务器异常 {e}");
        }
        finally
        {
            mIsTcpServerStart = false;
        }
    }


    public void SendMessage(int protocolId, string message, Socket remoteSocket, bool isIPv4 = true)
    {
        ByteArray byteArray = ByteArray.GetByteArray();
        byteArray.EncodingGetBytes(message, Encoding.UTF8, 0);

        SendMessage(protocolId, byteArray, remoteSocket, isIPv4);
    }

    public void SendMessage(int protocolId, ByteArray message, Socket remoteSocket, bool isIPv4 = true)
    {
        TcpSocketMessage messageDat = new TcpSocketMessage();
        messageDat.mRemoteSocket = remoteSocket;
        messageDat.mSendMesage = message;


        SocketHead head = SocketHead.GetSocketHead(protocolId, message.mDataRealLength, 1);
        head.AppendMessageHead(messageDat.mSendMesage);
        SocketHead.RecycleSocketHead(head);
        
        
        Debug.Log($"SendMessage protocolId={protocolId}  ::  {message}");

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
                if (mIsSendEaneble == false || mAllWillSendoutDatas.Count == 0)
                {
                    Thread.Sleep(S_SendMessageThreadInterval);
                    continue;
                }

                if (mAllWillSendoutDatas.TryDequeue(out var messageData))
                {
                    int sendDataLength = 0;


                    Socket targetSocket = messageData.mRemoteSocket; //messageData.mIsIpv4 ? mIpv4Socket : mIpv6Socket;

                    if (targetSocket == null)
                    {
                        Debug.LogError($"当前的Socket 对象创建失败 是否是IPv4:{true}");
                        continue;
                    }

                    sendDataLength = targetSocket.Send(messageData.mSendMesage.mBytes, 0, messageData.mSendMesage.mDataRealLength, SocketFlags.None);


                    if (sendDataLength != messageData.mSendMesage.mDataRealLength)
                        Debug.LogError($"数据发送异常，发送的数据量不一致{sendDataLength}  {messageData.mSendMesage.mDataRealLength}");

                    //           Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(Encoding.UTF8.GetString(messageData.Message), messageData.mRemoteSocket.LocalEndPoint); });

                    //    Debug.Log($"发送{messageData.mRemoteSocket.LocalEndPoint} 消息 {messageData.mSendMesage.mDataRealLength}  --> {Encoding.UTF8.GetString(messageData.mSendMesage.mBytes, SocketHead.S_HeadLength, messageData.mSendMesage.mDataRealLength - SocketHead.S_HeadLength)}");
//                    Debug.Log($"发送{messageData.mRemoteSocket.LocalEndPoint} 消息 {messageData.mSendMesage.mBytes}");
                }

                Thread.Sleep(S_SendMessageThreadInterval);
            }
        }
        catch (ThreadAbortException e)
        {
        } //线程结束时候回抛出异常
        catch (Exception e)
        {
            Debug.LogError($"发现消息异常{e.ToString()}");
        }
    }

    /// <summary>/// 接受消息的线程/// </summary>
    private void BeginReceiveMessage(object obj)
    {
        try
        {
            while (true)
            {
                if (mIsReceiveEaneble == false || mIpv4Socket == null || mIsTcpServerStart == false)
                {
                    Thread.Sleep(S_ReceiveMessageThreadInterval);
                    continue;
                }

                foreach (var socket in mAllConnectTcpClients.Values)
                {
                    if (socket.mTcpClient == null) continue;
                    if (socket.mIsConnect == false) continue;
                    if (socket.mIsReceivIngData) continue;
                    ConnectSocketBeginReceive(socket);
                }

                mIpv4Socket.BeginAccept(OnBeginAccept, mIpv4Socket);
                Thread.Sleep(50);
            }
        }
        catch (ThreadAbortException e)
        {
        } //线程结束时候回抛出异常
        catch (Exception e)
        {
            Debug.LogError($"接受数据异常 {e}");
        }
    }


    protected void OnBeginAccept(IAsyncResult asyncResult)
    {
        Socket clientSocket = (asyncResult.AsyncState as Socket).EndAccept(asyncResult);
        mAllConnectTcpClients.TryAdd(clientSocket, new TcpSocketClient(clientSocket));

        Debug.Log($"客户端{clientSocket.RemoteEndPoint} 连接服务器");
    }

    protected void ConnectSocketBeginReceive(TcpSocketClient tcpSocketClient)
    {
        if (tcpSocketClient.mDataCount >= S_BufferSize)
            Debug.Log($"ConnectSocketBeginReceive {tcpSocketClient.mDataCount}");
        if (tcpSocketClient.mIsReceivIngData) return;
        tcpSocketClient.mIsReceivIngData = true;

        tcpSocketClient.mTcpClient.BeginReceive(tcpSocketClient.mReceiveBuffer, tcpSocketClient.mDataCount, S_BufferSize, SocketFlags.None, OnReceiveCallback, tcpSocketClient);
    }

    protected void OnReceiveCallback(IAsyncResult asyncResult)
    {
        TcpSocketClient tcpSocketClient = null;
        try
        {
            tcpSocketClient = asyncResult.AsyncState as TcpSocketClient;


            int dataLength = tcpSocketClient.mTcpClient.EndReceive(asyncResult); //            socket.Receive(mBuffer, 0, S_BufferSize, SocketFlags.None);
            if (dataLength > 0)
            {
                if (dataLength > S_BufferSize)
                {
                    Debug.LogError($"接收的数据{dataLength}超过最大的接受上限{S_BufferSize}");
                    return;
                }

                Debug.Log($"接收到来自{tcpSocketClient.mTcpClient.LocalEndPoint} 的长度为{dataLength}");

                int realDataLength = 0;
                int offset = 0;
                while (true)
                {
                    realDataLength = SocketHead.GetPacketLength(tcpSocketClient.mReceiveBuffer, offset);
                    if (realDataLength == 0)
                    {
                        ByteArray temp = ByteArray.GetByteArray();
                        temp.CopyBytes(tcpSocketClient.mReceiveBuffer, 0, dataLength, dataLength, 0);

                        Debug.LogError($"得到的数据长度是0  {temp}");
                        ByteArray.RecycleByteArray(temp);
                        break;
                    }

                    //  Debug.Log($"r2222222 ealDataLength={realDataLength}  dataLength={dataLength}  offset={offset}");
                    if (realDataLength > dataLength - offset)
                    {
                        tcpSocketClient.mDataCount = realDataLength;
                        if (offset != 0)
                            System.Array.Copy(tcpSocketClient.mReceiveBuffer, offset, tcpSocketClient.mReceiveBuffer, 0, realDataLength); //移动到开头
                        break; //收到的是半包 继续留这
                    }

                    byte[] dataMessage = new byte [realDataLength];
                    System.Array.Copy(tcpSocketClient.mReceiveBuffer, offset, dataMessage, 0, realDataLength);
                    ServerMessageProcess.Instance.AddRequestMessage(tcpSocketClient.mTcpClient, dataMessage);

                    offset += realDataLength;
                    if (offset == dataLength)
                    {
                        tcpSocketClient.mDataCount = 0; //数据刚好处理完几个整包
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"接受到{tcpSocketClient.mTcpClient.RemoteEndPoint} 的数据长度为0是否已经断开连接");
                tcpSocketClient.mIsConnect = false;
          //      mAllConnectTcpClients.TryRemove(tcpSocketClient.mTcpClient, out var client);
            }
        }
        catch (System.ObjectDisposedException e)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"数据接受异常{e}");
        }
        finally
        {
            if (tcpSocketClient != null)
            {
                tcpSocketClient.mIsReceivIngData = false; //准备下一次接收数据
            }
        }
    }

    #endregion

    public void Dispose()
    {
        mIpv4Socket?.Dispose();
        //     mIpv6Socket?.Dispose();
    }
}