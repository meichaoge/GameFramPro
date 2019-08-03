using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro.EventSystemEx
{
    /// <summary>
    /// 输入事件系统
    /// </summary>
    public class InputEventManager : Single<InputEventManager>
    {
        private IInputManager mCurrentIInputManager;  //当前的输入输出事件检测系统


        protected override void InitialSingleton()
        {
            base.InitialSingleton();
#if UNITY_EDITOR|| UNITY_STANDALONE_WIN
            mCurrentIInputManager = new StandonInputManager();
#endif

#if UNITY_EDITOR && UNITY_ANDROID
             mCurrentIInputManager = new MobileInputManager();
#endif
        }



    }
}