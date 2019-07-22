---@class CS.GameFramePro.ResourcesEx.ILoadAssetRecord
CS.GameFramePro.ResourcesEx.ILoadAssetRecord = {}

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.InstanceID = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.AssetUrl = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.ReferenceCount = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.AssetLoadedType = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.RemainTimeToBeDelete = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.MarkToDeleteTime = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.TargetAsset = nil

---@property readonly CS.GameFramePro.ResourcesEx.ILoadAssetRecord.BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.ILoadAssetRecord.BelongAssetManager = nil

---@param tickTime : CS.System.Single
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:TimeTick(tickTime)
end

function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:AddReference()
end

---@param isforceDelete : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:ReduceReference(isforceDelete)
end

function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:NotifyNoReference()
end

---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:NotifyReReference()
end

function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:NotifyReferenceChange()
end

function CS.GameFramePro.ResourcesEx.ILoadAssetRecord:NotifyReleaseRecord()
end