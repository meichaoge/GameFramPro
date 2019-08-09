using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 管理当资源的引用为0时候,继续等待一段时间，如果再起被启用则不销毁
    /// </summary>
    public sealed class AssetDelayDeleteManager : Single<AssetDelayDeleteManager>, IUpdateTimeTick
    {
        //按照剩余存在时间排序
        private static readonly LinkedList<LoadAssetBaseRecord> s_AllDelayDeleteAssetInfor = new LinkedList<LoadAssetBaseRecord>();


        #region IUpdateTick Interface

        protected float lastRecordTime = 0; //上一次记录的时间
        public float TickPerTimeInterval { get; private set; } = 30; //约等于30秒检测一次

        public bool CheckIfNeedUpdateTick(float curTime)
        {
            if (lastRecordTime == 0f)
            {
                lastRecordTime = curTime;
                return true;
            }

            if (curTime - lastRecordTime >= TickPerTimeInterval)
                return true;

            return false;
        }


        public void UpdateTick(float currentTime)
        {
            if (CheckIfNeedUpdateTick(currentTime) == false)
                return;
            float timeSpane = currentTime - lastRecordTime;
            lastRecordTime = currentTime;

            if (s_AllDelayDeleteAssetInfor.Count == 0)
                return;

            var target = s_AllDelayDeleteAssetInfor.First;
            var temp = target;
            while (target != null)
            {
                bool isEnable = target.Value.TimeTick(timeSpane);
                temp = target.Next;
                if (isEnable == false)
                {
# if UNITY_EDITOR
                    Debug.LogEditorInfor($"某个资源{target.Value.AssetUrl}   没有引用后超过可以存活的时间 被释放了！！");
#endif

                    target.Value.NotifyReleaseRecord(); //释放资源
                    s_AllDelayDeleteAssetInfor.Remove(target);
                }

                target = temp;
            }
        }


        /// <summary>/// 开始后台追踪这个资源的状态/// </summary>
        public static void RecycleNoReferenceLoadAssetRecord(LoadAssetBaseRecord assetInfor)
        {
#if UNITY_EDITOR
            Debug.LogEditorInfor($"资源{assetInfor.AssetUrl} 加载记录已经没有引用了,进入待回收链表中 ");
#endif

            LinkedListNode<LoadAssetBaseRecord> newNode = new LinkedListNode<LoadAssetBaseRecord>(assetInfor);
            AddNode(newNode);
        }

        /// <summary>
        /// 强制回收空的加载记录
        /// </summary>
        /// <param name="assetInfor"></param>
        public static LoadAssetBaseRecord TryGetILoadAssetRecord(string assetPath)
        {
            var target = s_AllDelayDeleteAssetInfor.First;
            while (target != null && target.Value.AssetUrl != assetPath)
            {
                target = target.Next;
            }

            if (target != null)
            {
                //        target.Value.ReduceReference(true);

                target.Value.NotifyReleaseRecord(); //2019/8/7 修改
#if UNITY_EDITOR
                Debug.LogEditorInfor($"资源{assetPath} 加载在被真正回收前被重新引用，移除回收链表 ");
#endif
                return target.Value;
//                else
//                {
//                    target.Value.NotifyReleaseRecord(); //销毁自身
//                    s_AllDelayDeleteAssetInfor.Remove(target);
//                    return null;
//                }//由于某些条件不满足返回null
            } //找到了这个对象并重新启用

            return null;
        }

        #endregion

        #region  添加或者删除节点

        /// <summary>
        /// 按照剩余时间由少到多插入
        /// </summary>
        /// <param name="newNode"></param>
        private static void AddNode(LinkedListNode<LoadAssetBaseRecord> newNode)
        {
            if (s_AllDelayDeleteAssetInfor.Count == 0)
            {
                s_AllDelayDeleteAssetInfor.AddFirst(newNode);
                return;
            } //第一个元素

            LinkedListNode<LoadAssetBaseRecord> targetNode = s_AllDelayDeleteAssetInfor.Last;
            while (true)
            {
                if (targetNode.Previous == null)
                {
                    s_AllDelayDeleteAssetInfor.AddFirst(newNode);
                    return;
                } //已经查找到最开始的节点则直接插入

                if (targetNode.Value.RemainTimeToBeDelete <= newNode.Value.RemainTimeToBeDelete)
                {
                    s_AllDelayDeleteAssetInfor.AddAfter(targetNode, newNode);
                    break;
                }

                targetNode = targetNode.Previous;
            } //从向前查找插入点
        }

        #endregion
    }
}
