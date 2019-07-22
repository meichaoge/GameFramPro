---@class CS.Newtonsoft.Json.Utilities.WrapperMethodBuilder : CS.System.Object
CS.Newtonsoft.Json.Utilities.WrapperMethodBuilder = {}

---@param realObjectType : CS.System.Type
---@param proxyBuilder : CS.System.Reflection.Emit.TypeBuilder
---@return CS.Newtonsoft.Json.Utilities.WrapperMethodBuilder
function CS.Newtonsoft.Json.Utilities.WrapperMethodBuilder(realObjectType, proxyBuilder)
end

---@param newMethod : CS.System.Reflection.MethodInfo
function CS.Newtonsoft.Json.Utilities.WrapperMethodBuilder:Generate(newMethod)
end