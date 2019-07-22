---@class CS.Newtonsoft.Json.Serialization.DynamicValueProvider : CS.System.Object
CS.Newtonsoft.Json.Serialization.DynamicValueProvider = {}

---@param memberInfo : CS.System.Reflection.MemberInfo
---@return CS.Newtonsoft.Json.Serialization.DynamicValueProvider
function CS.Newtonsoft.Json.Serialization.DynamicValueProvider(memberInfo)
end

---@param target : CS.System.Object
---@param value : CS.System.Object
function CS.Newtonsoft.Json.Serialization.DynamicValueProvider:SetValue(target, value)
end

---@param target : CS.System.Object
---@return CS.System.Object
function CS.Newtonsoft.Json.Serialization.DynamicValueProvider:GetValue(target)
end