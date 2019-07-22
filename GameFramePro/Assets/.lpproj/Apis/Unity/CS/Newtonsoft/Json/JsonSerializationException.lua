---@class CS.Newtonsoft.Json.JsonSerializationException : CS.System.Exception
CS.Newtonsoft.Json.JsonSerializationException = {}

---@return CS.Newtonsoft.Json.JsonSerializationException
function CS.Newtonsoft.Json.JsonSerializationException()
end

---@param message : CS.System.String
---@return CS.Newtonsoft.Json.JsonSerializationException
function CS.Newtonsoft.Json.JsonSerializationException(message)
end

---@param message : CS.System.String
---@param innerException : CS.System.Exception
---@return CS.Newtonsoft.Json.JsonSerializationException
function CS.Newtonsoft.Json.JsonSerializationException(message, innerException)
end

---@param info : CS.System.Runtime.Serialization.SerializationInfo
---@param context : CS.System.Runtime.Serialization.StreamingContext
---@return CS.Newtonsoft.Json.JsonSerializationException
function CS.Newtonsoft.Json.JsonSerializationException(info, context)
end