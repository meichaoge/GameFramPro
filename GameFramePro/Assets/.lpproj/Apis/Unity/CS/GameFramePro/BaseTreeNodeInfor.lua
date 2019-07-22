---@class CS.GameFramePro.BaseTreeNodeInfor : CS.System.Object
CS.GameFramePro.BaseTreeNodeInfor = {}

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.ViewShowStr : CS.System.String
CS.GameFramePro.BaseTreeNodeInfor.ViewShowStr = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.TreeNodePathInfor : CS.System.String
CS.GameFramePro.BaseTreeNodeInfor.TreeNodePathInfor = nil

---@property readonly CS.GameFramePro.BaseTreeNodeInfor.TreeNodePathRelativeRoot : CS.System.String
CS.GameFramePro.BaseTreeNodeInfor.TreeNodePathRelativeRoot = nil

---@property readonly CS.GameFramePro.BaseTreeNodeInfor.IsTreeLeafNode : CS.System.Boolean
CS.GameFramePro.BaseTreeNodeInfor.IsTreeLeafNode = nil

---@property readonly CS.GameFramePro.BaseTreeNodeInfor.IsRootNode : CS.System.Boolean
CS.GameFramePro.BaseTreeNodeInfor.IsRootNode = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.IsInitialed : CS.System.Boolean
CS.GameFramePro.BaseTreeNodeInfor.IsInitialed = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.TreeNodeDeep : CS.System.Int32
CS.GameFramePro.BaseTreeNodeInfor.TreeNodeDeep = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.IsTreeNodeSelected : CS.System.Boolean
CS.GameFramePro.BaseTreeNodeInfor.IsTreeNodeSelected = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.ParentTreeNode : CS.GameFramePro.ITreeNodeInfor
CS.GameFramePro.BaseTreeNodeInfor.ParentTreeNode = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.RootTreeView : CS.GameFramePro.ITreeView
CS.GameFramePro.BaseTreeNodeInfor.RootTreeView = nil

---@property readwrite CS.GameFramePro.BaseTreeNodeInfor.AllSubNodesInfor : CS.System.Collections.Generic.List
CS.GameFramePro.BaseTreeNodeInfor.AllSubNodesInfor = nil

---@return CS.GameFramePro.BaseTreeNodeInfor
function CS.GameFramePro.BaseTreeNodeInfor()
end

---@param rootTreeView : CS.GameFramePro.ITreeView
---@param showStr : CS.System.String
---@param treeNodePath : CS.System.String
---@param parent : CS.GameFramePro.ITreeNodeInfor
---@param isDefaultSelected : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:InitialedTreeNode(rootTreeView, showStr, treeNodePath, parent, isDefaultSelected)
end

---@param deepOffset : CS.System.Single
---@param maxDeep : CS.System.Int32
function CS.GameFramePro.BaseTreeNodeInfor:DrawTreeNode(deepOffset, maxDeep)
end

---@param targetNode : CS.GameFramePro.ITreeNodeDraw
---@param isExpand : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:TreeNodeExpandOrFold(targetNode, isExpand)
end

---@return CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:ChckeIfCanShowTreeNodeInfor()
end

---@param isSelected : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:SetTreeNodeSelected(isSelected)
end

---@param parent : CS.GameFramePro.ITreeNodeInfor
---@param keepChildNodeState : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:SetParent(parent, keepChildNodeState)
end

---@param eventNode : CS.GameFramePro.ITreeNodeInfor
---@param isAddNode : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:NotifyParentStructureChange(eventNode, isAddNode)
end

---@param eventNode : CS.GameFramePro.ITreeNodeInfor
function CS.GameFramePro.BaseTreeNodeInfor:NotifyChildsNodeDeepChange(eventNode)
end

---@param childNode : CS.GameFramePro.ITreeNodeInfor
---@param keepChildNodeState : CS.System.Boolean
---@param index : CS.System.Int32
function CS.GameFramePro.BaseTreeNodeInfor:AddChildNode(childNode, keepChildNodeState, index)
end

---@param childNodes : CS.System.Collections.Generic.IEnumerable
---@param keepState : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:AddRangeChildNodes(childNodes, keepState)
end

---@param childNode : CS.GameFramePro.ITreeNodeInfor
function CS.GameFramePro.BaseTreeNodeInfor:RemoveChildNode(childNode)
end

---@param index : CS.System.Int32
function CS.GameFramePro.BaseTreeNodeInfor:RemoveChildNodeByIndex(index)
end

---@param includeSelf : CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:RemoveAllChildNodes(includeSelf)
end

function CS.GameFramePro.BaseTreeNodeInfor:OnBeforeDeleteTreeNode()
end

---@param targetNode : CS.GameFramePro.BaseTreeNodeInfor
---@return CS.System.Boolean
function CS.GameFramePro.BaseTreeNodeInfor:CheckIfChildTreeNode(targetNode)
end

function CS.GameFramePro.BaseTreeNodeInfor:ShowTreeNodeInfor()
end

function CS.GameFramePro.BaseTreeNodeInfor:Dispose()
end