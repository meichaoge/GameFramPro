using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 异步&协程管理器
    /// </summary>
    public class AsyncManager : Single_Mono_AutoCreateNotDestroy<AsyncManager>
    {
        //***WaitForSecondsRealtime Todo
        public readonly YieldInstruction WaitFor_OneSecond = new WaitForSeconds(1);
        public readonly YieldInstruction WaitFor_HalfSecond = new WaitForSeconds(0.5f);
        public readonly YieldInstruction WaitFor_OneMinute = new WaitForSeconds(60);
        public readonly YieldInstruction WaitFor_EndOfFrame = new WaitForEndOfFrame();
        public readonly YieldInstruction WaitFor_FixedUpdate = new WaitForFixedUpdate();


        #region Data
        private Dictionary<IEnumerator, Coroutine> mAllCreateCoroutines = new Dictionary<IEnumerator, Coroutine>();       //创建的协程
        #endregion


        #region 内部实现
        private bool RegisterCoroutine(IEnumerator routine, Coroutine coroutinue)
        {
            Coroutine record = null;
            if(mAllCreateCoroutines.TryGetValue(routine,out record))
            {
                if (object.ReferenceEquals(record, coroutinue))
                {
                    Debug.LogEditorError(string.Format("RegisterCoroutine Fail, Already Exit {0}", routine.Current.ToString()));
                    return false;
                }

                Debug.LogEditorError(string.Format("RegisterCoroutine Fail, Already Exit,but reference not equal {0}", routine.Current.ToString()));
                return false;
            }

            mAllCreateCoroutines[routine] = record;
            return true;
        }

        private bool UnRegisterCoroutine( Coroutine coroutinue)
        {
            IEnumerator key=null;
            foreach (var item in mAllCreateCoroutines)
            {
                if (object.ReferenceEquals(item.Value, coroutinue))
                {
                    key = item.Key;
                    break;
                }
            }

            if (key != null)
            {
                mAllCreateCoroutines.Remove(key);
                return true;
            }
            Debug.LogEditorError(string.Format("UnRegisterCoroutine Fail, not Exit {0}", coroutinue.ToString()));
            return false;
        
        }


        private IEnumerator StartAsyncOperate(AsyncOperation async, Action<AsyncOperation> completeCallback, Action<float> procressCallback)
        {
            while (true)
            {
                if (async.isDone)
                {
                    if (procressCallback != null)
                        procressCallback.Invoke(async.progress);

                    if (completeCallback != null)
                        completeCallback.Invoke(async);
                    yield break;
                }

                if (procressCallback != null)
                    procressCallback.Invoke(async.progress);
            }
        }

        #endregion

        #region Interface

        /// <summary>
        /// 开启一个协程任务
        /// </summary>
        /// <param name="routine"></param>
        public Coroutine StartCoroutineEx(IEnumerator routine)
        {
            Coroutine coroutinue = StartCoroutine(routine);
            RegisterCoroutine(routine, coroutinue);
            AsyncTracker.S_Instance.TrackAsyncTask(coroutinue);

            return coroutinue;
        }

        /// <summary>
        /// 停止一个协程
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutineEx(Coroutine routine)
        {
            AsyncTracker.S_Instance.UnTrackAsyncTask(routine);
            UnRegisterCoroutine(routine);
            StopCoroutine(routine);
            routine = null; //注意这里 多加了一个=null 来清理资源
        }


        public void StartAsyncOperation(AsyncOperation async,Action<AsyncOperation> completeCallback,Action<float> procressCallback)
        {
            StartCoroutineEx(StartAsyncOperate(async, completeCallback, procressCallback));
        }


        #endregion


    }
}