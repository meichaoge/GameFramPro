---@class CS.UnityEngine.AndroidJavaRunnable : CS.System.MulticastDelegate
CS.UnityEngine.AndroidJavaRunnable = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.UnityEngine.AndroidJavaRunnable
function CS.UnityEngine.AndroidJavaRunnable(object, method)
end

function CS.UnityEngine.AndroidJavaRunnable:Invoke()
end

---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.UnityEngine.AndroidJavaRunnable:BeginInvoke(callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.UnityEngine.AndroidJavaRunnable:EndInvoke(result)
end