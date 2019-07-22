using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 枚举属性显示成LayerMask 类似样式
/// </summary>
public class FlagEnumAttribute 
{
    private string headTitle;

    /// <summary>
    ///使用[EnumFlagsAttribute("......")] 情况
    /// </summary>
    /// <param name="title"></param>
    public FlagEnumAttribute(string title)
    {
        headTitle = title;
    }


    public FlagEnumAttribute()
    {

    }
}
