---@class CS.LitJson.OrderedDictionaryEnumerator : CS.System.Object
CS.LitJson.OrderedDictionaryEnumerator = {}

---@property readonly CS.LitJson.OrderedDictionaryEnumerator.Current : CS.System.Object
CS.LitJson.OrderedDictionaryEnumerator.Current = nil

---@property readonly CS.LitJson.OrderedDictionaryEnumerator.Entry : CS.System.Collections.DictionaryEntry
CS.LitJson.OrderedDictionaryEnumerator.Entry = nil

---@property readonly CS.LitJson.OrderedDictionaryEnumerator.Key : CS.System.Object
CS.LitJson.OrderedDictionaryEnumerator.Key = nil

---@property readonly CS.LitJson.OrderedDictionaryEnumerator.Value : CS.System.Object
CS.LitJson.OrderedDictionaryEnumerator.Value = nil

---@param enumerator : CS.System.Collections.Generic.IEnumerator
---@return CS.LitJson.OrderedDictionaryEnumerator
function CS.LitJson.OrderedDictionaryEnumerator(enumerator)
end

---@return CS.System.Boolean
function CS.LitJson.OrderedDictionaryEnumerator:MoveNext()
end

function CS.LitJson.OrderedDictionaryEnumerator:Reset()
end