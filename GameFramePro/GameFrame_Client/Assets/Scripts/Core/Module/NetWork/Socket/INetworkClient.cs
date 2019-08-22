using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;


namespace GameFramePro.NetWorkEx
{
    public enum NetWorkUsage
    {
        Start,
        Connecting,
        Close,
        Error
    }

    public delegate void NetWorkStateDelegate(NetWorkUsage state, INetworkClient client);

    public delegate void NetWorkMessageDelegate(byte[] data, IPEndPoint endPoint);

    /// <summary>/// 定义了TCP/UDP 客户端公用的接口/// </summary>
    public interface INetworkClient
    {
        /// <summary>/// 开始收发消息/// </summary>
         void StartClient(IPEndPoint endPoint);
        
        void StopClient();

        void SendData(byte[] data);


        event NetWorkStateDelegate OnConectEvent;
        event NetWorkStateDelegate OnDisConectEvent;
        event NetWorkStateDelegate OnConectErrorEvent;

        event NetWorkMessageDelegate OnReceiveMessageEvent;
        event NetWorkMessageDelegate OnSendMessageEvent;


    }
}