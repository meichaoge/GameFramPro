using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.NetWorkEx
{
    /// <summary>
    /// 下载器管理辅助类
    /// </summary>
    /// <typeparam name="W"> 真实的下载器类型(UnityWebRequest)</typeparam>
    public class DownloadUtility<W> : Single<DownloadUtility<W>>
    {

        /// <summary>
        /// 如果有正在下载的任务且没有完成回调 ，则直接把这个任务插入回调中
        /// </summary>
        /// <param name="taskUrl"></param>
        /// <param name="completeCallback"></param>
        /// <param name="priorityEnum"></param>
        /// <returns>返回值标示是否成功的将当前需要下载的任务插入正在下载的任务回调中</returns>
        public static bool TryAddCallbackOnWorkingTask<U>(string taskUrl, System.Action<W, bool, string> completeCallback, Dictionary<string, U> allDownloadingTasks, UnityTaskPriorityEnum priorityEnum = UnityTaskPriorityEnum.Normal) where U : BaseDownloadTask<W>
        {
            if (allDownloadingTasks == null || allDownloadingTasks.Count == 0)
                return false;

            foreach (var workingTask in allDownloadingTasks.Values)
            {
                if (workingTask.TaskUrl == taskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(completeCallback, priorityEnum);
                        return true;
                    } //插入正在下载的任务回调中
                    else
                    {
                        Debug.LogError("TryAddCallbackOnWorkingTask Error,Task State Exception " + taskUrl);
                    }
                }
            }

            return false;
        }

        public static bool TryAddCallbackOnWorkingTask<U>(U task, Dictionary<string, U> allDownloadingTasks) where U : BaseDownloadTask<W>
        {
            if (allDownloadingTasks == null || allDownloadingTasks.Count == 0)
                return false;
            foreach (var workingTask in allDownloadingTasks.Values)
            {
                if (workingTask.TaskUrl == task.TaskUrl)
                {
                    if (workingTask.IsCompleteInvoke == false)
                    {
                        workingTask.AddCompleteCallback(task);
                        return true;
                    } //插入正在下载的任务回调中
                    else
                    {
                        Debug.LogError("TryAddCallbackOnWorkingTask Error,Task State Exception " + task.TaskUrl);
                    }
                }
            }
            return false;
        }




        /// <summary>
        ///  获取指定的任务需要插入到那个位置(调用前需要自行处理原有的链表没有元素的情况)
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="taskUrl"></param>
        /// <param name="priorityEnum"></param>
        /// <param name="allWaiteDownloadTasks"></param>
        /// <param name="targetNode">要插入的节点 需要根据返回值一起使用，</param>
        /// <returns>如果targetNode 不为空, =true时候则插入其回调中,否则新建一个节点插入之前；新建一个节点插入最后</returns>
        public static bool TryAddDownloadTaskAtWaitingList<U>(string taskUrl, UnityTaskPriorityEnum priorityEnum, LinkedList<U> allWaiteDownloadTasks, out LinkedListNode<U> targetNode) where U : BaseDownloadTask<W>
        {
            //if (allWaiteDownloadTasks.Count == 0)
            //    return false; 

            var target = allWaiteDownloadTasks.First;
            while (target != null)
            {
                if (target.Value.TaskPriorityEnum > priorityEnum)
                {
                    target = target.Next;
                    continue;
                }

                if (target.Value.TaskPriorityEnum == priorityEnum)
                {
                    if (target.Value.TaskUrl != taskUrl)
                    {
                        target = target.Next;
                        continue;
                    }
                    //     task.Dispose();  //公用一个下载器即可  次数可以想办法优化避免这里释放 TODO
                    targetNode = target;  //插入到另一个等待任务回调链中
                                          //     target.Value.AddCompleteCallback(completeCallback); //插入到另一个等待任务回调链中
                    return true;
                } //只能在这个优先级相等的地方
                else
                {
                    targetNode = target;   //根据优先级插入这个元素前
                                           //  allWaiteDownloadTasks.AddBefore(target, task);
                    return false;
                }
            }
            targetNode = null;
            return false;  //要插入到最后
            //     allWaiteDownloadTasks.AddLast(task);
        }

        /// <summary>
        /// 检测等待的任务
        /// </summary>
        public static void DoWaitingDownloadTask<U>(IDownloadManager<U> downloadManager) where U : BaseDownloadTask<W>
        {
            if (downloadManager.AllDownloadingTasks.Count >= downloadManager.MaxDownloadTaskCount) return;
            if (downloadManager.AllWaitDownloadTasks.Count == 0) return;

            var waitingTask = downloadManager.AllWaitDownloadTasks.First;
            while (waitingTask != null)
            {
                if (TryAddCallbackOnWorkingTask<U>(waitingTask.Value, downloadManager.AllDownloadingTasks) == false)
                {
#if UNITY_EDITOR
                    if (downloadManager.AllDownloadingTasks.ContainsKey(waitingTask.Value.TaskUrl))
                        Debug.LogEditorError("DOWaitingUnityWebRequestDownloadTask Fail,Already Exit Task " + waitingTask.Value.TaskUrl);
#endif

                    downloadManager.AllDownloadingTasks[waitingTask.Value.TaskUrl] = waitingTask.Value;
                    waitingTask.Value.StartDownloadTask();
                }

                if (downloadManager.AllDownloadingTasks.Count >= downloadManager.MaxDownloadTaskCount)
                    return;
                waitingTask = waitingTask.Next;
            }
        }



    }
}