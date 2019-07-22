---@class CS.Newtonsoft.Json.Utilities.CollectionUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.CollectionUtils = {}

---@param collection : CS.System.Collections.ICollection
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.CollectionUtils.IsNullOrEmpty(collection)
end

---@param initial : CS.System.Collections.IList
---@param collection : CS.System.Collections.IEnumerable
function CS.Newtonsoft.Json.Utilities.CollectionUtils.AddRange(initial, collection)
end

---@param listType : CS.System.Type
---@return CS.System.Collections.IList
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateGenericList(listType)
end

---@param keyType : CS.System.Type
---@param valueType : CS.System.Type
---@return CS.System.Collections.IDictionary
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateGenericDictionary(keyType, valueType)
end

---@param type : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.CollectionUtils.IsListType(type)
end

---@param type : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.CollectionUtils.IsCollectionType(type)
end

---@param type : CS.System.Type
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.CollectionUtils.IsDictionaryType(type)
end

---@param list : CS.System.Object
---@return CS.Newtonsoft.Json.Utilities.IWrappedCollection
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateCollectionWrapper(list)
end

---@param list : CS.System.Object
---@return CS.Newtonsoft.Json.Utilities.IWrappedList
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateListWrapper(list)
end

---@param dictionary : CS.System.Object
---@return CS.Newtonsoft.Json.Utilities.IWrappedDictionary
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateDictionaryWrapper(dictionary)
end

---@param listType : CS.System.Type
---@param populateList : CS.System.Action
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.CollectionUtils.CreateAndPopulateList(listType, populateList)
end

---@param initial : CS.System.Array
---@param type : CS.System.Type
---@return CS.System.Array
function CS.Newtonsoft.Json.Utilities.CollectionUtils.ToArray(initial, type)
end

---@param values : CS.System.Collections.IList
---@param type : CS.System.Type
---@param rank : CS.System.Int32
---@return CS.System.Array
function CS.Newtonsoft.Json.Utilities.CollectionUtils.ToMultidimensionalArray(values, type, rank)
end