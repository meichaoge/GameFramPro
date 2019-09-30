using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlatformManager
{
    private static AndroidJavaObject _current;
    public static AndroidJavaObject Current
    {
        get
        {
            if(_current==null)
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _current = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return _current;
        }
    }
}