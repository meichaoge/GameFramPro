---@class CS.Newtonsoft.Json.Utilities.StringBuffer : CS.System.Object
CS.Newtonsoft.Json.Utilities.StringBuffer = {}

---@property readwrite CS.Newtonsoft.Json.Utilities.StringBuffer.Position : CS.System.Int32
CS.Newtonsoft.Json.Utilities.StringBuffer.Position = nil

---@return CS.Newtonsoft.Json.Utilities.StringBuffer
function CS.Newtonsoft.Json.Utilities.StringBuffer()
end

---@param initalSize : CS.System.Int32
---@return CS.Newtonsoft.Json.Utilities.StringBuffer
function CS.Newtonsoft.Json.Utilities.StringBuffer(initalSize)
end

---@param value : CS.System.Char
function CS.Newtonsoft.Json.Utilities.StringBuffer:Append(value)
end

function CS.Newtonsoft.Json.Utilities.StringBuffer:Clear()
end

---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringBuffer:ToString()
end

---@param start : CS.System.Int32
---@param length : CS.System.Int32
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringBuffer:ToString(start, length)
end

---@return CS.System.Char[]
function CS.Newtonsoft.Json.Utilities.StringBuffer:GetInternalBuffer()
end