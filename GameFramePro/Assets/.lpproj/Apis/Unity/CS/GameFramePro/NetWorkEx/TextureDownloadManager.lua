---@class CS.GameFramePro.NetWorkEx.TextureDownloadManager : CS.GameFramePro.NetWorkEx.UnityWebRequestDownLoadManager
CS.GameFramePro.NetWorkEx.TextureDownloadManager = {}

---@property readonly CS.GameFramePro.NetWorkEx.TextureDownloadManager.S_Instance : CS.GameFramePro.NetWorkEx.TextureDownloadManager
CS.GameFramePro.NetWorkEx.TextureDownloadManager.S_Instance = nil

---@return CS.GameFramePro.NetWorkEx.TextureDownloadManager
function CS.GameFramePro.NetWorkEx.TextureDownloadManager()
end

---@param taskUrl : CS.System.String
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.NetWorkEx.TextureDownloadManager:GetDataFromUrl(taskUrl, callback, priorityEnum)
end