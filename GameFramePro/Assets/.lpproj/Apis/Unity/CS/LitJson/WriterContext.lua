---@class CS.LitJson.WriterContext : CS.System.Object
CS.LitJson.WriterContext = {}

---@field public CS.LitJson.WriterContext.Count : CS.System.Int32
CS.LitJson.WriterContext.Count = nil

---@field public CS.LitJson.WriterContext.InArray : CS.System.Boolean
CS.LitJson.WriterContext.InArray = nil

---@field public CS.LitJson.WriterContext.InObject : CS.System.Boolean
CS.LitJson.WriterContext.InObject = nil

---@field public CS.LitJson.WriterContext.ExpectingValue : CS.System.Boolean
CS.LitJson.WriterContext.ExpectingValue = nil

---@field public CS.LitJson.WriterContext.Padding : CS.System.Int32
CS.LitJson.WriterContext.Padding = nil

---@return CS.LitJson.WriterContext
function CS.LitJson.WriterContext()
end