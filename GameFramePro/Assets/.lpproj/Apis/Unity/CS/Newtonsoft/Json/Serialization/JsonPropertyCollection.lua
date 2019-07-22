---@class CS.Newtonsoft.Json.Serialization.JsonPropertyCollection : CS.System.Collections.ObjectModel.KeyedCollection
CS.Newtonsoft.Json.Serialization.JsonPropertyCollection = {}

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.Serialization.JsonPropertyCollection
function CS.Newtonsoft.Json.Serialization.JsonPropertyCollection(type)
end

---@param property : CS.Newtonsoft.Json.Serialization.JsonProperty
function CS.Newtonsoft.Json.Serialization.JsonPropertyCollection:AddProperty(property)
end

---@param propertyName : CS.System.String
---@return CS.Newtonsoft.Json.Serialization.JsonProperty
function CS.Newtonsoft.Json.Serialization.JsonPropertyCollection:GetClosestMatchProperty(propertyName)
end

---@param propertyName : CS.System.String
---@param comparisonType : CS.System.StringComparison
---@return CS.Newtonsoft.Json.Serialization.JsonProperty
function CS.Newtonsoft.Json.Serialization.JsonPropertyCollection:GetProperty(propertyName, comparisonType)
end