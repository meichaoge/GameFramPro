using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 资源信息节点
    /// </summary>
    public class AssetStateNodeInfor : BaseTreeNodeInfor
    {
        protected int mMaxExpandDeep = -1;   //只赋值一次 

        protected int mCurExpandDeep = -1; //当前绘制层次 当TreeNodeExpandOrFold 时候会重置这个值
        protected ITreeNodeDraw mTargetNode = null;  //==null 时候表示操作的是根节点
        protected bool mIsExpand = true;

    

        //TODO  后面需要处理层级管理
        public override void DrawTreeNode(float deepOffset, int maxExpandDeep = -1)
        {
            if (mMaxExpandDeep == -1)
                mMaxExpandDeep = maxExpandDeep;
            if (mCurExpandDeep == -1 && maxExpandDeep != -1)
                mCurExpandDeep += maxExpandDeep;


            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Empty, GUILayout.Width(deepOffset * TreeNodeDeep));
            GUILayout.Toggle(IsShowSelectedState(), string.Empty, GUILayout.Width(15));
            GUILayout.Label(ViewShowStr, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            foreach (var item in AllSubNodesInfor)
            {
                (item as BaseTreeNodeInfor).DrawTreeNode(deepOffset, maxExpandDeep);
            }

        }

        /// <summary>
        /// 检测当前节点是否可以绘制
        /// </summary>
        /// <returns></returns>
        public override bool ChckeIfCanShowTreeNodeInfor()
        {
            //if (mMaxExpandDeep == -1)
            //    return true;  //全部展开


            //int deep = GetDeepToTargetNode(mTargetNode as BaseTreeNodeInfor);
            //if (deep == int.MaxValue)
            //{
            //    if (mTargetNode == null)
            //        deep = this.TreeNodeDeep;
            //    else
            //        deep = this.TreeNodeDeep - (mTargetNode as BaseTreeNodeInfor).TreeNodeDeep;
            //}

            return mIsExpand;
        }

        public override void TreeNodeExpandOrFold(ITreeNodeDraw targetNode, bool isExpand)
        {
            mTargetNode = targetNode;
            mIsExpand = isExpand;

            foreach (var treeNodeInfor in AllSubNodesInfor)
            {
                (treeNodeInfor as BaseTreeNodeInfor).TreeNodeExpandOrFold(targetNode, false);
            }
        }


    }
}