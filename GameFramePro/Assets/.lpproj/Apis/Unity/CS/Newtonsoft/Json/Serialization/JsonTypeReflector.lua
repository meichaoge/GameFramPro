---@class CS.Newtonsoft.Json.Serialization.JsonTypeReflector : CS.System.Object
CS.Newtonsoft.Json.Serialization.JsonTypeReflector = {}

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.IdPropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.IdPropertyName = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.RefPropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.RefPropertyName = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.TypePropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.TypePropertyName = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ValuePropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ValuePropertyName = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ArrayValuesPropertyName : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ArrayValuesPropertyName = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ShouldSerializePrefix : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ShouldSerializePrefix = nil

---@field public CS.Newtonsoft.Json.Serialization.JsonTypeReflector.SpecifiedPostfix : CS.System.String
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.SpecifiedPostfix = nil

---@property readonly CS.Newtonsoft.Json.Serialization.JsonTypeReflector.DynamicCodeGeneration : CS.System.Boolean
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.DynamicCodeGeneration = nil

---@property readonly CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ReflectionDelegateFactory : CS.Newtonsoft.Json.Utilities.ReflectionDelegateFactory
CS.Newtonsoft.Json.Serialization.JsonTypeReflector.ReflectionDelegateFactory = nil

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.JsonContainerAttribute
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetJsonContainerAttribute(type)
end

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.JsonObjectAttribute
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetJsonObjectAttribute(type)
end

---@param type : CS.System.Type
---@return CS.Newtonsoft.Json.JsonArrayAttribute
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetJsonArrayAttribute(type)
end

---@param type : CS.System.Type
---@return CS.System.Runtime.Serialization.DataContractAttribute
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetDataContractAttribute(type)
end

---@param memberInfo : CS.System.Reflection.MemberInfo
---@return CS.System.Runtime.Serialization.DataMemberAttribute
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetDataMemberAttribute(memberInfo)
end

---@param objectType : CS.System.Type
---@return CS.Newtonsoft.Json.MemberSerialization
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetObjectMemberSerialization(objectType)
end

---@param attributeProvider : CS.System.Reflection.ICustomAttributeProvider
---@param targetConvertedType : CS.System.Type
---@return CS.Newtonsoft.Json.JsonConverter
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetJsonConverter(attributeProvider, targetConvertedType)
end

---@param type : CS.System.Type
---@return CS.System.ComponentModel.TypeConverter
function CS.Newtonsoft.Json.Serialization.JsonTypeReflector.GetTypeConverter(type)
end