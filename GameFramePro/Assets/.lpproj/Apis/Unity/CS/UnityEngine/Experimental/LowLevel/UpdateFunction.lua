---@class CS.UnityEngine.Experimental.LowLevel.UpdateFunction : CS.System.MulticastDelegate
CS.UnityEngine.Experimental.LowLevel.UpdateFunction = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.UnityEngine.Experimental.LowLevel.UpdateFunction
function CS.UnityEngine.Experimental.LowLevel.UpdateFunction(object, method)
end

function CS.UnityEngine.Experimental.LowLevel.UpdateFunction:Invoke()
end

---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.UnityEngine.Experimental.LowLevel.UpdateFunction:BeginInvoke(callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.UnityEngine.Experimental.LowLevel.UpdateFunction:EndInvoke(result)
end