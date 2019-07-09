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

        public override void DrawTreeView(Rect viewRect, bool isExpand = false)
        {
            DrawTreeView(viewRect, -1, isExpand);
        }

        /// <summary>
        /// 绘制操作
        /// </summary>
        /// <param name="viewRect"></param>
        /// <param name="showDeepPerExpand">每次展开一个节点的时候默认显示多少层级</param>
        /// <param name="isExpand"></param>
        public void DrawTreeView(Rect viewRect, int showDeepPerExpand, bool isExpand = false)
        {
            if (AllRootTreeNodes != null)
            {
                EditorGUILayout.BeginScrollView(new Vector2(viewRect.x, viewRect.y));
                if (AllRootTreeNodes.Count == 0)
                {
                    GUILayout.Label("No Data", GUILayout.ExpandHeight(true));
                }
                for (int dex = 0; dex < AllRootTreeNodes.Count; dex++)
                {
                    AllRootTreeNodes[dex].DrawTreeNode(35);
                }
                EditorGUILayout.EndScrollView();

            }
        }


    }
}