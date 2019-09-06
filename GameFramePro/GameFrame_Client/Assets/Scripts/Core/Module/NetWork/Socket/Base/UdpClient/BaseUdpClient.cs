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
    /// <summary>/// Udp 客户端基类/// </summary>
    public class BaseUdpClient : BaseSocketClient
    {
        #region 属性

        public bool mIsGroupListener { get; protected set; } = false; //标示是否加入了组播中

        #endregion


        #region 构造函数

        public BaseUdpClient(AddressFamily addressFamily,string clientName) : base(clientName,addressFamily, SocketType.Dgram, ProtocolType.Udp)
        {
            mSocketClientType = SocketClientUsage.UdpClient;
            try
            {
                mClientSocket = new Socket(addressFamily, mSocketType, mProtocolType);
                mClientSocket.Bind(new IPEndPoint(mAddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0));
                mIsSocketInitialed = true;
                StartReceiveAndSendThread();
            }
            catch (Exception e)
            {
                OnInitialedClientFail($"初始化 Socket 客户端 异常{e}");
            }
        }

        #endregion


        #region 接口

        public void SendBrocast(int messageId, ByteArray message, int port)
        {
            SendMessage(messageId, message, new IPEndPoint(IPAddress.Broadcast, port), true);
        }

        public void SendMessage(int messageId, ByteArray message, string ipAddress, int port)
        {
            SendMessage(messageId, message, new IPEndPoint(IPAddress.Parse(ipAddress), port), false);
        }

        public void SendMessage(int messageId, ByteArray message, EndPoint endPoint, bool isBrocast = false)
        {
            ByteArray sendByteArray = ByteArray.GetByteArray();
            sendByteArray.CloneFromByteArray(message); //克隆数据 避免污染源数据
            
            var sendMessage = BaseSocketSendMessage.GetSocketSendMessageData(messageId, message, endPoint, isBrocast);
            mBaseSocketMessageManager?.CacheSocketSendData(sendMessage);
        }


        //加入组
        public void JoinGroup(IPAddress ipAddress, int timeToLive = 0)
        {
            if (mIsSocketInitialed == false)
            {
                OnSocketException($"请先初始化Socket upd 客户端");
                return;
            }

            Debug.Log($"加入组{ipAddress}");
            SetBrocastEnableState(true); //组播需要开启接受和发送广播
            mClientSocket.SetSocketOption(mAddressFamily == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.AddMembership, new MulticastOption(ipAddress));
            if (timeToLive != 0)
                mClientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, timeToLive);
            mIsGroupListener = true;
        }

        //离开组
        public void LeaveGroup(IPAddress ipAddress)
        {
            if (mIsSocketInitialed == false)
            {
                OnSocketException($"请先初始化Socket upd 客户端");
                return;
            }

            Debug.Log($"离开组{ipAddress}");
            mClientSocket.SetSocketOption(mAddressFamily == AddressFamily.InterNetwork ? SocketOptionLevel.IP : SocketOptionLevel.IPv6, SocketOptionName.DropMembership, new MulticastOption(ipAddress));
            mIsGroupListener = false;
        }


        /// <summary>/// 设置当前UDP 是否收发广播消息/// </summary>
        public void SetBrocastEnableState(bool isEnable)
        {
            if (mClientSocket != null)
                mClientSocket.EnableBroadcast = isEnable;
        }

        #endregion

        #region 基类重写

        protected override void BeginSendMessageThread(object obj)
        {
            BaseSocketSendMessage sendMessageData = null;
            while (true)
            {
                try
                {
                    if (mClientSocket == null || mBaseSocketMessageManager == null || mBaseSocketMessageManager.mAllSendMessageQueue.Count == 0)
                    {
                        Thread.Sleep(S_SendMessageThreadInterval);
                        continue;
                    }

                    if (mBaseSocketMessageManager.GetWillSendSocketData(out sendMessageData))
                    {
                        int dataLength = 0;

                        if (sendMessageData.mIsBrocast)
                        {
                            bool isBrocaseEnable = mClientSocket.EnableBroadcast; //记录发送消息前的状态以便于复位

                            if (isBrocaseEnable == false)
                                mClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            dataLength = mClientSocket.SendTo(sendMessageData.mSendMessageByteArray.mBytes, 0, sendMessageData.mSendMessageByteArray.mDataRealLength, SocketFlags.None, sendMessageData.mEndPoint);

                            if (isBrocaseEnable == false)
                                mClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
                        } //广播 发送完成立刻复位
                        else
                        {
                            dataLength = mClientSocket.SendTo(sendMessageData.mSendMessageByteArray.mBytes, 0, sendMessageData.mSendMessageByteArray.mDataRealLength, SocketFlags.None, sendMessageData.mEndPoint);
                        }


                        if (dataLength != sendMessageData.mSendMessageByteArray.mDataRealLength)
                            OnSocketException($"数据发送异常，实际发送数据量{dataLength} 总共{sendMessageData.mSendMessageByteArray.mDataRealLength}");

                        OnSendMessage(sendMessageData);
                        BaseSocketSendMessage.RecycleSocketMessageData(sendMessageData);
                        sendMessageData = null;
                    }

                    Thread.Sleep(S_SendMessageThreadInterval);
                }
                catch (SocketException e)
                {
                    OnSocketException($"指定要发送的Socket {sendMessageData.mEndPoint} 无效");
                }
                catch (ArgumentOutOfRangeException e)
                {
                    OnSocketException($"指定要发送的Socket {sendMessageData.mSendMessageByteArray.mDataRealLength} 数据超过Buffer 长度");
                }
                catch (ArgumentNullException e)
                {
                    OnSocketException($"指定要发送的Socket {sendMessageData.mSendMessageByteArray.mDataRealLength} 数据为null");
                }
                catch (Exception e)
                {
                    OnSocketException($"发送消息失败{e.GetType()} {e}");
                }
            }
        }

//TODO 考虑到每次接受的数据包可能是多个udp 数据，这里需要合理安排每次处理的最大的数据量  mClientSocket.Available
        protected override void BeginReceiveMessageThread(object obj)
        {
            while (true)
            {
                try
                {
                    if (mClientSocket != null)
                    {
                        EndPoint endPoint = new IPEndPoint(mClientSocket.AddressFamily == AddressFamily.InterNetwork ? IPAddress.Any : IPAddress.IPv6Any, 0);
                        int dataLength = mClientSocket.ReceiveFrom(mBuffer, 0, S_BufferSize, SocketFlags.None, ref endPoint); //子线程被阻塞在这里 直到有数据到达
                        if (dataLength == 0) continue;


                        if (dataLength < SocketHead.S_HeadLength)
                        {
                            OnSocketException($"接受到一个数据包长度小于头部长度{dataLength} <{SocketHead.S_HeadLength}");
                            continue;
                        }

                        int packetLength = SocketHead.GetPacketLength(mBuffer, 0);
                        if (dataLength < packetLength)
                        {
                            OnSocketException($"还有数据待接收");
                            continue;
                        }

                        var receiveData = ByteArray.GetByteArray();
                        receiveData.CopyBytes(mBuffer, 0, packetLength, packetLength, 0);


                        var receiveMessage = BaseSocketReceiveMessage.GetSocketReceiveMessageData(0, receiveData, endPoint);
                        mBaseSocketMessageManager.SaveReceiveData(receiveMessage);
                    }
                }
                catch (Exception e)
                {
                    OnSocketException($"接受数据异常{e}");
                    throw;
                }

                Thread.Sleep(S_ReceiveMessageThreadInterval);
            }
        }

        #endregion
    }
}