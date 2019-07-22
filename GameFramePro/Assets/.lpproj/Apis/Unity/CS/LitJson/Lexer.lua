---@class CS.LitJson.Lexer : CS.System.Object
CS.LitJson.Lexer = {}

---@property readwrite CS.LitJson.Lexer.AllowComments : CS.System.Boolean
CS.LitJson.Lexer.AllowComments = nil

---@property readwrite CS.LitJson.Lexer.AllowSingleQuotedStrings : CS.System.Boolean
CS.LitJson.Lexer.AllowSingleQuotedStrings = nil

---@property readonly CS.LitJson.Lexer.EndOfInput : CS.System.Boolean
CS.LitJson.Lexer.EndOfInput = nil

---@property readonly CS.LitJson.Lexer.Token : CS.System.Int32
CS.LitJson.Lexer.Token = nil

---@property readonly CS.LitJson.Lexer.StringValue : CS.System.String
CS.LitJson.Lexer.StringValue = nil

---@param reader : CS.System.IO.TextReader
---@return CS.LitJson.Lexer
function CS.LitJson.Lexer(reader)
end

---@return CS.System.Boolean
function CS.LitJson.Lexer:NextToken()
end