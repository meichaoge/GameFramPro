---@class CS.Newtonsoft.Json.Serialization.JsonObjectContract : CS.Newtonsoft.Json.Serialization.JsonContract
CS.Newtonsoft.Json.Serialization.JsonObjectContract = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonObjectContract.MemberSerialization : CS.Newtonsoft.Json.MemberSerialization
CS.Newtonsoft.Json.Serialization.JsonObjectContract.MemberSerialization = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonObjectContract.Properties : CS.Newtonsoft.Json.Serialization.JsonPropertyCollection
CS.Newtonsoft.Json.Serialization.JsonObjectContract.Properties = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonObjectContract.ConstructorParameters : CS.Newtonsoft.Json.Serialization.JsonPropertyCollection
CS.Newtonsoft.Json.Serialization.JsonObjectContract.ConstructorParameters = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonObjectContract.OverrideConstructor : CS.System.Reflection.ConstructorInfo
CS.Newtonsoft.Json.Serialization.JsonObjectContract.OverrideConstructor = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonObjectContract.ParametrizedConstructor : CS.System.Reflection.ConstructorInfo
CS.Newtonsoft.Json.Serialization.JsonObjectContract.ParametrizedConstructor = nil

---@param underlyingType : CS.System.Type
---@return CS.Newtonsoft.Json.Serialization.JsonObjectContract
function CS.Newtonsoft.Json.Serialization.JsonObjectContract(underlyingType)
end