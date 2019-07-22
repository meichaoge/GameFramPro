---@class CS.Newtonsoft.Json.JsonReader : CS.System.Object
CS.Newtonsoft.Json.JsonReader = {}

---@property readwrite CS.Newtonsoft.Json.JsonReader.CloseInput : CS.System.Boolean
CS.Newtonsoft.Json.JsonReader.CloseInput = nil

---@property readwrite CS.Newtonsoft.Json.JsonReader.QuoteChar : CS.System.Char
CS.Newtonsoft.Json.JsonReader.QuoteChar = nil

---@property readonly CS.Newtonsoft.Json.JsonReader.TokenType : CS.Newtonsoft.Json.JsonToken
CS.Newtonsoft.Json.JsonReader.TokenType = nil

---@property readonly CS.Newtonsoft.Json.JsonReader.Value : CS.System.Object
CS.Newtonsoft.Json.JsonReader.Value = nil

---@property readonly CS.Newtonsoft.Json.JsonReader.ValueType : CS.System.Type
CS.Newtonsoft.Json.JsonReader.ValueType = nil

---@property readonly CS.Newtonsoft.Json.JsonReader.Depth : CS.System.Int32
CS.Newtonsoft.Json.JsonReader.Depth = nil

---@return CS.System.Boolean
function CS.Newtonsoft.Json.JsonReader:Read()
end

---@return CS.System.Byte[]
function CS.Newtonsoft.Json.JsonReader:ReadAsBytes()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonReader:ReadAsDecimal()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonReader:ReadAsDateTimeOffset()
end

function CS.Newtonsoft.Json.JsonReader:Skip()
end

function CS.Newtonsoft.Json.JsonReader:Close()
end