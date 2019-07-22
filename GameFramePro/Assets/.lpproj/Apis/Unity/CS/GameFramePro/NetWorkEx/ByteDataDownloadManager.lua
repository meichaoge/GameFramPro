---@class CS.GameFramePro.NetWorkEx.ByteDataDownloadManager : CS.GameFramePro.NetWorkEx.UnityWebRequestDownLoadManager
CS.GameFramePro.NetWorkEx.ByteDataDownloadManager = {}

---@property readonly CS.GameFramePro.NetWorkEx.ByteDataDownloadManager.S_Instance : CS.GameFramePro.NetWorkEx.ByteDataDownloadManager
CS.GameFramePro.NetWorkEx.ByteDataDownloadManager.S_Instance = nil

---@return CS.GameFramePro.NetWorkEx.ByteDataDownloadManager
function CS.GameFramePro.NetWorkEx.ByteDataDownloadManager()
end

---@param taskUrl : CS.System.String
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.NetWorkEx.ByteDataDownloadManager:GetDataFromUrl(taskUrl, callback, priorityEnum)
end