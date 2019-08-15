using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace GameFramePro
{
    /// <summary>
    /// 绘制节点相关
    /// </summary>
    public interface ITreeNodeDraw 
    {

        /// <summary>
        /// 绘制这个子节点 
        /// </summary>
        /// <param name="deepOffset">两个层级间距</param>
        /// <param name="maxExpandDeep">=-1时候表示绘制所有的子节点并且递归，否则 每个节点会显示显示层数</param>
        void DrawTreeNode(float deepOffset,int maxExpandDeep = -1);
        /// <summary>
        /// 从targetNode 节点开始展开或者折叠
        /// </summary>
        /// <param name="targetNode">操作的节点</param>
        /// <param name="isExpand"></param>
        void TreeNodeExpandOrFold(ITreeNodeDraw targetNode,bool isExpand);

        /// <summary>
        /// 检测当前节点是否可以绘制
        /// </summary>
        /// <returns></returns>
        bool ChckeIfCanShowTreeNodeInfor();
    }
}