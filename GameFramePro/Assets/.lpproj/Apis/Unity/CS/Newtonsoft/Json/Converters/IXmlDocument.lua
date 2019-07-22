---@class CS.Newtonsoft.Json.Converters.IXmlDocument
CS.Newtonsoft.Json.Converters.IXmlDocument = {}

---@property readonly CS.Newtonsoft.Json.Converters.IXmlDocument.DocumentElement : CS.Newtonsoft.Json.Converters.IXmlElement
CS.Newtonsoft.Json.Converters.IXmlDocument.DocumentElement = nil

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateComment(text)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateTextNode(text)
end

---@param data : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateCDataSection(data)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateWhitespace(text)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateSignificantWhitespace(text)
end

---@param version : CS.System.String
---@param encoding : CS.System.String
---@param standalone : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateXmlDeclaration(version, encoding, standalone)
end

---@param target : CS.System.String
---@param data : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateProcessingInstruction(target, data)
end

---@param elementName : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlElement
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateElement(elementName)
end

---@param qualifiedName : CS.System.String
---@param namespaceURI : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlElement
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateElement(qualifiedName, namespaceURI)
end

---@param name : CS.System.String
---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateAttribute(name, value)
end

---@param qualifiedName : CS.System.String
---@param namespaceURI : CS.System.String
---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlDocument:CreateAttribute(qualifiedName, namespaceURI, value)
end