---@class CS.GameFramePro.AsyncManager : CS.Single_Mono_AutoCreateNotDestroy
CS.GameFramePro.AsyncManager = {}

---@field public CS.GameFramePro.AsyncManager.WaitFor_OneSecond : CS.UnityEngine.YieldInstruction
CS.GameFramePro.AsyncManager.WaitFor_OneSecond = nil

---@field public CS.GameFramePro.AsyncManager.WaitFor_HalfSecond : CS.UnityEngine.YieldInstruction
CS.GameFramePro.AsyncManager.WaitFor_HalfSecond = nil

---@field public CS.GameFramePro.AsyncManager.WaitFor_OneMinute : CS.UnityEngine.YieldInstruction
CS.GameFramePro.AsyncManager.WaitFor_OneMinute = nil

---@field public CS.GameFramePro.AsyncManager.WaitFor_EndOfFrame : CS.UnityEngine.YieldInstruction
CS.GameFramePro.AsyncManager.WaitFor_EndOfFrame = nil

---@field public CS.GameFramePro.AsyncManager.WaitFor_FixedUpdate : CS.UnityEngine.YieldInstruction
CS.GameFramePro.AsyncManager.WaitFor_FixedUpdate = nil

---@return CS.GameFramePro.AsyncManager
function CS.GameFramePro.AsyncManager()
end

---@param routine : CS.System.Collections.IEnumerator
---@return CS.UnityEngine.Coroutine
function CS.GameFramePro.AsyncManager:StartCoroutineEx(routine)
end

---@param routine : CS.UnityEngine.Coroutine
function CS.GameFramePro.AsyncManager:StopCoroutineEx(routine)
end

---@param async : CS.UnityEngine.AsyncOperation
---@param completeCallback : CS.System.Action
---@param procressCallback : CS.System.Action
function CS.GameFramePro.AsyncManager:StartAsyncOperation(async, completeCallback, procressCallback)
end