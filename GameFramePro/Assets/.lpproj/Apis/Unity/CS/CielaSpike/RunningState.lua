---@class CS.CielaSpike.RunningState : CS.System.Enum
CS.CielaSpike.RunningState = {}

---@field public CS.CielaSpike.RunningState.value__ : CS.System.Int32
CS.CielaSpike.RunningState.value__ = nil

---@field public CS.CielaSpike.RunningState.Init : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.Init = nil

---@field public CS.CielaSpike.RunningState.RunningAsync : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.RunningAsync = nil

---@field public CS.CielaSpike.RunningState.PendingYield : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.PendingYield = nil

---@field public CS.CielaSpike.RunningState.ToBackground : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.ToBackground = nil

---@field public CS.CielaSpike.RunningState.RunningSync : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.RunningSync = nil

---@field public CS.CielaSpike.RunningState.CancellationRequested : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.CancellationRequested = nil

---@field public CS.CielaSpike.RunningState.Done : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.Done = nil

---@field public CS.CielaSpike.RunningState.Error : CS.CielaSpike.RunningState
CS.CielaSpike.RunningState.Error = nil