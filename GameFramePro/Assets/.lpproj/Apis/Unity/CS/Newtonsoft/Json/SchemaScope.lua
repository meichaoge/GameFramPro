---@class CS.Newtonsoft.Json.SchemaScope : CS.System.Object
CS.Newtonsoft.Json.SchemaScope = {}

---@property readwrite CS.Newtonsoft.Json.SchemaScope.CurrentPropertyName : CS.System.String
CS.Newtonsoft.Json.SchemaScope.CurrentPropertyName = nil

---@property readwrite CS.Newtonsoft.Json.SchemaScope.ArrayItemCount : CS.System.Int32
CS.Newtonsoft.Json.SchemaScope.ArrayItemCount = nil

---@property readonly CS.Newtonsoft.Json.SchemaScope.Schemas : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.SchemaScope.Schemas = nil

---@property readonly CS.Newtonsoft.Json.SchemaScope.RequiredProperties : CS.System.Collections.Generic.Dictionary
CS.Newtonsoft.Json.SchemaScope.RequiredProperties = nil

---@property readonly CS.Newtonsoft.Json.SchemaScope.TokenType : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.SchemaScope.TokenType = nil

---@param tokenType : CS.Newtonsoft.Json.Linq.JTokenType
---@param schemas : CS.System.Collections.Generic.IList
---@return CS.Newtonsoft.Json.SchemaScope
function CS.Newtonsoft.Json.SchemaScope(tokenType, schemas)
end