using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{

    /// <summary>
    /// 管理当资源的引用为0时候,继续等待一段时间，如果再起被启用则不销毁
    /// </summary>
    public  class AssetDelayDeleteManager:Single<AssetDelayDeleteManager>, IUpdateTick
    {
        //按照剩余存在时间排序
        private static LinkedList<ILoadAssetRecord> s_AllDelayDeleteAssetInfor = new LinkedList<ILoadAssetRecord>();


        #region IUpdateTick Interface
        protected int curUpdateCount = 0; //当前的帧基数
        protected float lastRecordTime = 0; // 上一次记录的时间
        public uint TickPerUpdateCount { get; protected set; } = 30;

        public bool CheckIfNeedUpdateTick()
        {
            ++curUpdateCount;
            if (curUpdateCount == 1)
                return true;  //确保第一次被调用

            if (curUpdateCount < TickPerUpdateCount)
                return false;

            curUpdateCount = 0;
            return true;
        }



        public void UpdateTick(float currentTime)
        {
            if (lastRecordTime == 0)
                lastRecordTime = currentTime;
            if (CheckIfNeedUpdateTick() == false) return;

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
                    target.Value.NotifyReleaseRecord(); //释放资源
                    s_AllDelayDeleteAssetInfor.Remove(target);
                }
                target = temp;
            }
        }


        /// <summary>
        /// 开始后台追踪这个资源的状态
        /// </summary>
        /// <param name="assetInfor"></param>
        public static void RecycleNoReferenceLoadAssetRecord(ILoadAssetRecord assetInfor)
        {
            LinkedListNode<ILoadAssetRecord> newNode = new LinkedListNode<ILoadAssetRecord>(assetInfor);
            AddNode(newNode);
        }

        /// <summary>
        /// 强制回收空的加载记录
        /// </summary>
        /// <param name="assetInfor"></param>
        public static ILoadAssetRecord TryGetILoadAssetRecord(string assetPath)
        {
            var target = s_AllDelayDeleteAssetInfor.First;
            while (target !=null&& target.Value.AssetUrl != assetPath)
            {
                target = target.Next;
            }

            if (target != null)
            {
                if (target.Value.NotifyReReference())
                {
                    return target.Value;
                }
                else
                {
                    target.Value.NotifyReleaseRecord(); //销毁自身
                    s_AllDelayDeleteAssetInfor.Remove(target);
                    return null;
                }//由于某些条件不满足返回null
            }//找到了这个对象并重新启用
            return null;
        }


        #endregion

        #region  添加或者删除节点
        /// <summary>
        /// 按照剩余时间由少到多插入
        /// </summary>
        /// <param name="newNode"></param>
        private static void AddNode(LinkedListNode<ILoadAssetRecord> newNode)
        {
            if (s_AllDelayDeleteAssetInfor.Count == 0)
            {
                s_AllDelayDeleteAssetInfor.AddFirst(newNode);
                return;
            }//第一个元素

            LinkedListNode<ILoadAssetRecord> targetNode = s_AllDelayDeleteAssetInfor.Last;
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
            }//从向前查找插入点

        }

     
        #endregion



    }
}