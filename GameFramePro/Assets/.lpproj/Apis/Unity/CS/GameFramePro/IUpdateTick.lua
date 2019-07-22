---@class CS.GameFramePro.IUpdateTick
CS.GameFramePro.IUpdateTick = {}

---@property readonly CS.GameFramePro.IUpdateTick.TickPerUpdateCount : CS.System.UInt32
CS.GameFramePro.IUpdateTick.TickPerUpdateCount = nil

---@return CS.System.Boolean
function CS.GameFramePro.IUpdateTick:CheckIfNeedUpdateTick()
end

---@param currentTime : CS.System.Single
function CS.GameFramePro.IUpdateTick:UpdateTick(currentTime)
end