---@class CS.GameFramePro.XluaManager : CS.Single
CS.GameFramePro.XluaManager = {}

---@property readwrite CS.GameFramePro.XluaManager.LuaEngine : CS.XLua.LuaEnv
CS.GameFramePro.XluaManager.LuaEngine = nil

---@property readwrite CS.GameFramePro.XluaManager.TickPerUpdateCount : CS.System.UInt32
CS.GameFramePro.XluaManager.TickPerUpdateCount = nil

---@return CS.GameFramePro.XluaManager
function CS.GameFramePro.XluaManager()
end

function CS.GameFramePro.XluaManager:DisposeInstance()
end

---@return CS.System.Boolean
function CS.GameFramePro.XluaManager:CheckIfNeedUpdateTick()
end

---@param currentTime : CS.System.Single
function CS.GameFramePro.XluaManager:UpdateTick(currentTime)
end