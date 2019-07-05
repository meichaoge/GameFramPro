using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro
{
    /// <summary>
    /// 追踪异步&协程任务
    /// </summary>
    public class AsyncTracker : Single<AsyncTracker>
    {

        private Dictionary<int, YieldInstruction> mAllAsyncTaskRecord = new Dictionary<int, YieldInstruction>(); //所有的异步任务



        /// <summary>
        /// 跟踪记录异步信息
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool TrackAsyncTask(YieldInstruction task)
        {
            if (AppSetting.S_Instance.IsTrackAsyncTask == false || Application.isPlaying)
                return false;

            int hashCode = task.GetHashCode();
            YieldInstruction infor = null;
            if (mAllAsyncTaskRecord.TryGetValue(hashCode, out infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.LogError(string.Format("TrackAsyncTask Fail,Already Register Async Task of hashcode={0}", hashCode));
                    return false;
                }
                Debug.LogError(string.Format("TrackAsyncTask Fail,Already Register Async Task of hashcode={0} Reference Not Equal", hashCode));

                return false;
            }

            mAllAsyncTaskRecord[hashCode] = task;
            Debug.Log(string.Format("TrackAsyncTask success! hashcode={0}", hashCode));
            return true;
        }

        /// <summary>
        /// 取消 跟踪记录异步信息
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public bool UnTrackAsyncTask(YieldInstruction task)
        {
            if (AppSetting.S_Instance.IsTrackAsyncTask == false || Application.isPlaying)
                return false;

            int hashCode = task.GetHashCode();
            YieldInstruction infor = null;
            if (mAllAsyncTaskRecord.TryGetValue(hashCode, out infor))
            {
                if (object.ReferenceEquals(infor, task))
                {
                    Debug.Log(string.Format("UnTrackAsyncTask success! hashcode={0}", hashCode));
                    mAllAsyncTaskRecord.Remove(hashCode);
                    return true;
                }
                Debug.LogError(string.Format("UnTrackAsyncTask Fail, Track Async Task of hashcode={0} Reference Not Equal", hashCode));
                return false;
            }

            mAllAsyncTaskRecord[hashCode] = task;
            Debug.LogError(string.Format("UnTrackAsyncTask Fail, no Track Async Task of hashcode={0} ", hashCode));
            return false;
        }

    }
}