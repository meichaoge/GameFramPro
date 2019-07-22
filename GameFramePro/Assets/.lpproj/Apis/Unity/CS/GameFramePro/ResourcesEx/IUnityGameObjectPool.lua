---@class CS.GameFramePro.ResourcesEx.IUnityGameObjectPool
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool = {}

---@property readonly CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.BeforGetAction : CS.System.Action
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.BeforGetAction = nil

---@property readonly CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.BeforeRecycleAction : CS.System.Action
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.BeforeRecycleAction = nil

---@property readonly CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PoolContainer : CS.System.Collections.Generic.Stack
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PoolContainer = nil

---@property readonly CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PrefabTarget : CS.UnityEngine.GameObject
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PrefabTarget = nil

---@property readonly CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PoolManagerTarget : CS.UnityEngine.Transform
CS.GameFramePro.ResourcesEx.IUnityGameObjectPool.PoolManagerTarget = nil

---@param capacity : CS.System.Int32
---@param prefabTarget : CS.UnityEngine.GameObject
---@param beforGetAction : CS.System.Action
---@param beforeRecycleAction : CS.System.Action
function CS.GameFramePro.ResourcesEx.IUnityGameObjectPool:InitialedPool(capacity, prefabTarget, beforGetAction, beforeRecycleAction)
end

function CS.GameFramePro.ResourcesEx.IUnityGameObjectPool:ReleasPool()
end

---@return CS.UnityEngine.GameObject
function CS.GameFramePro.ResourcesEx.IUnityGameObjectPool:GetItemFromPool()
end

---@param item : CS.UnityEngine.GameObject
function CS.GameFramePro.ResourcesEx.IUnityGameObjectPool:RecycleItemToPool(item)
end