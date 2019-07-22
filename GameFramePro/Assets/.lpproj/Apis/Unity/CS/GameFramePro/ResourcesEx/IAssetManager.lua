---@class CS.GameFramePro.ResourcesEx.IAssetManager
CS.GameFramePro.ResourcesEx.IAssetManager = {}

---@property readonly CS.GameFramePro.ResourcesEx.IAssetManager.MaxAliveTimeAfterNoReference : CS.System.Single
CS.GameFramePro.ResourcesEx.IAssetManager.MaxAliveTimeAfterNoReference = nil

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.IAssetManager:NotifyAssetReferenceChange(record)
end

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.IAssetManager:NotifyAssetRelease(record)
end