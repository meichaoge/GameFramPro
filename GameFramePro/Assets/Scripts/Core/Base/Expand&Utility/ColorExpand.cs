using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExpand
{

    private static Dictionary<string, Color> mTempColorDic = new Dictionary<string, Color>();  //缓存的颜色数据


    /// <summary>
    /// 16进制(以#开头)转成颜色
    /// </summary>
    /// <param name="htmlString"></param>
    /// <returns></returns>
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

}
