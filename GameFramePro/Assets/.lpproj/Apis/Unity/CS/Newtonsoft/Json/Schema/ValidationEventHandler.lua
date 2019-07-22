---@class CS.Newtonsoft.Json.Schema.ValidationEventHandler : CS.System.MulticastDelegate
CS.Newtonsoft.Json.Schema.ValidationEventHandler = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.Newtonsoft.Json.Schema.ValidationEventHandler
function CS.Newtonsoft.Json.Schema.ValidationEventHandler(object, method)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.Schema.ValidationEventArgs
function CS.Newtonsoft.Json.Schema.ValidationEventHandler:Invoke(sender, e)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.Schema.ValidationEventArgs
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.Newtonsoft.Json.Schema.ValidationEventHandler:BeginInvoke(sender, e, callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.Newtonsoft.Json.Schema.ValidationEventHandler:EndInvoke(result)
end