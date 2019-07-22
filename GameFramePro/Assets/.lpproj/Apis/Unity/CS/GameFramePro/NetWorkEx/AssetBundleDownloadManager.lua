---@module CS.GameFramePro.NetWorkEx
CS.GameFramePro.NetWorkEx = {}

---@class CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager : CS.GameFramePro.NetWorkEx.UnityWebRequestDownLoadManager
CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager = {}

---@property readonly CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager.S_Instance : CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager
CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager.S_Instance = nil

---@return CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager
function CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager()
end

---@param taskUrl : CS.System.String
---@param crc : CS.System.UInt32
---@param callback : CS.System.Action
---@param priorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
function CS.GameFramePro.NetWorkEx.AssetBundleDownloadManager:GetDataFromUrl(taskUrl, crc, callback, priorityEnum)
end