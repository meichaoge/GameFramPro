---@class CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord : CS.System.Object
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord = {}

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_AssetUrl = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_InstanceID = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_ReferenceCount = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_AssetLoadedType = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_TargetAsset = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_RemainTimeToBeDelete = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_MarkToDeleteTime = nil

---@field public CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.Debug_BelongAssetManager = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.InstanceID = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.AssetUrl = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.ReferenceCount = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.AssetLoadedType = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.TargetAsset = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.RemainTimeToBeDelete = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.MarkToDeleteTime = nil

---@property readwrite CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord.BelongAssetManager = nil

---@return CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord()
end

---@param assetPath : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
---@return CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord(assetPath, typeEnum, asset, manager)
end

function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:UpdateData()
end

---@param assetPath : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:Initial(assetPath, typeEnum, asset, manager)
end

function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:AddReference()
end

---@param isforceDelete : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:ReduceReference(isforceDelete)
end

---@param tickTime : CS.System.Single
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:TimeTick(tickTime)
end

function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:NotifyNoReference()
end

---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:NotifyReReference()
end

function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:NotifyReleaseRecord()
end

function CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord:NotifyReferenceChange()
end