using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace GameFramePro
{
    /// <summary>/// 标示 协程对象 的状态/// </summary>
    public enum CoroutineStateUsage
    {
        Initialed, //初始化
        Running, //正在运行
        Break, //打断结束任务
        Error, //执行错误
        Complete, // 完成
    }

    public delegate void CompleteSuperCoroutineDelegate(SuperCoroutine coroutine);

    /// <summary>/// 扩展 Unity 自带的 Coroutine 支持多线程的异步协程和主线程子线程的调转/// </summary>
    /// 注意嵌套的 CoroutineEx 无法通过外层的CoroutineEx 结束，与MonoBehavior 中的协程一样
    ///  参考插件 Thread Ninja
    //**************目前发现不能在子线程中创建启动这个协程对象*******
    public class SuperCoroutine : IEnumerator
    {
        /// <summary>/// 标示 SuperCoroutine 内部的 的状态/// </summary>
        protected enum SuperCoroutineStateUsage
        {
            Initialed, //初始化
            ToMainThread,
            ToBackgroundThread,

            //  MainRunning, //主线程 正在运行
            BackgroundRuning, //线程池中子线程运行中
            YieldReturn, //任务返回了一个值需要切换状态 切换状态
            Break, //打断结束任务
            Error, //错误异常
            Complete, // 完成
        }

        private static int s_CoroutineID = 0; //生成协程ID 的对象


        #region 内部状态

        protected object mLocker { get; } = new object();

        protected IEnumerator mInnerEnumerator = null; //用户要做的任务
        protected bool mIsStartOnMainThread = true; //标示是否在主线程启动运行

        protected object mYieldReturnData { get; set; } = null; //任务返回的值  用于在切换状态的时候保存当前的状态


        /// <summary>/// 切换状态前任务的状态/// </summary>
        protected SuperCoroutineStateUsage mPreviousSuperCoroutineState = SuperCoroutineStateUsage.Initialed;

        /// <summary>/// 当前帧任务的状态/// </summary>
        protected SuperCoroutineStateUsage mSuperCoroutineState = SuperCoroutineStateUsage.Initialed;

        #endregion


        #region 公开的状态

        /// <summary>/// 协程的id 唯一/// </summary>
        public int CoroutineID { get; protected set; }

        public event CompleteSuperCoroutineDelegate CompleteSuperCoroutineEvent;

        /// <summary>/// 当前协程的状态/// </summary>
        public CoroutineStateUsage CoroutineState
        {
            get
            {
                switch (mSuperCoroutineState)
                {
                    case SuperCoroutineStateUsage.Initialed:
                        return CoroutineStateUsage.Initialed;

                    case SuperCoroutineStateUsage.ToMainThread:
                    case SuperCoroutineStateUsage.ToBackgroundThread:
                    case SuperCoroutineStateUsage.BackgroundRuning:
                    case SuperCoroutineStateUsage.YieldReturn:
                        return CoroutineStateUsage.Running;

                    case SuperCoroutineStateUsage.Break:
                        return CoroutineStateUsage.Break;
                    case SuperCoroutineStateUsage.Error:
                        return CoroutineStateUsage.Error;
                    case SuperCoroutineStateUsage.Complete:
                        return CoroutineStateUsage.Complete;
                    default:
                        Debug.LogError($"获取状态异常 没有定义的状态 mSuperCoroutineState={mSuperCoroutineState}");
                        return CoroutineStateUsage.Error;
                }
            }
        }

        #endregion


        #region 构造函数

        public SuperCoroutine(IEnumerator task, bool isRunOnMainThead = true)
        {
            mInnerEnumerator = task;
            CoroutineID = GetNextCoroutineID();
            mSuperCoroutineState = SuperCoroutineStateUsage.Initialed;
            mIsStartOnMainThread = isRunOnMainThead;
        }

        #endregion

        #region 对外接口

        public SuperCoroutine StartCoroutine()
        {
            AsyncManager.S_Instance.StartCoroutine(this);
            return this;
        }

        public void StopCoroutine()
        {
            AsyncManager.S_Instance.StopCoroutine(this);

            if (mSuperCoroutineState < SuperCoroutineStateUsage.Break)
                ChangeSuperCoroutineState(SuperCoroutineStateUsage.Break);
        }

        public IEnumerator WaitDone(bool isAutoStart = true)
        {
            if (mSuperCoroutineState == SuperCoroutineStateUsage.Initialed && isAutoStart)
                StartCoroutine();

            while (mSuperCoroutineState < SuperCoroutineStateUsage.Break)
                yield return AsyncManager.WaitFor_Null;
        }

        #endregion


        #region IEnumerator 接口

        //返回 false 时候停止
        public bool MoveNext()
        {
            return InnerMoveNext();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }


        public object Current { get; protected set; } = null;

        #endregion

        #region 内部实现

        //外部接口调用协程时候的返回的状态
        private bool InnerMoveNext()
        {
            if (mInnerEnumerator == null)
                return false;

            Current = null; //确保上一次缓存的值不会被使用 
            while (true) //这里使用一个循环是为了切换状态的时候能够及时的跳转，否则要处理大量逻辑的条条件
            {
                //    Debug.Log($"Frame={Time.frameCount}  mSuperCoroutineState={mSuperCoroutineState}");
                switch (mSuperCoroutineState)
                {
                    case SuperCoroutineStateUsage.Initialed:
                        if (mIsStartOnMainThread)
                            ChangeSuperCoroutineState(SuperCoroutineStateUsage.ToMainThread);
                        else
                            ChangeSuperCoroutineState(SuperCoroutineStateUsage.ToBackgroundThread);
                        break;

                    case SuperCoroutineStateUsage.ToMainThread:
                        GetInnerTaskState();
                        break;
                    case SuperCoroutineStateUsage.ToBackgroundThread:
                        ChangeSuperCoroutineState(SuperCoroutineStateUsage.BackgroundRuning);
                        RunOnBackground();
                        break;
                    case SuperCoroutineStateUsage.BackgroundRuning:
                        return true; //后台运行时每次主动查询状态时候都是返回true 除非子线程只执行完更新状态
                    case SuperCoroutineStateUsage.YieldReturn:
                        if (mYieldReturnData == AsyncManager.JumpToUnity)
                        {
                            ChangeSuperCoroutineState(SuperCoroutineStateUsage.ToMainThread);
                        } //用户想切换到主线程
                        else if (mYieldReturnData == AsyncManager.JumpToBackground)
                        {
                            ChangeSuperCoroutineState(SuperCoroutineStateUsage.ToBackgroundThread);
                        } //用户想切换子线程
                        else
                        {
                            Current = mYieldReturnData; //使得Unity 能够获取内部任务的返回值

                            //继续回到原来的线程
                            if (mPreviousSuperCoroutineState == SuperCoroutineStateUsage.ToMainThread)
                                mYieldReturnData = AsyncManager.JumpToUnity;
                            else
                                mYieldReturnData = AsyncManager.JumpToBackground;
                            return true;
                        } //任务返回有值 维持之前的线程状态

                        break;
                    case SuperCoroutineStateUsage.Break:
                    case SuperCoroutineStateUsage.Error:
                        return false;
                    case SuperCoroutineStateUsage.Complete:
                        if (CompleteSuperCoroutineEvent != null)
                            CompleteSuperCoroutineEvent.Invoke(this);
                        return false;
                    default:
                        Debug.LogError($"异常的任务状态={mSuperCoroutineState}");
                        return false;
                }
            }
        }


        /// <summary>/// 切换到新的状态  (内部实现了多线程安全机制)/// </summary>
        protected void ChangeSuperCoroutineState(SuperCoroutineStateUsage newState)
        {
            if (mSuperCoroutineState == newState) return;
            lock (mLocker)
            {
                mPreviousSuperCoroutineState = mSuperCoroutineState;
                mSuperCoroutineState = newState;
            }
        }

        /// <summary>/// 记录任务返回的值  用于在切换状态的时候保存当前的状态/// </summary>
        protected void SaveYieldReturnData(object data)
        {
            lock (mLocker)
            {
                mYieldReturnData = data;
            }
        }


        /// <summary>/// 开始在后台运行/// </summary>
        protected void RunOnBackground()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(BackgroundTask));
        }

        /// <summary>/// 真实的后台线程任务/// </summary>
        protected void BackgroundTask(object obj)
        {
            GetInnerTaskState();
        }


        /// <summary>/// 获取当前的任务状态是否可用/// </summary>
        protected void GetInnerTaskState()
        {
            try
            {
                bool isNextEnabel = mInnerEnumerator.MoveNext(); //获取内部任务的状态 判断是否结束了

                if (isNextEnabel)
                {
                    SaveYieldReturnData(mInnerEnumerator.Current);
                    ChangeSuperCoroutineState(SuperCoroutineStateUsage.YieldReturn); //说明协程任务有返回值
                }
                else
                {
                    ChangeSuperCoroutineState(SuperCoroutineStateUsage.Complete); //说明协程任务已经完成了
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception On MoveNext Error={e.ToString()}");
                ChangeSuperCoroutineState(SuperCoroutineStateUsage.Error);
            }
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