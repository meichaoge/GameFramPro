using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// UDP 客户端/// </summary>
    public class SimpleUdpClient : IDisposable
    {
        #region 属性

        private Socket mClientSocket = null;
        public int EndPort { get; private set; } //端口号
        public AddressFamily AddressFamilyUsage { get; private set; } //

        protected const int S_BufferSize = 65536;
        protected byte[] mBuffer = new byte[S_BufferSize];

        #endregion

        #region 构造函数

        public SimpleUdpClient()
        {
            //UdpClient
        }

        public SimpleUdpClient(int port) : this(AddressFamily.InterNetwork, port)
        {
            mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public SimpleUdpClient(AddressFamily addressFamily, int port)
        {
            mClientSocket = new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp);
        }

        #endregion

        #region 接口

        public void Bind()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, EndPort);
            mClientSocket.Bind(endPoint);
        }

        public void Close()
        {
            mClientSocket.Close();
        }

        public void SendData(byte[] data, string ipAddress, int port)
        {
            mClientSocket.SendTo(data, SocketFlags.None, new IPEndPoint(IPAddress.Parse(ipAddress), port));
            Debug.Log($"发送消息成功 {ipAddress} {port}");
        }


        public void ReceiveData(byte[] data, string ipAddress, int port)
        {
            System.Array.Clear(mBuffer, 0, mBuffer.Length);
            int dataLength = mClientSocket.Receive(mBuffer, SocketFlags.None);
            string dataStr = Encoding.UTF8.GetString(mBuffer, 0, dataLength);
            Debug.Log($"接受消息{dataStr}");
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
