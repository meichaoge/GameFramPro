using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


namespace GameFramePro
{
    public delegate void CompleteCoroutineExHandler(CoroutineEx coroutine);

    /// <summary>
    /// **扩展 Unity 内置的 Coroutine 提供完成协程的事件和标示, 方便获取协程的状态
    /// </summary>
    public class CoroutineEx : YieldInstruction
    {
        private static readonly AppManager s_AppManager = AppManager.S_Instance;
        private static int s_CoroutineID = 0;

        /// <summary>
        /// 标示是否完成了任务
        /// </summary>
        public bool IsComplete { get; private set; } = false;

        /// <summary>
        /// 协程的id 唯一
        /// </summary>
        public int CoroutineID { get; private set; }
        public event CompleteCoroutineExHandler OnCompleteCoroutineExEvent;


        private IEnumerator mTaskCoroutine = null; //用户要做的任务
        private Coroutine mCoroutine = null;

        #region 构造函数

        public CoroutineEx()
        {
            IsComplete = false;
            CoroutineID = GetNextCoroutineID();
        }

        public CoroutineEx(IEnumerator task ) : base()
        {
            mTaskCoroutine = task;
        }

        #endregion

        #region 对外接口

        /// <summary>
        /// 开始一个协程
        /// </summary>
        /// <returns></returns>
        public CoroutineEx StartCoroutine()
        {
            IsComplete = false;
            mCoroutine = s_AppManager.StartCoroutine(InnerIEnumerator());
            return this;
        }

        /// <summary>
        /// 结束一个协程
        /// </summary>
        /// <param name="routine"></param>
        public void StopCoroutine()
        {
            if (mCoroutine == null) return;
            s_AppManager.StopCoroutine(mCoroutine);
            IsComplete = false;
            OnCompleteCoroutineExEvent?.Invoke(this);
        }

        
        /// <summary>
        /// TODO  需要测试
        /// </summary>
        /// <returns></returns>
        public IEnumerator WaitDone()
        {
            while (true)
            {
                if (IsComplete)
                    yield break;
            }
        }
        
        
        #endregion
        

        #region 内部实现

        /// <summary>
        /// 内部执行用户的操作
        /// </summary>
        /// <returns></returns>
        private IEnumerator InnerIEnumerator()
        {
            if (mTaskCoroutine != null)
                yield return s_AppManager.StartCoroutine(mTaskCoroutine);
            IsComplete = true;
            mCoroutine = null;

            OnCompleteCoroutineExEvent?.Invoke(this);
            yield break;
        }


        //获取下一个可用的协程ID 确保每个协程唯一
        private static int GetNextCoroutineID()
        {
            Interlocked.Increment(ref s_CoroutineID);
            return s_CoroutineID;
        } 
        
        
        #endregion
    }
}