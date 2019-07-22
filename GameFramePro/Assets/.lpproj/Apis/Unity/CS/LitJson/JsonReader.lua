---@class CS.LitJson.JsonReader : CS.System.Object
CS.LitJson.JsonReader = {}

---@property readwrite CS.LitJson.JsonReader.AllowComments : CS.System.Boolean
CS.LitJson.JsonReader.AllowComments = nil

---@property readwrite CS.LitJson.JsonReader.AllowSingleQuotedStrings : CS.System.Boolean
CS.LitJson.JsonReader.AllowSingleQuotedStrings = nil

---@property readwrite CS.LitJson.JsonReader.SkipNonMembers : CS.System.Boolean
CS.LitJson.JsonReader.SkipNonMembers = nil

---@property readonly CS.LitJson.JsonReader.EndOfInput : CS.System.Boolean
CS.LitJson.JsonReader.EndOfInput = nil

---@property readonly CS.LitJson.JsonReader.EndOfJson : CS.System.Boolean
CS.LitJson.JsonReader.EndOfJson = nil

---@property readonly CS.LitJson.JsonReader.Token : CS.LitJson.JsonToken
CS.LitJson.JsonReader.Token = nil

---@property readonly CS.LitJson.JsonReader.Value : CS.System.Object
CS.LitJson.JsonReader.Value = nil

---@param json_text : CS.System.String
---@return CS.LitJson.JsonReader
function CS.LitJson.JsonReader(json_text)
end

---@param reader : CS.System.IO.TextReader
---@return CS.LitJson.JsonReader
function CS.LitJson.JsonReader(reader)
end

function CS.LitJson.JsonReader:Close()
end

---@return CS.System.Boolean
function CS.LitJson.JsonReader:Read()
end