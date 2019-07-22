---@class CS.UnityEngine.Rendering.GraphicsSettings : CS.UnityEngine.Object
CS.UnityEngine.Rendering.GraphicsSettings = {}

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.transparencySortMode : CS.UnityEngine.TransparencySortMode
CS.UnityEngine.Rendering.GraphicsSettings.transparencySortMode = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.transparencySortAxis : CS.UnityEngine.Vector3
CS.UnityEngine.Rendering.GraphicsSettings.transparencySortAxis = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.realtimeDirectRectangularAreaLights : CS.System.Boolean
CS.UnityEngine.Rendering.GraphicsSettings.realtimeDirectRectangularAreaLights = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity : CS.System.Boolean
CS.UnityEngine.Rendering.GraphicsSettings.lightsUseLinearIntensity = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.lightsUseColorTemperature : CS.System.Boolean
CS.UnityEngine.Rendering.GraphicsSettings.lightsUseColorTemperature = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.useScriptableRenderPipelineBatching : CS.System.Boolean
CS.UnityEngine.Rendering.GraphicsSettings.useScriptableRenderPipelineBatching = nil

---@property readwrite CS.UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset : CS.UnityEngine.Rendering.RenderPipelineAsset
CS.UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset = nil

---@param tier : CS.UnityEngine.Rendering.GraphicsTier
---@param defineHash : CS.UnityEngine.Rendering.BuiltinShaderDefine
---@return CS.System.Boolean
function CS.UnityEngine.Rendering.GraphicsSettings.HasShaderDefine(tier, defineHash)
end

---@param defineHash : CS.UnityEngine.Rendering.BuiltinShaderDefine
---@return CS.System.Boolean
function CS.UnityEngine.Rendering.GraphicsSettings.HasShaderDefine(defineHash)
end

---@param type : CS.UnityEngine.Rendering.BuiltinShaderType
---@param mode : CS.UnityEngine.Rendering.BuiltinShaderMode
function CS.UnityEngine.Rendering.GraphicsSettings.SetShaderMode(type, mode)
end

---@param type : CS.UnityEngine.Rendering.BuiltinShaderType
---@return CS.UnityEngine.Rendering.BuiltinShaderMode
function CS.UnityEngine.Rendering.GraphicsSettings.GetShaderMode(type)
end

---@param type : CS.UnityEngine.Rendering.BuiltinShaderType
---@param shader : CS.UnityEngine.Shader
function CS.UnityEngine.Rendering.GraphicsSettings.SetCustomShader(type, shader)
end

---@param type : CS.UnityEngine.Rendering.BuiltinShaderType
---@return CS.UnityEngine.Shader
function CS.UnityEngine.Rendering.GraphicsSettings.GetCustomShader(type)
end