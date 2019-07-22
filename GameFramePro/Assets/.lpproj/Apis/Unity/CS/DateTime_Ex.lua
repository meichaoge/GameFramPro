---@class CS.DateTime_Ex : CS.System.Object
CS.DateTime_Ex = {}

---@field public CS.DateTime_Ex.TimestampBaseTime : CS.System.DateTime
CS.DateTime_Ex.TimestampBaseTime = nil

---@field public CS.DateTime_Ex.MaxTimestampBaseTime : CS.System.DateTime
CS.DateTime_Ex.MaxTimestampBaseTime = nil

---@field public CS.DateTime_Ex.TimestampBaseTimeTicks : CS.System.Int64
CS.DateTime_Ex.TimestampBaseTimeTicks = nil

---@param timestampSecond : CS.System.Int32
---@return CS.System.DateTime
function CS.DateTime_Ex.TimestampToDateTime(timestampSecond)
end

---@param utcDateTime : CS.System.DateTime
---@return CS.System.Int32
function CS.DateTime_Ex.ToTimestamp_Second(utcDateTime)
end

---@param timestampMillisecond : CS.System.Int64
---@return CS.System.DateTime
function CS.DateTime_Ex.TimestampToDateTime(timestampMillisecond)
end

---@param utcDateTime : CS.System.DateTime
---@return CS.System.Int64
function CS.DateTime_Ex.ToTimestamp_Millisecond(utcDateTime)
end

---@param targetDataTime : CS.System.DateTime
---@return CS.System.DateTime
function CS.DateTime_Ex.TruncatedDataTime_Second(targetDataTime)
end

---@param targetDataTime : CS.System.DateTime
---@return CS.System.DateTime
function CS.DateTime_Ex.TruncatedDataTime_Minute(targetDataTime)
end

---@param targetDataTime : CS.System.DateTime
---@return CS.System.DateTime
function CS.DateTime_Ex.TruncatedDataTime_Hour(targetDataTime)
end

---@param targetDataTime : CS.System.DateTime
---@return CS.System.DateTime
function CS.DateTime_Ex.TruncatedDataTime_Day(targetDataTime)
end