---@class CS.GameFramePro.ResourcesTracker : CS.System.Object
CS.GameFramePro.ResourcesTracker = {}

---@property readonly CS.GameFramePro.ResourcesTracker.S_Instance : CS.GameFramePro.ResourcesTracker
CS.GameFramePro.ResourcesTracker.S_Instance = nil

---@return CS.GameFramePro.ResourcesTracker
function CS.GameFramePro.ResourcesTracker()
end

---@param obj : CS.UnityEngine.Object
---@param stateEnum : CS.GameFramePro.TraceResourcesStateEnum
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.RegisterTraceResources(obj, stateEnum)
end

---@param obj : CS.UnityEngine.Object
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.UnRegisterTraceResources(obj)
end

---@param obj : CS.System.Object
---@param stateEnum : CS.GameFramePro.TraceNativeobjectStateEnum
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.RegistTraceNativeobject(obj, stateEnum)
end

---@param obj : CS.System.Object
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.RegistTraceNativeobject(obj)
end

---@param obj : CS.System.Object
---@param stateEnum : CS.GameFramePro.TraceNativeobjectStateEnum
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.UnRegistTraceNativeobject(obj, stateEnum)
end

---@param obj : CS.System.Object
---@return CS.System.Boolean
function CS.GameFramePro.ResourcesTracker.UnRegistTraceNativeobject(obj)
end