---@class CS.LitJson.JsonData : CS.System.Object
CS.LitJson.JsonData = {}

---@property readonly CS.LitJson.JsonData.Count : CS.System.Int32
CS.LitJson.JsonData.Count = nil

---@property readonly CS.LitJson.JsonData.IsArray : CS.System.Boolean
CS.LitJson.JsonData.IsArray = nil

---@property readonly CS.LitJson.JsonData.IsBoolean : CS.System.Boolean
CS.LitJson.JsonData.IsBoolean = nil

---@property readonly CS.LitJson.JsonData.IsDouble : CS.System.Boolean
CS.LitJson.JsonData.IsDouble = nil

---@property readonly CS.LitJson.JsonData.IsInt : CS.System.Boolean
CS.LitJson.JsonData.IsInt = nil

---@property readonly CS.LitJson.JsonData.IsLong : CS.System.Boolean
CS.LitJson.JsonData.IsLong = nil

---@property readonly CS.LitJson.JsonData.IsObject : CS.System.Boolean
CS.LitJson.JsonData.IsObject = nil

---@property readonly CS.LitJson.JsonData.IsString : CS.System.Boolean
CS.LitJson.JsonData.IsString = nil

---@property readonly CS.LitJson.JsonData.Keys : CS.System.Collections.Generic.ICollection
CS.LitJson.JsonData.Keys = nil

---@property readwrite CS.LitJson.JsonData.Item : CS.LitJson.JsonData
CS.LitJson.JsonData.Item = nil

---@property readwrite CS.LitJson.JsonData.Item : CS.LitJson.JsonData
CS.LitJson.JsonData.Item = nil

---@return CS.LitJson.JsonData
function CS.LitJson.JsonData()
end

---@param boolean : CS.System.Boolean
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(boolean)
end

---@param number : CS.System.Double
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(number)
end

---@param number : CS.System.Int32
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(number)
end

---@param number : CS.System.Int64
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(number)
end

---@param obj : CS.System.Object
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(obj)
end

---@param str : CS.System.String
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData(str)
end

---@param data : CS.System.Boolean
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData.op_Implicit(data)
end

---@param data : CS.System.Double
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData.op_Implicit(data)
end

---@param data : CS.System.Int32
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData.op_Implicit(data)
end

---@param data : CS.System.Int64
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData.op_Implicit(data)
end

---@param data : CS.System.String
---@return CS.LitJson.JsonData
function CS.LitJson.JsonData.op_Implicit(data)
end

---@param data : CS.LitJson.JsonData
---@return CS.System.Boolean
function CS.LitJson.JsonData.op_Explicit(data)
end

---@param data : CS.LitJson.JsonData
---@return CS.System.Double
function CS.LitJson.JsonData.op_Explicit(data)
end

---@param data : CS.LitJson.JsonData
---@return CS.System.Int32
function CS.LitJson.JsonData.op_Explicit(data)
end

---@param data : CS.LitJson.JsonData
---@return CS.System.Int64
function CS.LitJson.JsonData.op_Explicit(data)
end

---@param data : CS.LitJson.JsonData
---@return CS.System.String
function CS.LitJson.JsonData.op_Explicit(data)
end

---@param value : CS.System.Object
---@return CS.System.Int32
function CS.LitJson.JsonData:Add(value)
end

function CS.LitJson.JsonData:Clear()
end

---@param x : CS.LitJson.JsonData
---@return CS.System.Boolean
function CS.LitJson.JsonData:Equals(x)
end

---@return CS.LitJson.JsonType
function CS.LitJson.JsonData:GetJsonType()
end

---@param type : CS.LitJson.JsonType
function CS.LitJson.JsonData:SetJsonType(type)
end

---@return CS.System.String
function CS.LitJson.JsonData:ToJson()
end

---@param writer : CS.LitJson.JsonWriter
function CS.LitJson.JsonData:ToJson(writer)
end

---@return CS.System.String
function CS.LitJson.JsonData:ToString()
end