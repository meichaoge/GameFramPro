using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.Localization
{
    /// <summary>/// 对本地化模块屏蔽外部支持的语言 ，这里是本地化使用的语言/// </summary>
    ///每种新增的语言都需要制定值 避免增加语言时候覆盖其他的配置
    internal enum LocalizationLanguage
    {
        Unknow=0,  // 未知
        Chinese = 1, //简体中文
        English = 3, //英语
    }
    
    
    
    
    
    
}
