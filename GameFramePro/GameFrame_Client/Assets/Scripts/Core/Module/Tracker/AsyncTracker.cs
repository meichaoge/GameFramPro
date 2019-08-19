using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.AnalysisEx
{
    /// <summary>/// 追踪异步&协程任务/// </summary>
    public class AsyncTracker : Single<AsyncTracker>
    {
        private Dictionary<int, SuperCoroutine> mAllAsyncTaskRecord = new Dictionary<int, SuperCoroutine>(); //所有的异步任务


        /// <summary>/// 跟踪记录异步信息/// </summary>
        public bool TrackAsyncTask(SuperCoroutine task)
        {
            if (AppSetting.S_IsTrackAsyncTask == false || Application.isPlaying)
                return false;

            if (mAllAsyncTaskRecord.TryGetValue(task.CoroutineID, out var infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.LogError($"TrackAsyncTask Fail,Already Register Async Task of CoroutineID={task.CoroutineID}");
                    return false;
                }

                Debug.LogError($"TrackAsyncTask Fail,Already Register Async Task of CoroutineID={task.CoroutineID} Reference Not Equal");

                return false;
            }

            mAllAsyncTaskRecord[task.CoroutineID] = task;
            Debug.Log($"TrackAsyncTask success! CoroutineID={task.CoroutineID}");
            return true;
        }

        /// <summary>/// 取消 跟踪记录异步信息/// </summary>
        public bool UnTrackAsyncTask(SuperCoroutine task)
        {
            if (AppSetting.S_IsTrackAsyncTask == false || Application.isPlaying)
                return false;

            if (mAllAsyncTaskRecord.TryGetValue(task.CoroutineID, out var infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.Log($"UnTrackAsyncTask success! CoroutineID={task.CoroutineID}");
                    mAllAsyncTaskRecord.Remove(task.CoroutineID);
                    return true;
                }

                Debug.LogError($"UnTrackAsyncTask Fail, Track Async Task of CoroutineID={task.CoroutineID} Reference Not Equal");
                return false;
            }

            mAllAsyncTaskRecord[task.CoroutineID] = task;
            Debug.LogError($"UnTrackAsyncTask Fail, no Track Async Task of CoroutineID={task.CoroutineID} ");
            return false;
        }
    }
}
