---@class CS.Newtonsoft.Json.Utilities.TypeExtensions : CS.System.Object
CS.Newtonsoft.Json.Utilities.TypeExtensions = {}

---@param type : CS.System.Type
---@param name : CS.System.String
---@param parameterTypes : CS.System.Type[]
---@return CS.System.Reflection.MethodInfo
function CS.Newtonsoft.Json.Utilities.TypeExtensions.GetGenericMethod(type, name, parameterTypes)
end

---@param method : CS.System.Reflection.MethodInfo
---@param parameterTypes : CS.System.Type[]
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.TypeExtensions.HasParameters(method, parameterTypes)
end

---@param target : CS.System.Type
---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Utilities.TypeExtensions.AllInterfaces(target)
end

---@param target : CS.System.Type
---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Utilities.TypeExtensions.AllMethods(target)
end