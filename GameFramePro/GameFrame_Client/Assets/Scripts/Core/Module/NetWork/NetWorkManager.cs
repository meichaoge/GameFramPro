using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using GameFramePro.NetWorkEx;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>/// 负责对内网络状态的控制对外提供网络回调/// </summary>
    public class NetWorkManager : Single<NetWorkManager>
    {
        #region 数据

        private List<INetWorkEventCallback> mAllSocketClients = new List<INetWorkEventCallback>(5);

        #endregion


        #region 接口

        public SimpleTcpEventCallback GetSimpleTcpClient()
        {
            var client = new SimpleTcpEventCallback();
            mAllSocketClients.Add(client);

            return client;
        }

        #endregion

        #region 注册事件

        private void OnBeginConnectCallback()
        {
            
        }

        #endregion
        
        
    }
}