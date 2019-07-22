---@class CS.Newtonsoft.Json.Serialization.JsonDictionaryContract : CS.Newtonsoft.Json.Serialization.JsonContract
CS.Newtonsoft.Json.Serialization.JsonDictionaryContract = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonDictionaryContract.PropertyNameResolver : CS.System.Func
CS.Newtonsoft.Json.Serialization.JsonDictionaryContract.PropertyNameResolver = nil

---@param underlyingType : CS.System.Type
---@return CS.Newtonsoft.Json.Serialization.JsonDictionaryContract
function CS.Newtonsoft.Json.Serialization.JsonDictionaryContract(underlyingType)
end