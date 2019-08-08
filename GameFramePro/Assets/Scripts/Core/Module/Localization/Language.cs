using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 本地化语言环境 (UnityEngine.SystemLanguage 也有定义)
    /// </summary>
    [Flags]
    public enum Language
    {
      //  None=0,  //未知的语言 使用了Flag 不能用0
        zh_CN=1,
        zh_HK=2,  //繁体中文
        en_US=4,  //美式英语
        en_GB=8,  //英式英语

    }
}