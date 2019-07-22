---@class CS.GameFramePro.ResourcesEx.AssetBundleManager : CS.Single
CS.GameFramePro.ResourcesEx.AssetBundleManager = {}

---@property readonly CS.GameFramePro.ResourcesEx.AssetBundleManager.S_LocalAssetBundleTopDirectoryPath : CS.System.String
CS.GameFramePro.ResourcesEx.AssetBundleManager.S_LocalAssetBundleTopDirectoryPath = nil

---@property readonly CS.GameFramePro.ResourcesEx.AssetBundleManager.Debug_mAllLoadAssetBundleCache : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.ResourcesEx.AssetBundleManager.Debug_mAllLoadAssetBundleCache = nil

---@property readonly CS.GameFramePro.ResourcesEx.AssetBundleManager.Debug_mAllLoadedAssetBundleSubAssetRecord : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.ResourcesEx.AssetBundleManager.Debug_mAllLoadedAssetBundleSubAssetRecord = nil

---@property readonly CS.GameFramePro.ResourcesEx.AssetBundleManager.MaxAliveTimeAfterNoReference : CS.System.Single
CS.GameFramePro.ResourcesEx.AssetBundleManager.MaxAliveTimeAfterNoReference = nil

---@return CS.GameFramePro.ResourcesEx.AssetBundleManager
function CS.GameFramePro.ResourcesEx.AssetBundleManager()
end

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.AssetBundleManager:NotifyAssetRelease(record)
end

---@param record : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesEx.AssetBundleManager:NotifyAssetReferenceChange(record)
end

---@param assetPath : CS.System.String
---@return CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetFromCache(assetPath)
end

---@param assetBundlePath : CS.System.String
---@param assetBundleRecord : CS.GameFramePro.ResourcesEx.AssetBundleAssetDepdenceRecord
---@return CS.UnityEngine.AssetBundle
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetBundleSync(assetBundlePath, assetBundleRecord)
end

---@param assetBundlePath : CS.System.String
---@param assetBundleDepdence : CS.System.Action
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetBundleAsync(assetBundlePath, assetBundleDepdence)
end

---@param assetPath : CS.System.String
---@param loadCallback : CS.System.Action
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetSync(assetPath, loadCallback)
end

---@param assetPath : CS.System.String
---@param assetBundlePath : CS.System.String
---@param assetName : CS.System.String
---@param loadCallback : CS.System.Action
---@param isNeedTranslateAssetName : CS.System.Boolean
---@param isNeedCheckCaheRecord : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetSync(assetPath, assetBundlePath, assetName, loadCallback, isNeedTranslateAssetName, isNeedCheckCaheRecord)
end

---@param assetPath : CS.System.String
---@param assetBundlePath : CS.System.String
---@param assetName : CS.System.String
---@param isNeedTranslateAssetName : CS.System.Boolean
---@param isNeedCheckCaheRecord : CS.System.Boolean
---@return CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetSync(assetPath, assetBundlePath, assetName, isNeedTranslateAssetName, isNeedCheckCaheRecord)
end

---@param assetPath : CS.System.String
---@return CS.GameFramePro.ResourcesEx.AssetBundleSubAssetLoadRecord
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetSync(assetPath)
end

---@param assetPath : CS.System.String
---@param loadCallback : CS.System.Action
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetAsync(assetPath, loadCallback)
end

---@param assetPath : CS.System.String
---@param assetBundlePath : CS.System.String
---@param assetName : CS.System.String
---@param loadCallback : CS.System.Action
---@param isNeedTranslateAssetName : CS.System.Boolean
---@param isNeedCheckCaheRecord : CS.System.Boolean
function CS.GameFramePro.ResourcesEx.AssetBundleManager:LoadAssetAsync(assetPath, assetBundlePath, assetName, loadCallback, isNeedTranslateAssetName, isNeedCheckCaheRecord)
end

---@param handle : CS.UnityEngine.Networking.DownloadHandlerBuffer
---@param assetBundleName : CS.System.String
function CS.GameFramePro.ResourcesEx.AssetBundleManager:SaveAssetBundleFromDownload(handle, assetBundleName)
end