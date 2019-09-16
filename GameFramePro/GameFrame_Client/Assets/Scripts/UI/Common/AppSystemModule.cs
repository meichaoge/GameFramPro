using System.Collections;
using System.Collections.Generic;
using GameFramePro.NetWorkEx;
using GameFramePro.Protocol.LoginModule;
using GameFramePro.Protocol.SystemModule;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>  /// 处理与整个App 相关的/// </summary>
    public class AppSystemModule : Single<AppSystemModule>
    {
        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            NetWorkManager.S_Instance.RegisterNetWorkCallback<HeartbeatResponse>((int) ProtocolCommand.ResponseLogin, OnResponseHeartBeat);
        }

        private void OnResponseHeartBeat(object objectResponse)
        {
            HeartbeatResponse response = objectResponse as HeartbeatResponse;
            Debug.Log("心跳包网络回调成功");
        }
    }
}