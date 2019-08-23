using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;


namespace GameFramePro.NetWorkEx
{
    /// <summary>/// Tcp 客户端/// </summary>
    public class SimpleTcpClient : IDisposable
    {
        #region 属性

        public Socket mClientSocket { get; private set; } = null; //可能是IPv4 或者 ipv6
        protected IPEndPoint mIpEndPoint { get; private set; } //端口号 
        protected AddressFamily mAddressFamily { get; set; }

        #endregion

        #region 状态

        public bool mIsConnected { get; protected set; } = false;

        #endregion


        #region 构造函数

        public SimpleTcpClient(IPEndPoint ipEndPoint, AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            mAddressFamily = addressFamily;
            mIpEndPoint = ipEndPoint;
            try
            {
                mClientSocket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
                mClientSocket.Bind(mIpEndPoint);
            }
            catch (Exception e)
            {
                Debug.LogError($"初始化 SimpleTcpClient 异常{e}");
                throw;
            }
        }

        #endregion

        #region 对外接口

        public void Connect(IPAddress ipAddress, int port)
        {
            Connect(new IPEndPoint(ipAddress, port));
        }

        public void Connect(IPEndPoint remoteEP)
        {
            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                mClientSocket.Connect(remoteEP);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.LogError($"TCP 连接Socket {remoteEP} 不可用{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                Debug.LogError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TCP {remoteEP} 连接失败{e}");
                throw;
            }
        }


        public IAsyncResult BeginConnect(IPAddress address, int port, object state)
        {
            return BeginConnect(new IPEndPoint(address, port), state);
        }

        public IAsyncResult BeginConnect(IPEndPoint remoteEP, object state)
        {
            try
            {
                if (mClientSocket == null)
                    throw new ArgumentNullException($"Socket 没有初始化");

                mClientSocket.BeginConnect(remoteEP, OnBeginConnectCallback, state);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Debug.LogError($"TCP 连接 Socket {remoteEP} 端口号 不可用{e}");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.LogError($"TCP 连接Socket {remoteEP} 不可用{e}");
            }
            catch (System.ObjectDisposedException e)
            {
                Debug.LogError($"TCP 连接Socket  {remoteEP} 已经关闭{e}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TCP {remoteEP} 连接失败{e}");
                throw;
            }
            return null;
        }

        #endregion


        #region 内部实现

        private void OnBeginConnectCallback(IAsyncResult a)
        {
        }

        #endregion


        public void Dispose()
        {
        }
    }
}