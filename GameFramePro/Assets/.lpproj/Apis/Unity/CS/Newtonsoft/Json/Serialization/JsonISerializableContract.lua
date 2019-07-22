---@class CS.Newtonsoft.Json.Serialization.JsonISerializableContract : CS.Newtonsoft.Json.Serialization.JsonContract
CS.Newtonsoft.Json.Serialization.JsonISerializableContract = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonISerializableContract.ISerializableCreator : CS.Newtonsoft.Json.Serialization.ObjectConstructor
CS.Newtonsoft.Json.Serialization.JsonISerializableContract.ISerializableCreator = nil

---@param underlyingType : CS.System.Type
---@return CS.Newtonsoft.Json.Serialization.JsonISerializableContract
function CS.Newtonsoft.Json.Serialization.JsonISerializableContract(underlyingType)
end