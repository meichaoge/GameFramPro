---@class CS.CielaSpike.Task : CS.System.Object
CS.CielaSpike.Task = {}

---@property readwrite CS.CielaSpike.Task.Current : CS.System.Object
CS.CielaSpike.Task.Current = nil

---@property readonly CS.CielaSpike.Task.State : CS.CielaSpike.TaskState
CS.CielaSpike.Task.State = nil

---@property readwrite CS.CielaSpike.Task.Exception : CS.System.Exception
CS.CielaSpike.Task.Exception = nil

---@param routine : CS.System.Collections.IEnumerator
---@return CS.CielaSpike.Task
function CS.CielaSpike.Task(routine)
end

---@return CS.System.Boolean
function CS.CielaSpike.Task:MoveNext()
end

function CS.CielaSpike.Task:Reset()
end

function CS.CielaSpike.Task:Cancel()
end

---@return CS.System.Collections.IEnumerator
function CS.CielaSpike.Task:Wait()
end