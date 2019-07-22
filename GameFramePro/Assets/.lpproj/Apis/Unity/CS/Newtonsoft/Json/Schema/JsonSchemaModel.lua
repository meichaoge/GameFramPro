---@class CS.Newtonsoft.Json.Schema.JsonSchemaModel : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchemaModel = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Required : CS.System.Boolean
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Required = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Type : CS.Newtonsoft.Json.Schema.JsonSchemaType
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Type = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.MinimumLength : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.MinimumLength = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.MaximumLength : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.MaximumLength = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.DivisibleBy : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.DivisibleBy = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Minimum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Minimum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Maximum : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Maximum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.ExclusiveMinimum : CS.System.Boolean
CS.Newtonsoft.Json.Schema.JsonSchemaModel.ExclusiveMinimum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.ExclusiveMaximum : CS.System.Boolean
CS.Newtonsoft.Json.Schema.JsonSchemaModel.ExclusiveMaximum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.MinimumItems : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.MinimumItems = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.MaximumItems : CS.System.Nullable
CS.Newtonsoft.Json.Schema.JsonSchemaModel.MaximumItems = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Patterns : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Patterns = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Items : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Items = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Properties : CS.System.Collections.Generic.IDictionary
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Properties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.PatternProperties : CS.System.Collections.Generic.IDictionary
CS.Newtonsoft.Json.Schema.JsonSchemaModel.PatternProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.AdditionalProperties : CS.Newtonsoft.Json.Schema.JsonSchemaModel
CS.Newtonsoft.Json.Schema.JsonSchemaModel.AdditionalProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.AllowAdditionalProperties : CS.System.Boolean
CS.Newtonsoft.Json.Schema.JsonSchemaModel.AllowAdditionalProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Enum : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Enum = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaModel.Disallow : CS.Newtonsoft.Json.Schema.JsonSchemaType
CS.Newtonsoft.Json.Schema.JsonSchemaModel.Disallow = nil

---@return CS.Newtonsoft.Json.Schema.JsonSchemaModel
function CS.Newtonsoft.Json.Schema.JsonSchemaModel()
end

---@param schemata : CS.System.Collections.Generic.IList
---@return CS.Newtonsoft.Json.Schema.JsonSchemaModel
function CS.Newtonsoft.Json.Schema.JsonSchemaModel.Create(schemata)
end