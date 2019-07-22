---@class CS.Newtonsoft.Json.Serialization.JsonProperty : CS.System.Object
CS.Newtonsoft.Json.Serialization.JsonProperty = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.PropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonProperty.PropertyName = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Order : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.Order = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.UnderlyingName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonProperty.UnderlyingName = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.ValueProvider : CS.Newtonsoft.Json.Serialization.IValueProvider
CS.Newtonsoft.Json.Serialization.JsonProperty.ValueProvider = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.PropertyType : CS.System.Type
CS.Newtonsoft.Json.Serialization.JsonProperty.PropertyType = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Converter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Serialization.JsonProperty.Converter = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.MemberConverter : CS.Newtonsoft.Json.JsonConverter
CS.Newtonsoft.Json.Serialization.JsonProperty.MemberConverter = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Ignored : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.JsonProperty.Ignored = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Readable : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.JsonProperty.Readable = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Writable : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.JsonProperty.Writable = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.DefaultValue : CS.System.Object
CS.Newtonsoft.Json.Serialization.JsonProperty.DefaultValue = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.Required : CS.Newtonsoft.Json.Required
CS.Newtonsoft.Json.Serialization.JsonProperty.Required = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.IsReference : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.IsReference = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.NullValueHandling : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.NullValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.DefaultValueHandling : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.DefaultValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.ReferenceLoopHandling : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.ReferenceLoopHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.ObjectCreationHandling : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.ObjectCreationHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.TypeNameHandling : CS.System.Nullable
CS.Newtonsoft.Json.Serialization.JsonProperty.TypeNameHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.ShouldSerialize : CS.System.Predicate
CS.Newtonsoft.Json.Serialization.JsonProperty.ShouldSerialize = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.GetIsSpecified : CS.System.Predicate
CS.Newtonsoft.Json.Serialization.JsonProperty.GetIsSpecified = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonProperty.SetIsSpecified : CS.System.Action
CS.Newtonsoft.Json.Serialization.JsonProperty.SetIsSpecified = nil

---@return CS.Newtonsoft.Json.Serialization.JsonProperty
function CS.Newtonsoft.Json.Serialization.JsonProperty()
end

---@return CS.System.String
function CS.Newtonsoft.Json.Serialization.JsonProperty:ToString()
end