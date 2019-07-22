---@class CS.Newtonsoft.Json.Schema.SchemaExtensions : CS.System.Object
CS.Newtonsoft.Json.Schema.SchemaExtensions = {}

---@param source : CS.Newtonsoft.Json.Linq.JToken
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Schema.SchemaExtensions.IsValid(source, schema)
end

---@param source : CS.Newtonsoft.Json.Linq.JToken
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.SchemaExtensions.Validate(source, schema)
end

---@param source : CS.Newtonsoft.Json.Linq.JToken
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@param validationEventHandler : CS.Newtonsoft.Json.Schema.ValidationEventHandler
function CS.Newtonsoft.Json.Schema.SchemaExtensions.Validate(source, schema, validationEventHandler)
end