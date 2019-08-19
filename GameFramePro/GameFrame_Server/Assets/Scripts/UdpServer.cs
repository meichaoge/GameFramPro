using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpServer : IDisposable
{
    private Socket mServerSocket;
    private int mBindPort;
    private const int BufferLength = 65536;
    private byte[] mBuffer = new byte[BufferLength];

    private Thread receiveThread;

    #region 构造函数

    public UdpServer()
    {
    }

    public UdpServer(int endport)
    {
        mBindPort = endport;
    }

    #endregion

    #region 接口

    public void StartServer()
    {
        mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        mServerSocket.Bind(new IPEndPoint(IPAddress.Any, mBindPort));
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }

    #endregion


    #region 内部实现

    private void ReceiveData(object obj)
    {
        while (true)
        {
            System.Array.Clear(mBuffer, 0, BufferLength);

            int dataLength = mServerSocket.Receive(mBuffer, SocketFlags.None);
            if (dataLength > 0)
            {
                string message = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
                Debug.Log($"接受到消息 {message}");
            }

            Thread.Sleep(100);
        }
    }

    #endregion


    public void Dispose()
    {
    }
}
