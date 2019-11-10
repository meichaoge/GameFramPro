using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;


namespace GameFramePro.AnalysisEx
{
    /// <summary>/// 追踪异步&协程任务/// </summary>
    public static class AsyncTracker
    {
        private static ConcurrentDictionary<int, SuperCoroutine> s_AllAsyncTaskRecord = new ConcurrentDictionary<int, SuperCoroutine>(); //所有的异步任务


        /// <summary>/// 跟踪记录异步信息/// </summary>
        public static bool TrackAsyncTask(SuperCoroutine task)
        {
            if (ApplicationManager.mIsTrackAsyncTask == false)
                return false;

            if (s_AllAsyncTaskRecord.TryGetValue(task.CoroutineID, out var infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.LogError($"TrackAsyncTask Fail,Already Register Async Task of CoroutineID={task.CoroutineID}");
                    return false;
                }

                Debug.LogError($"TrackAsyncTask Fail,Already Register Async Task of CoroutineID={task.CoroutineID} Reference Not Equal");

                return false;
            }

            s_AllAsyncTaskRecord[task.CoroutineID] = task;
            //Debug.Log($"TrackAsyncTask success! CoroutineID={task.CoroutineID}");
            return true;
        }

        /// <summary>/// 取消 跟踪记录异步信息/// </summary>
        public static bool UnTrackAsyncTask(SuperCoroutine task)
        {
            if (ApplicationManager.mIsTrackAsyncTask == false)
                return false;

            if (s_AllAsyncTaskRecord.TryGetValue(task.CoroutineID, out var infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.Log($"UnTrackAsyncTask success! CoroutineID={task.CoroutineID}");
                    s_AllAsyncTaskRecord.TryRemove(task.CoroutineID, out var taskRecord);
                    return true;
                }

                Debug.LogError($"UnTrackAsyncTask Fail, Track Async Task of CoroutineID={task.CoroutineID} Reference Not Equal");
                return false;
            }

            s_AllAsyncTaskRecord[task.CoroutineID] = task;
            Debug.LogError($"UnTrackAsyncTask Fail, no Track Async Task of CoroutineID={task.CoroutineID} ");
            return false;
        }
    }
}