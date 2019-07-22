---@class CS.GameFramePro.ResourcesEx.LocalResourcesManager : CS.Single
CS.GameFramePro.ResourcesEx.LocalResourcesManager = {}

---@property readonly CS.GameFramePro.ResourcesEx.LocalResourcesManager.Debug_mAllLoadedAssetRecord : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.ResourcesEx.LocalResourcesManager.Debug_mAllLoadedAssetRecord = nil

---@property readonly CS.GameFramePro.ResourcesEx.LocalResourcesManager.Debug_mAllResoucesLoadAssetInstanceIds : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.ResourcesEx.LocalResourcesManager.Debug_mAllResoucesLoadAssetInstanceIds = nil

---@property readonly CS.GameFramePro.ResourcesEx.LocalResourcesManager.MaxAliveTimeAfterNoReference : CS.System.Single
CS.GameFramePro.ResourcesEx.LocalResourcesManager.MaxAliveTimeAfterNoReference = nil

---@return CS.GameFramePro.ResourcesEx.LocalResourcesManager
function CS.GameFramePro.ResourcesEx.LocalResourcesManager()
end

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:NotifyAssetRelease(record)
end

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:NotifyAssetReferenceChange(record)
end

---@param assetpath : CS.System.String
---@return CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:LoadAssetFromCache(assetpath)
end

---@param assetpath : CS.System.String
---@param loadCallback : CS.System.Action
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:LoadAssetSync(assetpath, loadCallback)
end

---@param assetpath : CS.System.String
---@return CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:LoadAssetSync(assetpath)
end

---@param assetpath : CS.System.String
---@param loadCallback : CS.System.Action
---@param procressCallback : CS.System.Action
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:ResourcesLoadAssetAsync(assetpath, loadCallback, procressCallback)
end

---@param assetpath : CS.System.String
---@param asset : CS.UnityEngine.Object
---@return CS.GameFramePro.ResourcesEx.ResourcesLoadAssetRecord
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:RecordResourcesLoadAsset(assetpath, asset)
end

---@param asset : CS.UnityEngine.Object
---@param isforceDelete : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:UnRecordResourcesLoadAsset(asset, isforceDelete)
end

---@param instanceID : CS.System.Int32
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.LocalResourcesManager:IsAssetLoadedByResource(instanceID)
end