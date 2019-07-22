---@class CS.Newtonsoft.Json.Linq.JTokenReader : CS.Newtonsoft.Json.JsonReader
CS.Newtonsoft.Json.Linq.JTokenReader = {}

---@param token : CS.Newtonsoft.Json.Linq.JToken
---@return CS.Newtonsoft.Json.Linq.JTokenReader
function CS.Newtonsoft.Json.Linq.JTokenReader(token)
end

---@return CS.System.Byte[]
function CS.Newtonsoft.Json.Linq.JTokenReader:ReadAsBytes()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JTokenReader:ReadAsDecimal()
end

---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JTokenReader:ReadAsDateTimeOffset()
end

---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JTokenReader:Read()
end