using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;


namespace GameFramePro
{
    public delegate void CompleteCoroutineExHandler(CoroutineEx coroutine);

    /// <summary>/// 标示 CoroutineEx 的状态/// </summary>
    public enum CoroutineExStateEnum
    {
        Initialed, //初始化
        Running, //正在运行
        Break, //打断结束任务
        Complete, // 完成
    }


    /// <summary>/// **扩展 Unity 内置的 Coroutine 提供完成协程的事件和标示, 方便获取协程的状态/// </summary>
    /// 注意嵌套的 CoroutineEx 无法通过外层的CoroutineEx 结束，与MonoBehavior 中的协程一样
    public class CoroutineEx : YieldInstruction
    {
        private static readonly AsyncManager s_AsyncManager = AsyncManager.S_Instance;
        private static int s_CoroutineID = 0;

        /// <summary>/// 标示当前的协程任务状态/// </summary>
        public CoroutineExStateEnum CoroutineState { get; private set; }

        /// <summary>/// 协程的id 唯一/// </summary>
        public int CoroutineID { get; private set; }

        public event CompleteCoroutineExHandler OnCompleteCoroutineExEvent;

        private IEnumerator mTaskCoroutine = null; //用户要做的任务
        private Coroutine mCoroutine = null;

        #region 构造函数

        public CoroutineEx()
        {
            CoroutineID = GetNextCoroutineID();
            CoroutineState = CoroutineExStateEnum.Initialed;
        }

        public CoroutineEx(IEnumerator task) : this()
        {
            if (task == null)
            {
                Debug.LogError("初始化 CoroutineEx 失败，指定的参数为null");
                return;
            }

            mTaskCoroutine = task;
        }

        #endregion

        #region 对外接口

        /// <summary>/// 开始一个协程/// </summary>
        public CoroutineEx StartCoroutine()
        {
            if (CoroutineState != CoroutineExStateEnum.Initialed)
            {
                Debug.LogError("StartCoroutine Fail,Not Initialed State");
                return this;
            }

            mCoroutine = s_AsyncManager.StartCoroutine(InnerIEnumerator());

            // Debug.Log(mCoroutine.GetHashCode());
            return this;
        }

        /// <summary>/// 结束一个协程/// </summary>
        public void StopCoroutine()
        {
            if (mCoroutine == null) return;

            if (CoroutineState != CoroutineExStateEnum.Running)
            {
                Debug.LogError("StopCoroutine Fail,Not Running State");
                return;
            }

            //   Debug.Log(mCoroutine.GetHashCode());
            s_AsyncManager.StopCoroutine(mCoroutine);
            CoroutineState = CoroutineExStateEnum.Break;
            OnCompleteCoroutineExEvent?.Invoke(this);
        }


        /// <summary>/// 等待当前任务完成/// </summary>
        /// <param name="isAutoStart">标示如果协程是初始化没有启动状态，是否自动启动协程，默认为true</param>
        public IEnumerator WaitDone(bool isAutoStart = true)
        {
            if (CoroutineState == CoroutineExStateEnum.Complete)
                yield break;

            if (CoroutineState != CoroutineExStateEnum.Running)
            {
                if (isAutoStart && CoroutineState == CoroutineExStateEnum.Initialed)
                    StartCoroutine();
                else
                {
                    Debug.LogError("协程不是运行状态 无法使用这个接口");
                    yield break;
                }
            }

            while (true)
            {
                if (CoroutineState == CoroutineExStateEnum.Complete || CoroutineState == CoroutineExStateEnum.Break)
                    yield break;
                else
                    yield return AsyncManager.WaitFor_Null;
            }
        }

        #endregion


        #region 内部实现

        /// <summary>/// 内部执行用户的操作/// </summary>
        private IEnumerator InnerIEnumerator()
        {
            CoroutineState = CoroutineExStateEnum.Running;
            //     yield return s_AsyncManager.StartCoroutine(mTaskCoroutine); //这里不能再次启动一个协程 否则外层协程无法结束内部协程
            yield return mTaskCoroutine;
            CoroutineState = CoroutineExStateEnum.Complete;
            mCoroutine = null;

            OnCompleteCoroutineExEvent?.Invoke(this);
        }


        /// <summary>/// 获取下一个可用的协程ID 确保每个协程唯一/// </summary>
        private static int GetNextCoroutineID()
        {
            Interlocked.Increment(ref s_CoroutineID);
            return s_CoroutineID;
        }

        #endregion
    }
}
