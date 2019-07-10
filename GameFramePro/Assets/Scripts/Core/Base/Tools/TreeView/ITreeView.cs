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

        void InitialTreeView(IEnumerable<BaseTreeNodeInfor> rootTreeNodes);

        /// <summary>
        /// 添加根节点
        /// </summary>
        /// <param name="rootTreeNode"></param>
        /// <param name="index"></param>
        void AddRootTreeNode(BaseTreeNodeInfor rootTreeNode, int index=-1);

        IEnumerable<BaseTreeNodeInfor> GetRootTreeNodes();

        /// <summary>
        /// 绘制树结构
        /// </summary>
        /// <param name="isExpand">是否展开全部 默认false(只显示根节点)</param>
        void DrawTreeView(bool isExpand=false);

        /// <summary>
        /// 获取一个节点
        /// </summary>
        /// <param name="showStr"></param>
        /// <param name="parent"></param>
        /// <param name="isDefaultSelected"></param>
        /// <returns></returns>
        BaseTreeNodeInfor GetTreeNode(string showStr, string treeNodePath, BaseTreeNodeInfor parent, bool isDefaultSelected);

        /// <summary>
        /// 删除一个节点
        /// </summary>
        /// <param name="treeNode"></param>
        void DeleteTreeNodeFrom(ITreeNodeInfor treeNode);


    }
}