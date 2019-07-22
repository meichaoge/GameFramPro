---@class CS.LitJson.FsmContext : CS.System.Object
CS.LitJson.FsmContext = {}

---@field public CS.LitJson.FsmContext.Return : CS.System.Boolean
CS.LitJson.FsmContext.Return = nil

---@field public CS.LitJson.FsmContext.NextState : CS.System.Int32
CS.LitJson.FsmContext.NextState = nil

---@field public CS.LitJson.FsmContext.L : CS.LitJson.Lexer
CS.LitJson.FsmContext.L = nil

---@field public CS.LitJson.FsmContext.StateStack : CS.System.Int32
CS.LitJson.FsmContext.StateStack = nil

---@return CS.LitJson.FsmContext
function CS.LitJson.FsmContext()
end