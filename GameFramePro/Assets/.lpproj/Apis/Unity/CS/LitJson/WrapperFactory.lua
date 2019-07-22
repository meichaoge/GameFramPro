---@class CS.LitJson.WrapperFactory : CS.System.MulticastDelegate
CS.LitJson.WrapperFactory = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.LitJson.WrapperFactory
function CS.LitJson.WrapperFactory(object, method)
end

---@return CS.LitJson.IJsonWrapper
function CS.LitJson.WrapperFactory:Invoke()
end

---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.LitJson.WrapperFactory:BeginInvoke(callback, object)
end

---@param result : CS.System.IAsyncResult
---@return CS.LitJson.IJsonWrapper
function CS.LitJson.WrapperFactory:EndInvoke(result)
end