using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 提供Socket 网络通信常用的接口/// </summary>
    public static class SocketUtility
    {
        /// <summary>/// 获取所有正在被占用的端口信息/// </summary>
        public static Dictionary<int, EndPoint> GetAllUsingPort()
        {
            Dictionary<int, EndPoint> result = new Dictionary<int, EndPoint>(500);

            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties(); //获取本地计算机的网络连接和通信统计数据的信息
            IPEndPoint[] ipUdp = ipGlobalProperties.GetActiveUdpListeners(); //获取所有监听UDP的程序端口信息
            IPEndPoint[] ipTcp = ipGlobalProperties.GetActiveTcpListeners(); //获取所有监听TCP的程序端口信息
            //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (var udpEndPoint in ipUdp)
            {
                if (result.ContainsKey(udpEndPoint.Port) == false)
                    result.Add(udpEndPoint.Port, udpEndPoint);
            }

            foreach (var tcpEndPoint in ipTcp)
            {
                if (result.ContainsKey(tcpEndPoint.Port) == false)
                    result.Add(tcpEndPoint.Port, tcpEndPoint);
            }

            foreach (var tcpConnectionInformation in tcpConnInfoArray)
            {
                var endPoint = tcpConnectionInformation.LocalEndPoint;
                if (result.ContainsKey(endPoint.Port) == false)
                    result.Add(endPoint.Port, endPoint);
            }

            return result;
        }


        /// <summary>/// 获取下一个可用的端口号/// </summary>
        public static int GetNextAvailablePort()
        {
            int beginePort = 2500; //1000以内的端口号可能被系统定义了
            int endPort = 60000; //系统tcp/udp 端口最大支持65535       


            var usingEndPoints = GetAllUsingPort();
            for (int dex = beginePort; dex < endPort; dex++)
            {
                if (usingEndPoints.ContainsKey(dex)) continue;

                return dex;
            }

            Debug.LogError($"无法获取可用的端口号，检查是否有异常");
            return -1;
        }


        /// <summary>/// 获取本机的IP 地址/// </summary>
        public static IPAddress GetLocalIpAddress(bool isIpv4 = true)
        {
            var allAddress = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var infor in allAddress.AddressList)
            {
                if (isIpv4)
                {
                    if (infor.AddressFamily == AddressFamily.InterNetwork)
                        return IPAddress.Parse(infor.ToString());
                }
                else
                {
                    if (infor.AddressFamily == AddressFamily.InterNetworkV6)
                        return IPAddress.Parse(infor.ToString());
                }
            }

            Debug.LogError($"获取本机的IP{isIpv4} 地址失败");
            return IPAddress.None;
        }
    }
}