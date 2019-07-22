---@class CS.Newtonsoft.Json.Schema.JsonSchemaResolver : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchemaResolver = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaResolver.LoadedSchemas : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Schema.JsonSchemaResolver.LoadedSchemas = nil

---@return CS.Newtonsoft.Json.Schema.JsonSchemaResolver
function CS.Newtonsoft.Json.Schema.JsonSchemaResolver()
end

---@param id : CS.System.String
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaResolver:GetSchema(id)
end