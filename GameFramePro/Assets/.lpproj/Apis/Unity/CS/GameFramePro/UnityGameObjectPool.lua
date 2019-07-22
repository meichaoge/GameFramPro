---@class CS.GameFramePro.UnityGameObjectPool : CS.System.Object
CS.GameFramePro.UnityGameObjectPool = {}

---@property readwrite CS.GameFramePro.UnityGameObjectPool.BeforGetAction : CS.System.Action
CS.GameFramePro.UnityGameObjectPool.BeforGetAction = nil

---@property readwrite CS.GameFramePro.UnityGameObjectPool.BeforeRecycleAction : CS.System.Action
CS.GameFramePro.UnityGameObjectPool.BeforeRecycleAction = nil

---@property readwrite CS.GameFramePro.UnityGameObjectPool.PoolContainer : CS.System.Collections.Generic.Stack
CS.GameFramePro.UnityGameObjectPool.PoolContainer = nil

---@property readwrite CS.GameFramePro.UnityGameObjectPool.PrefabTarget : CS.UnityEngine.GameObject
CS.GameFramePro.UnityGameObjectPool.PrefabTarget = nil

---@property readonly CS.GameFramePro.UnityGameObjectPool.PoolManagerTarget : CS.UnityEngine.Transform
CS.GameFramePro.UnityGameObjectPool.PoolManagerTarget = nil

---@return CS.GameFramePro.UnityGameObjectPool
function CS.GameFramePro.UnityGameObjectPool()
end

---@param capacity : CS.System.Int32
---@param prefabTarget : CS.UnityEngine.GameObject
---@param beforGetAction : CS.System.Action
---@param beforeRecycleAction : CS.System.Action
function CS.GameFramePro.UnityGameObjectPool:InitialedPool(capacity, prefabTarget, beforGetAction, beforeRecycleAction)
end

function CS.GameFramePro.UnityGameObjectPool:ReleasPool()
end

---@return CS.UnityEngine.GameObject
function CS.GameFramePro.UnityGameObjectPool:GetItemFromPool()
end

---@param item : CS.UnityEngine.GameObject
function CS.GameFramePro.UnityGameObjectPool:RecycleItemToPool(item)
end