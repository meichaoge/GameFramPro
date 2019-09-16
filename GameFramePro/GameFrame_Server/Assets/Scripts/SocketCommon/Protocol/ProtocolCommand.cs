using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramePro.NetWorkEx
{
    /// <summary>/// 服务器定义的协议id/// </summary>
    public static class ProtocolCommand
    {
        public readonly static int RequestHearBeat = 5001; //心跳
        public readonly static int ResponseHearBeat = 5002; //心跳


        public readonly static int RequestLogin = 5003; //登录消息
        public readonly static int ResponseLogin = 5004; //登录回包
    }
}