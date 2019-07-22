---@class CS.Newtonsoft.Json.Utilities.StringUtils : CS.System.Object
CS.Newtonsoft.Json.Utilities.StringUtils = {}

---@field public CS.Newtonsoft.Json.Utilities.StringUtils.CarriageReturnLineFeed : CS.System.String
CS.Newtonsoft.Json.Utilities.StringUtils.CarriageReturnLineFeed = nil

---@field public CS.Newtonsoft.Json.Utilities.StringUtils.Empty : CS.System.String
CS.Newtonsoft.Json.Utilities.StringUtils.Empty = nil

---@field public CS.Newtonsoft.Json.Utilities.StringUtils.CarriageReturn : CS.System.Char
CS.Newtonsoft.Json.Utilities.StringUtils.CarriageReturn = nil

---@field public CS.Newtonsoft.Json.Utilities.StringUtils.LineFeed : CS.System.Char
CS.Newtonsoft.Json.Utilities.StringUtils.LineFeed = nil

---@field public CS.Newtonsoft.Json.Utilities.StringUtils.Tab : CS.System.Char
CS.Newtonsoft.Json.Utilities.StringUtils.Tab = nil

---@param format : CS.System.String
---@param provider : CS.System.IFormatProvider
---@param args : CS.System.Object[]
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.FormatWith(format, provider, args)
end

---@param s : CS.System.String
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.StringUtils.ContainsWhiteSpace(s)
end

---@param s : CS.System.String
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.StringUtils.IsWhiteSpace(s)
end

---@param target : CS.System.String
---@param value : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.EnsureEndsWith(target, value)
end

---@param s : CS.System.String
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Utilities.StringUtils.IsNullOrEmptyOrWhiteSpace(s)
end

---@param value : CS.System.String
---@param action : CS.System.Action
function CS.Newtonsoft.Json.Utilities.StringUtils.IfNotNullOrEmpty(value, action)
end

---@param s : CS.System.String
---@param indentation : CS.System.Int32
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.Indent(s, indentation)
end

---@param s : CS.System.String
---@param indentation : CS.System.Int32
---@param indentChar : CS.System.Char
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.Indent(s, indentation, indentChar)
end

---@param s : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.NumberLines(s)
end

---@param s : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.NullEmptyString(s)
end

---@param s : CS.System.String
---@param replacement : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.ReplaceNewLines(s, replacement)
end

---@param s : CS.System.String
---@param maximumLength : CS.System.Int32
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.Truncate(s, maximumLength)
end

---@param s : CS.System.String
---@param maximumLength : CS.System.Int32
---@param suffix : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.Truncate(s, maximumLength, suffix)
end

---@param capacity : CS.System.Int32
---@return CS.System.IO.StringWriter
function CS.Newtonsoft.Json.Utilities.StringUtils.CreateStringWriter(capacity)
end

---@param value : CS.System.String
---@return CS.System.Nullable
function CS.Newtonsoft.Json.Utilities.StringUtils.GetLength(value)
end

---@param c : CS.System.Char
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.ToCharAsUnicode(c)
end

---@param writer : CS.System.IO.TextWriter
---@param c : CS.System.Char
function CS.Newtonsoft.Json.Utilities.StringUtils.WriteCharAsUnicode(writer, c)
end

---@param s : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Utilities.StringUtils.ToCamelCase(s)
end