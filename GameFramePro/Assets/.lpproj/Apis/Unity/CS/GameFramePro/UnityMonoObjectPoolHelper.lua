---@class CS.GameFramePro.UnityMonoObjectPoolHelper : CS.Single_Mono_AutoCreateNotDestroy
CS.GameFramePro.UnityMonoObjectPoolHelper = {}

---@return CS.GameFramePro.UnityMonoObjectPoolHelper
function CS.GameFramePro.UnityMonoObjectPoolHelper()
end

---@param poolName : CS.System.String
---@return CS.UnityEngine.Transform
function CS.GameFramePro.UnityMonoObjectPoolHelper:GetUnityPoolManagerTransParent(poolName)
end

---@param poolName : CS.System.String
function CS.GameFramePro.UnityMonoObjectPoolHelper:RecycleUnityPoolManagerTransParent(poolName)
end