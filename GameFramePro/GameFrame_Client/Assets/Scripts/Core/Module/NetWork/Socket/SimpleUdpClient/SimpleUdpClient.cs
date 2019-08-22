using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameFramePro.NetWorkEx
{
    public delegate void SocketMessageDelagate(byte[] message, EndPoint endPoint);

    /// <summary>/// UDP 客户端/// </summary>
    public class SimpleUdpClient : IDisposable
    {
        /// <summary>/// 需要发送的消息队列/// </summary>
        protected class SocketPacket
        {
            public byte[] messageData;
            public EndPoint mEndPoint;
            public bool mIsBrocast = false;
        }

        #region 属性

        public Socket mClientSocket { get; private set; } = null; //可能是IPv4 或者 ipv6
        protected int EndPort { get; private set; } //端口号SocketWillSendMessageData
        protected AddressFamily mAddressFamily { get; set; }
        public bool mIsUdpClientStart { get; protected set; } //标示是否已经启动了UDP 客户端
        public bool mIsEnableBrocast { get; protected set; } = false; //默认不接受广播

        public bool mIsGroupListener { get; protected set; } = false; //标示是否加入了组播中


        protected const int S_BufferSize = 65536;
        protected readonly byte[] mBuffer = new byte[S_BufferSize];
        protected Thread mReceiveMessageThread = null;
        public SocketMessageDelagate OnReceiveMessageEvent;
        protected const int S_ReceiveMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔


        protected ConcurrentQueue<SocketPacket> mAllWillSendoutDatas; //线程安全的队列
        protected const int S_SendMessageThreadInterval = 50; //发送消息的线程Sleep 时间间隔
        protected Thread mSendMessageThread = null;
        public SocketMessageDelagate OnSendMessageEvent;

        #endregion

        #region 构造函数

        public SimpleUdpClient(int port, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            EndPort = port;
            mAddressFamily = addressFamily;
        }

        #endregion

        #region 接口

        public void StartClient()
        {
            if (mIsUdpClientStart)
            {
                Debug.LogError("UDP 服务器已经启动");
                return;
            }

            try
            {
                mClientSocket = new Socket(mAddressFamily, SocketType.Dgram, ProtocolType.Udp);
                mClientSocket?.Bind(new IPEndPoint(mAddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, EndPort));
                if (mClientSocket != null)
                    mClientSocket.EnableBroadcast = mIsEnableBrocast;

                mAllWillSendoutDatas = new ConcurrentQueue<SocketPacket>();

                mReceiveMessageThread = new Thread(BeginReceiveMessage);
                mReceiveMessageThread.IsBackground = false;
                mReceiveMessageThread.Start();

                mSendMessageThread = new Thread(BeginSendMessage);
                mSendMessageThread.Start();

                mIsUdpClientStart = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"启动UDP客户端异常 {e}");
                throw;
            }
        }

        public void StopClient()
        {
            if (mIsUdpClientStart == false)
                return;
            try
            {
                mReceiveMessageThread.Abort();
                mSendMessageThread.Abort();
                mAllWillSendoutDatas = null;
                OnReceiveMessageEvent = OnSendMessageEvent = null;
                mClientSocket?.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"结束UDP客户端异常 {e}");
                throw;
            }
        }

        public void SendBrocast(byte[] message, int port)
        {
            SendMessage(message, new IPEndPoint(IPAddress.Broadcast, port), true);
        }

        public void SendMessage(byte[] message, string ipAddress, int port)
        {
            SendMessage(message, new IPEndPoint(IPAddress.Parse(ipAddress), port), false);
        }

        public void SendMessage(byte[] message, EndPoint endPoint, bool isBrocast = false)
        {
            SocketPacket messageData = new SocketPacket()
            {
                messageData = message,
                mEndPoint = endPoint,
                mIsBrocast = isBrocast,
            };

            mAllWillSendoutDatas.Enqueue(messageData);
        }

        //加入组
        public void JoinGroup(IPAddress ipAddress, int timeToLive = 0)
        {
            if (mIsUdpClientStart == false)
            {
                Debug.LogError($"请先初始化Socket upd 客户端");
                return;
            }

            Debug.Log($"加入组{ipAddress}");
            mIsEnableBrocast = true; //组播需要开启接受和发送广播
            mClientSocket.SetSocketOption(mAddressFamily == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new MulticastOption(ipAddress));
            if (timeToLive != 0)
                mClientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
            mIsGroupListener = true;
        }

        //离开组
        public void LeaveGroup(IPAddress ipAddress)
        {
            if (mIsUdpClientStart == false)
            {
                Debug.LogError($"请先初始化Socket upd 客户端");
                return;
            }

            Debug.Log($"离开组{ipAddress}");
            mClientSocket.SetSocketOption(mAddressFamily == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new MulticastOption(ipAddress));
            mIsGroupListener = false;
        }

        #endregion

        #region 设置Socket 状态

        /// <summary>/// 设置当前UDP 是否收发广播消息/// </summary>
        public void SetBrocastEnableState(bool isEnable)
        {
            mIsEnableBrocast = isEnable;
            if (mClientSocket != null)
                mClientSocket.EnableBroadcast = isEnable;
        }

        #endregion

        #region 内部实现

        /// <summary>/// 发送消息的线程/// </summary>
        private void BeginSendMessage(object obj)
        {
            while (true)
            {
                SocketPacket sendMessageData = null;

                try
                {
                    if (mClientSocket == null)
                        continue;

                    if (mAllWillSendoutDatas == null || mAllWillSendoutDatas.Count == 0)
                        continue;

                    if (mAllWillSendoutDatas.TryDequeue(out sendMessageData))
                    {
                        int dataLength = 0;

                        if (sendMessageData.mIsBrocast)
                        {
                            bool isBrocaseEnable = mClientSocket.EnableBroadcast; //记录发送消息前的状态以便于复位

                            if (isBrocaseEnable == false)
                                mClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            dataLength = mClientSocket.SendTo(sendMessageData.messageData, 0, sendMessageData.messageData.Length, SocketFlags.None, sendMessageData.mEndPoint);

                            if (isBrocaseEnable == false)
                                mClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
                        } //广播 发送完成立刻复位
                        else
                        {
                            dataLength = mClientSocket.SendTo(sendMessageData.messageData, 0, sendMessageData.messageData.Length, SocketFlags.None, sendMessageData.mEndPoint);
                        }


                        if (dataLength != sendMessageData.messageData.Length)
                            Debug.LogError($"数据发送异常，实际发送数据量{dataLength} 总共{sendMessageData.messageData.Length}");

                        Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(sendMessageData.messageData, sendMessageData.mEndPoint); });

                        Debug.Log($"From{mClientSocket.LocalEndPoint} 发送{sendMessageData.mEndPoint} 消息 {sendMessageData.messageData}");
                    }
                }
                catch (SocketException e)
                {
                    Debug.LogError($"指定要发送的Socket {sendMessageData.mEndPoint} 无效");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogError($"指定要发送的Socket {sendMessageData.messageData.Length} 数据超过Buffer 长度");
                }
                catch (ArgumentNullException e)
                {
                    Debug.LogError($"指定要发送的Socket {sendMessageData.messageData.Length} 数据为null");
                }
                catch (Exception e)
                {
                    Debug.LogError($"发送消息失败{e.GetType()} {e}");
                }

                Thread.Sleep(S_SendMessageThreadInterval);
            }
        }

        /// <summary>/// 发送消息/// </summary>
        /// <param name="sendMessageData"></param>
        private void SendMessage(SocketPacket sendMessageData)
        {
        }


        /// <summary>
        /// TODO 考虑到每次接受的数据包可能是多个udp 数据，这里需要合理安排每次处理的最大的数据量  mClientSocket.Available
        /// </summary>
        /// <param name="obj"></param>
        private void BeginReceiveMessage(object obj)
        {
            while (true)
            {
                try
                {
                    if (mClientSocket != null)
                    {
                        EndPoint endPoint = new IPEndPoint(mClientSocket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                        int dataLength = mClientSocket.ReceiveFrom(mBuffer, 0, S_BufferSize, SocketFlags.None, ref endPoint); //子线程被阻塞在这里 直到有数据到达
                        if (dataLength > 0)
                        {
                            //   string message = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
                            Debug.Log($"{mClientSocket.EnableBroadcast} 接受到来自{endPoint} 的{dataLength} 长度消息{mBuffer}还有{mClientSocket.Available} 的数据没有处理");
                            Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(mBuffer, endPoint); });
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"接受数据异常{e}");
                    throw;
                }

                Thread.Sleep(S_ReceiveMessageThreadInterval);
            }
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            mClientSocket?.Dispose();
        }

        #endregion
    }
}