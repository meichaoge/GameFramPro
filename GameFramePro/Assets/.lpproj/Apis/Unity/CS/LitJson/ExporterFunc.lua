---@class CS.LitJson.ExporterFunc : CS.System.MulticastDelegate
CS.LitJson.ExporterFunc = {}

---@param object : CS.System.Object
---@param method : CS.System.IntPtr
---@return CS.LitJson.ExporterFunc
function CS.LitJson.ExporterFunc(object, method)
end

---@param obj : CS.System.Object
---@param writer : CS.LitJson.JsonWriter
function CS.LitJson.ExporterFunc:Invoke(obj, writer)
end

---@param obj : CS.System.Object
---@param writer : CS.LitJson.JsonWriter
---@param callback : CS.System.AsyncCallback
---@param object : CS.System.Object
---@return CS.System.IAsyncResult
function CS.LitJson.ExporterFunc:BeginInvoke(obj, writer, callback, object)
end

---@param result : CS.System.IAsyncResult
function CS.LitJson.ExporterFunc:EndInvoke(result)
end