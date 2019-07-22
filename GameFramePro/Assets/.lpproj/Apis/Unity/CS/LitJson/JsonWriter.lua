---@class CS.LitJson.JsonWriter : CS.System.Object
CS.LitJson.JsonWriter = {}

---@property readwrite CS.LitJson.JsonWriter.IndentValue : CS.System.Int32
CS.LitJson.JsonWriter.IndentValue = nil

---@property readwrite CS.LitJson.JsonWriter.PrettyPrint : CS.System.Boolean
CS.LitJson.JsonWriter.PrettyPrint = nil

---@property readonly CS.LitJson.JsonWriter.TextWriter : CS.System.IO.TextWriter
CS.LitJson.JsonWriter.TextWriter = nil

---@property readwrite CS.LitJson.JsonWriter.Validate : CS.System.Boolean
CS.LitJson.JsonWriter.Validate = nil

---@return CS.LitJson.JsonWriter
function CS.LitJson.JsonWriter()
end

---@param sb : CS.System.Text.StringBuilder
---@return CS.LitJson.JsonWriter
function CS.LitJson.JsonWriter(sb)
end

---@param writer : CS.System.IO.TextWriter
---@return CS.LitJson.JsonWriter
function CS.LitJson.JsonWriter(writer)
end

---@return CS.System.String
function CS.LitJson.JsonWriter:ToString()
end

function CS.LitJson.JsonWriter:Reset()
end

---@param boolean : CS.System.Boolean
function CS.LitJson.JsonWriter:Write(boolean)
end

---@param number : CS.System.Decimal
function CS.LitJson.JsonWriter:Write(number)
end

---@param number : CS.System.Double
function CS.LitJson.JsonWriter:Write(number)
end

---@param number : CS.System.Int32
function CS.LitJson.JsonWriter:Write(number)
end

---@param number : CS.System.Int64
function CS.LitJson.JsonWriter:Write(number)
end

---@param str : CS.System.String
function CS.LitJson.JsonWriter:Write(str)
end

---@param number : CS.System.UInt64
function CS.LitJson.JsonWriter:Write(number)
end

function CS.LitJson.JsonWriter:WriteArrayEnd()
end

function CS.LitJson.JsonWriter:WriteArrayStart()
end

function CS.LitJson.JsonWriter:WriteObjectEnd()
end

function CS.LitJson.JsonWriter:WriteObjectStart()
end

---@param property_name : CS.System.String
function CS.LitJson.JsonWriter:WritePropertyName(property_name)
end