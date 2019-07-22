---@class CS.LitJson.StateHandler : CS.System.MulticastDelegate
CS.LitJson.StateHandler = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.LitJson.StateHandler
function CS.LitJson.StateHandler(object, method)
end

---@param ctx : CS.LitJson.FsmContext
---@return CS.System.Boolean
function CS.LitJson.StateHandler:Invoke(ctx)
end

---@param ctx : CS.LitJson.FsmContext
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.LitJson.StateHandler:BeginInvoke(ctx, callback, object)
end

---@param result : CS.System.IAsyncResult
---@return CS.System.Boolean
function CS.LitJson.StateHandler:EndInvoke(result)
end