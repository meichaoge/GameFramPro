---@class CS.Newtonsoft.Json.Linq.JValue : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JValue = {}

---@property readonly CS.Newtonsoft.Json.Linq.JValue.HasValues : CS.System.Boolean
CS.Newtonsoft.Json.Linq.JValue.HasValues = nil

---@property readonly CS.Newtonsoft.Json.Linq.JValue.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JValue.Type = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JValue.Value : CS.System.Object
CS.Newtonsoft.Json.Linq.JValue.Value = nil

---@param other : CS.Newtonsoft.Json.Linq.JValue
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(other)
end

---@param value : CS.System.Int64
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.UInt64
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.Double
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.DateTime
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.Boolean
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.Guid
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.Uri
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.TimeSpan
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue(value)
end

---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue.CreateComment(value)
end

---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JValue
function CS.Newtonsoft.Json.Linq.JValue.CreateString(value)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JValue:WriteTo(writer, converters)
end

---@param other : CS.Newtonsoft.Json.Linq.JValue
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JValue:Equals(other)
end

---@param obj : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JValue:Equals(obj)
end

---@return CS.System.Int32
function CS.Newtonsoft.Json.Linq.JValue:GetHashCode()
end

---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JValue:ToString()
end

---@param format : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JValue:ToString(format)
end

---@param formatProvider : CS.System.IFormatProvider
---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JValue:ToString(formatProvider)
end

---@param format : CS.System.String
---@param formatProvider : CS.System.IFormatProvider
---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JValue:ToString(format, formatProvider)
end

---@param obj : CS.Newtonsoft.Json.Linq.JValue
---@return CS.System.Int32
function CS.Newtonsoft.Json.Linq.JValue:CompareTo(obj)
end