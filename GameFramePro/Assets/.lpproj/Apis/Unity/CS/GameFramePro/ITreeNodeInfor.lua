---@class CS.GameFramePro.ITreeNodeInfor
CS.GameFramePro.ITreeNodeInfor = {}

---@property readonly CS.GameFramePro.ITreeNodeInfor.ViewShowStr : CS.System.String
CS.GameFramePro.ITreeNodeInfor.ViewShowStr = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.TreeNodePathInfor : CS.System.String
CS.GameFramePro.ITreeNodeInfor.TreeNodePathInfor = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.IsTreeLeafNode : CS.System.Boolean
CS.GameFramePro.ITreeNodeInfor.IsTreeLeafNode = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.IsInitialed : CS.System.Boolean
CS.GameFramePro.ITreeNodeInfor.IsInitialed = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.TreeNodeDeep : CS.System.Int32
CS.GameFramePro.ITreeNodeInfor.TreeNodeDeep = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.IsTreeNodeSelected : CS.System.Boolean
CS.GameFramePro.ITreeNodeInfor.IsTreeNodeSelected = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.ParentTreeNode : CS.GameFramePro.ITreeNodeInfor
CS.GameFramePro.ITreeNodeInfor.ParentTreeNode = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.RootTreeView : CS.GameFramePro.ITreeView
CS.GameFramePro.ITreeNodeInfor.RootTreeView = nil

---@property readonly CS.GameFramePro.ITreeNodeInfor.AllSubNodesInfor : CS.System.Collections.Generic.List
CS.GameFramePro.ITreeNodeInfor.AllSubNodesInfor = nil

---@param rootTreeView : CS.GameFramePro.ITreeView
---@param showStr : CS.System.String
---@param treeNodePath : CS.System.String
---@param parent : CS.GameFramePro.ITreeNodeInfor
---@param isDefaultSelected : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:InitialedTreeNode(rootTreeView, showStr, treeNodePath, parent, isDefaultSelected)
end

---@param isSelected : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:SetTreeNodeSelected(isSelected)
end

---@param parent : CS.GameFramePro.ITreeNodeInfor
---@param keepChildNodeState : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:SetParent(parent, keepChildNodeState)
end

---@param eventNode : CS.GameFramePro.ITreeNodeInfor
---@param isAddNode : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:NotifyParentStructureChange(eventNode, isAddNode)
end

---@param eventNode : CS.GameFramePro.ITreeNodeInfor
function CS.GameFramePro.ITreeNodeInfor:NotifyChildsNodeDeepChange(eventNode)
end

---@param childNode : CS.GameFramePro.ITreeNodeInfor
---@param keepState : CS.System.Boolean
---@param index : CS.System.Int32
function CS.GameFramePro.ITreeNodeInfor:AddChildNode(childNode, keepState, index)
end

---@param childNodes : CS.System.Collections.Generic.IEnumerable
---@param keepState : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:AddRangeChildNodes(childNodes, keepState)
end

---@param childNode : CS.GameFramePro.ITreeNodeInfor
function CS.GameFramePro.ITreeNodeInfor:RemoveChildNode(childNode)
end

---@param index : CS.System.Int32
function CS.GameFramePro.ITreeNodeInfor:RemoveChildNodeByIndex(index)
end

---@param includeSelf : CS.System.Boolean
function CS.GameFramePro.ITreeNodeInfor:RemoveAllChildNodes(includeSelf)
end

function CS.GameFramePro.ITreeNodeInfor:OnBeforeDeleteTreeNode()
end

function CS.GameFramePro.ITreeNodeInfor:ShowTreeNodeInfor()
end