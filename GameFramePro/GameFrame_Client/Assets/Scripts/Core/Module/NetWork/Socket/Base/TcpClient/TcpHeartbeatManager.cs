using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// Tcp 心跳包管理器/// </summary>/// 一定时间内没有接受和发送数据就会发送消息
    internal class TcpHeartbeatManager : IDisposable
    {
        #region 公开属性

        public TimeSpan mHeartbeatTimeSpan { get; private set; } //5秒没没有网络数据接受和发送就发送消息
        public bool mIsHeartbeating { get; private set; } = false; //标识心跳包是否在运行中

        #endregion

        protected ByteArray mHeartbeatData;
        protected BaseTcpClient mTargetTcpClient;

        protected Thread mHeartbeatThread;
        protected System.Timers.Timer mHeartbeatTimer;

        #region 对外接口

        public void StartHeartbeat(BaseTcpClient tcpEventCallback, TimeSpan heartbeat)
        {
            try
            {
                if (tcpEventCallback == null)
                    throw new NullReferenceException($"StartHeartbeat 失败，参数tcp 客户端为null");

                if (mIsHeartbeating)
                {
                    Debug.LogError($"心跳包管理器运行中");
                    return;
                }

                mTargetTcpClient = tcpEventCallback;
                mHeartbeatTimeSpan = heartbeat;

                mHeartbeatData = ByteArray.GetByteArray();
                mIsHeartbeating = true;

                //**使用系统计时器而不是单独线程减少资源的使用
                mHeartbeatTimer = new System.Timers.Timer(heartbeat.TotalMilliseconds);
                mHeartbeatTimer.Elapsed += BeginHeatbeatTimer;
                mHeartbeatTimer.Start();
            }
            catch (Exception e)
            {
                Debug.LogError($"Tcp 心跳包启动异常 {e}");
                throw;
            }
        }

        public void StopHearbeat()
        {
            if (mIsHeartbeating == false)
                return;
            mIsHeartbeating = false;

            mHeartbeatThread?.Abort();
            if (mHeartbeatTimer != null)
            {
                mHeartbeatTimer.Elapsed -= BeginHeatbeatTimer;
                mHeartbeatTimer.Stop();
            }

            mTargetTcpClient = null;
        }

        #endregion

        #region 内部实现

        protected void BeginHeatbeatTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (mIsHeartbeating)
                {
                    if (mTargetTcpClient != null && mTargetTcpClient.mIsConnected)
                    {
                        mTargetTcpClient?.Send((int) ProtocolCommand.HearBeatCommand, mHeartbeatData);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.LogError($"Exception {e}");
            }
        }

//        protected void BeginHeatbeatThread(object obj)
//        {
//            try
//            {
//                Debug.Log($"启动心跳包");
//                while (true)
//                {
//                    if (mIsHeartbeating)
//                    {
//                        mTargetTcpClient?.Send((int) ProtocolCommand.HearBeatCommand, mHeartbeatData);
//                    }
//
//                    Thread.Sleep(mHeartbeatTimeSpan);
//                }
//            }
//            catch (ThreadAbortException e)
//            {
//            }
//            catch (Exception e)
//            {
//                Debug.LogError($"Exception {e}");
//                throw;
//            }
//        }

        #endregion

        public void Dispose()
        {
            mTargetTcpClient?.Dispose();
            mHeartbeatTimer?.Dispose();
        }
    }
}