---@class CS.Newtonsoft.Json.Linq.JContainer : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JContainer = {}

---@property readonly CS.Newtonsoft.Json.Linq.JContainer.HasValues : CS.System.Boolean
CS.Newtonsoft.Json.Linq.JContainer.HasValues = nil

---@property readonly CS.Newtonsoft.Json.Linq.JContainer.First : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JContainer.First = nil

---@property readonly CS.Newtonsoft.Json.Linq.JContainer.Last : CS.Newtonsoft.Json.Linq.JToken
CS.Newtonsoft.Json.Linq.JContainer.Last = nil

---@property readonly CS.Newtonsoft.Json.Linq.JContainer.Count : CS.System.Int32
CS.Newtonsoft.Json.Linq.JContainer.Count = nil

---@param value : CS.System.ComponentModel.ListChangedEventHandler
function CS.Newtonsoft.Json.Linq.JContainer:add_ListChanged(value)
end

---@param value : CS.System.ComponentModel.ListChangedEventHandler
function CS.Newtonsoft.Json.Linq.JContainer:remove_ListChanged(value)
end

---@param value : CS.Newtonsoft.Json.ObservableSupport.AddingNewEventHandler
function CS.Newtonsoft.Json.Linq.JContainer:add_AddingNew(value)
end

---@param value : CS.Newtonsoft.Json.ObservableSupport.AddingNewEventHandler
function CS.Newtonsoft.Json.Linq.JContainer:remove_AddingNew(value)
end

---@return CS.Newtonsoft.Json.Linq.JEnumerable
function CS.Newtonsoft.Json.Linq.JContainer:Children()
end

---@return CS.System.Collections.Generic.IEnumerable
function CS.Newtonsoft.Json.Linq.JContainer:Descendants()
end

---@param content : CS.System.Object
function CS.Newtonsoft.Json.Linq.JContainer:Add(content)
end

---@param content : CS.System.Object
function CS.Newtonsoft.Json.Linq.JContainer:AddFirst(content)
end

---@return CS.Newtonsoft.Json.JsonWriter
function CS.Newtonsoft.Json.Linq.JContainer:CreateWriter()
end

---@param content : CS.System.Object
function CS.Newtonsoft.Json.Linq.JContainer:ReplaceAll(content)
end

function CS.Newtonsoft.Json.Linq.JContainer:RemoveAll()
end