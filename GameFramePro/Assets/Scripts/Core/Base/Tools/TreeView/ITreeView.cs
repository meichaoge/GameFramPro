using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 树形结构视图接口
    /// </summary>
    public interface ITreeView
    {

        void InitialTreeView(IEnumerable<ITreeNodeInfor> rootTreeNodes);

        /// <summary>
        /// 添加根节点
        /// </summary>
        /// <param name="rootTreeNode"></param>
        /// <param name="index"></param>
        void AddRootTreeNode(ITreeNodeInfor rootTreeNode, int index=-1);

        IEnumerable<ITreeNodeInfor> GetRootTreeNodes();

        /// <summary>
        /// 绘制树结构
        /// </summary>
        /// <param name="viewRect">显示区域</param>
        /// <param name="isExpand">是否展开全部 默认false(只显示根节点)</param>
        void DrawTreeView(Rect viewRect, bool isExpand=false);

        /// <summary>
        /// 获取一个节点
        /// </summary>
        /// <param name="showStr"></param>
        /// <param name="parent"></param>
        /// <param name="isDefaultSelected"></param>
        /// <returns></returns>
        ITreeNodeInfor GetTreeNode(string showStr, string treeNodePath, ITreeNodeInfor parent, bool isDefaultSelected);

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="treeNode"></param>
        void DeleteTreeNodeFrom(ITreeNodeInfor treeNode);


    }
}