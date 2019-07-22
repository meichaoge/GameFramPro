---@class CS.GameFramePro.AsyncTracker : CS.Single
CS.GameFramePro.AsyncTracker = {}

---@return CS.GameFramePro.AsyncTracker
function CS.GameFramePro.AsyncTracker()
end

---@param task : CS.UnityEngine.YieldInstruction
---@return CS.System.Boolean
function CS.GameFramePro.AsyncTracker:TrackAsyncTask(task)
end

---@param task : CS.UnityEngine.YieldInstruction
---@return CS.System.Boolean
function CS.GameFramePro.AsyncTracker:UnTrackAsyncTask(task)
end