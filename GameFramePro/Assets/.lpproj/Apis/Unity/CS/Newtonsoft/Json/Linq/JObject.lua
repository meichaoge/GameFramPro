---@class CS.Newtonsoft.Json.Linq.JObject : CS.Newtonsoft.Json.Linq.JContainer
CS.Newtonsoft.Json.Linq.JObject = {}

---@property readonly CS.Newtonsoft.Json.Linq.JObject.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JObject.Type = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JObject.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JObject.Item = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JObject.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JObject.Item = nil

---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject()
end

---@param other : CS.Newtonsoft.Json.Linq.JObject
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject(other)
end

---@param content : CS.System.Object[]
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject(content)
end

---@param content : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject(content)
end

---@param value : CS.System.ComponentModel.PropertyChangedEventHandler
function CS.Newtonsoft.Json.Linq.JObject:add_PropertyChanged(value)
end

---@param value : CS.System.ComponentModel.PropertyChangedEventHandler
function CS.Newtonsoft.Json.Linq.JObject:remove_PropertyChanged(value)
end

---@param value : CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler
function CS.Newtonsoft.Json.Linq.JObject:add_PropertyChanging(value)
end

---@param value : CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler
function CS.Newtonsoft.Json.Linq.JObject:remove_PropertyChanging(value)
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Linq.JObject:Properties()
end

---@param name : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JProperty
function CS.Newtonsoft.Json.Linq.JObject:Property(name)
end

---@return CS.Newtonsoft.Json.Linq.JEnumerable
function CS.Newtonsoft.Json.Linq.JObject:PropertyValues()
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject.Load(reader)
end

---@param json : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject.Parse(json)
end

---@param o : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject.FromObject(o)
end

---@param o : CS.System.Object
---@param jsonSerializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.Newtonsoft.Json.Linq.JObject
function CS.Newtonsoft.Json.Linq.JObject.FromObject(o, jsonSerializer)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JObject:WriteTo(writer, converters)
end

---@param propertyName : CS.System.String
---@param value : CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JObject:Add(propertyName, value)
end

---@param propertyName : CS.System.String
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JObject:Remove(propertyName)
end

---@param propertyName : CS.System.String
---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JObject:TryGetValue(propertyName, value)
end

---@return CS.System.Collections.Generic.IEnumerator
function CS.Newtonsoft.Json.Linq.JObject:GetEnumerator()
end