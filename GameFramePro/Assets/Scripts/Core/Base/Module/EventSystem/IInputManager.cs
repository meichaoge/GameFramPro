using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.EventSystemEx
{
    /// <summary>
    /// 事件发生点的信息
    /// </summary>
    public class InputFigerInfor
    {
        public Vector2 PointPosition; //
    }


    //按下事件
    public delegate void OnInputEventDownHandler(InputFigerInfor pointInfor);
    //抬起
    public delegate void OnInputEventUpHandler(InputFigerInfor pointInfor);
    //点击
    public delegate void OnInputEventClickHandler(InputFigerInfor pointInfor);
    //双击
    public delegate void OnInputEventDoubleClickHandler(InputFigerInfor pointInfor);


    public interface IInputManager
    {
        void CheckEventState();
    }
}