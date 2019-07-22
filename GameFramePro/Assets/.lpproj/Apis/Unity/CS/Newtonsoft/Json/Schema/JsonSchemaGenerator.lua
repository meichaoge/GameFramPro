---@class CS.Newtonsoft.Json.Schema.JsonSchemaGenerator : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchemaGenerator = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaGenerator.UndefinedSchemaIdHandling : CS.Newtonsoft.Json.Schema.UndefinedSchemaIdHandling
CS.Newtonsoft.Json.Schema.JsonSchemaGenerator.UndefinedSchemaIdHandling = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaGenerator.ContractResolver : CS.Newtonsoft.Json.Serialization.IContractResolver
CS.Newtonsoft.Json.Schema.JsonSchemaGenerator.ContractResolver = nil

---@return CS.Newtonsoft.Json.Schema.JsonSchemaGenerator
function CS.Newtonsoft.Json.Schema.JsonSchemaGenerator()
end

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaGenerator:Generate(type)
end

---@param type : CS.System.Type
---@param resolver : CS.Newtonsoft.Json.Schema.JsonSchemaResolver
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaGenerator:Generate(type, resolver)
end

---@param type : CS.System.Type
---@param rootSchemaNullable : CS.System.Boolean
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaGenerator:Generate(type, rootSchemaNullable)
end

---@param type : CS.System.Type
---@param resolver : CS.Newtonsoft.Json.Schema.JsonSchemaResolver
---@param rootSchemaNullable : CS.System.Boolean
---@return CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaGenerator:Generate(type, resolver, rootSchemaNullable)
end