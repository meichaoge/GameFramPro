---@class CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager : CS.Single
CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager = {}

---@property readwrite CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager.TickPerUpdateCount : CS.System.UInt32
CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager.TickPerUpdateCount = nil

---@return CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager
function CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager()
end

---@return CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager:CheckIfNeedUpdateTick()
end

---@param currentTime : CS.System.Single
function CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager:UpdateTick(currentTime)
end

---@param assetInfor : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager.RecycleNoReferenceLoadAssetRecord(assetInfor)
end

---@param assetPath : CS.System.String
---@return CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.AssetDelayDeleteManager.TryGetILoadAssetRecord(assetPath)
end