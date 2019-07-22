---@class CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader : CS.Newtonsoft.Json.Serialization.JsonSerializerInternalBase
CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader = {}

---@param serializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader
function CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader(serializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param target : CS.System.Object
function CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader:Populate(reader, target)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader:Deserialize(reader, objectType)
end