using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpand
{
    private static Dictionary<string, Color> mTempColorDic = new Dictionary<string, Color>(20); //缓存的颜色数据
    private static Dictionary<Color, string> mTempColorHtmDic = new Dictionary<Color, string>(20); //缓存的颜色数据


    /// <summary>/// 16进制(以#开头)转成颜色/// </summary>
    public static Color HtmToColor(string htmlString)
    {
        Color result;
        if (mTempColorDic.TryGetValue(htmlString, out result))
            return result;
        result = Color.white;

        if (ColorUtility.TryParseHtmlString(htmlString, out result) == false)
        {
            Debug.LogError("HtmToColor Format Fail!! " + htmlString);
            return result;
        }

        mTempColorDic[htmlString] = result;
        return result;
    }

    /// <summary>/// 颜色转成 16进制/// </summary>
    public static string ColotToHtm(Color color)
    {
        string resultColor = string.Empty;
        if (mTempColorHtmDic.TryGetValue(color, out resultColor))
            return resultColor;
        resultColor = $"#{ColorUtility.ToHtmlStringRGBA(color)}"; // 这里必须加上 #号
        mTempColorHtmDic[color] = resultColor;
        return resultColor;
    }
}
