using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace GameFramePro
{
    /// <summary>
    ///  树形结构的节点信息
    /// </summary>
    public abstract class BaseTreeNodeInfor : ITreeNodeInfor, ITreeNodeDraw
    {
        public string ViewShowStr { get; protected set; } = string.Empty;
        public string TreeNodePathInfor { get; protected set; } = string.Empty;
        protected string mTreeNodePathRelativeRoot = string.Empty;
        public string TreeNodePathRelativeRoot
        {
            get
            {
                if (string.IsNullOrEmpty(mTreeNodePathRelativeRoot))
                {
                    if (IsRootNode)
                        mTreeNodePathRelativeRoot = TreeNodePathInfor;
                    else
                        mTreeNodePathRelativeRoot = TreeViewUtility<BaseTreeNodeInfor>.GetTreeNodePathToOtherTreeNode(this, null);
                }
                return mTreeNodePathRelativeRoot;
            }
        }//获取当前节点到根节点的路径

        public bool IsTreeLeafNode { get { return AllSubNodesInfor.Count == 0; } }
        public bool IsRootNode { get { return ParentTreeNode == null; } }
        public bool IsInitialed { get; protected set; } = false;
        public int TreeNodeDeep { get; protected set; } = 0;

        public bool IsTreeNodeSelected { get; protected set; } = false; //标示自身的选中状态

        public ITreeNodeInfor ParentTreeNode { get; protected set; } = null;

        public ITreeView RootTreeView { get; protected set; } = null;
        public List<ITreeNodeInfor> AllSubNodesInfor { get; protected set; }
        protected int SubNodeInitialedCount = 3;

        #region 构造函数
        public BaseTreeNodeInfor()
        {
            ViewShowStr = string.Empty;
            ParentTreeNode = null;
            AllSubNodesInfor = new List<ITreeNodeInfor>(SubNodeInitialedCount);
            RootTreeView = null;
            IsTreeNodeSelected = false;
            IsInitialed = false; //此时没有设置视图控制器没有完成初始化
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="showStr"></param>
        /// <param name="parent"></param>
        /// <param name="isDefaultSelected">标示是否默认选择</param>
        public void InitialedTreeNode(ITreeView rootTreeView, string showStr, string treeNodePath, ITreeNodeInfor parent, bool isDefaultSelected)
        {
            ViewShowStr = showStr;
            TreeNodePathInfor = treeNodePath;
            ParentTreeNode = parent;
            AllSubNodesInfor = new List<ITreeNodeInfor>(SubNodeInitialedCount);

            IsTreeNodeSelected = isDefaultSelected;
            RootTreeView = rootTreeView;
            IsInitialed = true;
        }


        #region 节点绘制相关

        public abstract void DrawTreeNode(float deepOffset, int maxDeep = -1);
        public abstract void TreeNodeExpandOrFold(ITreeNodeDraw targetNode, bool isExpand);
        public abstract bool ChckeIfCanShowTreeNodeInfor();
        #endregion

        #region 实现接口

        public virtual void SetTreeNodeSelected(bool isSelected)
        {
            IsTreeNodeSelected = isSelected;
            if (IsTreeLeafNode)
                return;
            foreach (var childTreeNode in AllSubNodesInfor)
                childTreeNode.SetTreeNodeSelected(isSelected);
        }

        public virtual void SetParent(ITreeNodeInfor parent, bool keepChildNodeState)
        {
            ParentTreeNode = parent;
            NotifyChildsNodeDeepChange(this);

            if (keepChildNodeState == false)
            {
                if (parent == null)
                    return;
                SetTreeNodeSelected(parent.IsTreeNodeSelected);
            }
        }


        //public virtual void NotifyParentStateChange(ITreeNodeInfor eventNode)
        //{
        //    if (eventNode != this)
        //    {
        //    }
        //    if (ParentTreeNode == null)
        //        return;
        //    ParentTreeNode.NotifyParentStateChange(eventNode);
        //}

        public virtual void NotifyParentStructureChange(ITreeNodeInfor eventNode, bool isAddNode)
        {

        }

        public void NotifyChildsNodeDeepChange(ITreeNodeInfor eventNode)
        {
            if (ParentTreeNode != null)
                TreeNodeDeep = ParentTreeNode.TreeNodeDeep + 1;
            else
                TreeNodeDeep = 0;

            mTreeNodePathRelativeRoot = string.Empty; //标示需要更新路径

            foreach (var treeNode in AllSubNodesInfor)
            {
                treeNode.NotifyChildsNodeDeepChange(eventNode);
            }
        }


        public virtual void AddChildNode(ITreeNodeInfor childNode, bool keepChildNodeState, int index = -1)
        {
            if (index >= 0)
            {
                if (index >= AllSubNodesInfor.Count)
                {
                    Debug.LogError(string.Format("AddChildNode Of Node={0} index={1} not avaliable", this, index));
                    AllSubNodesInfor.Add(childNode);
                }
                else
                    AllSubNodesInfor.Insert(index, childNode);
            }
            else
                AllSubNodesInfor.Add(childNode);

            childNode.SetParent(this, keepChildNodeState);
            //if (keepChildNodeState)
            //{
            //    NotifyParentStateChange(this);
            //}
            NotifyParentStructureChange(this, true);
        }

        public virtual void AddRangeChildNodes(IEnumerable<ITreeNodeInfor> childNodes, bool keepState)
        {
            AllSubNodesInfor.AddRange(childNodes);
            foreach (var item in childNodes)
            {
                item.SetParent(this, keepState);
            }

            //if (keepState)
            //{
            //    NotifyParentStateChange(this);
            //}
            NotifyParentStructureChange(this, true);
        }

        public virtual void RemoveChildNode(ITreeNodeInfor childNode)
        {
            if (IsTreeLeafNode)
            {
                Debug.LogError("RemoveChildNode Fail, Current Node {0} Contained No Child", this);
                return;
            }
            RootTreeView.DeleteTreeNodeFrom(childNode);

            if (AllSubNodesInfor.Remove(childNode))
            {
               // NotifyParentStateChange(this);
                NotifyParentStructureChange(this, false);
                return;
            }

            Debug.LogError("RemoveChildNode Fail,Node {0} Not Exit!", childNode);
        }

        public virtual void RemoveChildNodeByIndex(int index)
        {
            if (IsTreeLeafNode)
            {
                Debug.LogError("RemoveChildNode Fail, Current Node Contained No Child", this);
                return;
            }

            if (index < 0 || index >= AllSubNodesInfor.Count)
            {
                Debug.LogError("RemoveChildNode Fail,Node  Index {0} Not Exit!", index);
                return;
            }
            RootTreeView.DeleteTreeNodeFrom(AllSubNodesInfor[index]);

            AllSubNodesInfor.RemoveAt(index);

          //  NotifyParentStateChange(this);

            NotifyParentStructureChange(this, false);
            return;

        }

        public virtual void RemoveAllChildNodes(bool includeSelf)
        {
            foreach (var item in AllSubNodesInfor)
                RootTreeView.DeleteTreeNodeFrom(item);

            AllSubNodesInfor.Clear();
            if (includeSelf)
            {
                if (ParentTreeNode != null)
                    ParentTreeNode.RemoveChildNode(this); //交给父节点处理结构变化 自己不用再发出事件消息
                return;
            }
            else
            {
            //    NotifyParentStateChange(this);
                NotifyParentStructureChange(this, false);
            }
        }



        public void OnBeforeDeleteTreeNode()
        {
            IsInitialed = false;
            ViewShowStr = string.Empty;
            SetParent(null, true);
            RootTreeView = null;
            IsTreeNodeSelected = false;
            AllSubNodesInfor.Clear();
        }

        #endregion

        #region 辅助
        public bool CheckIfChildTreeNode(BaseTreeNodeInfor targetNode)
        {
            if (IsTreeLeafNode)
                return false;

            foreach (var item in AllSubNodesInfor)
            {
                if ((item as BaseTreeNodeInfor).CheckIfChildTreeNode(targetNode))
                    return true;
            }
            return false;
        }

        public void ShowTreeNodeInfor()
        {
            if (ParentTreeNode != null)
            {
                if (IsTreeLeafNode)
                    Debug.Log(string.Format("LeafNode: ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t ParentTreeNode={3} \t State={4}",
                        ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, ParentTreeNode.ViewShowStr, IsTreeNodeSelected));
                else
                    Debug.Log(string.Format("NormalNode: ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t ParentTreeNode={3} \t ChildCount={4} \t State={5}",
                        ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, ParentTreeNode.ViewShowStr, AllSubNodesInfor.Count, IsTreeNodeSelected));
            }
            else
            {
                Debug.Log(string.Format("RootNode!!! ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t State={3}",
                    ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, IsTreeNodeSelected));
            }

            foreach (var item in AllSubNodesInfor)
                item.ShowTreeNodeInfor();
        }

        #endregion


        public void Dispose()
        {
            ViewShowStr = string.Empty;
            ParentTreeNode = null;
            AllSubNodesInfor = null;
        }
    }
}