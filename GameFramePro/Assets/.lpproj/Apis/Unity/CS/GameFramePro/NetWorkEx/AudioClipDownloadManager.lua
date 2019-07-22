---@class CS.GameFramePro.NetWorkEx.AudioClipDownloadManager : CS.GameFramePro.NetWorkEx.UnityWebRequestDownLoadManager
CS.GameFramePro.NetWorkEx.AudioClipDownloadManager = {}

---@property readonly CS.GameFramePro.NetWorkEx.AudioClipDownloadManager.S_Instance : CS.GameFramePro.NetWorkEx.AudioClipDownloadManager
CS.GameFramePro.NetWorkEx.AudioClipDownloadManager.S_Instance = nil

---@return CS.GameFramePro.NetWorkEx.AudioClipDownloadManager
function CS.GameFramePro.NetWorkEx.AudioClipDownloadManager()
end

---@param taskUrl : CS.System.String
---@param type : CS.UnityEngine.AudioType
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.NetWorkEx.AudioClipDownloadManager:GetDataFromUrl(taskUrl, type, callback, priorityEnum)
end