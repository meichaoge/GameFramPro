---@class CS.Newtonsoft.Json.Serialization.ErrorEventArgs : CS.System.EventArgs
CS.Newtonsoft.Json.Serialization.ErrorEventArgs = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.ErrorEventArgs.CurrentObject : CS.System.Object
CS.Newtonsoft.Json.Serialization.ErrorEventArgs.CurrentObject = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.ErrorEventArgs.ErrorContext : CS.Newtonsoft.Json.Serialization.ErrorContext
CS.Newtonsoft.Json.Serialization.ErrorEventArgs.ErrorContext = nil

---@param currentObject : CS.System.Object
---@param errorContext : CS.Newtonsoft.Json.Serialization.ErrorContext
---@return CS.Newtonsoft.Json.Serialization.ErrorEventArgs
function CS.Newtonsoft.Json.Serialization.ErrorEventArgs(currentObject, errorContext)
end