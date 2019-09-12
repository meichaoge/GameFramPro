using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GameFramePro;
using GameFramePro.NetWorkEx;
using UnityEngine;


public delegate void SocketMessageDelagate(string message, EndPoint endPoint);

public class UdpServer : IDisposable
{
    /// <summary>/// 需要发送的数据/// </summary>
    protected class SocketWillSendMessageData
    {
        public string message;
        public EndPoint mEndPoint;
        public bool mIsBrocast = false;
        public bool mIsIPv4 = true; //标示是否使用IPv4 发出
    }


    #region 属性

    protected Socket mIpv4Socket = null; //服务器需要同时支持IPV4 和IPV6
    protected Socket mIpv6Socket = null;

    // public AddressFamily mAddressFamily { get; protected set; } = AddressFamily.Unknown; //使用的协议
    public int EndPort { get; protected set; } //端口号
    public bool mIsUdpServerStart { get; protected set; } //标示是否已经启动了UDP 服务器


    protected const int S_BufferSize = 65536;
    protected readonly byte[] mBuffer = new byte[S_BufferSize];
    protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mReceiveMessageThread = null; //接收消息的后台线程
    public SocketMessageDelagate OnReceiveMessageEvent;

    protected ConcurrentQueue<SocketWillSendMessageData> mAllWillSendoutDatas; //线程安全的队列
    protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
    protected Thread mSendMessageThread = null; //发送消息的后台线程
    public SocketMessageDelagate OnSendMessageEvent;

    #endregion


    #region UdpClient 类中的方法

    /// <summary>/// 获取或者设置允许 Internet 协议 (IP) 的数据报进行分片/// </summary>
    public bool DontFragmentIpv4
    {
        get
        {
            if (this.mIpv4Socket != null)
                return this.mIpv4Socket.DontFragment;
            return false;
        }
        set
        {
            if (this.mIpv4Socket != null)
                this.mIpv4Socket.DontFragment = value;
        }
    }

    // <summary>/// 获取或者设置允许 Internet 协议 (IP) 的数据报进行分片/// </summary>
    public bool DontFragmentIpv6
    {
        get
        {
            if (this.mIpv6Socket != null)
                return this.mIpv6Socket.DontFragment;
            return false;
        }
        set
        {
            if (this.mIpv6Socket != null)
                this.mIpv6Socket.DontFragment = value;
        }
    }

    #endregion

    #region 构造函数

    public UdpServer(int port)
    {
        EndPort = port;
    }

    #endregion

    #region 接口

