---@class CS.Newtonsoft.Json.JsonValidatingReader : CS.Newtonsoft.Json.JsonReader
CS.Newtonsoft.Json.JsonValidatingReader = {}

---@property readonly CS.Newtonsoft.Json.JsonValidatingReader.Value : CS.System.Object
CS.Newtonsoft.Json.JsonValidatingReader.Value = nil

---@property readonly CS.Newtonsoft.Json.JsonValidatingReader.Depth : CS.System.Int32
CS.Newtonsoft.Json.JsonValidatingReader.Depth = nil

---@property readwrite CS.Newtonsoft.Json.JsonValidatingReader.QuoteChar : CS.System.Char
CS.Newtonsoft.Json.JsonValidatingReader.QuoteChar = nil

---@property readonly CS.Newtonsoft.Json.JsonValidatingReader.TokenType : CS.Newtonsoft.Json.JsonToken
CS.Newtonsoft.Json.JsonValidatingReader.TokenType = nil

---@property readonly CS.Newtonsoft.Json.JsonValidatingReader.ValueType : CS.System.Type
CS.Newtonsoft.Json.JsonValidatingReader.ValueType = nil

---@property readwrite CS.Newtonsoft.Json.JsonValidatingReader.Schema : CS.Newtonsoft.Json.Schema.JsonSchema
CS.Newtonsoft.Json.JsonValidatingReader.Schema = nil

---@property readonly CS.Newtonsoft.Json.JsonValidatingReader.Reader : CS.Newtonsoft.Json.JsonReader
CS.Newtonsoft.Json.JsonValidatingReader.Reader = nil

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.JsonValidatingReader
function CS.Newtonsoft.Json.JsonValidatingReader(reader)
end

---@param value : CS.Newtonsoft.Json.Schema.ValidationEventHandler
function CS.Newtonsoft.Json.JsonValidatingReader:add_ValidationEventHandler(value)
end

---@param value : CS.Newtonsoft.Json.Schema.ValidationEventHandler
function CS.Newtonsoft.Json.JsonValidatingReader:remove_ValidationEventHandler(value)
end

---@return CS.System.Byte[]
function CS.Newtonsoft.Json.JsonValidatingReader:ReadAsBytes()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonValidatingReader:ReadAsDecimal()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonValidatingReader:ReadAsDateTimeOffset()
end

---@return CS.System.Boolean
function CS.Newtonsoft.Json.JsonValidatingReader:Read()
end