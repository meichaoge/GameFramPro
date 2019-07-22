---@class CS.LitJson.JsonException : CS.System.Exception
CS.LitJson.JsonException = {}

---@return CS.LitJson.JsonException
function CS.LitJson.JsonException()
end

---@param message : CS.System.String
---@return CS.LitJson.JsonException
function CS.LitJson.JsonException(message)
end

---@param message : CS.System.String
---@param inner_exception : CS.System.Exception
---@return CS.LitJson.JsonException
function CS.LitJson.JsonException(message, inner_exception)
end