using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//ref https://blog.csdn.net/CSDN_1593923161/article/details/79880315
public class Clipboard
{
#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void _CopyTextToClipboard(string text);
    [DllImport("__Internal")]
    private static extern string _GetClipboardText();
#endif

    public static void CopyToClipboard(string input)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        // 对Android的调用
        AndroidJavaObject androidObject = new AndroidJavaObject("com.test.gtqp.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
            return;
        // 复制到剪贴板
        androidObject.Call("copyTextToClipboard", activity, input);     
#elif UNITY_IPHONE && !UNITY_EDITOR
        _CopyTextToClipboard(input);
#elif UNITY_EDITOR
        TextEditor t = new TextEditor();
        t.text = input == null || input.Length == 0 ? " " : input;
        t.OnFocus();
        t.Copy();
#endif
    }
    /// <summary>
    /// 获取剪贴板内容
    /// </summary>
    /// <returns></returns>
    public static string GetClipboardContent()
    {
        string str = null;
#if UNITY_ANDROID && !UNITY_EDITOR
          // 对Android的调用
  AndroidJavaObject androidObject = new AndroidJavaObject("com.test.gtqp.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
            return str;
        // 从剪贴板中获取文本
        string tempText = androidObject.Call<string>("getTextFromClipboard");
        str = tempText;
#elif UNITY_IPHONE && !UNITY_EDITOR
        str = _GetClipboardText();   
#elif UNITY_EDITOR
        str = GUIUtility.systemCopyBuffer;
#endif
        return str;
    }
}
