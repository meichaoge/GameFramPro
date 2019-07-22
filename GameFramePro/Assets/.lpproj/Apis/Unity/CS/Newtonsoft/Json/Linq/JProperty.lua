---@class CS.Newtonsoft.Json.Linq.JProperty : CS.Newtonsoft.Json.Linq.JContainer
CS.Newtonsoft.Json.Linq.JProperty = {}

---@property readonly CS.Newtonsoft.Json.Linq.JProperty.Name : CS.System.String
CS.Newtonsoft.Json.Linq.JProperty.Name = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JProperty.Value : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JProperty.Value = nil

---@property readonly CS.Newtonsoft.Json.Linq.JProperty.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JProperty.Type = nil

---@param other : CS.Newtonsoft.Json.Linq.JProperty
---@return CS.Newtonsoft.Json.Linq.JProperty
function CS.Newtonsoft.Json.Linq.JProperty(other)
end

---@param name : CS.System.String
---@param content : CS.System.Object[]
---@return CS.Newtonsoft.Json.Linq.JProperty
function CS.Newtonsoft.Json.Linq.JProperty(name, content)
end

---@param name : CS.System.String
---@param content : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JProperty
function CS.Newtonsoft.Json.Linq.JProperty(name, content)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JProperty:WriteTo(writer, converters)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JProperty
function CS.Newtonsoft.Json.Linq.JProperty.Load(reader)
end