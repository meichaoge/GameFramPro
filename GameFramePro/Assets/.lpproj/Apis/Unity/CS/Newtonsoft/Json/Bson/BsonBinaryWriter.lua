---@class CS.Newtonsoft.Json.Bson.BsonBinaryWriter : CS.System.Object
CS.Newtonsoft.Json.Bson.BsonBinaryWriter = {}

---@property readwrite CS.Newtonsoft.Json.Bson.BsonBinaryWriter.DateTimeKindHandling : CS.System.DateTimeKind
CS.Newtonsoft.Json.Bson.BsonBinaryWriter.DateTimeKindHandling = nil

---@param stream : CS.System.IO.Stream
---@return CS.Newtonsoft.Json.Bson.BsonBinaryWriter
function CS.Newtonsoft.Json.Bson.BsonBinaryWriter(stream)
end

function CS.Newtonsoft.Json.Bson.BsonBinaryWriter:Flush()
end

function CS.Newtonsoft.Json.Bson.BsonBinaryWriter:Close()
end

---@param t : CS.Newtonsoft.Json.Bson.BsonToken
function CS.Newtonsoft.Json.Bson.BsonBinaryWriter:WriteToken(t)
end