using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.Protocol.LoginModule
{
    /// <summary>
    /// 网络请求回包
    /// </summary>
    public class LoginResponse
    {
        public bool mIsSuccess;
        public string mUserName;
        public string mToken;
    }
}
