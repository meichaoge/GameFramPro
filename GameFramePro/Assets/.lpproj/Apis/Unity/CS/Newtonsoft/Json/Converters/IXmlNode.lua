---@class CS.Newtonsoft.Json.Converters.IXmlNode
CS.Newtonsoft.Json.Converters.IXmlNode = {}

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.NodeType : CS.System.Xml.XmlNodeType
CS.Newtonsoft.Json.Converters.IXmlNode.NodeType = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.LocalName : CS.System.String
CS.Newtonsoft.Json.Converters.IXmlNode.LocalName = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.ChildNodes : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Converters.IXmlNode.ChildNodes = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.Attributes : CS.System.Collections.Generic.IList
CS.Newtonsoft.Json.Converters.IXmlNode.Attributes = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.ParentNode : CS.Newtonsoft.Json.Converters.IXmlNode
CS.Newtonsoft.Json.Converters.IXmlNode.ParentNode = nil

---@property readwrite CS.Newtonsoft.Json.Converters.IXmlNode.Value : CS.System.String
CS.Newtonsoft.Json.Converters.IXmlNode.Value = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.NamespaceURI : CS.System.String
CS.Newtonsoft.Json.Converters.IXmlNode.NamespaceURI = nil

---@property readonly CS.Newtonsoft.Json.Converters.IXmlNode.WrappedNode : CS.System.Object
CS.Newtonsoft.Json.Converters.IXmlNode.WrappedNode = nil

---@param newChild : CS.Newtonsoft.Json.Converters.IXmlNode
---@return CS.Newtonsoft.Json.Converters.IXmlNode
function CS.Newtonsoft.Json.Converters.IXmlNode:AppendChild(newChild)
end