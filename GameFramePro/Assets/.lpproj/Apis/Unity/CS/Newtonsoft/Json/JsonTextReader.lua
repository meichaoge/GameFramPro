---@class CS.Newtonsoft.Json.JsonTextReader : CS.Newtonsoft.Json.JsonReader
CS.Newtonsoft.Json.JsonTextReader = {}

---@property readwrite CS.Newtonsoft.Json.JsonTextReader.Culture : CS.System.Globalization.CultureInfo
CS.Newtonsoft.Json.JsonTextReader.Culture = nil

---@property readonly CS.Newtonsoft.Json.JsonTextReader.LineNumber : CS.System.Int32
CS.Newtonsoft.Json.JsonTextReader.LineNumber = nil

---@property readonly CS.Newtonsoft.Json.JsonTextReader.LinePosition : CS.System.Int32
CS.Newtonsoft.Json.JsonTextReader.LinePosition = nil

---@param reader : CS.System.IO.TextReader
---@return CS.Newtonsoft.Json.JsonTextReader
function CS.Newtonsoft.Json.JsonTextReader(reader)
end

---@return CS.System.Boolean
function CS.Newtonsoft.Json.JsonTextReader:Read()
end

---@return CS.System.Byte[]
function CS.Newtonsoft.Json.JsonTextReader:ReadAsBytes()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonTextReader:ReadAsDecimal()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.JsonTextReader:ReadAsDateTimeOffset()
end

function CS.Newtonsoft.Json.JsonTextReader:Close()
end

---@return CS.System.Boolean
function CS.Newtonsoft.Json.JsonTextReader:HasLineInfo()
end