using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using GameFramePro;
using GameFramePro.NetWorkEx;
using GameFramePro.Protocol.LoginModule;
using UnityEngine;


/// <summary>/// 服务器消息处理中心/// </summary>
public class ServerMessageProcess : MonoBehaviour
{
    protected class AllTcpRequestData
    {
        public Socket mClientSocket;
        public byte[] RequestMessage;
    }


    protected ConcurrentQueue<AllTcpRequestData> mAllTcpRequestDatas = new ConcurrentQueue<AllTcpRequestData>(); //所有的Tcp 请求

    public void AddRequestMessage(Socket client, byte[] request)
    {
        if (client == null || request == null)
        {
            Debug.LogError($"接收消息失败");
            return;
        }

        lock (new object())
        {
            AllTcpRequestData tcpRequestData = new AllTcpRequestData()
            {
                mClientSocket = client,
                RequestMessage = request,
            };
            mAllTcpRequestDatas.Enqueue(tcpRequestData);
        }
    }

    public static ServerMessageProcess Instance;

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        AllTcpRequestData requestData = null;
        while (mAllTcpRequestDatas.TryDequeue(out requestData))
        {
            int protocolID = SocketHead.GetPacketProtocolID(requestData.RequestMessage, 4);

            if (protocolID == ProtocolCommand.HearBeatCommand)
            {
                Debug.Log($"收到{requestData.mClientSocket.RemoteEndPoint} 心跳包");
            }
            else if (protocolID == ProtocolCommand.RequestLogin)
            {
                string message = Encoding.UTF8.GetString(requestData.RequestMessage, SocketHead.S_HeadLength, requestData.RequestMessage.Length - SocketHead.S_HeadLength);
                Debug.Log($"接收到来自{requestData.mClientSocket.RemoteEndPoint} 的登录信息{message}");


                LoginRequest request = SerializeManager.DeserializeObject<LoginRequest>(message);

                LoginResponse response = new LoginResponse();
                response.mUserName = request.mUserName;
                response.mToken = "xyz";
                response.mIsSuccess = true;

                string responseStr = SerializeManager.SerializeObject(response);
                TcpServerUIComponent.Instance.mTcpServer.SendMessage(ProtocolCommand.ResponseLogin, responseStr, requestData.mClientSocket);
            }
        }
    }
}