---@class CS.Newtonsoft.Json.Converters.XmlElementWrapper : CS.Newtonsoft.Json.Converters.XmlNodeWrapper
CS.Newtonsoft.Json.Converters.XmlElementWrapper = {}

---@param element : CS.System.Xml.XmlElement
---@return CS.Newtonsoft.Json.Converters.XmlElementWrapper
function CS.Newtonsoft.Json.Converters.XmlElementWrapper(element)
end

---@param attribute : CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlElementWrapper:SetAttributeNode(attribute)
end

---@param namespaceURI : CS.System.String
---@return CS.System.String
function CS.Newtonsoft.Json.Converters.XmlElementWrapper:GetPrefixOfNamespace(namespaceURI)
end