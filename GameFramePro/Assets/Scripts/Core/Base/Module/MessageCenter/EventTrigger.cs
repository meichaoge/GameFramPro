﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{

    public delegate void MessageHandler(int enumValue);
    public delegate void MessageHandler<T>(int enumValue, T parameter1);
    public delegate void MessageHandler<T, V>(int enumValue, T parameter1, V parameter2);
    public delegate void MessageHandler<T, V, W>(int enumValue, T parameter1, V parameter2, W parameter3);
    public delegate void MessageHandler<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4);



    /// <summary>
    /// 用于各个模块转发消息事件
    /// </summary>
    public static class EventTrigger
    {

        private static Dictionary<int, LinkedList<HandlerRecord>> mAllMessageHandlers = new Dictionary<int, LinkedList<HandlerRecord>>(); //所有的消息处理中心
        private static NativeObjectPool<HandlerRecord> s_MessageHandlerRecordPool = new NativeObjectPool<HandlerRecord>(50, OnBeforeGetHandlerRecord, OnBeforeRecycleHandlerRecord);

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

        #region 注册监听

        public static void RegisterMessageHandler(int enumValue, MessageHandler process)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }

            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(HandlerTypeEnum.None_Paramter, process);
            messageHandlers.AddLast(handlerRecord);
        }

        public static void RegisterMessageHandler<T>(int enumValue, MessageHandler<T> process)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }

            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(HandlerTypeEnum.One_Parameter, process);
            messageHandlers.AddLast(handlerRecord);
        }

        public static void RegisterMessageHandler<T, V>(int enumValue, MessageHandler<T, V> process)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }
            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(HandlerTypeEnum.Two_Parameter, process);
            messageHandlers.AddLast(handlerRecord);
        }

        public static void RegisterMessageHandler<T, V, W>(int enumValue, MessageHandler<T, V, W> process)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }
            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(HandlerTypeEnum.Three_Parameter, process);
            messageHandlers.AddLast(handlerRecord);
        }
        public static void RegisterMessageHandler<T, V, W, U>(int enumValue, MessageHandler<T, V, W, U> process)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }
            HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
            handlerRecord.InitialedHandlerRecord(HandlerTypeEnum.Four_Parameter, process);
            messageHandlers.AddLast(handlerRecord);
        }
        #endregion

        #region 移除监听

        public static bool UnRegisterMessageHandler(int enumValue, MessageHandler process)
        {
            if (process == null)
            {
                Debug.LogError("UnRegisterMessageHandler Fail,Parameter is null");
                return false;
            }

            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                HandlerRecord messageHandler = null;
                while (handlerNode != null)
                {
                    messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == HandlerTypeEnum.None_Paramter && messageHandler.HandlerFunction == process)
                    {
                        messageHandlers.Remove(handlerNode);
                        s_MessageHandlerRecordPool.RecycleItemToPool(messageHandler);
                        return true;
                    }
                }
            }
            Debug.LogError("UnRegisterMessageHandler Fail, ID={0}  Handler={1}", enumValue, process.Method);
            return false;
        }

        public static bool UnRegisterMessageHandler<T>(int enumValue, MessageHandler<T> process)
        {
            if (process == null)
            {
                Debug.LogError("UnRegisterMessageHandler<T> Fail,Parameter is null");
                return false;
            }

            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                HandlerRecord messageHandler = null;
                while (handlerNode != null)
                {
                    messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == HandlerTypeEnum.One_Parameter && messageHandler.HandlerFunction == process)
                    {
                        messageHandlers.Remove(handlerNode);
                        s_MessageHandlerRecordPool.RecycleItemToPool(messageHandler);
                        return true;
                    }
                }
            }
            Debug.LogError("UnRegisterMessageHandler<T> Fail, ID={0}  Handler={1}", enumValue, process.Method);
            return false;
        }

        public static bool UnRegisterMessageHandler<T, V>(int enumValue, MessageHandler<T, V> process)
        {
            if (process == null)
            {
                Debug.LogError("UnRegisterMessageHandler<T,V> Fail,Parameter is null");
                return false;
            }

            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                HandlerRecord messageHandler = null;
                while (handlerNode != null)
                {
                    messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == HandlerTypeEnum.Two_Parameter && messageHandler.HandlerFunction == process)
                    {
                        messageHandlers.Remove(handlerNode);
                        s_MessageHandlerRecordPool.RecycleItemToPool(messageHandler);
                        return true;
                    }
                }
            }
            Debug.LogError("UnRegisterMessageHandler<T,V> Fail, ID={0}  Handler={1}", enumValue, process.Method);
            return false;
        }
        public static bool UnRegisterMessageHandler<T, V, W>(int enumValue, MessageHandler<T, V, W> process)
        {
            if (process == null)
            {
                Debug.LogError("UnRegisterMessageHandler<T,V,W> Fail,Parameter is null");
                return false;
            }

            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                HandlerRecord messageHandler = null;
                while (handlerNode != null)
                {
                    messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == HandlerTypeEnum.Three_Parameter && messageHandler.HandlerFunction == process)
                    {
                        messageHandlers.Remove(handlerNode);
                        s_MessageHandlerRecordPool.RecycleItemToPool(messageHandler);
                        return true;
                    }
                }
            }
            Debug.LogError("UnRegisterMessageHandler<T,V,W> Fail, ID={0}  Handler={1}", enumValue, process.Method);
            return false;
        }

        public static bool UnRegisterMessageHandler<T, V, W, U>(int enumValue, MessageHandler<T, V, W, U> process)
        {
            if (process == null)
            {
                Debug.LogError("UnRegisterMessageHandler<T,V,W,U> Fail,Parameter is null");
                return false;
            }

            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                HandlerRecord messageHandler = null;
                while (handlerNode != null)
                {
                    messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == HandlerTypeEnum.Four_Parameter && messageHandler.HandlerFunction == process)
                    {
                        messageHandlers.Remove(handlerNode);
                        s_MessageHandlerRecordPool.RecycleItemToPool(messageHandler);
                        return true;
                    }
                }
            }
            Debug.LogError("UnRegisterMessageHandler<T,V,W,U> Fail, ID={0}  Handler={1}", enumValue, process.Method);
            return false;
        }


        public static bool UnRegisterAllMessageHandler(int enumValue)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                    s_MessageHandlerRecordPool.RecycleItemToPool(handlerNode.Value);

                messageHandlers.Clear();

            }
            return true;
        }
        #endregion

        #region 触发消息

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <typeparam name="int"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="parameter"></param>
        public static void TriggerMessage(int enumValue)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent(enumValue, handlerNode, false);
                    handlerNode = handlerNode.Next;
                }
            }
        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumValue"></param>
        /// <param name="parameter1"></param>
        /// <param name="isDownPassEvent">是否向下传递事件，结果是参数比当前少的也会被触发</param>
        public static void TriggerMessage<T>(int enumValue, T parameter1, bool isDownPassEvent = false)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T>(enumValue, parameter1, handlerNode, isDownPassEvent);
                    handlerNode = handlerNode.Next;
                }
            }
        }

        public static void TriggerMessage<T, V>(int enumValue, T parameter1, V parameter2, bool isDownPassEvent = false)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V>(enumValue, parameter1, parameter2, handlerNode, isDownPassEvent);
                    handlerNode = handlerNode.Next;
                }
            }
        }
        public static void TriggerMessage<T, V, W>(int enumValue, T parameter1, V parameter2, W parameter3, bool isDownPassEvent = false)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V, W>(enumValue, parameter1, parameter2, parameter3, handlerNode, isDownPassEvent);
                    handlerNode = handlerNode.Next;
                }
            }
        }

        public static void TriggerMessage<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4, bool isDownPassEvent = false)
        {
            LinkedList<HandlerRecord> messageHandlers = null;
            MessageHandler<T, V, W, U> process = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V, W, U>(enumValue, parameter1, parameter2, parameter3, parameter4, handlerNode, isDownPassEvent);
                    handlerNode = handlerNode.Next;
                }
            }
        }


        #region 消息向下传递
        private static void PassEvent(int enumValue, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.None_Paramter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.None_Paramter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler);
                if (process != null)
                    process.Invoke(enumValue);
                else
                    Debug.LogError("TriggerMessage 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction.Method);
            }
        }

        private static void PassEvent<T>(int enumValue, T parameter1, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.One_Parameter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.One_Parameter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler<T>);
                if (process != null)
                    process.Invoke(enumValue, parameter1);
                else
                    Debug.LogError("TriggerMessage<T> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction.Method);
            }

            if (isDownPassEvent)
                PassEvent(enumValue, handlerNode, isDownPassEvent);
        }

        private static void PassEvent<T, V>(int enumValue, T parameter1, V parameter2, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.Two_Parameter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.Two_Parameter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler<T, V>);
                if (process != null)
                    process.Invoke(enumValue, parameter1, parameter2);
                else
                    Debug.LogError("TriggerMessage<T,V> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction.Method);
            }
            if (isDownPassEvent)
                PassEvent<T>(enumValue, parameter1, handlerNode, isDownPassEvent);
        }

        private static void PassEvent<T, V, W>(int enumValue, T parameter1, V parameter2, W parameter3, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.Three_Parameter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.Three_Parameter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler<T, V, W>);
                if (process != null)
                    process.Invoke(enumValue, parameter1, parameter2, parameter3);
                else
                    Debug.LogError("TriggerMessage<T,V,W> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction.Method);
            }
            if (isDownPassEvent)
                PassEvent<T, V>(enumValue, parameter1, parameter2, handlerNode, isDownPassEvent);
        }

        private static void PassEvent<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.Four_Parameter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.Four_Parameter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler<T, V, W, U>);
                if (process != null)
                    process.Invoke(enumValue, parameter1, parameter2, parameter3, parameter4);
                else
                    Debug.LogError("TriggerMessage<T,V,W,U> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction.Method);
            }
            if (isDownPassEvent)
                PassEvent<T, V, W>(enumValue, parameter1, parameter2, parameter3, handlerNode, isDownPassEvent);
        }
        #endregion

        #endregion



    }
}