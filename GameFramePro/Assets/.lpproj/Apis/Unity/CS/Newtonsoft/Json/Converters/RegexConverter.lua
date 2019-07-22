---@class CS.Newtonsoft.Json.Converters.RegexConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Converters.RegexConverter = {}

---@return CS.Newtonsoft.Json.Converters.RegexConverter
function CS.Newtonsoft.Json.Converters.RegexConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.RegexConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.RegexConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Converters.RegexConverter:CanConvert(objectType)
end