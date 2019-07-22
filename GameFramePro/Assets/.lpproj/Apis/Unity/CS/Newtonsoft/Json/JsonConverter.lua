---@class CS.Newtonsoft.Json.JsonConverter : CS.System.Object
CS.Newtonsoft.Json.JsonConverter = {}

---@property readonly CS.Newtonsoft.Json.JsonConverter.CanRead : CS.System.Boolean
CS.Newtonsoft.Json.JsonConverter.CanRead = nil

---@property readonly CS.Newtonsoft.Json.JsonConverter.CanWrite : CS.System.Boolean
CS.Newtonsoft.Json.JsonConverter.CanWrite = nil

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.JsonConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.JsonConverter:CanConvert(objectType)
end

---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.JsonConverter:GetSchema()
end