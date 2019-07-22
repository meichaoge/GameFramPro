---@class CS.GameFramePro.NetWorkEx.IDownloadTaskProcess
CS.GameFramePro.NetWorkEx.IDownloadTaskProcess = {}

---@property readonly CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.TaskUrl : CS.System.String
CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.TaskUrl = nil

---@property readonly CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.IsCompleteInvoke : CS.System.Boolean
CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.IsCompleteInvoke = nil

---@property readonly CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.TaskPriorityEnum : CS.GameFramePro.NetWorkEx.UnityTaskPriorityEnum
CS.GameFramePro.NetWorkEx.IDownloadTaskProcess.TaskPriorityEnum = nil

function CS.GameFramePro.NetWorkEx.IDownloadTaskProcess:StartDownloadTask()
end

function CS.GameFramePro.NetWorkEx.IDownloadTaskProcess:CancelDownloadTask()
end

function CS.GameFramePro.NetWorkEx.IDownloadTaskProcess:ClearDownloadTask()
end