    public void StartServer()
    {
        if (mIsUdpServerStart)
        {
            Debug.LogError("UDP 服务器已经启动");
            return;
        }

        try
        {
            //IPv4 
            if (Socket.OSSupportsIPv4)
            {
                mIpv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //    mServerSocket.Bind(new IPEndPoint(SocketUtility.GetLocalIpAddress(), EndPort));
                mIpv4Socket?.Bind(new IPEndPoint(IPAddress.Any, EndPort));
                //   mIpv4Socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.Linger,new LingerOption(false,0));
                Debug.Log($"服务器启动支持IPv4 的Socket,绑定端口{EndPort}");
            }

            //IPv6 
            if (Socket.OSSupportsIPv6)
            {
                mIpv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                mIpv6Socket?.Bind(new IPEndPoint(IPAddress.IPv6Any, EndPort));
                //  mIpv6Socket.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.Linger,0);
                Debug.Log($"服务器启动支持IPv6 的Socket,绑定端口{EndPort}");
            }


            mAllWillSendoutDatas = new ConcurrentQueue<SocketWillSendMessageData>();

            mReceiveMessageThread = new Thread(BeginReceiveMessage) {IsBackground = true};
            mReceiveMessageThread.Start();

            mSendMessageThread = new Thread(BeginSendMessage) {IsBackground = true};
            mSendMessageThread.Start();

            mIsUdpServerStart = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"启动UDP服务器异常 {e}");
            throw;
        }
    }

    public void StopServer()
    {
        if (mIsUdpServerStart == false)
            return;

        try
        {
            mAllWillSendoutDatas = null;
            OnReceiveMessageEvent = OnSendMessageEvent = null;

            mReceiveMessageThread.Abort();
            mSendMessageThread.Abort();

            //   mIpv4Socket?.InternalShutdown(SocketShutdown.Both);
            mIpv4Socket?.Close();
            mIpv6Socket?.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"结束UDP服务器异常 {e}");
            throw;
        }
        finally
        {
            mIsUdpServerStart = false;
        }
    }

    /// <summary>/// 发送广播的接口/// </summary>
    public void SendBrocast(string message, int port, bool isIPv4 = true)
    {
        SendMessage(message, new IPEndPoint(IPAddress.Broadcast, port), true, isIPv4);
    }

    public void SendMessage(string message, string ipAddress, int port, bool isIPv4 = true)
    {
        SendMessage(message, new IPEndPoint(IPAddress.Parse(ipAddress), port), false, isIPv4);
    }

    public void SendMessage(string message, EndPoint endPoint, bool isBrocast = false, bool isIPv4 = true)
    {
        SocketWillSendMessageData messageData = new SocketWillSendMessageData()
        {
            message = message,
            mEndPoint = endPoint,
            mIsBrocast = isBrocast,
            mIsIPv4 = isIPv4,
        };

        mAllWillSendoutDatas.Enqueue(messageData);
    }

    //加入组
    public void JoinGroup(IPAddress ipAddress)
    {
        Debug.Log($"加入组{ipAddress}");
        mIpv4Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ipAddress));
    }

    //离开组
    public void LeaveGroup(IPAddress ipAddress)
    {
        Debug.Log($"离开组{ipAddress}");
        mIpv4Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(ipAddress));
    }

    #endregion

    #region 状态设置

    /// <summary>/// 设置当前UDP 是否收发广播消息/// </summary>
    public void SetBrocastEnableState(bool isEnable, bool isIpv4 = true)
    {
        if (isIpv4)
        {
            if (mIpv4Socket != null)
                mIpv4Socket.EnableBroadcast = isEnable;
        }
        else
        {
            if (mIpv6Socket != null)
                mIpv6Socket.EnableBroadcast = isEnable;
        }
    }

    #endregion

    #region 内部实现

    /// <summary>/// 发送消息的线程/// </summary>
    private void BeginSendMessage(object obj)
    {
        while (true)
        {
            try
            {
                if (mAllWillSendoutDatas.Count == 0)
                    continue;

                if (mAllWillSendoutDatas.TryDequeue(out var messageData))
                {
                    byte[] data = Encoding.UTF8.GetBytes(messageData.message);
                    int sendDataLength = 0;


                    Socket targetSocket = messageData.mIsIPv4 ? mIpv4Socket : mIpv6Socket;

                    if (targetSocket == null)
                    {
                        Debug.LogError($"当前的Socket 对象创建失败 是否是IPv4:{messageData.mIsIPv4}");
                        continue;
                    }

                    if (messageData.mIsBrocast)
                    {
                        if (targetSocket.EnableBroadcast == false)
                            targetSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                        sendDataLength = mIpv4Socket.SendTo(data, 0, data.Length, SocketFlags.None, messageData.mEndPoint);
                    } //广播消息
                    else
                    {
                        if (targetSocket.EnableBroadcast)
                            targetSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0); //停止接受广播
                        sendDataLength = targetSocket.SendTo(data, 0, data.Length, SocketFlags.None, messageData.mEndPoint);
                    }


                    if (sendDataLength != data.Length)
                        Debug.LogError($"数据发送异常，发送的数据量不一致{sendDataLength}  {data.Length}");

                    Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(messageData.message, messageData.mEndPoint); });

                    Debug.Log($"发送{messageData.mEndPoint} 消息 {messageData.message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"发现消息异常{e.ToString()}");
                throw;
            }


            Thread.Sleep(S_SendMessageThreadInterval);
        }
    }

    /// <summary>/// 接受消息的线程/// </summary>
    private void BeginReceiveMessage(object obj)
    {
        while (true)
        {
            try
            {
                if (mIsUdpServerStart == false)
                    break;

                if (mIpv4Socket != null)
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);

                    int dataLength = mIpv4Socket.ReceiveFrom(mBuffer, 0, S_BufferSize, SocketFlags.None, ref endPoint);
                    if (dataLength > 0)
                    {
                        if (dataLength > S_BufferSize)
                            Debug.LogError($"接收的数据{dataLength}超过最大的接受上限{S_BufferSize}");

                        string message = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
                        Debug.Log($"接受到来自{endPoint} 的消息{message}");

                        Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(message, endPoint); });
                    }
                } //接受IPv4 消息


                if (mIpv6Socket != null)
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.IPv6Any, 0);

                    int dataLength = mIpv6Socket.ReceiveFrom(mBuffer, 0, S_BufferSize, SocketFlags.None, ref endPoint);
                    if (dataLength > 0)
                    {
                        if (dataLength > S_BufferSize)
                            Debug.LogError($"接收的数据{dataLength}超过最大的接受上限{S_BufferSize}");
                        string message = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
                        Debug.Log($"接受到来自{endPoint} 的消息{message}");
                        Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(message, endPoint); });
                    }
                } //接受IPv6 消息
            }
            catch (Exception e)
            {
                Debug.LogError($"{e}");
                throw;
            }

            Thread.Sleep(S_ReceiveMessageThreadInterval);
        }
    }

    #endregion

    public void Dispose()
    {
        mIpv4Socket?.Dispose();
        mIpv6Socket?.Dispose();
    }
}