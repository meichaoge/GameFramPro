---@class CS.Newtonsoft.Json.Linq.JToken : CS.System.Object
CS.Newtonsoft.Json.Linq.JToken = {}

---@property readonly CS.Newtonsoft.Json.Linq.JToken.EqualityComparer : CS.Newtonsoft.Json.Linq.JTokenEqualityComparer
CS.Newtonsoft.Json.Linq.JToken.EqualityComparer = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JToken.Parent : CS.Newtonsoft.Json.Linq.JContainer
CS.Newtonsoft.Json.Linq.JToken.Parent = nil

---@property readonly CS.Newtonsoft.Json.Linq.JToken.Root : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.Root = nil

---@property readonly CS.Newtonsoft.Json.Linq.JToken.Type : CS.Newtonsoft.Json.Linq.JTokenType
CS.Newtonsoft.Json.Linq.JToken.Type = nil

---@property readonly CS.Newtonsoft.Json.Linq.JToken.HasValues : CS.System.Boolean
CS.Newtonsoft.Json.Linq.JToken.HasValues = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JToken.Next : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.Next = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JToken.Previous : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.Previous = nil

---@property readwrite CS.Newtonsoft.Json.Linq.JToken.Item : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.Item = nil

---@property readonly CS.Newtonsoft.Json.Linq.JToken.First : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.First = nil

---@property readonly CS.Newtonsoft.Json.Linq.JToken.Last : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JToken.Last = nil

---@param t1 : CS.Newtonsoft.Json.Linq.JToken
---@param t2 : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JToken.DeepEquals(t1, t2)
end

---@param content : CS.System.Object
function CS.Newtonsoft.Json.Linq.JToken:AddAfterSelf(content)
end

---@param content : CS.System.Object
function CS.Newtonsoft.Json.Linq.JToken:AddBeforeSelf(content)
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Linq.JToken:Ancestors()
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Linq.JToken:AfterSelf()
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Linq.JToken:BeforeSelf()
end

---@return CS.Newtonsoft.Json.Linq.JEnumerable
function CS.Newtonsoft.Json.Linq.JToken:Children()
end

function CS.Newtonsoft.Json.Linq.JToken:Remove()
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken:Replace(value)
end

---@param writer : CS.Newtonsoft.Json.JsonWriter
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
function CS.Newtonsoft.Json.Linq.JToken:WriteTo(writer, converters)
end

---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JToken:ToString()
end

---@param formatting : CS.Newtonsoft.Json.Formatting
---@param converters : CS.Newtonsoft.Json.JsonConverter[]
---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JToken:ToString(formatting, converters)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.DateTimeOffset
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Int64
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Int32
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Int16
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.UInt16
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.DateTime
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Decimal
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Double
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Single
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.String
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.UInt32
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.UInt64
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Byte[]
function CS.Newtonsoft.Json.Linq.JToken.op_Explicit(value)
end

---@param value : CS.System.Boolean
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.DateTimeOffset
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Int64
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Int16
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.UInt16
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Int32
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.DateTime
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Decimal
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Nullable
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Double
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Single
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.UInt32
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.UInt64
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@param value : CS.System.Byte[]
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.op_Implicit(value)
end

---@return CS.Newtonsoft.Json.JsonReader
function CS.Newtonsoft.Json.Linq.JToken:CreateReader()
end

---@param o : CS.System.Object
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.FromObject(o)
end

---@param o : CS.System.Object
---@param jsonSerializer : CS.Newtonsoft.Json.JsonSerializer
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.FromObject(o, jsonSerializer)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.ReadFrom(reader)
end

---@param json : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.Parse(json)
end

---@param reader : CS.Newtonsoft.Json.JsonReader
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken.Load(reader)
end

---@param path : CS.System.String
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken:SelectToken(path)
end

---@param path : CS.System.String
---@param errorWhenNoMatch : CS.System.Boolean
---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken:SelectToken(path, errorWhenNoMatch)
end

---@return CS.Newtonsoft.Json.Linq.JToken
function CS.Newtonsoft.Json.Linq.JToken:DeepClone()
end