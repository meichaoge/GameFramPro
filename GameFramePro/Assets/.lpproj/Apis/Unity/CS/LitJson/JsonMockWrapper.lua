---@class CS.LitJson.JsonMockWrapper : CS.System.Object
CS.LitJson.JsonMockWrapper = {}

---@property readonly CS.LitJson.JsonMockWrapper.IsArray : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsArray = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsBoolean : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsBoolean = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsDouble : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsDouble = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsInt : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsInt = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsLong : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsLong = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsObject : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsObject = nil

---@property readonly CS.LitJson.JsonMockWrapper.IsString : CS.System.Boolean
CS.LitJson.JsonMockWrapper.IsString = nil

---@return CS.LitJson.JsonMockWrapper
function CS.LitJson.JsonMockWrapper()
end

---@return CS.System.Boolean
function CS.LitJson.JsonMockWrapper:GetBoolean()
end

---@return CS.System.Double
function CS.LitJson.JsonMockWrapper:GetDouble()
end

---@return CS.System.Int32
function CS.LitJson.JsonMockWrapper:GetInt()
end

---@return CS.LitJson.JsonType
function CS.LitJson.JsonMockWrapper:GetJsonType()
end

---@return CS.System.Int64
function CS.LitJson.JsonMockWrapper:GetLong()
end

---@return CS.System.String
function CS.LitJson.JsonMockWrapper:GetString()
end

---@param val : CS.System.Boolean
function CS.LitJson.JsonMockWrapper:SetBoolean(val)
end

---@param val : CS.System.Double
function CS.LitJson.JsonMockWrapper:SetDouble(val)
end

---@param val : CS.System.Int32
function CS.LitJson.JsonMockWrapper:SetInt(val)
end

---@param type : CS.LitJson.JsonType
function CS.LitJson.JsonMockWrapper:SetJsonType(type)
end

---@param val : CS.System.Int64
function CS.LitJson.JsonMockWrapper:SetLong(val)
end

---@param val : CS.System.String
function CS.LitJson.JsonMockWrapper:SetString(val)
end

---@return CS.System.String
function CS.LitJson.JsonMockWrapper:ToJson()
end

---@param writer : CS.LitJson.JsonWriter
function CS.LitJson.JsonMockWrapper:ToJson(writer)
end