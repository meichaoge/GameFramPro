---@module CS.JsonDotNet.Extras.CustomConverters
CS.JsonDotNet.Extras.CustomConverters = {}

---@class CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter : CS.Newtonsoft.Json.JsonConverter
CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter = {}

---@property readonly CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter.CanRead : CS.System.Boolean
CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter.CanRead = nil

---@return CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter
function CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param objectType : CS.System.Type
---@return CS.System.Boolean
function CS.JsonDotNet.Extras.CustomConverters.Matrix4x4Converter:CanConvert(objectType)
end