---@module CS.Newtonsoft.Json.Schema
CS.Newtonsoft.Json.Schema = {}

---@class CS.Newtonsoft.Json.Schema.JsonSchema : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchema = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Id : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Id = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Title : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Title = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Required : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Required = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.ReadOnly : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.ReadOnly = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Hidden : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Hidden = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Transient : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Transient = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Description : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Description = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Type : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Type = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Pattern : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Pattern = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.MinimumLength : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.MinimumLength = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.MaximumLength : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.MaximumLength = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.DivisibleBy : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.DivisibleBy = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Minimum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Minimum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Maximum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Maximum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.ExclusiveMinimum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.ExclusiveMinimum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.ExclusiveMaximum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.ExclusiveMaximum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.MinimumItems : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.MinimumItems = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.MaximumItems : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.MaximumItems = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Items : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchema.Items = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Properties : CS.System.Collections.Generic.IDictionary
CS.Newtonsoft.Json.Schema.JsonSchema.Properties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.AdditionalProperties : CS.Newtonsoft.Json.Schema.JsonSchema
CS.Newtonsoft.Json.Schema.JsonSchema.AdditionalProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.PatternProperties : CS.System.Collections.Generic.IDictionary
CS.Newtonsoft.Json.Schema.JsonSchema.PatternProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.AllowAdditionalProperties : CS.System.Boolean
CS.Newtonsoft.Json.Schema.JsonSchema.AllowAdditionalProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Requires : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Requires = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Identity : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchema.Identity = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Enum : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchema.Enum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Options : CS.System.Collections.Generic.IDictionary
CS.Newtonsoft.Json.Schema.JsonSchema.Options = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Disallow : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchema.Disallow = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Default : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Schema.JsonSchema.Default = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Extends : CS.Newtonsoft.Json.Schema.JsonSchema
CS.Newtonsoft.Json.Schema.JsonSchema.Extends = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchema.Format : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchema.Format = nil

---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchema()
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchema.Read(reader)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param resolver : CS.Newtonsoft.Json.Schema.JsonSchemaResolver
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchema.Read(reader, resolver)
end

---@param json : CS.System.String
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchema.Parse(json)
end

---@param json : CS.System.String
---@param resolver : CS.Newtonsoft.Json.Schema.JsonSchemaResolver
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchema.Parse(json, resolver)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
function CS.Newtonsoft.Json.Schema.JsonSchema:WriteTo(writer)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param resolver : CS.Newtonsoft.Json.Schema.JsonSchemaResolver
function CS.Newtonsoft.Json.Schema.JsonSchema:WriteTo(writer, resolver)
end

---@return CS.System.String
function CS.Newtonsoft.Json.Schema.JsonSchema:ToString()
end