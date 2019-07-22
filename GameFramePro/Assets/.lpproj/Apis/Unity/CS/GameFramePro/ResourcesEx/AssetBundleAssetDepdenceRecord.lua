---@class CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord : CS.System.Object
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord = {}

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_InstanceID = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AssetUrl = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_ReferenceCount = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AssetLoadedType = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_TargetAsset = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_RemainTimeToBeDelete = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_MarkToDeleteTime = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_BelongAssetManager = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AllDepdenceAssetBundleRecord : CS.System.Collections.Generic.List
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_AllDepdenceAssetBundleRecord = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_mAllBeReferenceAssetRecord : CS.System.Collections.Generic.List
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.Debug_mAllBeReferenceAssetRecord = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.InstanceID = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.AssetUrl = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.ReferenceCount = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.AssetLoadedType = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.TargetAsset = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.RemainTimeToBeDelete = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.MarkToDeleteTime = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord.BelongAssetManager = nil

---@return CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord()
end

---@param assetPath : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
---@return CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord(assetPath, typeEnum, asset, manager)
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:UpdateData()
end

---@param assetPath : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:Initial(assetPath, typeEnum, asset, manager)
end

---@param depdence : CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:AddDepdence(depdence)
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:ClearAllDepdence()
end

---@param depdence : CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:ReduceDepdence(depdence)
end

---@param record : CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:AddSubAssetReference(record)
end

---@param record : CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:ReduceSubAssetReference(record)
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:AddReference()
end

---@param isforceDelete : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:ReduceReference(isforceDelete)
end

---@param tickTime : CS.System.Single
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:TimeTick(tickTime)
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:NotifyNoReference()
end

---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:NotifyReReference()
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:NotifyReleaseRecord()
end

function CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord:NotifyReferenceChange()
end