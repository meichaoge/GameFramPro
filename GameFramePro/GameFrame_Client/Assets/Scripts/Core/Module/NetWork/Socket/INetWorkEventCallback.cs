using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>///     Socket 客户端事件回调接口/// </summary>
    public interface INetWorkEventCallback
    {
        event System.Action OnBeginConnectEvent; //开始连接
        event System.Action OnConnectSuccessEvent; //连接成功
        event System.Action<string> OnConnectErrorEvent; //连接失败
        event System.Action OnConnectTimeOutEvent; //连接超时

        event System.Action OnDisConnectEvent; //连接断开
        event System.Action<string> OnSocketErrorEvent; //Socket异常败

        event System.Action<SocketMessageData> OnSendMessageEvent; //发送消息
        event System.Action<SocketMessageData> OnReceiveMessageEvent; //接受到消息
    }
}