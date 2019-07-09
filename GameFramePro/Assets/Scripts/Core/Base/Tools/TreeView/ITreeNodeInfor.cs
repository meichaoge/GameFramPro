using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramePro
{
    ///// <summary>
    ///// 当前节点的选择状态
    ///// </summary>
    //public enum TreeNodeStateEnum
    //{
    //    None = 0,  //初始状态 不可用

    //    //**当时叶子节点时候的状态
    //    LeafNodeSelected = 1,
    //    LeafNodeUnSelected = 2,

    //    //**当是非叶子节点时候的状态

    //    AllSubTreeUnSelected = 10,  //子节点都没有被选择
    //    AllSubTreeSelected, //子节点全部选择
    //                        //  OnlySelfSelected, //只有自己被选择 无意义的状态 需要过滤
    //    AnySubTreeNodeSelected, //子节点部分选择
    //}


    /// <summary>
    /// 树形结构节点信息
    /// </summary>
    public interface ITreeNodeInfor:IDisposable
    {
        /// <summary>
        /// 显示节点时候的内容
        /// </summary>
        string ViewShowStr { get; }  
        /// <summary>
        /// 当前树形结构中保存的路径信息
        /// </summary>
        string TreeNodePathInfor { get; }


        /// <summary>
        /// 标识是否是叶子节点
        /// </summary>
        bool IsTreeLeafNode { get; } 
        /// <summary>
        /// 是否已经初始化
        /// </summary>
        bool IsInitialed { get; }
        /// <summary>
        /// 相对根节点的深度
        /// </summary>
        int TreeNodeDeep { get; }


        /// <summary>
        /// 标示自身是否被选择 需要配合 TreeNodeState 才能确定状态
        /// </summary>
        bool IsTreeNodeSelected { get; }

        /// <summary>
        /// 父节点
        /// </summary>
        ITreeNodeInfor ParentTreeNode { get; }
        /// <summary>
        /// 树视图
        /// </summary>
        ITreeView RootTreeView { get; }
        /// <summary>
        /// 包含的子节点 （嵌套结构）
        /// </summary>
        List<ITreeNodeInfor> AllSubNodesInfor { get; }

       
        #region 设置节点的状态

        /// <summary>
        /// 初始化获取和设置子节点的状态
        /// </summary>
        void InitialedTreeNode(ITreeView rootTreeView,string showStr, string treeNodePath, ITreeNodeInfor parent, bool isDefaultSelected);


        /// <summary>
        /// 设置节点的状态 
        /// </summary>
        /// <param name="isSelected"></param>
        void SetTreeNodeSelected(bool isSelected);
        /// <summary>
        ///  设置父节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="keepChildNodeState">是否保持自己的状态,当=false时候，需要考虑父节点状态设置自身状态</param>
        void SetParent(ITreeNodeInfor parent, bool keepChildNodeState);

        #endregion


        #region 节点通知消息

        /// <summary>
        /// 通知父节点 自身 状态改变, 父节点需要刷新状态
        /// </summary>
        /// <param name="eventNode">触发这个消息的节点 (避免自身处理这个消息)</param>
        void NotifyParentStateChange(ITreeNodeInfor eventNode);
        /// <summary>
        /// 通知父节点 自身 树形结构改变(通常由于增加或者删除节点引起) 父节点需要刷新状态
        /// </summary>
        /// <param name="eventNode">触发这个消息的节点 (避免自身处理这个消息)</param>
        /// <param name="isAddNode">标示是否是节点增加了</param>
        void NotifyParentStructureChange(ITreeNodeInfor eventNode,bool isAddNode);
        ///// <summary>
        ///// 通知子节点自己的选择状态被修改，子节点需要修改选择状态
        ///// </summary>
        ///// <param name="eventNode">触发这个消息的节点 (避免自身处理这个消息)</param>
        ///// <param name="isSelected">当前事件节点选择与否</param>
        //void NotifyChildsStateChange(ITreeNodeInfor eventNode, bool isSelected);
        /// <summary>
        /// 通知子节点需要更新层级
        /// </summary>
        /// <param name="eventNode"></param>
        void NotifyChildsNodeDeepChange(ITreeNodeInfor eventNode);

        #endregion


        #region 增加或者删除节点相关

        /// <summary>
        /// 增加子节点 (默认增加到末尾)
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="index">=-1标示增加到最后</param>
        void AddChildNode(ITreeNodeInfor childNode, bool keepState, int index = -1);
        /// <summary>
        /// 添加一系列
        /// </summary>
        /// <param name="childNodes"></param>
        void AddRangeChildNodes(IEnumerable<ITreeNodeInfor> childNodes, bool keepState);

        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="childNode"></param>
        void RemoveChildNode(ITreeNodeInfor childNode);
        /// <summary>
        /// 移除子节点
        /// </summary>
        /// <param name="index"></param>
        void RemoveChildNodeByIndex(int index  );

        /// <summary>
        /// 移除所有的子节点
        /// </summary>
        /// <param name="includeSelf"></param>
        void RemoveAllChildNodes(bool includeSelf);

        /// <summary>
        /// 删除节点时候的操作
        /// </summary>
        void OnBeforeDeleteTreeNode();
        #endregion


        #region 其他辅助接口
        void ShowTreeNodeInfor(); //显示节点信息

        #endregion

    }


}