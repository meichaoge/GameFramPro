---@class CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection
CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection = {}

---@param messageId : CS.System.Guid
---@param callback : CS.UnityEngine.Events.UnityAction
function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:Register(messageId, callback)
end

---@param messageId : CS.System.Guid
---@param callback : CS.UnityEngine.Events.UnityAction
function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:Unregister(messageId, callback)
end

function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:DisconnectAll()
end

---@param callback : CS.UnityEngine.Events.UnityAction
function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:RegisterConnection(callback)
end

---@param callback : CS.UnityEngine.Events.UnityAction
function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:RegisterDisconnection(callback)
end

---@param messageId : CS.System.Guid
---@param data : CS.System.Byte[]
function CS.UnityEngine.Networking.PlayerConnection.IEditorPlayerConnection:Send(messageId, data)
end