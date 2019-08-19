using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



namespace GameFramePro
{
    /// <summary>
    /// 树形结构视图
    /// </summary>
    public abstract class BaseTreeView<T> : ITreeView where T : BaseTreeNodeInfor, new()
    {
        /// <summary>
        /// 一个或者多个根节点
        /// </summary>
        public List<T> AllRootTreeNodes { get; protected set; }
        protected NativeObjectPool<T> mTreeViewPoolManager = null;


        public virtual void InitialTreeView(IEnumerable<BaseTreeNodeInfor> rootTreeNodes)
        {
            mTreeViewPoolManager = new NativeObjectPool<T>(10, null, null);

            if (rootTreeNodes == null)
                AllRootTreeNodes = new List<T>(1);
            else
            {
                AllRootTreeNodes = new List<T>(10);
                foreach (var item in rootTreeNodes)
                    AllRootTreeNodes.Add(item as T);
            }

        }

        public virtual void AddRootTreeNode(BaseTreeNodeInfor rootTreeNode, int index = -1)
        {
            rootTreeNode.SetParent(null, true);


            if (index < 0 || index >= AllRootTreeNodes.Count)
                AllRootTreeNodes.Add(rootTreeNode as T);
            else
                AllRootTreeNodes.Insert(index, rootTreeNode as T);
        }

        public virtual IEnumerable<BaseTreeNodeInfor> GetRootTreeNodes()
        {
            return AllRootTreeNodes;
        }

        /// <summary>
        /// 需要自行实现绘制
        /// </summary>
        public abstract void DrawTreeView( bool isExpand = false);


        #region 获取和删除节点

        public virtual BaseTreeNodeInfor GetTreeNode(string showStr, string treeNodePath, BaseTreeNodeInfor parent, bool isDefaultSelected)
        {
            T treeNode = mTreeViewPoolManager.GetItemFromPool();
            treeNode.InitialedTreeNode(this, showStr, treeNodePath, parent, isDefaultSelected);
            return treeNode;
        }

        public virtual void DeleteTreeNodeFrom(ITreeNodeInfor treeNode)
        {
            treeNode.OnBeforeDeleteTreeNode();
            mTreeViewPoolManager.RecycleItemToPool(treeNode as T);
        }

        #endregion


        public void ShowTreeViewNodes()
        {
            foreach (var item in AllRootTreeNodes)
            {
                item.ShowTreeNodeInfor();
            }
        }


    }
}
