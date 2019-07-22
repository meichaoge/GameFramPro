---@class CS.Unity.Experimental.Audio.ExecuteJobFunction : CS.System.MulticastDelegate
CS.Unity.Experimental.Audio.ExecuteJobFunction = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.Unity.Experimental.Audio.ExecuteJobFunction
function CS.Unity.Experimental.Audio.ExecuteJobFunction(object, method)
end

---@param updateJobData : CS.Updater
---@param jobData : CS.Job
---@param unused1 : CS.System.IntPtr
---@param unused2 : CS.System.IntPtr
---@param ranges : CS.Unity.Jobs.LowLevel.Unsafe.JobRanges
---@param ignored2 : CS.System.Int32
function CS.Unity.Experimental.Audio.ExecuteJobFunction:Invoke(updateJobData, jobData, unused1, unused2, ranges, ignored2)
end

---@param updateJobData : CS.Updater
---@param jobData : CS.Job
---@param unused1 : CS.System.IntPtr
---@param unused2 : CS.System.IntPtr
---@param ranges : CS.Unity.Jobs.LowLevel.Unsafe.JobRanges
---@param ignored2 : CS.System.Int32
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.Unity.Experimental.Audio.ExecuteJobFunction:BeginInvoke(updateJobData, jobData, unused1, unused2, ranges, ignored2, callback, object)
end

---@param updateJobData : CS.Updater
---@param jobData : CS.Job
---@param ranges : CS.Unity.Jobs.LowLevel.Unsafe.JobRanges
---@param result : CS.System.IAsyncResult
function CS.Unity.Experimental.Audio.ExecuteJobFunction:EndInvoke(updateJobData, jobData, ranges, result)
end