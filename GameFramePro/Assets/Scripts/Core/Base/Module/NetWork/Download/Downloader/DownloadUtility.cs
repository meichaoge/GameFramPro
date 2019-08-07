//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//
//namespace GameFramePro.NetWorkEx
//{
//    /// <summary>/// 下载器管理辅助类/// </summary>
//    /// <typeparam name="W"> 真实的下载器类型(UnityWebRequest)</typeparam>
//    public class DownloadUtility<W> : Single<DownloadUtility<W>>
//    {
//        /// <summary>/// 将缓存的消息加入到正式的下载队列中/// </summary>
//        public static void TryDealWithCacheDownloadTask<U>(BaseDownloadManager<U, W> downloadManager) where U : BaseDownloadTask<W>
//        {
//            if (downloadManager == null || downloadManager.AllCacheDownloadTaskLinkedList == null || downloadManager.AllCacheDownloadTaskLinkedList.Count == 0)
//                return;
//            var targetNode = downloadManager.AllCacheDownloadTaskLinkedList.First;
//            while (targetNode != null)
//            {
//                TayAddCacheTaskToDownloadTaskLinklist<U>(targetNode.Value, downloadManager);
//                targetNode = targetNode.Next;
//            }
//
//            downloadManager.AllCacheDownloadTaskLinkedList.Clear();
//        }
//
//        /// <summary>/// 将指定的缓存任务加入到正式的下载链表中/// </summary>
//        public static void TayAddCacheTaskToDownloadTaskLinklist<U>(U cacheTask, BaseDownloadManager<U, W> downloadManager) where U : BaseDownloadTask<W>
//        {
//            if (cacheTask == null) return;
//            var targetNode = downloadManager.AllDownloadTaskLinkedList.First;
//            while (targetNode != null)
//            {
//                if (targetNode.Value.TaskPriorityEnum > cacheTask.TaskPriorityEnum)
//                {
//                    targetNode = targetNode.Next;
//                    continue;
//                } //优先级不同
//
//                var targetNodeValue = targetNode.Value;
//                if (targetNodeValue.TaskPriorityEnum == cacheTask.TaskPriorityEnum)
//                {
//                    if (targetNodeValue.TaskUrl == cacheTask.TaskUrl)
//                    {
//                        if (targetNodeValue.IsCompleteInvoke == false)
//                        {
//                            targetNodeValue.AddCompleteCallback(cacheTask);
//                            return;
//                        }
//                        else
//                        {
//                            Debug.LogError("当前下载任务已经完成，无法添加回调 " + targetNodeValue.TaskUrl);
//                        }
//                    }
//                }
//
//                if (targetNodeValue.TaskPriorityEnum < cacheTask.TaskPriorityEnum)
//                {
//                    cacheTask.ChangeDownloadState(TaskStateEum.Initialed);
//                    downloadManager.AllDownloadTaskLinkedList.AddBefore(targetNode, cacheTask); //添加新的任务到后面
//                    return;
//                }
//
//                targetNode = targetNode.Next;
//            }
//
//            cacheTask.ChangeDownloadState(TaskStateEum.Initialed);
//            downloadManager.AllDownloadTaskLinkedList.AddLast(cacheTask); //添加新的任务到最后面
//        }
//    }
//}
