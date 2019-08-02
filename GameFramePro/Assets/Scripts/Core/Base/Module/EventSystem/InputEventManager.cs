using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.EventSystemEx
{
    /// <summary>
    /// 输入事件系统
    /// </summary>
    public class InputEventManager
    {
        private void InputCheck()
        {
            // if (Input.anyKey)
        }


        private void CheckLeftButtonClick()
        {
            bool isClick = false;


#if UNITY_EDITOR|| UNITY_STANDALONE_WIN
            isClick = Input.GetMouseButtonDown(0);
#endif

#if UNITY_EDITOR && UNITY_ANDROID
            isClick = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
        }
    }
}