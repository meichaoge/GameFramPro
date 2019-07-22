---@class CS.Newtonsoft.Json.JsonTextWriter : CS.Newtonsoft.Json.JsonWriter
CS.Newtonsoft.Json.JsonTextWriter = {}

---@property readwrite CS.Newtonsoft.Json.JsonTextWriter.Indentation : CS.System.Int32
CS.Newtonsoft.Json.JsonTextWriter.Indentation = nil

---@property readwrite CS.Newtonsoft.Json.JsonTextWriter.QuoteChar : CS.System.Char
CS.Newtonsoft.Json.JsonTextWriter.QuoteChar = nil

---@property readwrite CS.Newtonsoft.Json.JsonTextWriter.IndentChar : CS.System.Char
CS.Newtonsoft.Json.JsonTextWriter.IndentChar = nil

---@property readwrite CS.Newtonsoft.Json.JsonTextWriter.QuoteName : CS.System.Boolean
CS.Newtonsoft.Json.JsonTextWriter.QuoteName = nil

---@param textWriter : CS.System.IO.TextWriter
---@return CS.Newtonsoft.Json.JsonTextWriter
function CS.Newtonsoft.Json.JsonTextWriter(textWriter)
end

function CS.Newtonsoft.Json.JsonTextWriter:Flush()
end

function CS.Newtonsoft.Json.JsonTextWriter:Close()
end

function CS.Newtonsoft.Json.JsonTextWriter:WriteStartObject()
end

function CS.Newtonsoft.Json.JsonTextWriter:WriteStartArray()
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WriteStartConstructor(name)
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WritePropertyName(name)
end

function CS.Newtonsoft.Json.JsonTextWriter:WriteNull()
end

function CS.Newtonsoft.Json.JsonTextWriter:WriteUndefined()
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WriteRaw(json)
end

---@param value : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Int32
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.UInt32
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Int64
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.UInt64
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Single
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Double
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Boolean
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Int16
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.UInt16
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Char
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Byte
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.SByte
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Decimal
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.DateTime
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Byte[]
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.DateTimeOffset
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Guid
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.TimeSpan
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param value : CS.System.Uri
function CS.Newtonsoft.Json.JsonTextWriter:WriteValue(value)
end

---@param text : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WriteComment(text)
end

---@param ws : CS.System.String
function CS.Newtonsoft.Json.JsonTextWriter:WriteWhitespace(ws)
end