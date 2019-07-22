---@class CS.Newtonsoft.Json.Schema.JsonSchemaNode : CS.System.Object
CS.Newtonsoft.Json.Schema.JsonSchemaNode = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.Id : CS.System.String
CS.Newtonsoft.Json.Schema.JsonSchemaNode.Id = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.Schemas : CS.System.Collections.ObjectModel.ReadOnlyCollection
CS.Newtonsoft.Json.Schema.JsonSchemaNode.Schemas = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.Properties : CS.System.Collections.Generic.Dictionary
CS.Newtonsoft.Json.Schema.JsonSchemaNode.Properties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.PatternProperties : CS.System.Collections.Generic.Dictionary
CS.Newtonsoft.Json.Schema.JsonSchemaNode.PatternProperties = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.Items : CS.System.Collections.Generic.List
CS.Newtonsoft.Json.Schema.JsonSchemaNode.Items = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaNode.AdditionalProperties : CS.Newtonsoft.Json.Schema.JsonSchemaNode
CS.Newtonsoft.Json.Schema.JsonSchemaNode.AdditionalProperties = nil

---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.Newtonsoft.Json.Schema.JsonSchemaNode
function CS.Newtonsoft.Json.Schema.JsonSchemaNode(schema)
end

---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.Newtonsoft.Json.Schema.JsonSchemaNode
function CS.Newtonsoft.Json.Schema.JsonSchemaNode:Combine(schema)
end

---@param schemata : CS.System.Collections.Generic.IEnumerable
---@return CS.System.String
function CS.Newtonsoft.Json.Schema.JsonSchemaNode.GetId(schemata)
end