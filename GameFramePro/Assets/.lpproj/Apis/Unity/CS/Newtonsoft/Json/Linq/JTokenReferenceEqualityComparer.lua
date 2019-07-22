---@class CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer : CS.System.Object
CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer = {}

---@field public CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer.Instance : CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer
CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer.Instance = nil

---@return CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer
function CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer()
end

---@param x : CS.Newtonsoft.Json.Linq.JToken
---@param y : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Boolean
function CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer:Equals(x, y)
end

---@param obj : CS.Newtonsoft.Json.Linq.JToken
---@return CS.System.Int32
function CS.Newtonsoft.Json.Linq.JTokenReferenceEqualityComparer:GetHashCode(obj)
end