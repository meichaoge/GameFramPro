---@class CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler : CS.System.MulticastDelegate
CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler
function CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler(object, method)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventArgs
function CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler:Invoke(sender, e)
end

---@param sender : CS.System.Object
---@param e : CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventArgs
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler:BeginInvoke(sender, e, callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.Newtonsoft.Json.ObservableSupport.NotifyCollectionChangedEventHandler:EndInvoke(result)
end