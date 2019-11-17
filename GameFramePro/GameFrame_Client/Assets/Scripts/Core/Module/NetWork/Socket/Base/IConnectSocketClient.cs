using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>///    需要建立连接的Socket 客户端接口/// </summary>
    public interface IConnectSocketClient : ISocketClient
    {

        bool mIsConnected { get; } //标识是否已经连接

        event System.Action<BaseSocketClient> OnBeginConnectEvent; //开始连接
        event System.Action<BaseSocketClient> OnConnectSuccessEvent; //连接成功
        event System.Action<BaseSocketClient, string> OnConnectErrorEvent; //连接失败
        event System.Action<BaseSocketClient> OnConnectTimeOutEvent; //连接超时

        event System.Action<BaseSocketClient> OnBeDisConnectEvent; //连接断开(被动)

        event System.Action<BaseSocketClient> OnDisConnectEvent; //连接断开(主动断开)


    }
}