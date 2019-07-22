---@class CS.Newtonsoft.Json.Linq.JConstructor : CS.Newtonsoft.Json.Linq.JContainer
CS.Newtonsoft.Json.Linq.JConstructor = {}

---@property readwrite CS.Newtonsoft.Json.Linq.JConstructor.Name : CS.System.String
CS.Newtonsoft.Json.Linq.JConstructor.Name = nil

---@property readonly CS.Newtonsoft.Json.Linq.JConstructor.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JConstructor.Type = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JConstructor.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JConstructor.Item = nil

---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor()
end

---@param other : CS.Newtonsoft.Json.Linq.JConstructor
---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor(other)
end

---@param name : CS.System.String
---@param content : CS.System.Object[]
---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor(name, content)
end

---@param name : CS.System.String
---@param content : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor(name, content)
end

---@param name : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor(name)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JConstructor:WriteTo(writer, converters)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JConstructor
function CS.Newtonsoft.Json.Linq.JConstructor.Load(reader)
end