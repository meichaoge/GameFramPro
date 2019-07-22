---@class CS.Newtonsoft.Json.Converters.XmlNodeWrapper : CS.System.Object
CS.Newtonsoft.Json.Converters.XmlNodeWrapper = {}

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.WrappedNode : CS.System.Object
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.WrappedNode = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.NodeType : CS.System.Xml.XmlNodeType
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.NodeType = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Name : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Name = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.LocalName : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.LocalName = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.ChildNodes : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.ChildNodes = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Attributes : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Attributes = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.ParentNode : CS.Newtonsoft.Json.Converters.IXmlNode
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.ParentNode = nil

---@property readwrite CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Value : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Value = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Prefix : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.Prefix = nil

---@property readonly CS.Newtonsoft.Json.Converters.XmlNodeWrapper.NamespaceURI : CS.System.String
CS.Newtonsoft.Json.Converters.XmlNodeWrapper.NamespaceURI = nil

---@param node : CS.System.Xml.XmlNode
---@return CS.Newtonsoft.Json.Converters.XmlNodeWrapper
function CS.Newtonsoft.Json.Converters.XmlNodeWrapper(node)
end

---@param newChild : CS.Newtonsoft.Json.Converters.IXmlNode
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.XmlNodeWrapper:AppendChild(newChild)
end