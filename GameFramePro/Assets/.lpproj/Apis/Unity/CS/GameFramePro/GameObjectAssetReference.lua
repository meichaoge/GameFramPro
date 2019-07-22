---@class CS.GameFramePro.GameObjectAssetReference : CS.GameFramePro.BaseAssetReference
CS.GameFramePro.GameObjectAssetReference = {}

---@property readwrite CS.GameFramePro.GameObjectAssetReference.mTargetAssetInstance : CS.UnityEngine.GameObject
CS.GameFramePro.GameObjectAssetReference.mTargetAssetInstance = nil

---@return CS.GameFramePro.GameObjectAssetReference
function CS.GameFramePro.GameObjectAssetReference()
end

---@param component : CS.UnityEngine.Transform
---@param newAssetRecord : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
---@param getAssetFromRecordAction : CS.GameFramePro.GetAssetFromRecordHandler
---@return CS.GameFramePro.BaseAssetReference
function CS.GameFramePro.GameObjectAssetReference:AttachComponentReference(component, newAssetRecord, getAssetFromRecordAction)
end

---@param component : CS.UnityEngine.Transform
---@param allTransformAssetReference : CS.System.Collections.Generic.LinkedList
---@return CS.GameFramePro.IAssetReference
function CS.GameFramePro.GameObjectAssetReference.GetTransformAssetReference(component, allTransformAssetReference)
end

---@param targetParent : CS.UnityEngine.Transform
---@param assetRecord : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
---@return CS.GameFramePro.ResourcesEx.BaseBeReferenceAssetInfor
function CS.GameFramePro.GameObjectAssetReference.GetGameObjectInstance(targetParent, assetRecord)
end