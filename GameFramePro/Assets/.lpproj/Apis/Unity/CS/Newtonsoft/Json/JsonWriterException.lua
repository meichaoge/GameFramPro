---@class CS.Newtonsoft.Json.JsonWriterException : CS.System.Exception
CS.Newtonsoft.Json.JsonWriterException = {}

---@return CS.Newtonsoft.Json.JsonWriterException
function CS.Newtonsoft.Json.JsonWriterException()
end

---@param message : CS.System.String
---@return CS.Newtonsoft.Json.JsonWriterException
function CS.Newtonsoft.Json.JsonWriterException(message)
end

---@param message : CS.System.String
---@param innerException : CS.System.Exception
---@return CS.Newtonsoft.Json.JsonWriterException
function CS.Newtonsoft.Json.JsonWriterException(message, innerException)
end

---@param info : CS.System.Runtime.Serialization.SerializationInfo
---@param context : CS.System.Runtime.Serialization.StreamingContext
---@return CS.Newtonsoft.Json.JsonWriterException
function CS.Newtonsoft.Json.JsonWriterException(info, context)
end