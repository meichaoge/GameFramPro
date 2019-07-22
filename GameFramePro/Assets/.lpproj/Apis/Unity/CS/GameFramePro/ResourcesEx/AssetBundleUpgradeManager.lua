---@class CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager : CS.Single
CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager = {}

---@return CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager
function CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager()
end

function CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager:BeginUpdateAssetBundle()
end

---@param assetPath : CS.System.String
---@return CS.System.String
function CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager:GetBundleNameByAssetPath(assetPath)
end

---@param assetBundlePath : CS.System.String
---@return CS.System.String[]
function CS.GameFramePro.ResourcesEx.AssetBundleUpgradeManager:GetAllDependencies(assetBundlePath)
end