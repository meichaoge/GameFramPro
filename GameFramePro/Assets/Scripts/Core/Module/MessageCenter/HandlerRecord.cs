using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 标示消息的监听处理的参数个数
    /// </summary>
    public enum HandlerTypeEnum
    {
        None_Paramter,
        One_Parameter,
        Two_Parameter,
        Three_Parameter,
        Four_Parameter,
    }


    /// <summary>
    /// 处理某一个类型的消息
    /// </summary>
    public class HandlerRecord
    {
        public Delegate HandlerFunction { get; protected set; }
        public HandlerTypeEnum HandlerType { get; protected set; }

        public HandlerRecord() { }


        public HandlerRecord(HandlerTypeEnum handlerType, Delegate handler)
        {
            InitialedHandlerRecord(handlerType,handler);
        }

        public void InitialedHandlerRecord(HandlerTypeEnum handlerType, Delegate handler)
        {
            HandlerFunction = handler;
            HandlerType = handlerType;
        }


        public void ClearHandlerRecord()
        {
            HandlerFunction = null;
            HandlerType = HandlerTypeEnum.None_Paramter;
        }

    }
}