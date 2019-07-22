---@class CS.GameFramePro.NetWorkEx.FileDownloadManager : CS.GameFramePro.NetWorkEx.UnityWebRequestDownLoadManager
CS.GameFramePro.NetWorkEx.FileDownloadManager = {}

---@property readonly CS.GameFramePro.NetWorkEx.FileDownloadManager.S_Instance : CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager
CS.GameFramePro.NetWorkEx.FileDownloadManager.S_Instance = nil

---@return CS.GameFramePro.NetWorkEx.FileDownloadManager
function CS.GameFramePro.NetWorkEx.FileDownloadManager()
end

---@param taskUrl : CS.System.String
---@param saveFilePath : CS.System.String
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.NetWorkEx.FileDownloadManager:GetDataFromUrl(taskUrl, saveFilePath, callback, priorityEnum)
end