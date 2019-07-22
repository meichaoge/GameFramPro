---@class CS.UnityEngine.LocationService : CS.System.Object
CS.UnityEngine.LocationService = {}

---@property readonly CS.UnityEngine.LocationService.isEnabledByUser : CS.System.Boolean
CS.UnityEngine.LocationService.isEnabledByUser = nil

---@property readonly CS.UnityEngine.LocationService.status : CS.UnityEngine.LocationServiceStatus
CS.UnityEngine.LocationService.status = nil

---@property readonly CS.UnityEngine.LocationService.lastData : CS.UnityEngine.LocationInfo
CS.UnityEngine.LocationService.lastData = nil

---@return CS.UnityEngine.LocationService
function CS.UnityEngine.LocationService()
end

---@param desiredAccuracyInMeters : CS.System.Single
---@param updateDistanceInMeters : CS.System.Single
function CS.UnityEngine.LocationService:Start(desiredAccuracyInMeters, updateDistanceInMeters)
end

---@param desiredAccuracyInMeters : CS.System.Single
function CS.UnityEngine.LocationService:Start(desiredAccuracyInMeters)
end

function CS.UnityEngine.LocationService:Start()
end

function CS.UnityEngine.LocationService:Stop()
end