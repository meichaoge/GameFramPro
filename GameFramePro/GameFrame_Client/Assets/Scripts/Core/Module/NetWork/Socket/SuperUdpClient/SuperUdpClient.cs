﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 封装C# UdpClient提供更多控制/// </summary>
    public class SuperUdpClient : INetworkClient
    {
        protected readonly ConcurrentQueue<byte[]> mAllNeedSendMessages = new ConcurrentQueue<byte[]>();
        protected readonly UdpClient mUdpClient = null;
        protected IPEndPoint mConnectEndPoint;

        protected bool mIsInitialed = false;
        protected Thread mSendMessageThread = null;
        protected Thread mReceiveMessageThread = null;


        #region 构造函数

        public SuperUdpClient(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            mUdpClient = new UdpClient(addressFamily);
        }

        public SuperUdpClient(int port)
        {
            mUdpClient = new UdpClient(port);
        }

        public SuperUdpClient(int port, AddressFamily family)
        {
            mUdpClient = new UdpClient(port, family);
        }

        public SuperUdpClient(IPEndPoint localEP)
        {
            mUdpClient = new UdpClient(localEP);
        }

        public SuperUdpClient(string hostname, int port)
        {
            mUdpClient = new UdpClient(hostname, port);
        }

        #endregion

        #region INetworkClient 接口实现

        public event NetWorkStateDelegate OnConectEvent;
        public event NetWorkStateDelegate OnDisConectEvent;
        public event NetWorkStateDelegate OnConectErrorEvent;
        public event NetWorkMessageDelegate OnReceiveMessageEvent;
        public event NetWorkMessageDelegate OnSendMessageEvent;

        /// <summary>/// 开始收发消息/// </summary>
        public void StartClient(IPEndPoint endPoint)
        {
            if (mIsInitialed)
            {
                Debug.LogError("UDP 客户端已经启动 了");
                return;
            }

            mConnectEndPoint = endPoint;
            mUdpClient.Connect(endPoint);

            mSendMessageThread = new Thread(BeginSendMessageThread);
            mSendMessageThread.Start();
            
            mReceiveMessageThread = new Thread(BeginReceiveMessageThread);
            mReceiveMessageThread.Start();
        }

        public void StopClient()
        {
            mUdpClient?.Close();
            mUdpClient?.Dispose();
        }

        public void SendData(byte[] data)
        {
            if (mUdpClient == null)
            {
                Debug.LogError($"数据发送失败 没有初始化");

                OnConectErrorEvent?.Invoke(NetWorkUsage.Error, this);
                return;
            }

            mAllNeedSendMessages.Enqueue(data);
        }

        #endregion


        #region 单独的收发消息线程

        protected void BeginSendMessageThread(object obj)
        {
            while (true)
            {
                if (mAllNeedSendMessages.Count > 0)
                {
                    if (mAllNeedSendMessages.TryDequeue(out var data))
                    {
                        int dataLength = mUdpClient.Send(data, data.Length);

                        if (dataLength != data.Length)
                            Debug.LogError($"发送的实际数据量{dataLength} 需要发送的数据量{data.Length}");

                        Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(data, mConnectEndPoint); });
                    }
                }

                Thread.Sleep(100);
            }
        }
        
        protected void BeginReceiveMessageThread(object obj)
        {
            while (true)
            {
                
//                mUdpClient.ReceiveAsync()()
//                
//                
//                
//                if (mAllNeedSendMessages.Count > 0)
//                {
//                    if (mAllNeedSendMessages.TryDequeue(out var data))
//                    {
//                        int dataLength = mUdpClient.Send(data, data.Length);
//
//                        if (dataLength != data.Length)
//                            Debug.LogError($"发送的实际数据量{dataLength} 需要发送的数据量{data.Length}");
//
//                        Loom.S_Instance.QueueOnMainThread(() => { OnSendMessageEvent?.Invoke(data, mConnectEndPoint); });
//                    }
//                }

                Thread.Sleep(100);
            }
        }

        #endregion
    }
}