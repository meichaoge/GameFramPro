using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rect 扩展类
/// </summary>
public static class Rect_Ex
{
    //相加
    public static Rect AddRect(this Rect a, Rect b)
    {
        return new Rect(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
    }

    //相减
    public static Rect MinusRect(this Rect a, Rect b)
    {
        return new Rect(a.x - b.x, a.y - b.y, a.width - b.width, a.height - b.height);
    }

}
