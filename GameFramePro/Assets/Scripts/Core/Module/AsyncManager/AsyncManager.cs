﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace GameFramePro
{
    /// <summary>/// 异步&协程管理器/// </summary>
    public class AsyncManager : Single_Mono<AsyncManager>
    {
        protected override bool IsNotDestroyedOnLoad { get; } = true;

        //***WaitForSecondsRealtime Todo
        public static readonly YieldInstruction WaitFor_OneSecond = new WaitForSeconds(1);
        public static readonly YieldInstruction WaitFor_HalfSecond = new WaitForSeconds(0.5f);
        public static readonly YieldInstruction WaitFor_OneMinute = new WaitForSeconds(60);
        public static readonly YieldInstruction WaitFor_EndOfFrame = new WaitForEndOfFrame();
        public static readonly YieldInstruction WaitFor_FixedUpdate = new WaitForFixedUpdate();
        public static readonly YieldInstruction WaitFor_Null = new YieldInstruction(); //等同 yield return null

        #region Data

        //Key =任务id
        private static Dictionary<int, CoroutineEx> s_AllCreateCoroutines = new Dictionary<int, CoroutineEx>(); //创建的协程

        #endregion


        #region 对外 Interface

        /// <summary>/// 开启一个协程任务/// </summary>
        public static CoroutineEx StartCoroutineEx(IEnumerator task)
        {
            if (task == null) return null;

            CoroutineEx coroutine = new CoroutineEx(task);
            coroutine.OnCompleteCoroutineExEvent += OnCompleteCoroutineEx;
            RegisterCoroutine(coroutine);
            AsyncTracker.S_Instance.TrackAsyncTask(coroutine);
            coroutine.StartCoroutine();
            return coroutine;
        }

        /// <summary>/// 停止一个协程/// </summary>
        public static void StopCoroutineEx(CoroutineEx coroutine)
        {
            AsyncTracker.S_Instance.UnTrackAsyncTask(coroutine);
            UnRegisterCoroutine(coroutine);
            coroutine.OnCompleteCoroutineExEvent -= OnCompleteCoroutineEx;
            coroutine.StopCoroutine();
            coroutine = null; //注意这里 多加了一个=null 来清理资源
        }

        /// <summary>/// 开启一个异步的任务/// </summary>  
        public static CoroutineEx StartAsyncOperation(AsyncOperation async, Action completeCallback, Action<float> procressCallback)
        {
            if (async == null)
            {
                Debug.LogError("StartAsyncOperation 参数异常");
                return null;
            }

            return StartCoroutineEx(StartAsyncOperate(async, completeCallback, procressCallback));
        }

        /// <summary>/// 延迟一段时间后执行操作/// </summary>
        public static void DelayAction(float delayTime, System.Action action)
        {
            if (delayTime <= 0)
            {
                action?.Invoke();
                return;
            }

            StartCoroutineEx(DelayDoAction(delayTime, action));
        }

        #endregion


        #region 内部实现

        //注册的协程
        private static bool RegisterCoroutine(CoroutineEx coroutine)
        {
            if (coroutine == null)
            {
                Debug.LogError("RegisterCoroutine Fail!!  参数为null");
                return false;
            }

            if (s_AllCreateCoroutines.TryGetValue(coroutine.CoroutineID, out var record))
            {
                if (object.ReferenceEquals(record, coroutine))
                {
                    Debug.LogEditorError(string.Format("RegisterCoroutine Fail, Already Exit {0}", coroutine.CoroutineID));
                    return false;
                }

                Debug.LogEditorError(string.Format("RegisterCoroutine Fail, Already Exit,but reference not equal {0}", coroutine.CoroutineID));
                return false;
            }

            s_AllCreateCoroutines[coroutine.CoroutineID] = record;
            return true;
        }

        //注册协程
        private static bool UnRegisterCoroutine(CoroutineEx coroutine)
        {
            if (coroutine == null)
            {
                Debug.LogError("UnRegisterCoroutine Fail!!  参数为null");
                return false;
            }

            if (s_AllCreateCoroutines.ContainsKey(coroutine.CoroutineID))
            {
                s_AllCreateCoroutines.Remove(coroutine.CoroutineID);
                return true;
            }

            Debug.LogEditorError(string.Format("UnRegisterCoroutine Fail, not Exit {0}", coroutine.ToString()));
            return false;
        }

        private static IEnumerator DelayDoAction(float delayTime, System.Action action)
        {
            if (delayTime >= 0f)
                yield return new WaitForSeconds(delayTime);
            action?.Invoke();
        }

        //开启一个协程任务
        private static IEnumerator StartAsyncOperate(AsyncOperation async, Action completeCallback, Action<float> processCallback)
        {
            float progress = 0f;
            while (true)
            {
                if (progress != async.progress)
                {
                    progress = async.progress;
                    processCallback?.Invoke(async.progress);
                }

                if (async.isDone)
                {
                    completeCallback?.Invoke();
                    yield break;
                }

                yield return WaitFor_Null;
            }
        }

        /// <summary>/// 协程完成回调/// </summary>
        private static void OnCompleteCoroutineEx(CoroutineEx coroutine)
        {
            if (coroutine == null) return;
            coroutine.OnCompleteCoroutineExEvent -= OnCompleteCoroutineEx;
            UnRegisterCoroutine(coroutine);
        }

        #endregion
    }
}