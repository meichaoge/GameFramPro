---@class CS.UnityEngine.AndroidJavaProxy : CS.System.Object
CS.UnityEngine.AndroidJavaProxy = {}

---@field public CS.UnityEngine.AndroidJavaProxy.javaInterface : CS.UnityEngine.AndroidJavaClass
CS.UnityEngine.AndroidJavaProxy.javaInterface = nil

---@param javaInterface : CS.System.String
---@return CS.UnityEngine.AndroidJavaProxy
function CS.UnityEngine.AndroidJavaProxy(javaInterface)
end

---@param javaInterface : CS.UnityEngine.AndroidJavaClass
---@return CS.UnityEngine.AndroidJavaProxy
function CS.UnityEngine.AndroidJavaProxy(javaInterface)
end

---@param methodName : CS.System.String
---@param args : CS.System.Object[]
---@return CS.UnityEngine.AndroidJavaObject
function CS.UnityEngine.AndroidJavaProxy:Invoke(methodName, args)
end

---@param methodName : CS.System.String
---@param javaArgs : CS.UnityEngine.AndroidJavaObject[]
---@return CS.UnityEngine.AndroidJavaObject
function CS.UnityEngine.AndroidJavaProxy:Invoke(methodName, javaArgs)
end

---@param obj : CS.UnityEngine.AndroidJavaObject
---@return CS.System.Boolean
function CS.UnityEngine.AndroidJavaProxy:equals(obj)
end

---@return CS.System.Int32
function CS.UnityEngine.AndroidJavaProxy:hashCode()
end

---@return CS.System.String
function CS.UnityEngine.AndroidJavaProxy:toString()
end