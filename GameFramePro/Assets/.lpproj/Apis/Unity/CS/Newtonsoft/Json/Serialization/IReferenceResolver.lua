---@class CS.Newtonsoft.Json.Serialization.IReferenceResolver
CS.Newtonsoft.Json.Serialization.IReferenceResolver = {}

---@param context : CS.System.Object
---@param reference : CS.System.String
---@return CS.System.Object
function CS.Newtonsoft.Json.Serialization.IReferenceResolver:ResolveReference(context, reference)
end

---@param context : CS.System.Object
---@param value : CS.System.Object
---@return CS.System.String
function CS.Newtonsoft.Json.Serialization.IReferenceResolver:GetReference(context, value)
end

---@param context : CS.System.Object
---@param value : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Serialization.IReferenceResolver:IsReferenced(context, value)
end

---@param context : CS.System.Object
---@param reference : CS.System.String
---@param value : CS.System.Object
function CS.Newtonsoft.Json.Serialization.IReferenceResolver:AddReference(context, reference, value)
end