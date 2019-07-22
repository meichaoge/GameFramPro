---@class CS.Newtonsoft.Json.Linq.JTokenWriter : CS.Newtonsoft.Json.JsonWriter
CS.Newtonsoft.Json.Linq.JTokenWriter = {}

---@property readonly CS.Newtonsoft.Json.Linq.JTokenWriter.Token : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JTokenWriter.Token = nil

---@param container : CS.Newtonsoft.Json.Linq.JContainer
---@return CS.Newtonsoft.Json.Linq.JTokenWriter
function CS.Newtonsoft.Json.Linq.JTokenWriter(container)
end

---@return CS.Newtonsoft.Json.Linq.JTokenWriter
function CS.Newtonsoft.Json.Linq.JTokenWriter()
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:Flush()
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:Close()
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteStartObject()
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteStartArray()
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteStartConstructor(name)
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.Linq.JTokenWriter:WritePropertyName(name)
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteNull()
end

function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteUndefined()
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteRaw(json)
end

---@param text : CS.System.String
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteComment(text)
end

---@param value : CS.System.String
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Int32
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.UInt32
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Int64
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.UInt64
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Single
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Double
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Int16
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.UInt16
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Char
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Byte
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.SByte
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Decimal
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.DateTime
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.DateTimeOffset
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Byte[]
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.TimeSpan
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Guid
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end

---@param value : CS.System.Uri
function CS.Newtonsoft.Json.Linq.JTokenWriter:WriteValue(value)
end