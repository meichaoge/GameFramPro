---@class CS.GameFramePro.AssetReferenceController : CS.UnityEngine.MonoBehaviour
CS.GameFramePro.AssetReferenceController = {}

---@field public CS.GameFramePro.AssetReferenceController.mAllComponentReferencesRecord : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.AssetReferenceController.mAllComponentReferencesRecord = nil

---@field public CS.GameFramePro.AssetReferenceController.mAllGameObjectReferenceController : CS.System.Collections.Generic.Dictionary
CS.GameFramePro.AssetReferenceController.mAllGameObjectReferenceController = nil

---@return CS.GameFramePro.AssetReferenceController
function CS.GameFramePro.AssetReferenceController()
end

function CS.GameFramePro.AssetReferenceController:OnRemoveAllReference()
end