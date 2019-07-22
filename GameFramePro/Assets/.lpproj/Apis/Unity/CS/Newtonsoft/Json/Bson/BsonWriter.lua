---@class CS.Newtonsoft.Json.Bson.BsonWriter : CS.Newtonsoft.Json.JsonWriter
CS.Newtonsoft.Json.Bson.BsonWriter = {}

---@property readwrite CS.Newtonsoft.Json.Bson.BsonWriter.DateTimeKindHandling : CS.System.DateTimeKind
CS.Newtonsoft.Json.Bson.BsonWriter.DateTimeKindHandling = nil

---@param stream : CS.System.IO.Stream
---@return CS.Newtonsoft.Json.Bson.BsonWriter
function CS.Newtonsoft.Json.Bson.BsonWriter(stream)
end

function CS.Newtonsoft.Json.Bson.BsonWriter:Flush()
end

---@param text : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteComment(text)
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteStartConstructor(name)
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteRaw(json)
end

---@param json : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteRawValue(json)
end

function CS.Newtonsoft.Json.Bson.BsonWriter:WriteStartArray()
end

function CS.Newtonsoft.Json.Bson.BsonWriter:WriteStartObject()
end

---@param name : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WritePropertyName(name)
end

function CS.Newtonsoft.Json.Bson.BsonWriter:Close()
end

function CS.Newtonsoft.Json.Bson.BsonWriter:WriteNull()
end

function CS.Newtonsoft.Json.Bson.BsonWriter:WriteUndefined()
end

---@param value : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Int32
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt32
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Int64
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt64
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Single
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Double
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Boolean
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Int16
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.UInt16
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Char
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Byte
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.SByte
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Decimal
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.DateTime
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.DateTimeOffset
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Byte[]
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Guid
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.TimeSpan
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Uri
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteValue(value)
end

---@param value : CS.System.Byte[]
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteObjectId(value)
end

---@param pattern : CS.System.String
---@param options : CS.System.String
function CS.Newtonsoft.Json.Bson.BsonWriter:WriteRegex(pattern, options)
end