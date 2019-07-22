---@class CS.GameFramePro.Loom : CS.UnityEngine.MonoBehaviour
CS.GameFramePro.Loom = {}

---@field public CS.GameFramePro.Loom._maxThreads : CS.System.Int32
CS.GameFramePro.Loom._maxThreads = nil

---@field public CS.GameFramePro.Loom._current : CS.GameFramePro.Loom
CS.GameFramePro.Loom._current = nil

---@return CS.GameFramePro.Loom
function CS.GameFramePro.Loom()
end

---@param action : CS.System.Action
function CS.GameFramePro.Loom:QueueOnMainThread(action)
end

---@param action : CS.System.Action
---@param time : CS.System.Single
function CS.GameFramePro.Loom:QueueOnMainThread(action, time)
end

---@param a : CS.System.Action
---@return CS.System.Threading.Thread
function CS.GameFramePro.Loom:RunAsync(a)
end