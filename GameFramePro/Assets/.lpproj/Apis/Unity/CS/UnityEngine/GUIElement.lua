---@class CS.UnityEngine.GUIElement : CS.UnityEngine.Behaviour
CS.UnityEngine.GUIElement = {}

---@return CS.UnityEngine.GUIElement
function CS.UnityEngine.GUIElement()
end

---@param screenPosition : CS.UnityEngine.Vector3
---@return CS.System.Boolean
function CS.UnityEngine.GUIElement:HitTest(screenPosition)
end

---@param screenPosition : CS.UnityEngine.Vector3
---@param camera : CS.UnityEngine.Camera
---@return CS.System.Boolean
function CS.UnityEngine.GUIElement:HitTest(screenPosition, camera)
end

---@param camera : CS.UnityEngine.Camera
---@return CS.UnityEngine.Rect
function CS.UnityEngine.GUIElement:GetScreenRect(camera)
end

---@return CS.UnityEngine.Rect
function CS.UnityEngine.GUIElement:GetScreenRect()
end