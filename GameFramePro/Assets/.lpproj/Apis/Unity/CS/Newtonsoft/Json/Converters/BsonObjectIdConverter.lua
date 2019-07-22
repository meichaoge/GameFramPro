---@module CS.Newtonsoft.Json.Converters
CS.Newtonsoft.Json.Converters = {}

---@class CS.Newtonsoft.Json.Converters.BsonObjectIdConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Converters.BsonObjectIdConverter = {}

---@return CS.Newtonsoft.Json.Converters.BsonObjectIdConverter
function CS.Newtonsoft.Json.Converters.BsonObjectIdConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.BsonObjectIdConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.BsonObjectIdConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Converters.BsonObjectIdConverter:CanConvert(objectType)
end