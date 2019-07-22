---@class CS.Newtonsoft.Json.JsonSerializer : CS.System.Object
CS.Newtonsoft.Json.JsonSerializer = {}

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.ReferenceResolver : CS.Newtonsoft.Json.Serialization.IReferenceResolver
CS.Newtonsoft.Json.JsonSerializer.ReferenceResolver = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.Binder : CS.System.Runtime.Serialization.SerializationBinder
CS.Newtonsoft.Json.JsonSerializer.Binder = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.TypeNameHandling : CS.Newtonsoft.Json.TypeNameHandling
CS.Newtonsoft.Json.JsonSerializer.TypeNameHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.TypeNameAssemblyFormat : CS.System.Runtime.Serialization.Formatters.FormatterAssemblyStyle
CS.Newtonsoft.Json.JsonSerializer.TypeNameAssemblyFormat = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.PreserveReferencesHandling : CS.Newtonsoft.Json.PreserveReferencesHandling
CS.Newtonsoft.Json.JsonSerializer.PreserveReferencesHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.ReferenceLoopHandling : CS.Newtonsoft.Json.ReferenceLoopHandling
CS.Newtonsoft.Json.JsonSerializer.ReferenceLoopHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.MissingMemberHandling : CS.Newtonsoft.Json.MissingMemberHandling
CS.Newtonsoft.Json.JsonSerializer.MissingMemberHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.NullValueHandling : CS.Newtonsoft.Json.NullValueHandling
CS.Newtonsoft.Json.JsonSerializer.NullValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.DefaultValueHandling : CS.Newtonsoft.Json.DefaultValueHandling
CS.Newtonsoft.Json.JsonSerializer.DefaultValueHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.ObjectCreationHandling : CS.Newtonsoft.Json.ObjectCreationHandling
CS.Newtonsoft.Json.JsonSerializer.ObjectCreationHandling = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.ConstructorHandling : CS.Newtonsoft.Json.ConstructorHandling
CS.Newtonsoft.Json.JsonSerializer.ConstructorHandling = nil

---@property readonly CS.Newtonsoft.Json.JsonSerializer.Converters : CS.Newtonsoft.Json.JsonConverterCollection
CS.Newtonsoft.Json.JsonSerializer.Converters = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.ContractResolver : CS.Newtonsoft.Json.Serialization.IContractResolver
CS.Newtonsoft.Json.JsonSerializer.ContractResolver = nil

---@property readwrite CS.Newtonsoft.Json.JsonSerializer.Context : CS.System.Runtime.Serialization.StreamingContext
CS.Newtonsoft.Json.JsonSerializer.Context = nil

---@return CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.JsonSerializer()
end

---@param value : CS.System.EventHandler
function CS.Newtonsoft.Json.JsonSerializer:add_Error(value)
end

---@param value : CS.System.EventHandler
function CS.Newtonsoft.Json.JsonSerializer:remove_Error(value)
end

---@param settings : CS.Newtonsoft.Json.JsonSerializerSettings
---@return CS.Newtonsoft.Json.JsonSerializer
function CS.Newtonsoft.Json.JsonSerializer.Create(settings)
end

---@param reader : CS.System.IO.TextReader
---@param target : CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Populate(reader, target)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param target : CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Populate(reader, target)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Deserialize(reader)
end

---@param reader : CS.System.IO.TextReader
---@param objectType : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Deserialize(reader, objectType)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@param objectType : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Deserialize(reader, objectType)
end

---@param textWriter : CS.System.IO.TextWriter
---@param value : CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Serialize(textWriter, value)
end

---@param jsonWriter : CS.Newtonsoft.Json.JsonWriter
---@param value : CS.System.Object
function CS.Newtonsoft.Json.JsonSerializer:Serialize(jsonWriter, value)
end