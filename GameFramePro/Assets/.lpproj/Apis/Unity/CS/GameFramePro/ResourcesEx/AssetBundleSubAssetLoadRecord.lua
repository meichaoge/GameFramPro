---@class CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord : CS.System.Object
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord = {}

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetUrl = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_InstanceID = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_ReferenceCount = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetLoadedType = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_TargetAsset = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_RemainTimeToBeDelete = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_MarkToDeleteTime = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_BelongAssetManager = nil

---@field public CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetBelongBundleName : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.Debug_AssetBelongBundleName = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.InstanceID : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.InstanceID = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetUrl : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetUrl = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.ReferenceCount : CS.System.Int32
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.ReferenceCount = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetLoadedType : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetLoadedType = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.TargetAsset : CS.UnityEngine.Object
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.TargetAsset = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.RemainTimeToBeDelete : CS.System.Single
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.RemainTimeToBeDelete = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.MarkToDeleteTime : CS.System.Int64
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.MarkToDeleteTime = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.BelongAssetManager : CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.BelongAssetManager = nil

---@property readwrite CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetBelongBundleName : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord.AssetBelongBundleName = nil

---@return CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord()
end

---@param assetUrl : CS.System.String
---@param assetBundleName : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
---@return CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord(assetUrl, assetBundleName, typeEnum, asset, manager)
end

function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:UpdateData()
end

---@param assetUrl : CS.System.String
---@param assetBundleName : CS.System.String
---@param typeEnum : CS.GameFramePro.ResourcesEx.LoadedAssetTypeEnum
---@param asset : CS.UnityEngine.Object
---@param manager : CS.GameFramePro.ResourcesEx.IAssetManager
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:Initial(assetUrl, assetBundleName, typeEnum, asset, manager)
end

---@param record1 : CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:IsReferenceSameAsset(record1)
end

function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:AddReference()
end

---@param isforceDelete : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:ReduceReference(isforceDelete)
end

---@param tickTime : CS.System.Single
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:TimeTick(tickTime)
end

function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:NotifyNoReference()
end

---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:NotifyReReference()
end

function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:NotifyReleaseRecord()
end

function CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord:NotifyReferenceChange()
end