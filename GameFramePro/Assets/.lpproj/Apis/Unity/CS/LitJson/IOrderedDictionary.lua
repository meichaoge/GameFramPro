---@class CS.LitJson.IOrderedDictionary
CS.LitJson.IOrderedDictionary = {}

---@property readwrite CS.LitJson.IOrderedDictionary.Item : CS.System.Object
CS.LitJson.IOrderedDictionary.Item = nil

---@return CS.System.Collections.IDictionaryEnumerator
function CS.LitJson.IOrderedDictionary:GetEnumerator()
end

---@param index : CS.System.Int32
---@param key : CS.System.Object
---@param value : CS.System.Object
function CS.LitJson.IOrderedDictionary:Insert(index, key, value)
end

---@param index : CS.System.Int32
function CS.LitJson.IOrderedDictionary:RemoveAt(index)
end