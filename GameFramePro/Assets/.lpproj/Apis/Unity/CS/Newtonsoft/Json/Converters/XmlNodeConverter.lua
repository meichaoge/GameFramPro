---@class CS.Newtonsoft.Json.Converters.XmlNodeConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Converters.XmlNodeConverter = {}

---@property readwrite CS.Newtonsoft.Json.Converters.XmlNodeConverter.DeserializeRootElementName : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeConverter.DeserializeRootElementName = nil

---@property readwrite CS.Newtonsoft.Json.Converters.XmlNodeConverter.WriteArrayAttribute : CS.System.Boolean
CS.Newtonsoft.Json.Converters.XmlNodeConverter.WriteArrayAttribute = nil

---@property readwrite CS.Newtonsoft.Json.Converters.XmlNodeConverter.OmitRootObject : CS.System.Boolean
CS.Newtonsoft.Json.Converters.XmlNodeConverter.OmitRootObject = nil

---@return CS.Newtonsoft.Json.Converters.XmlNodeConverter
function CS.Newtonsoft.Json.Converters.XmlNodeConverter()
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.Converters.XmlNodeConverter:WriteJson(writer, value, serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@param existingValue : CS.System.Object
---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.System.Object
function CS.Newtonsoft.Json.Converters.XmlNodeConverter:ReadJson(reader, objectType, existingValue, serializer)
end

---@param valueType : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Converters.XmlNodeConverter:CanConvert(valueType)
end