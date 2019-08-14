using System;
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

        public static readonly object JumpToUnity = new object(); //回到主线程
        public static readonly object JumpToBackground = new object(); //到后台线程

        //***WaitForSecondsRealtime Todo
        public static readonly YieldInstruction WaitFor_OneSecond = new WaitForSeconds(1);
        public static readonly YieldInstruction WaitFor_HalfSecond = new WaitForSeconds(0.5f);
        public static readonly YieldInstruction WaitFor_OneMinute = new WaitForSeconds(60);
        public static readonly YieldInstruction WaitFor_EndOfFrame = new WaitForEndOfFrame();
        public static readonly YieldInstruction WaitFor_FixedUpdate = new WaitForFixedUpdate();
        public static readonly YieldInstruction WaitFor_Null = new YieldInstruction(); //等同 yield return null

        #region Data

        #endregion


        #region 对外 Interface

        /// <summary>/// 开启一个协程任务/// </summary>
        public static SuperCoroutine StartCoroutineEx(IEnumerator task)
        {
            if (task == null) return null;

            SuperCoroutine coroutine = new SuperCoroutine(task);
            AsyncTracker.S_Instance.TrackAsyncTask(coroutine);
#if UNITY_EDITOR
            coroutine.CompleteSuperCoroutineEvent += OnCompleteCoroutineEx;
            RegisterCoroutine(coroutine);
#endif
            coroutine.StartCoroutine();
            return coroutine;
        }

        /// <summary>/// 停止一个协程/// </summary>
        public static void StopCoroutineEx(SuperCoroutine coroutine)
        {
#if UNITY_EDITOR
            UnRegisterCoroutine(coroutine);
            coroutine.CompleteSuperCoroutineEvent -= OnCompleteCoroutineEx;
#endif
            AsyncTracker.S_Instance.UnTrackAsyncTask(coroutine);
            coroutine.StopCoroutine();
            coroutine = null; //注意这里 多加了一个=null 来清理资源
        }

        /// <summary>/// 开启一个异步的任务/// </summary>  
        public static SuperCoroutine StartAsyncOperation(AsyncOperation async, Action completeCallback, Action<float> procressCallback)
        {
            if (async == null)
            {
                Debug.LogError("StartAsyncOperation 参数异常");
                return null;
            }

            return StartCoroutineEx(StartAsyncOperate(async, completeCallback, procressCallback));
        }

        /// <summary>/// 延迟一段时间后执行操作(类似MonoBehavior.Invoke)/// </summary>
        public static SuperCoroutine Invoke(float delayTime, System.Action action)
        {
            if (delayTime <= 0)
            {
                action?.Invoke();
                return null;
            }

            return StartCoroutineEx(DelayDoAction(delayTime, action));
        }

        /// <summary>/// 延迟一段时间后每repeatRate 秒执行一次操作操作(类似MonoBehavior.InvokeRepeating)/// </summary>
        public static SuperCoroutine InvokeRepeating(float time, float repeatRate, System.Action action)
        {
            if ((double) repeatRate <= 9.99999974737875E-06 && (double) repeatRate != 0.0)
                throw new UnityException("Invoke repeat rate has to be larger than 0.00001F)");

            return StartCoroutineEx(DelayDoActionRepeat(time, repeatRate, action));
        }

        #endregion


        #region 内部实现

        //延迟一段时间后执行一次
        private static IEnumerator DelayDoAction(float delayTime, System.Action action)
        {
            if (delayTime >= 0f)
                yield return new WaitForSeconds(delayTime);
            action?.Invoke();
        }

        /// <summary>/// 延迟一段时间后每repeatRate 秒执行一次 执行一次/// </summary>
        private static IEnumerator DelayDoActionRepeat(float time, float repeatRate, System.Action action)
        {
            if (time > 0)
                yield return new WaitForSeconds(time);

            action?.Invoke();

            YieldInstruction yieldInstruction = new WaitForSeconds(repeatRate);
            while (true)
            {
                yield return yieldInstruction;
                action?.Invoke();
            }
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

        #endregion


        #region 记录正在运行的协程

#if UNITY_EDITOR
        //Key =任务id
        private static Dictionary<int, SuperCoroutine> s_AllCreateCoroutines = new Dictionary<int, SuperCoroutine>(); //创建的协程


        //注册的协程
        private static bool RegisterCoroutine(SuperCoroutine coroutine)
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
                    Debug.LogEditorError($"RegisterCoroutine Fail, Already Exit {coroutine.CoroutineID}");
                    return false;
                }

                Debug.LogEditorError($"RegisterCoroutine Fail, Already Exit,but reference not equal {coroutine.CoroutineID}");
                return false;
            }

            s_AllCreateCoroutines[coroutine.CoroutineID] = record;
            return true;
        }

        //取消注册协程
        private static bool UnRegisterCoroutine(SuperCoroutine coroutine)
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

            Debug.LogEditorError($"UnRegisterCoroutine Fail, not Exit {coroutine.ToString()}");
            return false;
        }


        /// <summary>/// 协程完成回调/// </summary>
        private static void OnCompleteCoroutineEx(SuperCoroutine coroutine)
        {
            if (coroutine == null) return;
            coroutine.CompleteSuperCoroutineEvent -= OnCompleteCoroutineEx;
            UnRegisterCoroutine(coroutine);
        }

#endif

        #endregion
    }
}
