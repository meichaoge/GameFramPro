---@class CS.GameFramePro.ITaskProcess
CS.GameFramePro.ITaskProcess = {}

---@property readonly CS.GameFramePro.ITaskProcess.IsDone : CS.System.Boolean
CS.GameFramePro.ITaskProcess.IsDone = nil

---@property readonly CS.GameFramePro.ITaskProcess.IsError : CS.System.Boolean
CS.GameFramePro.ITaskProcess.IsError = nil

---@property readonly CS.GameFramePro.ITaskProcess.Progress : CS.System.Single
CS.GameFramePro.ITaskProcess.Progress = nil

---@param value : CS.System.Action
function CS.GameFramePro.ITaskProcess:add_OnProgressChangedEvent(value)
end

---@param value : CS.System.Action
function CS.GameFramePro.ITaskProcess:remove_OnProgressChangedEvent(value)
end

function CS.GameFramePro.ITaskProcess:Tick()
end

---@param isDone : CS.System.Boolean
---@param isError : CS.System.Boolean
---@param progress : CS.System.Single
function CS.GameFramePro.ITaskProcess:OnCompleted(isDone, isError, progress)
end