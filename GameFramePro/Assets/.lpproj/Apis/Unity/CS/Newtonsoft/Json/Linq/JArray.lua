---@module CS.Newtonsoft.Json.Linq
CS.Newtonsoft.Json.Linq = {}

---@class CS.Newtonsoft.Json.Linq.JArray : CS.Newtonsoft.Json.Linq.JContainer
CS.Newtonsoft.Json.Linq.JArray = {}

---@property readonly CS.Newtonsoft.Json.Linq.JArray.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JArray.Type = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JArray.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JArray.Item = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JArray.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JArray.Item = nil

---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray()
end

---@param other : CS.Newtonsoft.Json.Linq.JArray
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray(other)
end

---@param content : CS.System.Object[]
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray(content)
end

---@param content : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray(content)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray.Load(reader)
end

---@param json : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray.Parse(json)
end

---@param o : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray.FromObject(o)
end

---@param o : CS.System.Object
---@param jsonSerializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.Newtonsoft.Json.Linq.JArray
function CS.Newtonsoft.Json.Linq.JArray.FromObject(o, jsonSerializer)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JArray:WriteTo(writer, converters)
end

---@param item : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Int32
function CS.Newtonsoft.Json.Linq.JArray:IndexOf(item)
end

---@param index : CS.System.Int32
---@param item : CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JArray:Insert(index, item)
end

---@param index : CS.System.Int32
function CS.Newtonsoft.Json.Linq.JArray:RemoveAt(index)
end

---@param item : CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JArray:Add(item)
end

function CS.Newtonsoft.Json.Linq.JArray:Clear()
end

---@param item : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JArray:Contains(item)
end

---@param item : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JArray:Remove(item)
end