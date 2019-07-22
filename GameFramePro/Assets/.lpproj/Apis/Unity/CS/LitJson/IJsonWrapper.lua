---@class CS.LitJson.IJsonWrapper
CS.LitJson.IJsonWrapper = {}

---@property readonly CS.LitJson.IJsonWrapper.IsArray : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsArray = nil

---@property readonly CS.LitJson.IJsonWrapper.IsBoolean : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsBoolean = nil

---@property readonly CS.LitJson.IJsonWrapper.IsDouble : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsDouble = nil

---@property readonly CS.LitJson.IJsonWrapper.IsInt : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsInt = nil

---@property readonly CS.LitJson.IJsonWrapper.IsLong : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsLong = nil

---@property readonly CS.LitJson.IJsonWrapper.IsObject : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsObject = nil

---@property readonly CS.LitJson.IJsonWrapper.IsString : CS.System.Boolean
CS.LitJson.IJsonWrapper.IsString = nil

---@return CS.System.Boolean
function CS.LitJson.IJsonWrapper:GetBoolean()
end

---@return CS.System.Double
function CS.LitJson.IJsonWrapper:GetDouble()
end

---@return CS.System.Int32
function CS.LitJson.IJsonWrapper:GetInt()
end

---@return CS.LitJson.JsonType
function CS.LitJson.IJsonWrapper:GetJsonType()
end

---@return CS.System.Int64
function CS.LitJson.IJsonWrapper:GetLong()
end

---@return CS.System.String
function CS.LitJson.IJsonWrapper:GetString()
end

---@param val : CS.System.Boolean
function CS.LitJson.IJsonWrapper:SetBoolean(val)
end

---@param val : CS.System.Double
function CS.LitJson.IJsonWrapper:SetDouble(val)
end

---@param val : CS.System.Int32
function CS.LitJson.IJsonWrapper:SetInt(val)
end

---@param type : CS.LitJson.JsonType
function CS.LitJson.IJsonWrapper:SetJsonType(type)
end

---@param val : CS.System.Int64
function CS.LitJson.IJsonWrapper:SetLong(val)
end

---@param val : CS.System.String
function CS.LitJson.IJsonWrapper:SetString(val)
end

---@return CS.System.String
function CS.LitJson.IJsonWrapper:ToJson()
end

---@param writer : CS.LitJson.JsonWriter
function CS.LitJson.IJsonWrapper:ToJson(writer)
end