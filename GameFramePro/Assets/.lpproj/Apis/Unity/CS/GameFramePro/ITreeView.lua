---@class CS.GameFramePro.ITreeView
CS.GameFramePro.ITreeView = {}

---@param rootTreeNodes : CS.System.Collections.Generic.IEnumerable
function CS.GameFramePro.ITreeView:InitialTreeView(rootTreeNodes)
end

---@param rootTreeNode : CS.GameFramePro.BaseTreeNodeInfor
---@param index : CS.System.Int32
function CS.GameFramePro.ITreeView:AddRootTreeNode(rootTreeNode, index)
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.GameFramePro.ITreeView:GetRootTreeNodes()
end

---@param isExpand : CS.System.Boolean
function CS.GameFramePro.ITreeView:DrawTreeView(isExpand)
end

---@param showStr : CS.System.String
---@param treeNodePath : CS.System.String
---@param parent : CS.GameFramePro.BaseTreeNodeInfor
---@param isDefaultSelected : CS.System.Boolean
---@return CS.GameFramePro.BaseTreeNodeInfor
function CS.GameFramePro.ITreeView:GetTreeNode(showStr, treeNodePath, parent, isDefaultSelected)
end

---@param treeNode : CS.GameFramePro.ITreeNodeInfor
function CS.GameFramePro.ITreeView:DeleteTreeNodeFrom(treeNode)
end