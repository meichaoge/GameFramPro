---@class CS.Newtonsoft.Json.Utilities.ValidationUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.ValidationUtils = {}

---@field public CS.Newtonsoft.Json.Utilities.ValidationUtils.EmailAddressRegex : CS.System.String
CS.Newtonsoft.Json.Utilities.ValidationUtils.EmailAddressRegex = nil

---@field public CS.Newtonsoft.Json.Utilities.ValidationUtils.CurrencyRegex : CS.System.String
CS.Newtonsoft.Json.Utilities.ValidationUtils.CurrencyRegex = nil

---@field public CS.Newtonsoft.Json.Utilities.ValidationUtils.DateRegex : CS.System.String
CS.Newtonsoft.Json.Utilities.ValidationUtils.DateRegex = nil

---@field public CS.Newtonsoft.Json.Utilities.ValidationUtils.NumericRegex : CS.System.String
CS.Newtonsoft.Json.Utilities.ValidationUtils.NumericRegex = nil

---@param value : CS.System.String
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNullOrEmpty(value, parameterName)
end

---@param value : CS.System.String
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNullOrEmptyOrWhitespace(value, parameterName)
end

---@param enumType : CS.System.Type
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentTypeIsEnum(enumType, parameterName)
end

---@param collection : CS.System.Collections.ICollection
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNullOrEmpty(collection, parameterName)
end

---@param collection : CS.System.Collections.ICollection
---@param parameterName : CS.System.String
---@param message : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNullOrEmpty(collection, parameterName, message)
end

---@param value : CS.System.Object
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNull(value, parameterName)
end

---@param value : CS.System.Int32
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNegative(value, parameterName)
end

---@param value : CS.System.Int32
---@param parameterName : CS.System.String
---@param message : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotNegative(value, parameterName, message)
end

---@param value : CS.System.Int32
---@param parameterName : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotZero(value, parameterName)
end

---@param value : CS.System.Int32
---@param parameterName : CS.System.String
---@param message : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentNotZero(value, parameterName, message)
end

---@param value : CS.System.Int32
---@param parameterName : CS.System.String
---@param message : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentIsPositive(value, parameterName, message)
end

---@param disposed : CS.System.Boolean
---@param objectType : CS.System.Type
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ObjectNotDisposed(disposed, objectType)
end

---@param condition : CS.System.Boolean
---@param parameterName : CS.System.String
---@param message : CS.System.String
function CS.Newtonsoft.Json.Utilities.ValidationUtils.ArgumentConditionTrue(condition, parameterName, message)
end