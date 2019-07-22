---@class CS.Newtonsoft.Json.Schema.JsonSchemaException : CS.System.Exception
CS.Newtonsoft.Json.Schema.JsonSchemaException = {}

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaException.LineNumber : CS.System.Int32
CS.Newtonsoft.Json.Schema.JsonSchemaException.LineNumber = nil

---@property readwrite CS.Newtonsoft.Json.Schema.JsonSchemaException.LinePosition : CS.System.Int32
CS.Newtonsoft.Json.Schema.JsonSchemaException.LinePosition = nil

---@return CS.Newtonsoft.Json.Schema.JsonSchemaException
function CS.Newtonsoft.Json.Schema.JsonSchemaException()
end

---@param message : CS.System.String
---@return CS.Newtonsoft.Json.Schema.JsonSchemaException
function CS.Newtonsoft.Json.Schema.JsonSchemaException(message)
end

---@param message : CS.System.String
---@param innerException : CS.System.Exception
---@return CS.Newtonsoft.Json.Schema.JsonSchemaException
function CS.Newtonsoft.Json.Schema.JsonSchemaException(message, innerException)
end

---@param info : CS.System.Runtime.Serialization.SerializationInfo
---@param context : CS.System.Runtime.Serialization.StreamingContext
---@return CS.Newtonsoft.Json.Schema.JsonSchemaException
function CS.Newtonsoft.Json.Schema.JsonSchemaException(info, context)
end