---@class CS.GameFramePro.ComponentReferenceInfor : CS.System.Object
CS.GameFramePro.ComponentReferenceInfor = {}

---@field public CS.GameFramePro.ComponentReferenceInfor.mTargetComponent : CS.UnityEngine.Component
CS.GameFramePro.ComponentReferenceInfor.mTargetComponent = nil

---@field public CS.GameFramePro.ComponentReferenceInfor.Debug_ReferenceDetail : CS.System.Collections.Generic.List
CS.GameFramePro.ComponentReferenceInfor.Debug_ReferenceDetail = nil

---@field public CS.GameFramePro.ComponentReferenceInfor.mAllCurrentResourcesRecord : CS.System.Collections.Generic.List
CS.GameFramePro.ComponentReferenceInfor.mAllCurrentResourcesRecord = nil

---@field public CS.GameFramePro.ComponentReferenceInfor.mAllCurrentAssetBundleRecord : CS.System.Collections.Generic.List
CS.GameFramePro.ComponentReferenceInfor.mAllCurrentAssetBundleRecord = nil

---@return CS.GameFramePro.ComponentReferenceInfor
function CS.GameFramePro.ComponentReferenceInfor()
end

---@param component : CS.UnityEngine.Component
---@param allReferences : CS.System.Collections.Generic.LinkedList
---@return CS.GameFramePro.ComponentReferenceInfor
function CS.GameFramePro.ComponentReferenceInfor(component, allReferences)
end