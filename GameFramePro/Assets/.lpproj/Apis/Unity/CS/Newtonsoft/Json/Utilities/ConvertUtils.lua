---@class CS.Newtonsoft.Json.Utilities.ConvertUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.ConvertUtils = {}

---@param initialType : CS.System.Type
---@param targetType : CS.System.Type
---@param allowTypeNameToString : CS.System.Boolean
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ConvertUtils.CanConvertType(initialType, targetType, allowTypeNameToString)
end

---@param initialValue : CS.System.Object
---@param culture : CS.System.Globalization.CultureInfo
---@param targetType : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ConvertUtils.Convert(initialValue, culture, targetType)
end

---@param initialValue : CS.System.Object
---@param culture : CS.System.Globalization.CultureInfo
---@param targetType : CS.System.Type
---@param convertedValue : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ConvertUtils.TryConvert(initialValue, culture, targetType, convertedValue)
end

---@param initialValue : CS.System.Object
---@param culture : CS.System.Globalization.CultureInfo
---@param targetType : CS.System.Type
---@return CS.System.Object
function CS.Newtonsoft.Json.Utilities.ConvertUtils.ConvertOrCast(initialValue, culture, targetType)
end

---@param initialValue : CS.System.Object
---@param culture : CS.System.Globalization.CultureInfo
---@param targetType : CS.System.Type
---@param convertedValue : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ConvertUtils.TryConvertOrCast(initialValue, culture, targetType, convertedValue)
end

---@param value : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.ConvertUtils.IsInteger(value)
end