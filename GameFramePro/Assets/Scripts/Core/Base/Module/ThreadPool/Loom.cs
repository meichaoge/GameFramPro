using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace GameFramePro
{
    /// <summary>
    /// 用来控制子线程到Unity 线程的跳转
    /// </summary>
    public class Loom : Single<Loom> , IUpdateTick
    {
        public static int _maxThreads = 5;
        private static int _numThreads = 0;

        List<Action> _actions = new List<Action>();
        List<Action> _currentActions = new List<Action>();

        public struct DelayedQueueItem
        {
            public float time;
            public Action action;
        }

        List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();
        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();
  

        #region IUpdateTick 接口
        protected int curUpdateCount = 0; //当前的帧基数
        public uint TickPerUpdateCount { get; protected set; } = 1;

        public bool CheckIfNeedUpdateTick()
        {
            //++curUpdateCount;
            //if (curUpdateCount == 1)
            //    return true;  //确保第一次被调用
            //if (curUpdateCount < TickPerUpdateCount)
            //    return false;
            //curUpdateCount = 0;
            return true;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick() == false) return;
            UpdateAction();
        }
        #endregion


        public void QueueOnMainThread(Action action)
        {
            QueueOnMainThread(action, 0f);
        }

        public  void QueueOnMainThread(Action action, float time)
        {
            if (time != 0)
            {
                lock (S_Instance._delayed)
                {
                    S_Instance._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                }
            }
            else
            {
                lock (S_Instance._actions)
                {
                    S_Instance._actions.Add(action);
                }
            }
        }

        public  Thread RunAsync(Action a)
        {
            //Initialize();
            while (_numThreads >= _maxThreads)
            {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref _numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            return null;
        }

        private static void RunAction(object action)
        {
            try
            {
                ((Action)action)();
            }
            catch (Exception e)
            {
                Debug.LogError("RunAsync Error " + e);
            }
            finally
            {
                Interlocked.Decrement(ref _numThreads);
            }
        }
       

        private void UpdateAction()
        {
            if (_actions.Count > 0)
            {
                lock (_actions)
                {
                    _currentActions.Clear();
                    _currentActions.AddRange(_actions);
                    _actions.Clear();
                }

                for (int _dex = 0; _dex < _currentActions.Count; ++_dex)
                    _currentActions[_dex]();

            }

            if (_delayed.Count > 0)
            {
                lock (_delayed)
                {
                    _currentDelayed.Clear();
                    _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                    for (int _dex = 0; _dex < _currentDelayed.Count; ++_dex)
                        _delayed.RemoveAt(_dex);
                }
                for (int _dex = 0; _dex < _currentDelayed.Count; ++_dex)
                    _currentDelayed[_dex].action();
            }//if

        }
    }
}
