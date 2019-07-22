---@class CS.Newtonsoft.Json.Serialization.JsonSerializerProxy : CS.Newtonsoft.Json.JsonSerializer
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy = {}

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ReferenceResolver : CS.Newtonsoft.Json.Serialization.IReferenceResolver
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ReferenceResolver = nil

---@property readonly CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Converters : CS.Newtonsoft.Json.JsonConverterCollection
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Converters = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.DefaultValueHandling : CS.Newtonsoft.Json.DefaultValueHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.DefaultValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ContractResolver : CS.Newtonsoft.Json.Serialization.IContractResolver
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ContractResolver = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.MissingMemberHandling : CS.Newtonsoft.Json.MissingMemberHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.MissingMemberHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.NullValueHandling : CS.Newtonsoft.Json.NullValueHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.NullValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ObjectCreationHandling : CS.Newtonsoft.Json.ObjectCreationHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ObjectCreationHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ReferenceLoopHandling : CS.Newtonsoft.Json.ReferenceLoopHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ReferenceLoopHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.PreserveReferencesHandling : CS.Newtonsoft.Json.PreserveReferencesHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.PreserveReferencesHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.TypeNameHandling : CS.Newtonsoft.Json.TypeNameHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.TypeNameHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.TypeNameAssemblyFormat : CS.System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.TypeNameAssemblyFormat = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ConstructorHandling : CS.Newtonsoft.Json.ConstructorHandling
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.ConstructorHandling = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Binder : CS.System.Runtime.Serialization.SerializationBinder
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Binder = nil

---@property readwrite CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Context : CS.System.Runtime.Serialization.StreamingContext
CS.Newtonsoft.Json.Serialization.JsonSerializerProxy.Context = nil

---@param serializerReader : CS.Newtonsoft.Json.Serialization.JsonSerializerInternalReader
---@return CS.Newtonsoft.Json.Serialization.JsonSerializerProxy
function CS.Newtonsoft.Json.Serialization.JsonSerializerProxy(serializerReader)
end

---@param serializerWriter : CS.Newtonsoft.Json.Serialization.JsonSerializerInternalWriter
---@return CS.Newtonsoft.Json.Serialization.JsonSerializerProxy
function CS.Newtonsoft.Json.Serialization.JsonSerializerProxy(serializerWriter)
end

---@param value : CS.System.EventHandler
function CS.Newtonsoft.Json.Serialization.JsonSerializerProxy:add_Error(value)
end

---@param value : CS.System.EventHandler
function CS.Newtonsoft.Json.Serialization.JsonSerializerProxy:remove_Error(value)
end