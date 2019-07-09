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
    public abstract class BaseTreeNodeInfor : ITreeNodeInfor,ITreeNodeDraw
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

        public bool IsShowSelectedState()
        {
            switch (TreeNodeState)
            {
                case TreeNodeStateEnum.None:
                case TreeNodeStateEnum.LeafNodeUnSelected:
                case TreeNodeStateEnum.AllSubTreeUnSelected:
                    return false;
                case TreeNodeStateEnum.LeafNodeSelected:
                case TreeNodeStateEnum.AllSubTreeSelected:
                case TreeNodeStateEnum.AnySubTreeNodeSelected:
                    return true;
                default:
                    Debug.LogError("无法解析的状态 " + TreeNodeState);
                    return false;
            }
        }

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
        public void InitialedTreeNode(ITreeView rootTreeView,string showStr,string treeNodePath, ITreeNodeInfor parent, bool isDefaultSelected)
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
        public abstract  bool ChckeIfCanShowTreeNodeInfor();
        #endregion

        #region 实现接口


        public virtual void SetTreeNodeSelected(bool isSelected)
        {
            IsTreeNodeSelected = isSelected;
            if (IsTreeLeafNode) return;
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

        //public virtual void NotifyChildsStateChange(ITreeNodeInfor eventNode, bool isSelected)
        //{
        //    IsTreeNodeSelected = isSelected;

        //    //if (IsTreeLeafNode)
        //    //{
        //    //    TreeNodeState = isSelected ? TreeNodeStateEnum.LeafNodeSelected : TreeNodeStateEnum.LeafNodeUnSelected; //更新状态
        //    //    return;
        //    //}
        //    //TreeNodeState = isSelected ? TreeNodeStateEnum.AllSubTreeSelected : TreeNodeStateEnum.AllSubTreeUnSelected; //更新状态

        //    foreach (var item in AllSubNodesInfor)
        //    {
        //        item.NotifyChildsStateChange(eventNode, isSelected);
        //    }
        //}

        public virtual void NotifyParentStateChange(ITreeNodeInfor eventNode)
        {
            if (eventNode != this)
                TreeNodeState = GetTreeNodeIState(); //刷新自身的状态

            if (ParentTreeNode == null)
                return;

            ParentTreeNode.NotifyParentStateChange(eventNode);
        }

        public virtual void NotifyParentStructureChange(ITreeNodeInfor eventNode, bool isAddNode)
        {
        
        }

        public void NotifyChildsNodeDeepChange(ITreeNodeInfor eventNode)
        {
            //if (eventNode == this)
            //{
            //    if (ParentTreeNode != null)
            //        TreeNodeDeep = ParentTreeNode.TreeNodeDeep + 1;
            //    else
            //        TreeNodeDeep = 0;
            //    return;
            //}

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
            if (keepChildNodeState)
            {
                var curNodeState = GetTreeNodeIState();  //如果不需要子节点跟随自己的状态 则需要刷新自身状态
                if (TreeNodeState != curNodeState)
                {
                    TreeNodeState = curNodeState;
                    NotifyParentStateChange(this);
                }
            }
            NotifyParentStructureChange(this, true);
        }

        public virtual void AddRangeChildNodes(IEnumerable<ITreeNodeInfor> childNodes, bool keepState)
        {
            AllSubNodesInfor.AddRange(childNodes);
            foreach (var item in childNodes)
            {
                item.SetParent(this, keepState);
            }

            if (keepState)
            {
                var curNodeState = GetTreeNodeIState();  //如果不需要子节点跟随自己的状态 则需要刷新自身状态
                if (TreeNodeState != curNodeState)
                {
                    TreeNodeState = curNodeState;
                    NotifyParentStateChange(this);
                }
            }
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
                var curNodeState = GetTreeNodeIState();
                if (TreeNodeState != curNodeState)
                {
                    TreeNodeState = curNodeState;
                    NotifyParentStateChange(this);
                }
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
            var curNodeState = GetTreeNodeIState();
            if (TreeNodeState != curNodeState)
            {
                TreeNodeState = curNodeState;
                NotifyParentStateChange(this);
            }
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
                var curNodeState = GetTreeNodeIState();
                if (TreeNodeState != curNodeState)
                {
                    TreeNodeState = curNodeState;
                    NotifyParentStateChange(this);
                }
                NotifyParentStructureChange(this, false);
            }
        }

    

        public void OnBeforeDeleteTreeNode()
        {
            IsInitialed = false;
            ViewShowStr = string.Empty;
            SetParent(null, true);
            RootTreeView = null;
            TreeNodeState= TreeNodeStateEnum.None;
            AllSubNodesInfor.Clear();
        }

        #endregion

        #region 辅助
        /// <summary>
        /// 获取到指定节点的深度 返回
        /// </summary>
        /// <param name="targetNode">当返回值是int .MaxValue时表示不具有父子关系，当返回值小于0时候说明targetNode是当前节点的子节点；
        /// 等于=0 说明是自身;大于0说明是当前节点的父节点</param>
        /// <returns></returns>
        public int GetDeepToTargetNode(BaseTreeNodeInfor targetNode)
        {
            if (this == targetNode) return 0;
            bool isChild = CheckIfChildTreeNode(targetNode);
            ITreeNodeInfor node = null;
            int deep = 0;
            if (isChild)
            {
                node = targetNode;
                while (node!=this)
                {
                    ++deep;
                    node = node.ParentTreeNode;
                }
                deep = -1 * deep;
            }
            else
            {
                node = this;
                while (node != targetNode)
                {
                    if (node == null)
                        return int .MaxValue; //制定节点不是当前节点的父节点
                    ++deep;
                    node = node.ParentTreeNode;
                }
            }
            return deep;
        }

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
                if(IsTreeLeafNode)
                    Debug.Log(string.Format("LeafNode: ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t ParentTreeNode={3} \t State={4}", ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, ParentTreeNode.ViewShowStr, TreeNodeState));
                else
                    Debug.Log(string.Format("NormalNode: ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t ParentTreeNode={3} \t ChildCount={4} \t State={5}", 
                        ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, ParentTreeNode.ViewShowStr, AllSubNodesInfor.Count, TreeNodeState));
            }
            else
            {
                Debug.Log(string.Format("RootNode!!! ViewShowStr={0} \t TreeNodePathRelativeRoot={1} \t TreeNodeDeep={2} \t State={3}", ViewShowStr, TreeNodePathRelativeRoot, TreeNodeDeep, TreeNodeState));
            }

            foreach (var item in AllSubNodesInfor)
                item.ShowTreeNodeInfor();
        }

        #endregion

        #region 其他

        #endregion



        public void Dispose()
        {
            ViewShowStr = string.Empty;
            ParentTreeNode = null;
            AllSubNodesInfor = null;
        }
    }
}