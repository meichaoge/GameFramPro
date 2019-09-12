using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.NetWorkEx;
using GameFramePro.Protocol.LoginModule;

namespace GameFramePro
{
    /// <summary>/// 负责所有与登录有关的网络请求模块/// </summary>
    public class LoginNetworkModule : Single<LoginNetworkModule>
    {
        protected override void InitialSingleton()
        {
            base.InitialSingleton();
            NetWorkManager.S_Instance.RegisterNetWorkCallback<LoginResponse>((int) ProtocolCommand.ResponseLogin, OnResponseLogin);
        }


        /// <summary>/// 请求登录/// </summary>
        public void RequestLogin(string name, string password)
        {
            LoginRequest requset = new LoginRequest()
            {
                mUserName = name,
                mUserPassword = password,
            };

            ByteArray loginData = ByteArray.GetByteArray();
            loginData.SerilizeGetBytes(requset);

            SocketClientHelper.BaseLoginTcpClient.Send((int) ProtocolCommand.RequestLogin, loginData);
            ByteArray.RecycleByteArray(loginData);
//            //测试代码
//            AsyncManager.Invoke(1f, () =>
//            {
//                LoginResponse response = new LoginResponse();
//                response.mIsSuccess = true;
//                response.mUserName = name;
//                response.mToken = "123456";
//
//                OnResponseLogin(response);
//            });
        }

        private void OnResponseLogin(object objectResponse)
        {
            LoginResponse response = objectResponse as LoginResponse;
            Debug.Log("登录网络回调成功");
            EventTrigger.TriggerMessage<LoginResponse>((int) UIEventUsage.OnResponse_Login, response);
        }
    }
}