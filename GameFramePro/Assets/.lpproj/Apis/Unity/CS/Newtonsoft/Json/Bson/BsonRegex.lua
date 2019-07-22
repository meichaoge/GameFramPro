---@class CS.Newtonsoft.Json.Bson.BsonRegex : CS.Newtonsoft.Json.Bson.BsonToken
CS.Newtonsoft.Json.Bson.BsonRegex = {}

---@property readwrite CS.Newtonsoft.Json.Bson.BsonRegex.Pattern : CS.Newtonsoft.Json.Bson.BsonString
CS.Newtonsoft.Json.Bson.BsonRegex.Pattern = nil

---@property readwrite CS.Newtonsoft.Json.Bson.BsonRegex.Options : CS.Newtonsoft.Json.Bson.BsonString
CS.Newtonsoft.Json.Bson.BsonRegex.Options = nil

---@property readonly CS.Newtonsoft.Json.Bson.BsonRegex.Type : CS.Newtonsoft.Json.Bson.BsonType
CS.Newtonsoft.Json.Bson.BsonRegex.Type = nil

---@param pattern : CS.System.String
---@param options : CS.System.String
---@return CS.Newtonsoft.Json.Bson.BsonRegex
function CS.Newtonsoft.Json.Bson.BsonRegex(pattern, options)
end