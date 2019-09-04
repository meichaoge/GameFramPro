using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 心跳包管理器/// </summary>/// 一定时间内没有接受和发送数据就会发送消息
    internal class HeartbeatManager
    {
        #region 公开属性

        public TimeSpan mHeartbeatTimeSpan { get; private set; } //5秒没没有网络数据接受和发送就发送消息
        public DateTime LastNetWorkActivityTime { get; private set; } //上一次网络活动的时间
        public bool mIsHeartbeating { get; private set; } = false;

        #endregion


        protected ByteArray mHeartbeatData;

        protected BaseTcpClient MTargetTcpEventCallback;
        protected Thread mHeartbeaThread;

        #region 对外接口

        public void StartHeartbeat(BaseTcpClient tcpEventCallback, TimeSpan heartbeat)
        {
            if (tcpEventCallback == null)
                throw new NullReferenceException($"StartHeartbeat 失败，参数tcp 客户端为null");

            if (mIsHeartbeating)
            {
                Debug.LogError($"心跳包管理器运行中");
                return;
            }

            MTargetTcpEventCallback = tcpEventCallback;
            mHeartbeatTimeSpan = heartbeat;

//            mTargetTcpClient.OnReceiveMessageEvent += OnReceiveOrSendMessage;
//            mTargetTcpClient.OnSendMessageEvent += OnReceiveOrSendMessage;

            mHeartbeatData = ByteArrayPool.S_Instance.GetByteArray();

            LastNetWorkActivityTime = DateTime.UtcNow;
            mHeartbeaThread = new Thread(OnBeginHeartbeatThread);
            mHeartbeaThread.IsBackground = true;
            mHeartbeaThread.Start();

            mIsHeartbeating = true;
        }

        public void StopHearbeat()
        {
            if (mIsHeartbeating == false)
                return;
            mIsHeartbeating = false;

            mHeartbeaThread?.Abort();
            MTargetTcpEventCallback = null;
        }

        #endregion

        #region 内部实现

        /// 开启心跳线程/// </summary>
        private void OnBeginHeartbeatThread(object state)
        {
            try
            {
                while (true)
                {
                    if (MTargetTcpEventCallback.mIsConnected && DateTime.UtcNow - LastNetWorkActivityTime > mHeartbeatTimeSpan)
                    {
                        LastNetWorkActivityTime = DateTime.UtcNow;
                        MTargetTcpEventCallback.Send((int) ProtocolCommand.HearBeatCommand, mHeartbeatData);
                    }

                    Thread.Sleep(mHeartbeatTimeSpan);
                }
            }
            catch (ThreadAbortException e)
            {
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception {e}");
                throw;
            }
        }

//        private void OnReceiveOrSendMessage(byte[] message, EndPoint endPoint)
//        {
//            LastNetWorkActivityTime = DateTime.UtcNow;
//        }

        #endregion
    }
}