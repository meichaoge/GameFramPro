---@class CS.LitJson.JsonMapper : CS.System.Object
CS.LitJson.JsonMapper = {}

---@return CS.LitJson.JsonMapper
function CS.LitJson.JsonMapper()
end

---@param type : CS.System.Type
---@return CS.System.Reflection.PropertyInfo[]
function CS.LitJson.JsonMapper.GetPublicInstanceProperties(type)
end

---@param obj : CS.System.Object
---@return CS.System.String
function CS.LitJson.JsonMapper.ToJson(obj)
end

---@param obj : CS.System.Object
---@param writer : CS.LitJson.JsonWriter
function CS.LitJson.JsonMapper.ToJson(obj, writer)
end

---@param reader : CS.LitJson.JsonReader
---@return CS.LitJson.JsonData
function CS.LitJson.JsonMapper.ToObject(reader)
end

---@param reader : CS.System.IO.TextReader
---@return CS.LitJson.JsonData
function CS.LitJson.JsonMapper.ToObject(reader)
end

---@param json : CS.System.String
---@return CS.LitJson.JsonData
function CS.LitJson.JsonMapper.ToObject(json)
end

---@param toType : CS.System.Type
---@param json : CS.System.String
---@return CS.System.Object
function CS.LitJson.JsonMapper.ToObject(toType, json)
end

---@param factory : CS.LitJson.WrapperFactory
---@param reader : CS.LitJson.JsonReader
---@return CS.LitJson.IJsonWrapper
function CS.LitJson.JsonMapper.ToWrapper(factory, reader)
end

---@param factory : CS.LitJson.WrapperFactory
---@param json : CS.System.String
---@return CS.LitJson.IJsonWrapper
function CS.LitJson.JsonMapper.ToWrapper(factory, json)
end

function CS.LitJson.JsonMapper.UnregisterExporters()
end

function CS.LitJson.JsonMapper.UnregisterImporters()
end