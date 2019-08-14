using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameFramePro.Protocol.LoginModule;

namespace GameFramePro
{
    /// <summary>
    /// 负责所有与登录有关的网络请求模块
    /// </summary>
    public class LoginNetworkModule : Single<LoginNetworkModule>
    {
        /// <summary>/// 请求登录/// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public void RequestLogin(string name, string password)
        {
            LoginRequest requset = new LoginRequest()
            {
                mUserName = name,
                mUserPassword = password,
            };

            //测试代码
            AsyncManager.Invoke(1f, () =>
            {
                LoginResponse response = new LoginResponse();
                response.mIsSuccess = true;
                response.mUserName = name;
                response.mToken = "123456";

                OnResponseLogin(response);
            });
        }

        private void OnResponseLogin(LoginResponse response)
        {
            Debug.Log("登录网络回调成功");
            EventTrigger.TriggerMessage<LoginResponse>((int) UIEventUsage.OnResponse_Login, response);
        }
    }
}
