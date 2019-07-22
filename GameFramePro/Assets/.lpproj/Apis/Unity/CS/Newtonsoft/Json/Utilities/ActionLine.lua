---@class CS.Newtonsoft.Json.Utilities.ActionLine : CS.System.MulticastDelegate
CS.Newtonsoft.Json.Utilities.ActionLine = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.Newtonsoft.Json.Utilities.ActionLine
function CS.Newtonsoft.Json.Utilities.ActionLine(object, method)
end

---@param textWriter : CS.System.IO.TextWriter
---@param line : CS.System.String
function CS.Newtonsoft.Json.Utilities.ActionLine:Invoke(textWriter, line)
end

---@param textWriter : CS.System.IO.TextWriter
---@param line : CS.System.String
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.Newtonsoft.Json.Utilities.ActionLine:BeginInvoke(textWriter, line, callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.Newtonsoft.Json.Utilities.ActionLine:EndInvoke(result)
end