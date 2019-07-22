---@class CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter : CS.Newtonsoft.Json.Converters.DateTimeConverterBase
CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter = {}

---@return CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter
function CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.JavaScriptDateTimeConverter:ReadJson(reader, objectType, existingValue, serializer)
end