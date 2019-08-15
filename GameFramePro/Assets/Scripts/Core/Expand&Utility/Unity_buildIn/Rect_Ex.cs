using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>/// Rect 扩展类/// </summary>
public static class Rect_Ex
{
    #region 相加

    /// <summary>/// 两个矩形相加/// </summary>
    public static Rect AddRect(this Rect a, Rect b)
    {
        return new Rect(a.x + b.x, a.y + b.y, a.width + b.width, a.height + b.height);
    }

    /// <summary>/// 矩形加 +水平x 偏移/// </summary>
    public static Rect AddRectX(this Rect a, float x)
    {
        return new Rect(a.x + x, a.y, a.width, a.height);
    }

    /// <summary>/// 矩形加 +垂直y 偏移/// </summary>
    public static Rect AddRectY(this Rect a, float y)
    {
        return new Rect(a.x, a.y + y, a.width, a.height);
    }

    /// <summary>/// 矩形加 +宽度width 偏移/// </summary>
    public static Rect AddRectWidth(this Rect a, float width)
    {
        return new Rect(a.x, a.y, a.width + width, a.height);
    }

    /// <summary>/// 矩形加 +高度height 偏移/// </summary>
    public static Rect AddRectHeight(this Rect a, float height)
    {
        return new Rect(a.x, a.y, a.width, a.height + height);
    }

    /// <summary>/// 矩形加 +水平x和垂直y 偏移/// </summary>
    public static Rect AddRectOffset(this Rect a, Vector2 offset)
    {
        return new Rect(a.x + offset.x, a.y + offset.y, a.width, a.height);
    }

    /// <summary>/// 矩形加 +宽x高y 偏移/// </summary>
    public static Rect AddRectSize(this Rect a, Vector2 size)
    {
        return new Rect(a.x, a.y, a.width + size.x, a.height + size.y);
    }

    #endregion

    #region 相减

    /// <summary>/// 两个矩形相加/// </summary>
    public static Rect MinusRect(this Rect a, Rect b)
    {
        return new Rect(a.x - b.x, a.y - b.y, a.width - b.width, a.height - b.height);
    }

    /// <summary>/// 矩形减 -水平x 偏移/// </summary>
    public static Rect MinusRectX(this Rect a, float x)
    {
        return new Rect(a.x - x, a.y, a.width, a.height);
    }

    /// <summary>/// 矩形减 -垂直y 偏移/// </summary>
    public static Rect MinusRectY(this Rect a, float y)
    {
        return new Rect(a.x, a.y - y, a.width, a.height);
    }

    /// <summary>/// 矩形减 -宽度width 偏移/// </summary>
    public static Rect MinusRectWidth(this Rect a, float width)
    {
        return new Rect(a.x, a.y, a.width - width, a.height);
    }

    /// <summary>/// 矩形减 -高度height 偏移/// </summary>
    public static Rect MinusRectHeight(this Rect a, float height)
    {
        return new Rect(a.x, a.y, a.width, a.height - height);
    }

    /// <summary>/// 矩形减 -水平x和垂直y 偏移/// </summary>
    public static Rect MinusRectOffset(this Rect a, Vector2 offset)
    {
        return new Rect(a.x - offset.x, a.y - offset.y, a.width, a.height);
    }

    /// <summary>/// 矩形减 -宽x高y 偏移/// </summary>
    public static Rect MinusRectSize(this Rect a, Vector2 size)
    {
        return new Rect(a.x, a.y, a.width - size.x, a.height - size.y);
    }

    #endregion
}
