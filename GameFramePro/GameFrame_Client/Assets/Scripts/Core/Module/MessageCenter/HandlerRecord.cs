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

        #region 构造函数
        static HandlerRecord() {
            s_MessageHandlerRecordPool = new NativeObjectPool<HandlerRecord>(50, OnBeforeGetHandlerRecord, OnBeforeRecycleHandlerRecord);
        }
        public HandlerRecord() { }
        #endregion


        private static NativeObjectPool<HandlerRecord> s_MessageHandlerRecordPool;

        #region NativeObjectPool 接口

        private static void OnBeforeGetHandlerRecord(HandlerRecord record)
        {
        }

        private static void OnBeforeRecycleHandlerRecord(HandlerRecord record)
        {
            if (record == null) return;
            record.ClearHandlerRecord();
        }

        #endregion

        public static HandlerRecord  GetHandlerRecord(HandlerTypeEnum handlerType, Delegate handler)
        {
            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(handlerType, handler);
            return handlerRecord;
        }

        public static void ReleaseHandlerRecord(HandlerRecord handler)
        {
            s_MessageHandlerRecordPool.RecycleItemToPool(handler);
        }


        private void InitialedHandlerRecord(HandlerTypeEnum handlerType, Delegate handler)
        {
            HandlerFunction = handler;
            HandlerType = handlerType;
        }


        private void ClearHandlerRecord()
        {
            HandlerFunction = null;
            HandlerType = HandlerTypeEnum.None_Paramter;
        }

    }
}