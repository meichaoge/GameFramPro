---@class CS.Newtonsoft.Json.JsonPropertyAttribute : CS.System.Attribute
CS.Newtonsoft.Json.JsonPropertyAttribute = {}

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.NullValueHandling : CS.Newtonsoft.Json.NullValueHandling
CS.Newtonsoft.Json.JsonPropertyAttribute.NullValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.DefaultValueHandling : CS.Newtonsoft.Json.DefaultValueHandling
CS.Newtonsoft.Json.JsonPropertyAttribute.DefaultValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.ReferenceLoopHandling : CS.Newtonsoft.Json.ReferenceLoopHandling
CS.Newtonsoft.Json.JsonPropertyAttribute.ReferenceLoopHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.ObjectCreationHandling : CS.Newtonsoft.Json.ObjectCreationHandling
CS.Newtonsoft.Json.JsonPropertyAttribute.ObjectCreationHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.TypeNameHandling : CS.Newtonsoft.Json.TypeNameHandling
CS.Newtonsoft.Json.JsonPropertyAttribute.TypeNameHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.IsReference : CS.System.Boolean
CS.Newtonsoft.Json.JsonPropertyAttribute.IsReference = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.Order : CS.System.Int32
CS.Newtonsoft.Json.JsonPropertyAttribute.Order = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.PropertyName : CS.System.String
CS.Newtonsoft.Json.JsonPropertyAttribute.PropertyName = nil

---@property readwrite CS.Newtonsoft.Json.JsonPropertyAttribute.Required : CS.Newtonsoft.Json.Required
CS.Newtonsoft.Json.JsonPropertyAttribute.Required = nil

---@return CS.Newtonsoft.Json.JsonPropertyAttribute
function CS.Newtonsoft.Json.JsonPropertyAttribute()
end

---@param propertyName : CS.System.String
---@return CS.Newtonsoft.Json.JsonPropertyAttribute
function CS.Newtonsoft.Json.JsonPropertyAttribute(propertyName)
end