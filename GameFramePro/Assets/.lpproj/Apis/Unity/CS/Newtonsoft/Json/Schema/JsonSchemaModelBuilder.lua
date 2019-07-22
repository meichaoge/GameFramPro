---@class CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder = {}

---@return CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder()
end

---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.Newtonsoft.Json.Schema.JsonSchemaModel
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:Build(schema)
end

---@param existingNode : CS.Newtonsoft.Json.Schema.JsonSchemaNode
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.Newtonsoft.Json.Schema.JsonSchemaNode
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:AddSchema(existingNode, schema)
end

---@param source : CS.System.Collections.Generic.IDictionary
---@param target : CS.System.Collections.Generic.IDictionary
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:AddProperties(source, target)
end

---@param target : CS.System.Collections.Generic.IDictionary
---@param propertyName : CS.System.String
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:AddProperty(target, propertyName, schema)
end

---@param parentNode : CS.Newtonsoft.Json.Schema.JsonSchemaNode
---@param index : CS.System.Int32
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:AddItem(parentNode, index, schema)
end

---@param parentNode : CS.Newtonsoft.Json.Schema.JsonSchemaNode
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
function CS.Newtonsoft.Json.Schema.JsonSchemaModelBuilder:AddAdditionalProperties(parentNode, schema)
end