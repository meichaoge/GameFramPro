using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
   /// <summary>
   /// 下载器管理辅助
   /// </summary>
    public class DownloadUtility : Single<DownloadUtility>
    {

        /// <summary>
        /// 如果有正在下载的任务且没有完成回调 ，则直接把这个任务插入回调中
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        /// <returns>返回值标示是否成功的将当前需要下载的任务插入正在下载的任务回调中</returns>
        public static bool TryAddCallbackOnWorkingTask<T,U>(string taskUrl, System.Action<T, string> completeCallback,     Dictionary<string, U> allDownloadingTasks, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal) where U: UnityWebRequestDownloadTask<T>
        {
            foreach (var workingTask in allDownloadingTasks.Values)
            {
                if (workingTask.TaskUrl == taskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(completeCallback, priorityEnum);
                        return true;
                    } //插入正在下载的任务回调中
                }
            }

            return false;
        }

        public static bool TryAddCallbackOnWorkingTask<T,U>(U task, Dictionary<string, U> allDownloadingTasks) where U : UnityWebRequestDownloadTask<T>
        {
            foreach (var workingTask in allDownloadingTasks.Values)
            {
                if (workingTask.TaskUrl == task.TaskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(task);
                        return true;
                    } //插入正在下载的任务回调中
                }
            }
            return false;
        }

        /// <summary>
        /// 将当前任务插入到等待链表中
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        public static void PustUnityWebRequestTaskForWaitingList<T,U>(U task, System.Action<T, string> completeCallback,   LinkedList<U> allWaiteDownloadTasks) where U : UnityWebRequestDownloadTask<T>
        {
            LinkedListNode<UnityWebRequestDownloadTask<T>> node = new LinkedListNode<UnityWebRequestDownloadTask<T>>(task);

            if (allWaiteDownloadTasks.Count == 0)
            {
                allWaiteDownloadTasks.AddFirst(task);
                return;
            }

            var target = allWaiteDownloadTasks.First;
            while (target != null)
            {
                if (target.Value.mTaskPriorityEnum > task.mTaskPriorityEnum)
                {
                    target = target.Next;
                    continue;
                }

                if (target.Value.mTaskPriorityEnum == task.mTaskPriorityEnum)
                {
                    if (target.Value.TaskUrl != task.TaskUrl)
                    {
                        target = target.Next;
                        continue;
                    }

                    task.Dispose();  //公用一个下载器即可  次数可以想办法优化避免这里释放 TODO

                    target.Value.AddCompleteCallback(completeCallback); //插入到另一个等待任务回调链中
                    return;
                } //只能在这个优先级相等的地方
                else
                {
                    allWaiteDownloadTasks.AddBefore(target, task);
                    return;
                }
            }

            allWaiteDownloadTasks.AddLast(task);
        }

        /// <summary>
        /// 检测等待的任务
        /// </summary>
        public static void DoWaitingDownloadTask<T,U>(IDownloadManager<T,U> downloadManager) where U : UnityWebRequestDownloadTask<T>
        {
            if  (downloadManager. AllDownloadingTasks.Count >= downloadManager.MaxDownloadTaskCount) return;
            if (downloadManager.AllWaitDownloadTasks.Count == 0) return;

            var waitingTask = downloadManager.AllWaitDownloadTasks.First;
            while (waitingTask != null)
            {
                if (TryAddCallbackOnWorkingTask<T,U>(waitingTask.Value, downloadManager.AllDownloadingTasks) == false)
                {
#if UNITY_EDITOR
                    if (downloadManager.AllDownloadingTasks.ContainsKey(waitingTask.Value.TaskUrl))
                        Debug.LogEditorError("DOWaitingUnityWebRequestDownloadTask Fail,Already Exit Task " + waitingTask.Value.TaskUrl);
#endif

                    downloadManager.AllDownloadingTasks[waitingTask.Value.TaskUrl] = waitingTask.Value;
                    waitingTask.Value.StartTask();
                }

                if (downloadManager.AllDownloadingTasks.Count >= downloadManager.MaxDownloadTaskCount)
                    return;
                waitingTask = waitingTask.Next;
            }
        }



    }
}