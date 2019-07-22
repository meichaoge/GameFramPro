---@class CS.Newtonsoft.Json.Bson.BsonReader : CS.Newtonsoft.Json.JsonReader
CS.Newtonsoft.Json.Bson.BsonReader = {}

---@property readwrite CS.Newtonsoft.Json.Bson.BsonReader.JsonNet35BinaryCompatibility : CS.System.Boolean
CS.Newtonsoft.Json.Bson.BsonReader.JsonNet35BinaryCompatibility = nil

---@property readwrite CS.Newtonsoft.Json.Bson.BsonReader.ReadRootValueAsArray : CS.System.Boolean
CS.Newtonsoft.Json.Bson.BsonReader.ReadRootValueAsArray = nil

---@property readwrite CS.Newtonsoft.Json.Bson.BsonReader.DateTimeKindHandling : CS.System.DateTimeKind
CS.Newtonsoft.Json.Bson.BsonReader.DateTimeKindHandling = nil

---@param stream : CS.System.IO.Stream
---@return CS.Newtonsoft.Json.Bson.BsonReader
function CS.Newtonsoft.Json.Bson.BsonReader(stream)
end

---@param stream : CS.System.IO.Stream
---@param readRootValueAsArray : CS.System.Boolean
---@param dateTimeKindHandling : CS.System.DateTimeKind
---@return CS.Newtonsoft.Json.Bson.BsonReader
function CS.Newtonsoft.Json.Bson.BsonReader(stream, readRootValueAsArray, dateTimeKindHandling)
end

---@return CS.System.Byte[]
function CS.Newtonsoft.Json.Bson.BsonReader:ReadAsBytes()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.Bson.BsonReader:ReadAsDecimal()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.Bson.BsonReader:ReadAsDateTimeOffset()
end

---@return CS.System.Boolean
function CS.Newtonsoft.Json.Bson.BsonReader:Read()
end

function CS.Newtonsoft.Json.Bson.BsonReader:Close()
end