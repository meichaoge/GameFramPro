---@class CS.Newtonsoft.Json.Converters.XmlDocumentWrapper : CS.Newtonsoft.Json.Converters.XmlNodeWrapper
CS.Newtonsoft.Json.Converters.XmlDocumentWrapper = {}

---@property readonly CS.Newtonsoft.Json.Converters.XmlDocumentWrapper.DocumentElement : CS.Newtonsoft.Json.Converters.IXmlElement
CS.Newtonsoft.Json.Converters.XmlDocumentWrapper.DocumentElement = nil

---@param document : CS.System.Xml.XmlDocument
---@return CS.Newtonsoft.Json.Converters.XmlDocumentWrapper
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper(document)
end

---@param data : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateComment(data)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateTextNode(text)
end

---@param data : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateCDataSection(data)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateWhitespace(text)
end

---@param text : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateSignificantWhitespace(text)
end

---@param version : CS.System.String
---@param encoding : CS.System.String
---@param standalone : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateXmlDeclaration(version, encoding, standalone)
end

---@param target : CS.System.String
---@param data : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateProcessingInstruction(target, data)
end

---@param elementName : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlElement
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateElement(elementName)
end

---@param qualifiedName : CS.System.String
---@param namespaceURI : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlElement
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateElement(qualifiedName, namespaceURI)
end

---@param name : CS.System.String
---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateAttribute(name, value)
end

---@param qualifiedName : CS.System.String
---@param namespaceURI : CS.System.String
---@param value : CS.System.String
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlDocumentWrapper:CreateAttribute(qualifiedName, namespaceURI, value)
end