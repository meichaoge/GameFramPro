---@class CS.UnityEngine.HitInfo : CS.System.ValueType
CS.UnityEngine.HitInfo = {}

---@field public CS.UnityEngine.HitInfo.target : CS.UnityEngine.GameObject
CS.UnityEngine.HitInfo.target = nil

---@field public CS.UnityEngine.HitInfo.camera : CS.UnityEngine.Camera
CS.UnityEngine.HitInfo.camera = nil

---@param name : CS.System.String
function CS.UnityEngine.HitInfo:SendMessage(name)
end

---@param exists : CS.UnityEngine.HitInfo
---@return CS.System.Boolean
function CS.UnityEngine.HitInfo.op_Implicit(exists)
end

---@param lhs : CS.UnityEngine.HitInfo
---@param rhs : CS.UnityEngine.HitInfo
---@return CS.System.Boolean
function CS.UnityEngine.HitInfo.Compare(lhs, rhs)
end