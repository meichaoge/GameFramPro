---@class CS.Newtonsoft.Json.JsonConvert : CS.System.Object
CS.Newtonsoft.Json.JsonConvert = {}

---@field public CS.Newtonsoft.Json.JsonConvert.True : CS.System.String
CS.Newtonsoft.Json.JsonConvert.True = nil

---@field public CS.Newtonsoft.Json.JsonConvert.False : CS.System.String
CS.Newtonsoft.Json.JsonConvert.False = nil

---@field public CS.Newtonsoft.Json.JsonConvert.Null : CS.System.String
CS.Newtonsoft.Json.JsonConvert.Null = nil

---@field public CS.Newtonsoft.Json.JsonConvert.Undefined : CS.System.String
CS.Newtonsoft.Json.JsonConvert.Undefined = nil

---@field public CS.Newtonsoft.Json.JsonConvert.PositiveInfinity : CS.System.String
CS.Newtonsoft.Json.JsonConvert.PositiveInfinity = nil

---@field public CS.Newtonsoft.Json.JsonConvert.NegativeInfinity : CS.System.String
CS.Newtonsoft.Json.JsonConvert.NegativeInfinity = nil

---@field public CS.Newtonsoft.Json.JsonConvert.NaN : CS.System.String
CS.Newtonsoft.Json.JsonConvert.NaN = nil

---@param value : CS.System.DateTime
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.DateTimeOffset
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Boolean
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Char
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Enum
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Int32
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Int16
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.UInt16
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.UInt32
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Int64
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.UInt64
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Single
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Double
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Byte
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.SByte
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Decimal
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Guid
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.TimeSpan
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Uri
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.String
---@param delimter : CS.System.Char
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value, delimter)
end

---@param value : CS.System.Object
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.ToString(value)
end

---@param value : CS.System.Object
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeObject(value)
end

---@param value : CS.System.Object
---@param formatting : CS.Newtonsoft.Json.Formatting
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting)
end

---@param value : CS.System.Object
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeObject(value, converters)
end

---@param value : CS.System.Object
---@param formatting : CS.Newtonsoft.Json.Formatting
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting, converters)
end

---@param value : CS.System.Object
---@param formatting : CS.Newtonsoft.Json.Formatting
---@param settings : CS.Newtonsoft.Json.JsonSerializerSettings
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeObject(value, formatting, settings)
end

---@param value : CS.System.String
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.DeserializeObject(value)
end

---@param value : CS.System.String
---@param settings : CS.Newtonsoft.Json.JsonSerializerSettings
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.DeserializeObject(value, settings)
end

---@param value : CS.System.String
---@param type : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.DeserializeObject(value, type)
end

---@param value : CS.System.String
---@param type : CS.System.Type
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, converters)
end

---@param value : CS.System.String
---@param type : CS.System.Type
---@param settings : CS.Newtonsoft.Json.JsonSerializerSettings
---@return CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, settings)
end

---@param value : CS.System.String
---@param target : CS.System.Object
function CS.Newtonsoft.Json.JsonConvert.PopulateObject(value, target)
end

---@param value : CS.System.String
---@param target : CS.System.Object
---@param settings : CS.Newtonsoft.Json.JsonSerializerSettings
function CS.Newtonsoft.Json.JsonConvert.PopulateObject(value, target, settings)
end

---@param node : CS.System.Xml.XmlNode
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeXmlNode(node)
end

---@param node : CS.System.Xml.XmlNode
---@param formatting : CS.Newtonsoft.Json.Formatting
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeXmlNode(node, formatting)
end

---@param node : CS.System.Xml.XmlNode
---@param formatting : CS.Newtonsoft.Json.Formatting
---@param omitRootObject : CS.System.Boolean
---@return CS.System.String
function CS.Newtonsoft.Json.JsonConvert.SerializeXmlNode(node, formatting, omitRootObject)
end

---@param value : CS.System.String
---@return CS.System.Xml.XmlDocument
function CS.Newtonsoft.Json.JsonConvert.DeserializeXmlNode(value)
end

---@param value : CS.System.String
---@param deserializeRootElementName : CS.System.String
---@return CS.System.Xml.XmlDocument
function CS.Newtonsoft.Json.JsonConvert.DeserializeXmlNode(value, deserializeRootElementName)
end

---@param value : CS.System.String
---@param deserializeRootElementName : CS.System.String
---@param writeArrayAttribute : CS.System.Boolean
---@return CS.System.Xml.XmlDocument
function CS.Newtonsoft.Json.JsonConvert.DeserializeXmlNode(value, deserializeRootElementName, writeArrayAttribute)
end