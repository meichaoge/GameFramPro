---@class CS.Newtonsoft.Json.Schema.TypeSchema : CS.System.Object
CS.Newtonsoft.Json.Schema.TypeSchema = {}

---@property readwrite CS.Newtonsoft.Json.Schema.TypeSchema.Type : CS.System.Type
CS.Newtonsoft.Json.Schema.TypeSchema.Type = nil

---@property readwrite CS.Newtonsoft.Json.Schema.TypeSchema.Schema : CS.Newtonsoft.Json.Schema.JsonSchema
CS.Newtonsoft.Json.Schema.TypeSchema.Schema = nil

---@param type : CS.System.Type
---@param schema : CS.Newtonsoft.Json.Schema.JsonSchema
---@return CS.Newtonsoft.Json.Schema.TypeSchema
function CS.Newtonsoft.Json.Schema.TypeSchema(type, schema)
end