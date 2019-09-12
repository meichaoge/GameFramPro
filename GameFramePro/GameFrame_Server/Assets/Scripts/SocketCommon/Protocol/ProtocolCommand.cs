using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 服务器定义的协议id/// </summary>
    public static class ProtocolCommand
    {
        public readonly static int HearBeatCommand = 5000; //心跳

        public readonly static int RequestLogin = 5001; //登录消息
        public readonly static int ResponseLogin = 5002; //登录回包
    }
}