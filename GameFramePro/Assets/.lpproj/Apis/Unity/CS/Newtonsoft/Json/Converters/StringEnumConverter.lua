---@class CS.Newtonsoft.Json.Converters.StringEnumConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Converters.StringEnumConverter = {}

---@property readwrite CS.Newtonsoft.Json.Converters.StringEnumConverter.CamelCaseText : CS.System.Boolean
CS.Newtonsoft.Json.Converters.StringEnumConverter.CamelCaseText = nil

---@return CS.Newtonsoft.Json.Converters.StringEnumConverter
function CS.Newtonsoft.Json.Converters.StringEnumConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.StringEnumConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.StringEnumConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Converters.StringEnumConverter:CanConvert(objectType)
end