using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace GameFramePro
{

    /// <summary>
    ///   分解指定的 TreeNodePath 得到包含的 TreeNodePath，并且返回需要递归的数量
    /// </summary>
    /// <typeparam name="T">指定开始的节点</typeparam>
    /// <param name="treeNode"></param>
    /// <param name="allSubTreeNode">返回包含的子节点</param>
    /// <param name="recursiveCount">需要递归多少个子节点</param>
    public delegate void TreeNodePathRecursiveHandler<T>(in T treeNode, out IEnumerable<string> allSubTreeNode, out int recursiveCount) where T : BaseTreeNodeInfor;

    /// <summary>
    /// 协助生成树结构
    /// </summary>
    public static class TreeViewUtility<T> where T : BaseTreeNodeInfor
    {

        //根据指定的路径和解析路径方式，递归创建从startTreeNode开始的子树
        public static void CreateTreeNodeByPath(T startTreeNode, TreeNodePathRecursiveHandler<T> treeNodeHander, Func<string, T> createTreeNodeAct, bool isKeepState)
        {
            IEnumerable<string> allSubTreeNodePath = null; //比如这里按照目录获取所有一级的字目录和文件
            int needRecursiveCount = 0;//这里类似返回 allSubTreeNodePath 结果中有多少可以递归的
            treeNodeHander(in startTreeNode, out allSubTreeNodePath, out needRecursiveCount);

            int count = 0;
            foreach (var treeNodePath in allSubTreeNodePath)
            {
                ++count;
                T treeNode = createTreeNodeAct(treeNodePath);
                startTreeNode.AddChildNode(treeNode, isKeepState);
                if (count <= needRecursiveCount)
                    CreateTreeNodeByPath(treeNode, treeNodeHander, createTreeNodeAct, isKeepState);  //递归创建子节点
            }
        }


        /// <summary>
        /// 获取从treeNodeFrom 到 treeNodeTo的路径信息 
        /// </summary>
        /// <param name="treeNodeFrom"></param>
        /// <param name="treeNodeTo"></param>
        /// <returns></returns>
        public static string GetTreeNodePathToOtherTreeNode(BaseTreeNodeInfor treeNodeFrom, BaseTreeNodeInfor treeNodeTo)
        {
            int relationShip = GetTreeNodeRelationShip(treeNodeFrom, treeNodeTo);
            if (relationShip == int.MaxValue)
            {
                Debug.LogError(string.Format("GetTreeNodePathToOtherTreeNode Fail, Node={0} 与Node={1} 没有父子节点关系", treeNodeFrom, treeNodeTo));
                return string.Empty;
            }
            if (relationShip == 0)
                return string.Empty;

            List<StringBuilder> builderList = new List<StringBuilder>();
            ITreeNodeInfor temp = null;
            if (relationShip > 0)
            {
                temp = treeNodeFrom;
                while (temp != treeNodeTo)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(temp.TreeNodePathInfor);
                    builderList.Add(builder);
                    temp = temp.ParentTreeNode;
                }
            }
            else
            {
                temp = treeNodeTo;
                while (temp != treeNodeFrom)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(temp.TreeNodePathInfor);
                    builderList.Add(builder);
                    temp = temp.ParentTreeNode;
                }
            }
            builderList.Reverse();//路径必须反着取
            StringBuilder relativePathBuilder = new StringBuilder();
            for (int dex = 0; dex < builderList.Count; dex++)
            {
                var item = builderList[dex];
                relativePathBuilder.Append(item.ToString());
                if (dex != builderList.Count - 1)
                    relativePathBuilder.Append(System.IO.Path.AltDirectorySeparatorChar); //手动增加路径分隔符
            }
            string relativePath = relativePathBuilder.ToString();
            return relativePath;
        }

        /// <summary>
        /// 获取从节点 treeNodeFrom 到节点 treeNodeTo的层次关系
        /// </summary>
        /// <param name="treeNodeFrom"></param>
        /// <param name="treeNodeTo"></param>
        /// <returns>返回值为 int.MaxValue 则表明没有父子关系！返回大于0 则表明 treeNodeTo 是 treeNodeFrom的父节点；小于0 则标示 treeNodeTo 是 treeNodeFrom的子节点</returns>
        public static int GetTreeNodeRelationShip(BaseTreeNodeInfor treeNodeFrom, BaseTreeNodeInfor treeNodeTo)
        {
            if (treeNodeFrom == treeNodeTo)
                return 0;
            int deep = 0;
            ITreeNodeInfor temp = null;
            if (treeNodeFrom == null)
            {
                deep = 0;
                temp = treeNodeTo;
                while (temp != treeNodeFrom)
                {
                    temp = temp.ParentTreeNode;
                    ++deep;
                }//假定 treeNodeTo  是 treeNodeFrom  子节点
                return -1 * deep;
            }
            else if (treeNodeTo == null)
            {
                deep = 0;
                temp = treeNodeFrom;
                while (temp != treeNodeTo)
                {
                    temp = temp.ParentTreeNode;
                    ++deep;
                }//假定 treeNodeFrom 是 treeNodeTo 子节点
                return deep;
            }
            else
            {
                temp = treeNodeFrom;
                while (temp != treeNodeTo)
                {
                    if (temp == null)
                    {
                        deep = -1;
                        break;
                    } //说明假定 treeNodeFrom 是 treeNodeTo 子节点不成立

                    temp = temp.ParentTreeNode;
                    ++deep;
                }//假定 treeNodeFrom 是 treeNodeTo 子节点

                if (deep != -1)
                    return deep;

                deep = 0;
                temp = treeNodeTo;
                while (temp != treeNodeFrom)
                {
                    if (temp == null)
                    {
                        deep = -1;
                        break;
                    } //说明假定 treeNodeTo 是 treeNodeFrom 子节点不成立

                    temp = temp.ParentTreeNode;
                    ++deep;
                }//假定 treeNodeTo  是 treeNodeFrom  子节点

                if (deep == -1)
                    return int.MaxValue; //说明两者没有父子关系

                return -1 * deep;
            }
        }

    }
}