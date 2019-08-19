using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace GameFramePro.EditorEx
{
    /// <summary>
    /// 资源目录树形结构
    /// </summary>
    public class AssetDirectoryTreeView : BaseTreeView<AssetStateNodeInfor>
    {

        public override void DrawTreeView(bool isExpand = false)
        {
            DrawTreeView(-1, isExpand);
        }

        /// <summary>
        /// 绘制操作
        /// </summary>
        /// <param name="showDeepPerExpand">每次展开一个节点的时候默认显示多少层级</param>
        /// <param name="isExpand"></param>
        public void DrawTreeView(int showDeepPerExpand, bool isExpand = false)
        {
            if (AllRootTreeNodes != null)
            {
                if (AllRootTreeNodes.Count == 0)
                {
                    GUILayout.Label("No Data", GUILayout.ExpandHeight(true));
                }
                for (int dex = 0; dex < AllRootTreeNodes.Count; dex++)
                {
                    AllRootTreeNodes[dex].DrawTreeNode(35);
                }
            }
        }


    }
}