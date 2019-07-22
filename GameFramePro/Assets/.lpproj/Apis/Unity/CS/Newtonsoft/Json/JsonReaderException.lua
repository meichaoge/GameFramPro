---@class CS.Newtonsoft.Json.JsonReaderException : CS.System.Exception
CS.Newtonsoft.Json.JsonReaderException = {}

---@property readwrite CS.Newtonsoft.Json.JsonReaderException.LineNumber : CS.System.Int32
CS.Newtonsoft.Json.JsonReaderException.LineNumber = nil

---@property readwrite CS.Newtonsoft.Json.JsonReaderException.LinePosition : CS.System.Int32
CS.Newtonsoft.Json.JsonReaderException.LinePosition = nil

---@return CS.Newtonsoft.Json.JsonReaderException
function CS.Newtonsoft.Json.JsonReaderException()
end

---@param message : CS.System.String
---@return CS.Newtonsoft.Json.JsonReaderException
function CS.Newtonsoft.Json.JsonReaderException(message)
end

---@param message : CS.System.String
---@param innerException : CS.System.Exception
---@return CS.Newtonsoft.Json.JsonReaderException
function CS.Newtonsoft.Json.JsonReaderException(message, innerException)
end

---@param info : CS.System.Runtime.Serialization.SerializationInfo
---@param context : CS.System.Runtime.Serialization.StreamingContext
---@return CS.Newtonsoft.Json.JsonReaderException
function CS.Newtonsoft.Json.JsonReaderException(info, context)
end