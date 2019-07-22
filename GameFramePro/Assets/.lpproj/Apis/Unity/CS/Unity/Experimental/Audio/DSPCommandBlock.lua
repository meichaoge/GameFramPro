---@class CS.Unity.Experimental.Audio.DSPCommandBlock : CS.System.ValueType
CS.Unity.Experimental.Audio.DSPCommandBlock = {}

---@param node : CS.Unity.Experimental.Audio.DSPNode
function CS.Unity.Experimental.Audio.DSPCommandBlock:ReleaseDSPNode(node)
end

---@param output : CS.Unity.Experimental.Audio.DSPNode
---@param outputPort : CS.System.Int32
---@param input : CS.Unity.Experimental.Audio.DSPNode
---@param inputPort : CS.System.Int32
---@return CS.Unity.Experimental.Audio.DSPConnection
function CS.Unity.Experimental.Audio.DSPCommandBlock:Connect(output, outputPort, input, inputPort)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
function CS.Unity.Experimental.Audio.DSPCommandBlock:Disconnect(connection)
end

---@param output : CS.Unity.Experimental.Audio.DSPNode
---@param outputPort : CS.System.Int32
---@param input : CS.Unity.Experimental.Audio.DSPNode
---@param inputPort : CS.System.Int32
function CS.Unity.Experimental.Audio.DSPCommandBlock:Disconnect(output, outputPort, input, inputPort)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param value : CS.System.Single
---@param interpolationLength : CS.System.UInt32
function CS.Unity.Experimental.Audio.DSPCommandBlock:SetAttenuation(connection, value, interpolationLength)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
---@param interpolationLength : CS.System.UInt32
function CS.Unity.Experimental.Audio.DSPCommandBlock:SetAttenuation(connection, value1, value2, interpolationLength)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
---@param value3 : CS.System.Single
---@param interpolationLength : CS.System.UInt32
function CS.Unity.Experimental.Audio.DSPCommandBlock:SetAttenuation(connection, value1, value2, value3, interpolationLength)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
---@param value3 : CS.System.Single
---@param value4 : CS.System.Single
---@param interpolationLength : CS.System.UInt32
function CS.Unity.Experimental.Audio.DSPCommandBlock:SetAttenuation(connection, value1, value2, value3, value4, interpolationLength)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param value : CS.System.SinglePointer
---@param dimension : CS.System.Byte
---@param interpolationLength : CS.System.UInt32
function CS.Unity.Experimental.Audio.DSPCommandBlock:SetAttenuation(connection, value, dimension, interpolationLength)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
---@param value : CS.System.Single
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddAttenuationKey(connection, dspClock, value)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddAttenuationKey(connection, dspClock, value1, value2)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
---@param value3 : CS.System.Single
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddAttenuationKey(connection, dspClock, value1, value2, value3)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
---@param value1 : CS.System.Single
---@param value2 : CS.System.Single
---@param value3 : CS.System.Single
---@param value4 : CS.System.Single
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddAttenuationKey(connection, dspClock, value1, value2, value3, value4)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
---@param value : CS.System.SinglePointer
---@param dimension : CS.System.Byte
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddAttenuationKey(connection, dspClock, value, dimension)
end

---@param connection : CS.Unity.Experimental.Audio.DSPConnection
---@param dspClock : CS.System.UInt64
function CS.Unity.Experimental.Audio.DSPCommandBlock:SustainAttenuation(connection, dspClock)
end

---@param node : CS.Unity.Experimental.Audio.DSPNode
---@param channelCount : CS.System.Int32
---@param format : CS.Unity.Experimental.Audio.SoundFormat
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddInletPort(node, channelCount, format)
end

---@param node : CS.Unity.Experimental.Audio.DSPNode
---@param channelCount : CS.System.Int32
---@param format : CS.Unity.Experimental.Audio.SoundFormat
function CS.Unity.Experimental.Audio.DSPCommandBlock:AddOutletPort(node, channelCount, format)
end

function CS.Unity.Experimental.Audio.DSPCommandBlock:Complete()
end