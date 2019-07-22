---@module CS.XLua
CS.XLua = {}

---@class CS.XLua.CodeEmit : CS.System.Object
CS.XLua.CodeEmit = {}

---@return CS.XLua.CodeEmit
function CS.XLua.CodeEmit()
end

---@param groups : CS.System.Collections.Generic.IEnumerable
---@return CS.System.Type
function CS.XLua.CodeEmit:EmitDelegateImpl(groups)
end

---@param gen_interfaces : CS.System.Collections.Generic.List
function CS.XLua.CodeEmit:SetGenInterfaces(gen_interfaces)
end

---@param to_be_impl : CS.System.Type
---@return CS.System.Type
function CS.XLua.CodeEmit:EmitInterfaceImpl(to_be_impl)
end

---@param typeBuilder : CS.System.Reflection.Emit.TypeBuilder
---@param field : CS.System.Reflection.FieldInfo
---@param genGetter : CS.System.Boolean
---@return CS.System.Reflection.Emit.MethodBuilder
function CS.XLua.CodeEmit:emitFieldWrap(typeBuilder, field, genGetter)
end

---@param typeBuilder : CS.System.Reflection.Emit.TypeBuilder
---@param prop : CS.System.Reflection.PropertyInfo
---@param op : CS.System.Reflection.MethodInfo
---@param genGetter : CS.System.Boolean
---@return CS.System.Reflection.Emit.MethodBuilder
function CS.XLua.CodeEmit:emitPropertyWrap(typeBuilder, prop, op, genGetter)
end

---@param toBeWrap : CS.System.Type
---@return CS.System.Type
function CS.XLua.CodeEmit:EmitTypeWrap(toBeWrap)
end