---@class CS.RectTransform_Ex : CS.System.Object
CS.RectTransform_Ex = {}

---@param target : CS.UnityEngine.RectTransform
function CS.RectTransform_Ex.ResetRectTransProperty(target)
end

---@param target : CS.UnityEngine.RectTransform
---@param anchorPos : CS.UnityEngine.Vector2
function CS.RectTransform_Ex.ResetRectTransProperty(target, anchorPos)
end

---@param target : CS.UnityEngine.RectTransform
---@param anchorPos : CS.UnityEngine.Vector2
---@param size : CS.UnityEngine.Vector2
function CS.RectTransform_Ex.ResetRectTransProperty(target, anchorPos, size)
end

---@param target : CS.UnityEngine.RectTransform
---@param index : CS.System.Int32
---@return CS.UnityEngine.RectTransform
function CS.RectTransform_Ex.GetChildEX(target, index)
end

---@param trans : CS.UnityEngine.RectTransform
---@param c : CS.UnityEngine.Canvas
---@return CS.UnityEngine.Rect
function CS.RectTransform_Ex.GetCanvasRect(trans, c)
end

---@param trans : CS.UnityEngine.RectTransform
---@param relativeTrans : CS.UnityEngine.RectTransform
---@param canvas : CS.UnityEngine.Canvas
---@return CS.UnityEngine.Rect
function CS.RectTransform_Ex.GetRelativeRect(trans, relativeTrans, canvas)
end