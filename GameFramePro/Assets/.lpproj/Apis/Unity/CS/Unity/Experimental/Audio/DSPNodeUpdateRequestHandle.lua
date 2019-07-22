---@class CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle : CS.System.ValueType
CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle = {}

---@field public CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.m_Handle : CS.Unity.Experimental.Audio.AtomicAudioNode
CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.m_Handle = nil

---@field public CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.m_Graph : CS.Unity.Experimental.Audio.AtomicAudioNode
CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.m_Graph = nil

---@param requestHandle : CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle
---@return CS.System.VoidPointer
function CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.Internal_GetUpdateJobData(requestHandle)
end

---@param requestHandle : CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle
---@return CS.System.Boolean
function CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.Internal_HasError(requestHandle)
end

---@param requestHandle : CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle
---@param node : CS.Unity.Experimental.Audio.DSPNode
function CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.Internal_GetDSPNode(requestHandle, node)
end

---@param requestHandle : CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle
---@param fence : CS.Unity.Jobs.JobHandle
function CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.Internal_GetFence(requestHandle, fence)
end

---@param requestHandle : CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle
function CS.Unity.Experimental.Audio.DSPNodeUpdateRequestHandle.Internal_Dispose(requestHandle)
end