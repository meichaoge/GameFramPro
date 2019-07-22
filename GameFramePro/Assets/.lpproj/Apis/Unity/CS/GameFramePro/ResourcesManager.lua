---@class CS.GameFramePro.ResourcesManager : CS.Single
CS.GameFramePro.ResourcesManager = {}

---@return CS.GameFramePro.ResourcesManager
function CS.GameFramePro.ResourcesManager()
end

---@param goName : CS.System.String
---@return CS.UnityEngine.GameObject
function CS.GameFramePro.ResourcesManager.Instantiate(goName)
end

---@param go : CS.UnityEngine.GameObject
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesManager.MarkNotDestroyOnLoad(go)
end

---@param obj : CS.UnityEngine.Object
function CS.GameFramePro.ResourcesManager.Destroy(obj)
end

---@param obj : CS.UnityEngine.Object
function CS.GameFramePro.ResourcesManager.DestroyImmediate(obj)
end

---@param assetPath : CS.System.String
---@param isForceReload : CS.System.Boolean
---@return CS.System.String
function CS.GameFramePro.ResourcesManager.LoadTextAssettSync(assetPath, isForceReload)
end

---@param assetPath : CS.System.String
---@param textAssetAction : CS.System.Action
---@param isForceReload : CS.System.Boolean
function CS.GameFramePro.ResourcesManager.LoadTextAssettAsync(assetPath, textAssetAction, isForceReload)
end

---@param assetPath : CS.System.String
---@param targetParent : CS.UnityEngine.Transform
---@param getAssetFromRecordAction : CS.GameFramePro.GetAssetFromRecordHandler
---@param getAssetReference : CS.GameFramePro.GetCurReferenceHandler
function CS.GameFramePro.ResourcesManager.LoadGameObjectAssetSync(assetPath, targetParent, getAssetFromRecordAction, getAssetReference)
end

---@param assetRecord : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
function CS.GameFramePro.ResourcesManager.UnLoadResourceAsset(assetRecord)
end

---@param assetRecord : CS.GameFramePro.ResourcesEx.ILoadAssetRecord
---@param isUnloadAllLoadedObjects : CS.System.Boolean
function CS.GameFramePro.ResourcesManager.UnLoadAssetBundle(assetRecord, isUnloadAllLoadedObjects)
end