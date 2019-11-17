using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 网络状态
    /// </summary>
    public enum NetWorkStateUsage
    {
        None=0,  // 初始状态
      //  Initialed=1, //初始化
        Running=2, //正常运行中
        InitialFail=5, //初始化异常
    
        Connecting=10, //正在连接中
        Conected=15, //已经连接
        ConnectedTimeOut=20, //连接超时
        ConnectedFail = 25, //连接失败

        AuthenSuccess=30,  //认证成功
        AuthenException=35, //认证失败


        DisConnect = 40, //连接断开
        Error=50  //连接错误
    }
    /// <summary>///    通用的Socket 客户端接口/// </summary>
    public interface ISocketClient
    {
        event System.Action<ISocketClient, NetWorkStateUsage> OnSocketStateChangeEvent; //Socket State 改变
        event System.Action<ISocketClient, string> OnSocketErrorEvent; //Socket异常败
        event System.Action<ISocketClient, BaseSocketSendMessage> OnSendMessageEvent; //发送消息
        event System.Action<ISocketClient, BaseSocketReceiveMessage> OnReceiveMessageEvent; //接受到消息
    }



  

    /// <summary>
    ///   通用网络客户端
    /// </summary>
    public interface ISocketClient2
    {
        NetWorkStateUsage ClientState { get; }  

        bool IsStopClient { get; } //是否停止客户端

        event System.Action<ISocketClient2, NetWorkStateUsage,string > OnSocketStateChangeEvent; //Socket State 改变
        event System.Action<ISocketClient2, BaseSocketSendMessage> OnSendMessageEvent; //发送消息
        event System.Action<ISocketClient2, BaseSocketReceiveMessage> OnReceiveMessageEvent; //接受到消息
    }

    /// <summary>
    /// 连接型客户端
    /// </summary>
    public interface IConnectedSocketClient: ISocketClient2
    {
        bool IsConnected { get; } //标识是否已经连接
    }



}