using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>///    通用的Socket 客户端接口/// </summary>
    public interface ISocketClient
    {
        event System.Action<BaseSocketClient> OnInitialedFailEvent; //创建Socket 失败
      
        event System.Action<BaseSocketClient, string> OnSocketErrorEvent; //Socket异常败

        event System.Action<BaseSocketClient, BaseSocketSendMessage> OnSendMessageEvent; //发送消息
        event System.Action<BaseSocketClient, BaseSocketReceiveMessage> OnReceiveMessageEvent; //接受到消息
    }
}