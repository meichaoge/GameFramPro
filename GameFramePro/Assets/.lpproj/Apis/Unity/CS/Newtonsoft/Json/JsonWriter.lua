---@class CS.Newtonsoft.Json.JsonWriter : CS.System.Object
CS.Newtonsoft.Json.JsonWriter = {}

---@property readwrite CS.Newtonsoft.Json.JsonWriter.CloseOutput : CS.System.Boolean
CS.Newtonsoft.Json.JsonWriter.CloseOutput = nil

---@property readonly CS.Newtonsoft.Json.JsonWriter.WriteState : CS.Newtonsoft.Json.WriteState
CS.Newtonsoft.Json.JsonWriter.WriteState = nil

---@property readwrite CS.Newtonsoft.Json.JsonWriter.Formatting : CS.Newtonsoft.Json.Formatting
CS.Newtonsoft.Json.JsonWriter.Formatting = nil

function CS.Newtonsoft.Json.JsonWriter:Flush()
end

function CS.Newtonsoft.Json.JsonWriter:Close()
end

function CS.Newtonsoft.Json.JsonWriter:WriteStartObject()
end

function CS.Newtonsoft.Json.JsonWriter:WriteEndObject()
end

function CS.Newtonsoft.Json.JsonWriter:WriteStartArray()
end

function CS.Newtonsoft.Json.JsonWriter:WriteEndArray()
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteStartConstructor(name)
end

function CS.Newtonsoft.Json.JsonWriter:WriteEndConstructor()
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WritePropertyName(name)
end

function CS.Newtonsoft.Json.JsonWriter:WriteEnd()
end

---@param reader : CS.Newtonsoft.Json.JsonReader
function CS.Newtonsoft.Json.JsonWriter:WriteToken(reader)
end

function CS.Newtonsoft.Json.JsonWriter:WriteNull()
end

function CS.Newtonsoft.Json.JsonWriter:WriteUndefined()
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteRaw(json)
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteRawValue(json)
end

---@param value : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Int32
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt32
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Int64
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt64
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Single
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Double
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Boolean
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Int16
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt16
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Char
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Byte
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.SByte
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Decimal
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.DateTime
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.DateTimeOffset
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Guid
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.TimeSpan
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Nullable
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Byte[]
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Uri
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param value : CS.System.Object
function CS.Newtonsoft.Json.JsonWriter:WriteValue(value)
end

---@param text : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteComment(text)
end

---@param ws : CS.System.String
function CS.Newtonsoft.Json.JsonWriter:WriteWhitespace(ws)
end