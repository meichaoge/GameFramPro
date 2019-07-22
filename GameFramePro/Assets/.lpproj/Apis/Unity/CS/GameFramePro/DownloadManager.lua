---@class CS.GameFramePro.DownloadManager : CS.Single
CS.GameFramePro.DownloadManager = {}

---@property readwrite CS.GameFramePro.DownloadManager.TickPerUpdateCount : CS.System.UInt32
CS.GameFramePro.DownloadManager.TickPerUpdateCount = nil

---@return CS.GameFramePro.DownloadManager
function CS.GameFramePro.DownloadManager()
end

---@return CS.System.Boolean
function CS.GameFramePro.DownloadManager:CheckIfNeedUpdateTick()
end

---@param currentTime : CS.System.Single
function CS.GameFramePro.DownloadManager:UpdateTick(currentTime)
end

---@param taskUrl : CS.System.String
---@param crc : CS.System.UInt32
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.DownloadManager:GetAssetBundleFromUrl(taskUrl, crc, callback, priorityEnum)
end

---@param taskUrl : CS.System.String
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.DownloadManager:GetByteDataFromUrl(taskUrl, callback, priorityEnum)
end