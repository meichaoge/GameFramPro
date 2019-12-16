using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

namespace GameFramePro
{
    public delegate void MessageHandler(int enumValue);

    public delegate void MessageHandler<T>(int enumValue, T parameter1);

    public delegate void MessageHandler<T, V>(int enumValue, T parameter1, V parameter2);

    public delegate void MessageHandler<T, V, W>(int enumValue, T parameter1, V parameter2, W parameter3);

    public delegate void MessageHandler<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4);


    /// <summary>
    /// 泛型版本
    /// /// </summary>
    public static partial class EventManager
    {
        private static Dictionary<int, LinkedList<HandlerRecord>> mAllMessageHandlers = new Dictionary<int, LinkedList<HandlerRecord>>(); //所有的消息处理中心

        #region 注册监听

        public static void RegisterMessageHandler(int enumValue, MessageHandler process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.None_Paramter, process);
        }

        public static void RegisterMessageHandler<T>(int enumValue, MessageHandler<T> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.One_Parameter, process);
        }

        public static void RegisterMessageHandler<T, V>(int enumValue, MessageHandler<T, V> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Two_Parameter, process);
        }

        public static void RegisterMessageHandler<T, V, W>(int enumValue, MessageHandler<T, V, W> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Three_Parameter, process);
        }

        public static void RegisterMessageHandler<T, V, W, U>(int enumValue, MessageHandler<T, V, W, U> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Four_Parameter, process);
        }

        private static void MessageHandlerRegister(int enumValue, HandlerTypeEnum handlerType, Delegate handler)
        {
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers) == false)
            {
                messageHandlers = new LinkedList<HandlerRecord>();
                mAllMessageHandlers[enumValue] = messageHandlers;
            }
            if (IsRegisterMessageHandler(messageHandlers, handlerType, handler))
            {
                //     Debug.LogError($"重复注册事件{enumValue} ：{handler}");
                return;
            }

            //       Debug.Log($"------------{handler.GetHashCode()}   {handler}");

            HandlerRecord handlerRecord = HandlerRecord.GetHandlerRecord(handlerType, handler);
            messageHandlers.AddLast(handlerRecord);
        }

        private static bool IsRegisterMessageHandler(LinkedList<HandlerRecord> messageHandlers, HandlerTypeEnum handlerType, Delegate handler)
        {
            if (messageHandlers == null || messageHandlers.Count == 0 || handler == null)
                return false;
            foreach (var handlerRecord in messageHandlers)
            {
                if (handlerRecord == null || handlerRecord.HandlerFunction == null)
                    continue;
                if (handlerRecord.HandlerType != handlerType)
                    continue;
                if (handlerRecord.HandlerFunction.GetHashCode() == handler.GetHashCode())
                    return true;
            }
            return false;
        }

        #endregion

        #region 移除监听

        public static void UnRegisterMessageHandler(int enumValue, MessageHandler process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.None_Paramter, process);
        }

        public static void UnRegisterMessageHandler<T>(int enumValue, MessageHandler<T> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.One_Parameter, process);
        }

        public static void UnRegisterMessageHandler<T, V>(int enumValue, MessageHandler<T, V> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Two_Parameter, process);
        }

        public static void UnRegisterMessageHandler<T, V, W>(int enumValue, MessageHandler<T, V, W> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Three_Parameter, process);
        }

        public static void UnRegisterMessageHandler<T, V, W, U>(int enumValue, MessageHandler<T, V, W, U> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Four_Parameter, process);
        }

        private static void MessageHandlerUnRegister(int enumValue, HandlerTypeEnum handlerType, Delegate handler)
        {
            if (handler == null)
            {
                Debug.LogError($"参数异常 无法比较两个委托是否相等  {enumValue}  {handlerType}{handler}");
                return;
            }

            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    var messageHandler = handlerNode.Value;
                    if (messageHandler.HandlerType == handlerType && messageHandler.HandlerFunction == handler)
                    {
                        //Debug.Log($"UnRegisterMessageHandler<T,V,W,U> Fail, ID={enumValue}  Handler={handler}");
                        messageHandlers.Remove(handlerNode);
                        HandlerRecord.ReleaseHandlerRecord(messageHandler);
                        return;
                    }
                    handlerNode = handlerNode.Next;
                }
            }
            //Debug.LogError($"UnRegisterMessageHandler<T,V,W,U> Fail, ID={enumValue}  Handler={process.Method}");
        }



        public static void UnRegisterAllMessageHandler(int enumValue)
        {
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    HandlerRecord.ReleaseHandlerRecord(handlerNode.Value);
                    handlerNode = handlerNode.Next;
                }

                messageHandlers.Clear();
            }

            return;
        }

        #endregion

        #region 触发消息

        /// <summary>/// 处理消息/// </summary>
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

        /// <summary>/// 触发事件/// </summary>
        public static void TriggerMessage<T>(int enumValue, T parameter1)
        {
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T>(enumValue, parameter1, handlerNode, false);
                    handlerNode = handlerNode.Next;
                }
            }
        }

        public static void TriggerMessage<T, V>(int enumValue, T parameter1, V parameter2)
        {
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V>(enumValue, parameter1, parameter2, handlerNode);
                    handlerNode = handlerNode.Next;
                }
            }
        }

        public static void TriggerMessage<T, V, W>(int enumValue, T parameter1, V parameter2, W parameter3)
        {
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V, W>(enumValue, parameter1, parameter2, parameter3, handlerNode);
                    handlerNode = handlerNode.Next;
                }
            }
        }

        public static void TriggerMessage<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4)
        {
            MessageHandler<T, V, W, U> process = null;
            if (mAllMessageHandlers.TryGetValue(enumValue, out var messageHandlers))
            {
                var handlerNode = messageHandlers.First;
                while (handlerNode != null)
                {
                    PassEvent<T, V, W, U>(enumValue, parameter1, parameter2, parameter3, parameter4, handlerNode);
                    handlerNode = handlerNode.Next;
                }
            }
        }


        #region 消息向下传递 （由于测试时发现传递事件时候容易在处理事件时候清空了其他的事件 所以取消 isDownPassEvent 参数的作用）

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
                    Debug.LogError("TriggerMessage 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction);
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
                    Debug.LogError("TriggerMessage<T> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction);
            }

            //            if (isDownPassEvent)
            //                PassEvent(enumValue, handlerNode, isDownPassEvent);
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
                    Debug.LogError("TriggerMessage<T,V> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction);
            }

            //            if (isDownPassEvent)
            //                PassEvent<T>(enumValue, parameter1, handlerNode, isDownPassEvent);
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
                    Debug.LogError("TriggerMessage<T,V,W> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction);
            }

            //            if (isDownPassEvent)
            //                PassEvent<T, V>(enumValue, parameter1, parameter2, handlerNode, isDownPassEvent);
        }

        private static void PassEvent<T, V, W, U>(int enumValue, T parameter1, V parameter2, W parameter3, U parameter4, LinkedListNode<HandlerRecord> handlerNode, bool isDownPassEvent = false)
        {
            if (isDownPassEvent == false && handlerNode.Value.HandlerType != HandlerTypeEnum.Four_Parameter)
                return;

            if (handlerNode.Value.HandlerType == HandlerTypeEnum.Four_Parameter)
            {
                var process = (handlerNode.Value.HandlerFunction as MessageHandler<T, V, W, U>);
                if (process != null)
                {
                    process.Invoke(enumValue, parameter1, parameter2, parameter3, parameter4);
                    return;
                }
                var process2 = (handlerNode.Value.HandlerFunction as MessageHandler<object, object, object, object>);
                if (process2 != null)
                {
                    process2.Invoke(enumValue, parameter1, parameter2, parameter3, parameter4);
                    return;
                }
                else
                    Debug.LogError("TriggerMessage<T,V,W,U> 异常，无法转换处理过程 " + handlerNode.Value.HandlerFunction);
            }

            //            if (isDownPassEvent)
            //                PassEvent<T, V, W>(enumValue, parameter1, parameter2, parameter3, handlerNode, isDownPassEvent);
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// 通用object 类型参数
    ///  </summary>
    public static partial class EventManager
    {

        #region 注册监听

        public static void RegisterMessageHandler(int enumValue, MessageHandler<object> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.One_Parameter, process);
        }

        public static void RegisterMessageHandler(int enumValue, MessageHandler<object, object> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Two_Parameter, process);
        }

        public static void RegisterMessageHandler(int enumValue, MessageHandler<object, object, object> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Three_Parameter, process);
        }

        public static void RegisterMessageHandler(int enumValue, MessageHandler<object, object, object, object> process)
        {
            MessageHandlerRegister(enumValue, HandlerTypeEnum.Four_Parameter, process);
        }

        #endregion

        #region 移除监听

        public static void UnRegisterMessageHandler(int enumValue, MessageHandler<object> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.One_Parameter, process);
        }

        public static void UnRegisterMessageHandler(int enumValue, MessageHandler<object, object> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Two_Parameter, process);
        }

        public static void UnRegisterMessageHandler(int enumValue, MessageHandler<object, object, object> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Three_Parameter, process);
        }

        public static void UnRegisterMessageHandler(int enumValue, MessageHandler<object, object, object, object> process)
        {
            MessageHandlerUnRegister(enumValue, HandlerTypeEnum.Four_Parameter, process);
        }

        #endregion
    }

    /// <summary>
    /// 非泛型版本 调用 泛型
    /// </summary>
    public static partial class EventManager
    {
        private static readonly string UnRegisterMessageHandlerName = "UnRegisterMessageHandler";
        private static readonly string RegisterMessageHandlerName = "RegisterMessageHandler";

        //定义需要访问的 监听和取消监听对应的 委托类型
        internal delegate void MessageRegisterAdapter<T>(int enumValue, MessageHandler<T> process);
        internal delegate void MessageRegisterAdapter<T, V>(int enumValue, MessageHandler<T, V> process);
        internal delegate void MessageRegisterAdapter<T, V, W>(int enumValue, MessageHandler<T, V, W> process);
        internal delegate void MessageRegisterAdapter<T, V, W, U>(int enumValue, MessageHandler<T, V, W, U> process);

        //缓存需要调用的泛型签名对应的接口
        private static Dictionary<string, IBaseMessageRegister> mIBaseMessageRegisterMap = new Dictionary<string, IBaseMessageRegister>();

        internal interface IBaseMessageRegister
        {
        }

        /// <summary>
        /// 非泛型方法反射 获取 泛型的调用接口
        /// </summary>
        /// <param name="methodName">要调用的方法名</param>
        /// <param name="delegateType">要调用的泛型方法对应的委托类型</param>
        /// <param name="interfaceType"> 要调用的泛型方法对应的委托类型 可被访问的接口类型</param>
        /// <param name="genericTypes">要调用的泛型方法类型参数</param>
        /// <returns></returns>
        private static IBaseMessageRegister GetRegisterMessageNoGeneric( string methodName, Type delegateType, Type interfaceType, Type[] genericTypes)
        {
            string delegateKey = methodName;
            foreach (var item in genericTypes)
                delegateKey += item.Name;

            Type classType = typeof(EventManager);

            if (mIBaseMessageRegisterMap.TryGetValue(delegateKey, out var registerMessage) == false)
            {
                MethodInfo mi = classType.GetMethods().Single(x =>
                {
                    string name = x.Name;
                    var p = x.GetParameters();
                    var g = x.GetGenericArguments();
                    return x.Name == methodName &&
                      p.Length == 2 &&
                      g.Length == genericTypes.Length;
                    //    p[0].ParameterType == genericType &&
                    //  p[1].ParameterType == typeof(IList<>).MakeGenericType(g);
                    //   p[2].ParameterType == typeof(Attribute[]);
                });
                //       MethodInfo mi = classType.GetMethod(methodName, BindingFlags.Static);
                MethodInfo gmi = mi.MakeGenericMethod(genericTypes);
                Delegate gmd = Delegate.CreateDelegate(delegateType.MakeGenericType(genericTypes), gmi);
                registerMessage = Activator.CreateInstance(interfaceType.MakeGenericType(genericTypes), gmd) as IBaseMessageRegister;
                mIBaseMessageRegisterMap[delegateKey] = registerMessage;
            }
            return registerMessage;
        }

        #region 一个参数
        private static Dictionary<string, IMessageRegister> mIRegisterMessageMap = new Dictionary<string, IMessageRegister>();

        internal interface IMessageRegister : IBaseMessageRegister
        {
            void MessageHandlerRegister(int enumValue, MessageHandler<object> process1);
        }
        internal class RegisterMessage_NoGeneric<T> : IMessageRegister
        {
            private static Dictionary<int, Delegate> sMessageDelegateMap3 = new Dictionary<int, Delegate>();
            // 获取指定参数的泛型委托对象
            private static MessageHandler<T> GetMessageHandlerAdapter(MessageHandler<object> procress)
            {
                if (procress == null)
                    return null;

                if (sMessageDelegateMap3.TryGetValue(procress.GetHashCode(), out var resutlt) == false)
                {
                    MessageHandler<T> handler = (messageID, result1) => { procress?.Invoke(messageID, result1); };
                    resutlt = handler;
                    sMessageDelegateMap3[procress.GetHashCode()] = resutlt;
                }
                return resutlt as MessageHandler<T>;
            }
            private MessageRegisterAdapter<T> mResisterMessageAdapter;

            public RegisterMessage_NoGeneric(MessageRegisterAdapter<T> gmd)
            {
                mResisterMessageAdapter = gmd;
            }

            public void MessageHandlerRegister(int enumValue, MessageHandler<object> process1)
            {
                //mResisterMessageAdapter1(enumValue, (messageID, result) =>
                //{
                //    process1?.Invoke(messageID, result);
                //});  //这个方式会导致每次添加的监听都不一致

                MessageHandler<T> handler = GetMessageHandlerAdapter(process1);
                mResisterMessageAdapter(enumValue, handler);
            }
        }

        public static void UnRegisterMessageHandlerEx(int enumValue, Type type, MessageHandler<object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric("UnRegisterMessageHandler", typeof(MessageRegisterAdapter<>), typeof(RegisterMessage_NoGeneric<>), new Type[] { type });
            (baseMessageRegister as IMessageRegister).MessageHandlerRegister(enumValue, process);
        }
        public static void RegisterMessageHandlerEx(int enumValue, Type type, MessageHandler<object> process)
        {
            //**反射调用
            //MethodInfo mi = typeof(EventManager).GetMethod(RegisterMessageHandlerName);
            //MethodInfo gmi = mi.MakeGenericMethod(type);
            //gmi.Invoke(new object[] { enumValue, process });

            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric( RegisterMessageHandlerName, typeof(MessageRegisterAdapter<>), typeof(RegisterMessage_NoGeneric<>), new Type[] { type });
            (baseMessageRegister as IMessageRegister).MessageHandlerRegister(enumValue, process);
        }
       

        #endregion

        #region 两个参数

        internal interface IMessageRegister2 : IBaseMessageRegister
        {
            void MessageHandlerRegister(int enumValue, MessageHandler<object, object> process1);
        }
        internal class RegisterMessage_NoGeneric<T, V> : IMessageRegister2
        {

            private static Dictionary<int, Delegate> sMessageDelegateMap3 = new Dictionary<int, Delegate>();
            // 获取指定参数的泛型委托对象
            private static MessageHandler<T, V> GetMessageHandlerAdapter(MessageHandler<object, object> procress)
            {
                if (procress == null)
                    return null;

                if (sMessageDelegateMap3.TryGetValue(procress.GetHashCode(), out var resutlt) == false)
                {
                    MessageHandler<T, V> handler = (messageID, result1, resule2) => { procress?.Invoke(messageID, result1, resule2); };

                    resutlt = handler;
                    sMessageDelegateMap3[procress.GetHashCode()] = resutlt;
                }
                return resutlt as MessageHandler<T, V>;
            }
            private MessageRegisterAdapter<T, V> mResisterMessageAdapter;

            public RegisterMessage_NoGeneric(MessageRegisterAdapter<T, V> messageAdapter)
            {
                mResisterMessageAdapter = messageAdapter;
            }
            public void MessageHandlerRegister(int enumValue, MessageHandler<object, object> process1)
            {
                MessageHandler<T, V> handler = GetMessageHandlerAdapter(process1);
                mResisterMessageAdapter(enumValue, handler);
            }

        }


        public static void UnRegisterMessageHandlerEx(int enumValue, Type type1, Type type2, MessageHandler<object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric(UnRegisterMessageHandlerName, typeof(MessageRegisterAdapter<,>), typeof(RegisterMessage_NoGeneric<,>), new Type[] { type1, type2 });
            (baseMessageRegister as IMessageRegister2).MessageHandlerRegister(enumValue, process);

        }
        public static void RegisterMessageHandlerEx(int enumValue, Type type1, Type type2, MessageHandler<object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric( RegisterMessageHandlerName, typeof(MessageRegisterAdapter<,>), typeof(RegisterMessage_NoGeneric<,>), new Type[] { type1, type2 });
            (baseMessageRegister as IMessageRegister2).MessageHandlerRegister(enumValue, process);
        }

        #endregion

        #region 三个参数

        internal interface IMessageRegister3 : IBaseMessageRegister
        {
            void MessageHandlerRegister(int enumValue, MessageHandler<object, object, object> process1);
        }
        internal class RegisterMessage_NoGeneric<T, V, W> : IMessageRegister3
        {
            private static Dictionary<int, Delegate> sMessageDelegateMap3 = new Dictionary<int, Delegate>();
            // 获取指定参数的泛型委托对象
            private static MessageHandler<T, V, W> GetMessageHandlerAdapter(MessageHandler<object, object, object> procress)
            {
                if (procress == null)
                    return null;

                if (sMessageDelegateMap3.TryGetValue(procress.GetHashCode(), out var resutlt) == false)
                {
                    MessageHandler<T, V, W> handler = (messageID, result1, resule2, resule3) => { procress?.Invoke(messageID, result1, resule2, resule3); };

                    resutlt = handler;
                    sMessageDelegateMap3[procress.GetHashCode()] = resutlt;
                }
                return resutlt as MessageHandler<T, V, W>;
            }


            private MessageRegisterAdapter<T, V, W> mResisterMessageAdapter;

            public RegisterMessage_NoGeneric(MessageRegisterAdapter<T, V, W> messageAdapter)
            {
                mResisterMessageAdapter = messageAdapter;
            }
            public void MessageHandlerRegister(int enumValue, MessageHandler<object, object, object> process1)
            {
                MessageHandler<T, V, W> handler = GetMessageHandlerAdapter(process1);
                mResisterMessageAdapter(enumValue, handler);
            }
        }


        public static void UnRegisterMessageHandlerEx(int enumValue, Type type1, Type type2, Type type3, MessageHandler<object, object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric(UnRegisterMessageHandlerName, typeof(MessageRegisterAdapter<,,>), typeof(RegisterMessage_NoGeneric<,,>), new Type[] { type1, type2, type3 });
            (baseMessageRegister as IMessageRegister3).MessageHandlerRegister(enumValue, process);

        }
        public static void RegisterMessageHandlerEx(int enumValue, Type type1, Type type2, Type type3, MessageHandler<object, object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric( RegisterMessageHandlerName, typeof(MessageRegisterAdapter<,,>), typeof(RegisterMessage_NoGeneric<,,>), new Type[] { type1, type2, type3 });
            (baseMessageRegister as IMessageRegister3).MessageHandlerRegister(enumValue, process);
        }



        #endregion

        #region 四个参数


        internal interface IMessageRegister4 : IBaseMessageRegister
        {
            void MessageHandlerRegister(int enumValue, MessageHandler<object, object, object, object> process1);
        }
        internal class RegisterMessage_NoGeneric<T, V, W, U> : IMessageRegister4
        {
            private static Dictionary<int, Delegate> sMessageDelegateMap4 = new Dictionary<int, Delegate>();
            // 获取指定参数的泛型委托对象
            private static MessageHandler<T, V, W, U> GetMessageHandlerAdapter(MessageHandler<object, object, object, object> procress)
            {
                if (procress == null)
                    return null;

                if (sMessageDelegateMap4.TryGetValue(procress.GetHashCode(), out var resutlt) == false)
                {
                    MessageHandler<T, V, W, U> handler = (messageID, result1, resule2, resule3, resule4) => { procress?.Invoke(messageID, result1, resule2, resule3, resule4); };

                    resutlt = handler;
                    sMessageDelegateMap4[procress.GetHashCode()] = resutlt;
                }
                return resutlt as MessageHandler<T, V, W, U>;
            }

            private MessageRegisterAdapter<T, V, W, U> mResisterMessageAdapter;

            public RegisterMessage_NoGeneric(MessageRegisterAdapter<T, V, W, U> messageAdapter)
            {
                mResisterMessageAdapter = messageAdapter;
            }
            public void MessageHandlerRegister(int enumValue, MessageHandler<object, object, object, object> process1)
            {
                MessageHandler<T, V, W, U> handler = GetMessageHandlerAdapter(process1);

                mResisterMessageAdapter(enumValue, handler);
            }
        }


        public static void UnRegisterMessageHandlerEx(int enumValue, Type type1, Type type2, Type type3, Type type4, MessageHandler<object, object, object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric(UnRegisterMessageHandlerName, typeof(MessageRegisterAdapter<,,,>), typeof(RegisterMessage_NoGeneric<,,,>), new Type[] { type1, type2, type3, type4 });
            (baseMessageRegister as IMessageRegister4).MessageHandlerRegister(enumValue, process);
        }

        public static void RegisterMessageHandlerEx(int enumValue, Type type1, Type type2, Type type3, Type type4, MessageHandler<object, object, object, object> process)
        {
            IBaseMessageRegister baseMessageRegister = GetRegisterMessageNoGeneric( RegisterMessageHandlerName, typeof(MessageRegisterAdapter<,,,>), typeof(RegisterMessage_NoGeneric<,,,>), new Type[] { type1, type2, type3, type4 });
            (baseMessageRegister as IMessageRegister4).MessageHandlerRegister(enumValue, process);
        }

        #endregion


    }


    public static partial class EventManager
    {
        /// <summary>
        /// 标示消息的监听处理的参数个数
        /// </summary>
        private enum HandlerTypeEnum
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
        private class HandlerRecord
        {
            public Delegate HandlerFunction { get; protected set; }
            public HandlerTypeEnum HandlerType { get; protected set; }

            #region 构造函数
            static HandlerRecord()
            {
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
                record.HandlerFunction = null;
                record.HandlerType = HandlerTypeEnum.None_Paramter;

            }

            #endregion

            public static HandlerRecord GetHandlerRecord(HandlerTypeEnum handlerType, Delegate handler)
            {
                HandlerRecord handlerRecord = s_MessageHandlerRecordPool.GetItemFromPool();
                handlerRecord. HandlerFunction = handler;
                handlerRecord.HandlerType = handlerType;
                return handlerRecord;
            }

            public static void ReleaseHandlerRecord(HandlerRecord handler)
            {
                s_MessageHandlerRecordPool.RecycleItemToPool(handler);
            }

        }
    }
}
