---@class CS.UnityEngine.Compass : CS.System.Object
CS.UnityEngine.Compass = {}

---@property readonly CS.UnityEngine.Compass.magneticHeading : CS.System.Single
CS.UnityEngine.Compass.magneticHeading = nil

---@property readonly CS.UnityEngine.Compass.trueHeading : CS.System.Single
CS.UnityEngine.Compass.trueHeading = nil

---@property readonly CS.UnityEngine.Compass.headingAccuracy : CS.System.Single
CS.UnityEngine.Compass.headingAccuracy = nil

---@property readonly CS.UnityEngine.Compass.rawVector : CS.UnityEngine.Vector3
CS.UnityEngine.Compass.rawVector = nil

---@property readonly CS.UnityEngine.Compass.timestamp : CS.System.Double
CS.UnityEngine.Compass.timestamp = nil

---@property readwrite CS.UnityEngine.Compass.enabled : CS.System.Boolean
CS.UnityEngine.Compass.enabled = nil

---@return CS.UnityEngine.Compass
function CS.UnityEngine.Compass()
end