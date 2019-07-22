---@class CS.Newtonsoft.Json.Converters.KeyValuePairConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Converters.KeyValuePairConverter = {}

---@return CS.Newtonsoft.Json.Converters.KeyValuePairConverter
function CS.Newtonsoft.Json.Converters.KeyValuePairConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.KeyValuePairConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.KeyValuePairConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Converters.KeyValuePairConverter:CanConvert(objectType)
end