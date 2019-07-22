---@class CS.UnityEngine.Cubemap : CS.UnityEngine.Texture
CS.UnityEngine.Cubemap = {}

---@property readonly CS.UnityEngine.Cubemap.mipmapCount : CS.System.Int32
CS.UnityEngine.Cubemap.mipmapCount = nil

---@property readonly CS.UnityEngine.Cubemap.format : CS.UnityEngine.TextureFormat
CS.UnityEngine.Cubemap.format = nil

---@property readonly CS.UnityEngine.Cubemap.isReadable : CS.System.Boolean
CS.UnityEngine.Cubemap.isReadable = nil

---@param width : CS.System.Int32
---@param format : CS.UnityEngine.Experimental.Rendering.DefaultFormat
---@param flags : CS.UnityEngine.Experimental.Rendering.TextureCreationFlags
---@return CS.UnityEngine.Cubemap
function CS.UnityEngine.Cubemap(width, format, flags)
end

---@param width : CS.System.Int32
---@param format : CS.UnityEngine.Experimental.Rendering.GraphicsFormat
---@param flags : CS.UnityEngine.Experimental.Rendering.TextureCreationFlags
---@return CS.UnityEngine.Cubemap
function CS.UnityEngine.Cubemap(width, format, flags)
end

---@param width : CS.System.Int32
---@param textureFormat : CS.UnityEngine.TextureFormat
---@param mipChain : CS.System.Boolean
---@return CS.UnityEngine.Cubemap
function CS.UnityEngine.Cubemap(width, textureFormat, mipChain)
end

---@param nativeTexture : CS.System.IntPtr
function CS.UnityEngine.Cubemap:UpdateExternalTexture(nativeTexture)
end

---@param smoothRegionWidthInPixels : CS.System.Int32
function CS.UnityEngine.Cubemap:SmoothEdges(smoothRegionWidthInPixels)
end

function CS.UnityEngine.Cubemap:SmoothEdges()
end

---@param face : CS.UnityEngine.CubemapFace
---@param miplevel : CS.System.Int32
---@return CS.UnityEngine.Color[]
function CS.UnityEngine.Cubemap:GetPixels(face, miplevel)
end

---@param face : CS.UnityEngine.CubemapFace
---@return CS.UnityEngine.Color[]
function CS.UnityEngine.Cubemap:GetPixels(face)
end

---@param colors : CS.UnityEngine.Color[]
---@param face : CS.UnityEngine.CubemapFace
---@param miplevel : CS.System.Int32
function CS.UnityEngine.Cubemap:SetPixels(colors, face, miplevel)
end

---@param colors : CS.UnityEngine.Color[]
---@param face : CS.UnityEngine.CubemapFace
function CS.UnityEngine.Cubemap:SetPixels(colors, face)
end

---@param width : CS.System.Int32
---@param format : CS.UnityEngine.TextureFormat
---@param mipmap : CS.System.Boolean
---@param nativeTex : CS.System.IntPtr
---@return CS.UnityEngine.Cubemap
function CS.UnityEngine.Cubemap.CreateExternalTexture(width, format, mipmap, nativeTex)
end

---@param face : CS.UnityEngine.CubemapFace
---@param x : CS.System.Int32
---@param y : CS.System.Int32
---@param color : CS.UnityEngine.Color
function CS.UnityEngine.Cubemap:SetPixel(face, x, y, color)
end

---@param face : CS.UnityEngine.CubemapFace
---@param x : CS.System.Int32
---@param y : CS.System.Int32
---@return CS.UnityEngine.Color
function CS.UnityEngine.Cubemap:GetPixel(face, x, y)
end

---@param updateMipmaps : CS.System.Boolean
---@param makeNoLongerReadable : CS.System.Boolean
function CS.UnityEngine.Cubemap:Apply(updateMipmaps, makeNoLongerReadable)
end

---@param updateMipmaps : CS.System.Boolean
function CS.UnityEngine.Cubemap:Apply(updateMipmaps)
end

function CS.UnityEngine.Cubemap:Apply()
end