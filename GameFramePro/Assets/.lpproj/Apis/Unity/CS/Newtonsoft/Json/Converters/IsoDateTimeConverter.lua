---@class CS.Newtonsoft.Json.Converters.IsoDateTimeConverter : CS.Newtonsoft.Json.Converters.DateTimeConverterBase
CS.Newtonsoft.Json.Converters.IsoDateTimeConverter = {}

---@property readwrite CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.DateTimeStyles : CS.System.Globalization.DateTimeStyles
CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.DateTimeStyles = nil

---@property readwrite CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.DateTimeFormat : CS.System.String
CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.DateTimeFormat = nil

---@property readwrite CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.Culture : CS.System.Globalization.CultureInfo
CS.Newtonsoft.Json.Converters.IsoDateTimeConverter.Culture = nil

---@return CS.Newtonsoft.Json.Converters.IsoDateTimeConverter
function CS.Newtonsoft.Json.Converters.IsoDateTimeConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.IsoDateTimeConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.IsoDateTimeConverter:ReadJson(reader, objectType, existingValue, serializer)
end