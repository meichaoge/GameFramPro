using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro.ResourcesEx
{

    /// <summary>
    /// 管理当资源的引用为0时候,继续等待一段时间，如果再起被启用则不销毁
    /// </summary>
    public static class AssetDelayDeleteManager
    {
        //按照剩余存在时间排序
        private static LinkedList<BaseLoadedAssetInfor> mAllDelayDeleteAssetInfor = new LinkedList<BaseLoadedAssetInfor>();


        #region Interface
        /// <summary>
        /// 应该确保每秒调用一次
        /// </summary>
        public static void Tick()
        {

        }

        /// <summary>
        /// 开始追踪这个资源的状态
        /// </summary>
        /// <param name="assetInfor"></param>
        public static void RecycleNoReferenceAsset(BaseLoadedAssetInfor assetInfor)
        {
            LinkedListNode<BaseLoadedAssetInfor> newNode = new LinkedListNode<BaseLoadedAssetInfor>(assetInfor);
            AddNode(newNode);
        }

        #endregion

        #region  添加或者删除节点
        /// <summary>
        /// 按照剩余时间由少到多插入
        /// </summary>
        /// <param name="newNode"></param>
        private static void AddNode(LinkedListNode<BaseLoadedAssetInfor> newNode)
        {
            if(mAllDelayDeleteAssetInfor.Count==0)
            {
                mAllDelayDeleteAssetInfor.AddFirst(newNode);
                return;
            }//第一个元素

            LinkedListNode<BaseLoadedAssetInfor> targetNode = mAllDelayDeleteAssetInfor.Last;
            while (true)
            {
                if (targetNode.Previous == null)
                {
                    mAllDelayDeleteAssetInfor.AddFirst(newNode);
                    return;
                } //已经查找到最开始的节点则直接插入
                if(targetNode.Value.RemainTimeToDelete<= newNode.Value.RemainTimeToDelete)
                {
                    mAllDelayDeleteAssetInfor.AddAfter(targetNode, newNode);
                    break;
                }
                targetNode = targetNode.Previous;
            }//从向前查找插入点

        }
        #endregion



    }
}