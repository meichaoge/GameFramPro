---@class CS.Unity.Experimental.Audio.SampleProvider : CS.System.ValueType
CS.Unity.Experimental.Audio.SampleProvider = {}

---@property readonly CS.Unity.Experimental.Audio.SampleProvider.Valid : CS.System.Boolean
CS.Unity.Experimental.Audio.SampleProvider.Valid = nil

---@property readonly CS.Unity.Experimental.Audio.SampleProvider.NativeFormat : CS.Unity.Experimental.Audio.NativeFormatType
CS.Unity.Experimental.Audio.SampleProvider.NativeFormat = nil

---@property readonly CS.Unity.Experimental.Audio.SampleProvider.ChannelCount : CS.System.UInt16
CS.Unity.Experimental.Audio.SampleProvider.ChannelCount = nil

---@property readonly CS.Unity.Experimental.Audio.SampleProvider.SampleRate : CS.System.UInt32
CS.Unity.Experimental.Audio.SampleProvider.SampleRate = nil

---@param destination : CS.Unity.Collections.NativeSlice
---@param format : CS.Unity.Experimental.Audio.NativeFormatType
---@return CS.System.Int32
function CS.Unity.Experimental.Audio.SampleProvider:Read(destination, format)
end

---@param destination : CS.Unity.Collections.NativeSlice
---@param format : CS.Unity.Experimental.Audio.NativeFormatType
---@return CS.System.Int32
function CS.Unity.Experimental.Audio.SampleProvider:Read(destination, format)
end

---@param destination : CS.Unity.Collections.NativeSlice
---@return CS.System.Int32
function CS.Unity.Experimental.Audio.SampleProvider:Read(destination)
end

function CS.Unity.Experimental.Audio.SampleProvider:Release()
end