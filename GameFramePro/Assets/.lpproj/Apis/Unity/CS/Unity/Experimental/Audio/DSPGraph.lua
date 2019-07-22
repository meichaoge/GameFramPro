---@class CS.Unity.Experimental.Audio.DSPGraph : CS.System.ValueType
CS.Unity.Experimental.Audio.DSPGraph = {}

---@param outputFormat : CS.Unity.Experimental.Audio.SoundFormat
---@param outputChannels : CS.System.UInt32
---@param dspBufferSize : CS.System.UInt32
---@param sampleRate : CS.System.UInt32
---@return CS.Unity.Experimental.Audio.DSPGraph
function CS.Unity.Experimental.Audio.DSPGraph.Create(outputFormat, outputChannels, dspBufferSize, sampleRate)
end

---@return CS.Unity.Experimental.Audio.DSPGraph
function CS.Unity.Experimental.Audio.DSPGraph.GetDefaultGraph()
end

function CS.Unity.Experimental.Audio.DSPGraph:Dispose()
end

---@return CS.Unity.Experimental.Audio.DSPCommandBlock
function CS.Unity.Experimental.Audio.DSPGraph:CreateCommandBlock()
end

---@return CS.Unity.Experimental.Audio.DSPNode
function CS.Unity.Experimental.Audio.DSPGraph:GetRootDSP()
end

---@return CS.System.UInt64
function CS.Unity.Experimental.Audio.DSPGraph:GetDSPClock()
end

function CS.Unity.Experimental.Audio.DSPGraph:BeginMix()
end

---@param buffer : CS.Unity.Collections.NativeArray
function CS.Unity.Experimental.Audio.DSPGraph:ReadMix(buffer)
end

---@param handlerId : CS.System.UInt32
---@return CS.System.Boolean
function CS.Unity.Experimental.Audio.DSPGraph:RemoveNodeEventHandler(handlerId)
end

function CS.Unity.Experimental.Audio.DSPGraph:Update()
end