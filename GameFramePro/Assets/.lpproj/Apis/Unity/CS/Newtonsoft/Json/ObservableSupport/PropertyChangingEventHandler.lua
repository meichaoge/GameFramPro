---@class CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler : CS.System.MulticastDelegate
CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler
function CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler(object, method)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventArgs
function CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler:Invoke(sender, e)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventArgs
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler:BeginInvoke(sender, e, callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.Newtonsoft.Json.ObservableSupport.PropertyChangingEventHandler:EndInvoke(result)
end