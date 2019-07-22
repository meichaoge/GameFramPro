---@class CS.Newtonsoft.Json.Linq.JPropertyDescriptor : CS.System.ComponentModel.PropertyDescriptor
CS.Newtonsoft.Json.Linq.JPropertyDescriptor = {}

---@property readonly CS.Newtonsoft.Json.Linq.JPropertyDescriptor.ComponentType : CS.System.Type
CS.Newtonsoft.Json.Linq.JPropertyDescriptor.ComponentType = nil

---@property readonly CS.Newtonsoft.Json.Linq.JPropertyDescriptor.IsReadOnly : CS.System.Boolean
CS.Newtonsoft.Json.Linq.JPropertyDescriptor.IsReadOnly = nil

---@property readonly CS.Newtonsoft.Json.Linq.JPropertyDescriptor.PropertyType : CS.System.Type
CS.Newtonsoft.Json.Linq.JPropertyDescriptor.PropertyType = nil

---@param name : CS.System.String
---@param propertyType : CS.System.Type
---@return CS.Newtonsoft.Json.Linq.JPropertyDescriptor
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor(name, propertyType)
end

---@param component : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor:CanResetValue(component)
end

---@param component : CS.System.Object
---@return CS.System.Object
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor:GetValue(component)
end

---@param component : CS.System.Object
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor:ResetValue(component)
end

---@param component : CS.System.Object
---@param value : CS.System.Object
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor:SetValue(component, value)
end

---@param component : CS.System.Object
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JPropertyDescriptor:ShouldSerializeValue(component)
end