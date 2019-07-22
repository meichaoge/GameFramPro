---@class CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions : CS.System.Object
CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions = {}

---@param generator : CS.System.Reflection.Emit.ILGenerator
---@param type : CS.System.Type
function CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions.PushInstance(generator, type)
end

---@param generator : CS.System.Reflection.Emit.ILGenerator
---@param type : CS.System.Type
function CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions.BoxIfNeeded(generator, type)
end

---@param generator : CS.System.Reflection.Emit.ILGenerator
---@param type : CS.System.Type
function CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions.UnboxIfNeeded(generator, type)
end

---@param generator : CS.System.Reflection.Emit.ILGenerator
---@param methodInfo : CS.System.Reflection.MethodInfo
function CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions.CallMethod(generator, methodInfo)
end

---@param generator : CS.System.Reflection.Emit.ILGenerator
function CS.Newtonsoft.Json.Utilities.ILGeneratorExtensions.Return(generator)
end