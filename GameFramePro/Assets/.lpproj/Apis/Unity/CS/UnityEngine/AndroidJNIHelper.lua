---@class CS.UnityEngine.AndroidJNIHelper : CS.System.Object
CS.UnityEngine.AndroidJNIHelper = {}

---@property readwrite CS.UnityEngine.AndroidJNIHelper.debug : CS.System.Boolean
CS.UnityEngine.AndroidJNIHelper.debug = nil

---@param javaClass : CS.System.IntPtr
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetConstructorID(javaClass)
end

---@param javaClass : CS.System.IntPtr
---@param signature : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetConstructorID(javaClass, signature)
end

---@param javaClass : CS.System.IntPtr
---@param methodName : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetMethodID(javaClass, methodName)
end

---@param javaClass : CS.System.IntPtr
---@param methodName : CS.System.String
---@param signature : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetMethodID(javaClass, methodName, signature)
end

---@param javaClass : CS.System.IntPtr
---@param methodName : CS.System.String
---@param signature : CS.System.String
---@param isStatic : CS.System.Boolean
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetMethodID(javaClass, methodName, signature, isStatic)
end

---@param javaClass : CS.System.IntPtr
---@param fieldName : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetFieldID(javaClass, fieldName)
end

---@param javaClass : CS.System.IntPtr
---@param fieldName : CS.System.String
---@param signature : CS.System.String
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetFieldID(javaClass, fieldName, signature)
end

---@param javaClass : CS.System.IntPtr
---@param fieldName : CS.System.String
---@param signature : CS.System.String
---@param isStatic : CS.System.Boolean
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetFieldID(javaClass, fieldName, signature, isStatic)
end

---@param jrunnable : CS.UnityEngine.AndroidJavaRunnable
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.CreateJavaRunnable(jrunnable)
end

---@param proxy : CS.UnityEngine.AndroidJavaProxy
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.CreateJavaProxy(proxy)
end

---@param array : CS.System.Array
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.ConvertToJNIArray(array)
end

---@param args : CS.System.Object[]
---@return CS.UnityEngine.jvalue[]
function CS.UnityEngine.AndroidJNIHelper.CreateJNIArgArray(args)
end

---@param args : CS.System.Object[]
---@param jniArgs : CS.UnityEngine.jvalue[]
function CS.UnityEngine.AndroidJNIHelper.DeleteJNIArgArray(args, jniArgs)
end

---@param jclass : CS.System.IntPtr
---@param args : CS.System.Object[]
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetConstructorID(jclass, args)
end

---@param jclass : CS.System.IntPtr
---@param methodName : CS.System.String
---@param args : CS.System.Object[]
---@param isStatic : CS.System.Boolean
---@return CS.System.IntPtr
function CS.UnityEngine.AndroidJNIHelper.GetMethodID(jclass, methodName, args, isStatic)
end

---@param obj : CS.System.Object
---@return CS.System.String
function CS.UnityEngine.AndroidJNIHelper.GetSignature(obj)
end

---@param args : CS.System.Object[]
---@return CS.System.String
function CS.UnityEngine.AndroidJNIHelper.GetSignature(args)
end