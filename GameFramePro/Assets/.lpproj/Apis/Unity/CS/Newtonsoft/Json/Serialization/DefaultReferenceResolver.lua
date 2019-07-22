---@class CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver : CS.System.Object
CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver = {}

---@return CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver
function CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver()
end

---@param context : CS.System.Object
---@param reference : CS.System.String
---@return CS.System.Object
function CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver:ResolveReference(context, reference)
end

---@param context : CS.System.Object
---@param value : CS.System.Object
---@return CS.System.String
function CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver:GetReference(context, value)
end

---@param context : CS.System.Object
---@param reference : CS.System.String
---@param value : CS.System.Object
function CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver:AddReference(context, reference, value)
end

---@param context : CS.System.Object
---@param value : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Serialization.DefaultReferenceResolver:IsReferenced(context, value)
end