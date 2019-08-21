using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameFramePro.NetWorkEx
{
    public delegate void SocketMessageDelagate(string message, EndPoint endPoint);

    /// <summary>/// UDP 客户端/// </summary>
    public class SimpleUdpClient : IDisposable
    {
        private class SockeSendMessageInfor
        {
            public string message;
            public EndPoint mEndPoint;
            public bool mIsBrocast = false;
        }


        #region 属性

        private Socket mClientSocket = null;
        public int EndPort { get; private set; } //端口号

        protected const int S_BufferSize = 65536;
        protected readonly byte[] mBuffer = new byte[S_BufferSize];
        protected Thread mReceiveMessageThread = null;
        public SocketMessageDelagate OnReceiveMessageEvent;

        private readonly Stack<SockeSendMessageInfor> mAllWillSendMessageInfors = new Stack<SockeSendMessageInfor>(10);
        protected Thread mSendMessageThread = null;
        public SocketMessageDelagate OnSendMessageEvent;

        #endregion

        #region 构造函数

        public SimpleUdpClient(int port)
        {
            EndPort = port;
        }

        #endregion

        #region 接口

        public void StartClient()
        {
            mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //mClientSocket.Bind(new IPEndPoint( SocketUtility.GetLocalIpAddress(), EndPort));
            mClientSocket.Bind(new IPEndPoint( IPAddress.Any, EndPort));
            
            mReceiveMessageThread = new Thread(BeginReceiveMessage);
            mReceiveMessageThread.Start();

            mSendMessageThread = new Thread(BeginSendMessage);
            mSendMessageThread.Start();
        }

        public void StopClient()
        {
            mReceiveMessageThread.Abort();
            mSendMessageThread.Abort();
            mAllWillSendMessageInfors.Clear();
            OnReceiveMessageEvent = OnSendMessageEvent = null;
            mClientSocket.Close();
            mClientSocket.Dispose();
        }

        public void SendBrocast(string message, int port)
        {
            SendMessage(message, new IPEndPoint(IPAddress.Broadcast, port), true);
        }

        public void SendMessage(string message, string ipAddress, int port)
        {
            SendMessage(message, new IPEndPoint(IPAddress.Parse(ipAddress), port), false);
        }

        public void SendMessage(string message, EndPoint endPoint, bool isBrocast = false)
        {
            SockeSendMessageInfor messageInfor = new SockeSendMessageInfor()
            {
                message = message,
                mEndPoint = endPoint,
                mIsBrocast=isBrocast,
            };

            lock (mAllWillSendMessageInfors)
            {
                mAllWillSendMessageInfors.Push(messageInfor);
            }
        }

        //加入组
        public void JoinGroup(IPAddress ipAddress)
        {
            mClientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ipAddress));
        }

        //离开组
        public void LeaveGroup(IPAddress ipAddress)
        {
            mClientSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(ipAddress));
        }

        #endregion

        #region 内部实现

        private void BeginSendMessage(object obj)
        {
            while (true)
            {
                lock (mAllWillSendMessageInfors)
                {
                    if (mAllWillSendMessageInfors.Count > 0)
                    {
                        var messageInfor = mAllWillSendMessageInfors.Pop();
                        byte[] data = Encoding.UTF8.GetBytes(messageInfor.message);

                    //    Debug.Log($"发送{messageInfor.mEndPoint} 消息 {messageInfor.message}");

                        if (messageInfor.mIsBrocast)
                        {
                            mClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
                            mClientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, messageInfor.mEndPoint, OnBeginSendMessage, messageInfor);
                        } //广播
                        else
                        {
                            mClientSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, messageInfor.mEndPoint, OnBeginSendMessage, messageInfor);
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }

        private void OnBeginSendMessage(IAsyncResult asyncResult)
        {
            var messageInfor = asyncResult.AsyncState as SockeSendMessageInfor;

            Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(messageInfor.message, messageInfor.mEndPoint); });

            Debug.Log($"From{mClientSocket.LocalEndPoint} 发送{messageInfor.mEndPoint} 消息 {messageInfor.message}");
        }

        private void BeginReceiveMessage(object obj)
        {
            while (true)
            {
                System.Array.Clear(mBuffer, 0, S_BufferSize);
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                mClientSocket.BeginReceiveFrom(mBuffer, 0, S_BufferSize, SocketFlags.None, ref endPoint, OnBeginReceiveMessage, endPoint);

                Thread.Sleep(100);
            }
        }

        private void OnBeginReceiveMessage(IAsyncResult asyncResult)
        {
            EndPoint endPoint = asyncResult.AsyncState as EndPoint;
            int dataLength = mClientSocket.EndReceive(asyncResult);
            if (dataLength > 0)
            {
                string message = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
                Debug.Log($"接受到来自{endPoint} 的消息{message}");
                Loom.S_Instance.QueueOnMainThread(() => { OnReceiveMessageEvent?.Invoke(message, endPoint); });
            }
        }

        #endregion


        #region IDisposable

        public void Dispose()
        {
            System.Array.Clear(mBuffer, 0, mBuffer.Length);
            mClientSocket.Close();
            mClientSocket.Dispose();
        }

        #endregion
    }
}