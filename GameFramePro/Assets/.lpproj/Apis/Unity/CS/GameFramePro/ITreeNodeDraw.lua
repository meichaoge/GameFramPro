---@class CS.GameFramePro.ITreeNodeDraw
CS.GameFramePro.ITreeNodeDraw = {}

---@param deepOffset : CS.System.Single
---@param maxExpandDeep : CS.System.Int32
function CS.GameFramePro.ITreeNodeDraw:DrawTreeNode(deepOffset, maxExpandDeep)
end

---@param targetNode : CS.GameFramePro.ITreeNodeDraw
---@param isExpand : CS.System.Boolean
function CS.GameFramePro.ITreeNodeDraw:TreeNodeExpandOrFold(targetNode, isExpand)
end

---@return CS.System.Boolean
function CS.GameFramePro.ITreeNodeDraw:ChckeIfCanShowTreeNodeInfor()
end