---@class CS.Newtonsoft.Json.Serialization.DefaultContractResolver : CS.System.Object
CS.Newtonsoft.Json.Serialization.DefaultContractResolver = {}

---@property readonly CS.Newtonsoft.Json.Serialization.DefaultContractResolver.DynamicCodeGeneration : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.DefaultContractResolver.DynamicCodeGeneration = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.DefaultContractResolver.DefaultMembersSearchFlags : CS.System.Reflection.BindingFlags
CS.Newtonsoft.Json.Serialization.DefaultContractResolver.DefaultMembersSearchFlags = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.DefaultContractResolver.SerializeCompilerGeneratedMembers : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.DefaultContractResolver.SerializeCompilerGeneratedMembers = nil

---@return CS.Newtonsoft.Json.Serialization.DefaultContractResolver
function CS.Newtonsoft.Json.Serialization.DefaultContractResolver()
end

---@param shareCache : CS.System.Boolean
---@return CS.Newtonsoft.Json.Serialization.DefaultContractResolver
function CS.Newtonsoft.Json.Serialization.DefaultContractResolver(shareCache)
end

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.Serialization.JsonContract
function CS.Newtonsoft.Json.Serialization.DefaultContractResolver:ResolveContract(type)
end