using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    /// <summary>
    /// 对各种类型添加一个回调处理
    /// </summary>
    public static class FormatUtility<T, U>
    {
        //key 注册的对象 value( key 关注的类型，对应的处理回调)
        private static Dictionary<object, Dictionary<Type, Func<U, T>>> s_FormatProcess = new Dictionary<object, Dictionary<Type, Func<U, T>>>();


        public static void RegisterHandler(object obj, Type type, Func<U, T> process)
        {
            Dictionary<Type, Func<U, T>> recordProcess = null;
            if (s_FormatProcess.TryGetValue(obj, out recordProcess) == false)
            {
                recordProcess = new Dictionary<Type, Func<U, T>>();
                recordProcess[type] = process;
                s_FormatProcess[obj] = recordProcess;
                return;
            }
            recordProcess[type] = process;
        }

        public static void UnRegisterHandler(object obj, Type type)
        {
            Dictionary<Type, Func<U, T>> recordProcess = null;
            if (s_FormatProcess.TryGetValue(obj, out recordProcess) == false)
                return;

            if (recordProcess.ContainsKey(type))
                recordProcess.Remove(type);
        }

        public static void UnRegistHandler(object obj)
        {
            if (s_FormatProcess.ContainsKey(obj))
                s_FormatProcess.Remove(obj);
        }

        /// <summary>
        /// 在指定对象中处理指定的数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool FormatProcess(object obj, Type type, U data, out T result)
        {
            Dictionary<Type, Func<U, T>> recordProcess = null;
            if (s_FormatProcess.TryGetValue(obj, out recordProcess))
            {
                Func<U, T> process = null;
                if (recordProcess.TryGetValue(type, out process))
                {
                    if (process != null)
                    {
                        result = process(data);
                        return true;
                    }

                    Debug.LogErrorFormat("FormatProcess Fail,Not Register Object={0} of Type {1} Process is Null", obj.ToString(), type.ToString());
                    result = default(T);
                    return false;
                }
                else
                {
                    Debug.LogErrorFormat("FormatProcess Fail,Not Register Object={0} of Type {1} ", obj.ToString(), type.ToString());
                    result = default(T);
                    return false;
                }
            }
            else
            {
                Debug.LogErrorFormat("FormatProcess Fail,Not Register Object={0}  ", obj.ToString());
                result = default(T);
                return false;
            }
        }


    }
}