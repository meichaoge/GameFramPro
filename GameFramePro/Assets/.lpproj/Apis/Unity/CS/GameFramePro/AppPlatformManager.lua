---@class CS.GameFramePro.AppPlatformManager : CS.System.Object
CS.GameFramePro.AppPlatformManager = {}

---@param platform : CS.GameFramePro.AppPlatformEnum
---@return CS.System.String
function CS.GameFramePro.AppPlatformManager.GetPlatformFolderName(platform)
end

---@param platform : CS.GameFramePro.AppPlatformEnum
---@return CS.UnityEditor.BuildTarget
function CS.GameFramePro.AppPlatformManager.GetBuildTargetFromAppPlatformEnum(platform)
end

---@param buildTarget : CS.UnityEditor.BuildTarget
---@return CS.GameFramePro.AppPlatformEnum
function CS.GameFramePro.AppPlatformManager.GetAppPlatformEnumFromBuildTarget(buildTarget)
end

---@param platform : CS.GameFramePro.AppPlatformEnum
---@return CS.UnityEditor.BuildTargetGroup
function CS.GameFramePro.AppPlatformManager.GetBuildTargetGroupFromAppPlatformEnum(platform)
end

---@param buildTarget : CS.UnityEditor.BuildTarget
---@return CS.System.String
function CS.GameFramePro.AppPlatformManager.GetPlatformFolderName(buildTarget)
end

---@return CS.System.String
function CS.GameFramePro.AppPlatformManager.GetRuntimePlatformFolderName()
end