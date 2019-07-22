---@class CS.Newtonsoft.Json.Utilities.MiscellaneousUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.MiscellaneousUtils = {}

---@param objA : CS.System.Object
---@param objB : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.ValueEquals(objA, objB)
end

---@param paramName : CS.System.String
---@param actualValue : CS.System.Object
---@param message : CS.System.String
---@return CS.System.ArgumentOutOfRangeException
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.CreateArgumentOutOfRangeException(paramName, actualValue, message)
end

---@param value : CS.System.Object
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.ToString(value)
end

---@param hex : CS.System.String
---@return CS.System.Byte[]
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.HexToBytes(hex)
end

---@param bytes : CS.System.Byte[]
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.BytesToHex(bytes)
end

---@param bytes : CS.System.Byte[]
---@param removeDashes : CS.System.Boolean
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.BytesToHex(bytes, removeDashes)
end

---@param a1 : CS.System.Byte[]
---@param a2 : CS.System.Byte[]
---@return CS.System.Int32
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.ByteArrayCompare(a1, a2)
end

---@param qualifiedName : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.GetPrefix(qualifiedName)
end

---@param qualifiedName : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.GetLocalName(qualifiedName)
end

---@param qualifiedName : CS.System.String
---@param prefix : CS.System.String
---@param localName : CS.System.String
function CS.Newtonsoft.Json.Utilities.MiscellaneousUtils.GetQualifiedNameParts(qualifiedName, prefix, localName)
end