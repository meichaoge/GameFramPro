using System.Collections;
using System.Collections.Generic;
using GameFramePro.ResourcesEx;
using UnityEngine;

namespace GameFramePro
{
    /// <summary>/// 缓存组件与  GameObjectReferenceAssetInfor 之间的关系，避免GetComponent/// </summary>
    public static class ReferenceAssetUtility
    {
        private  static readonly  Dictionary<UnityEngine.Object, GameObjectReferenceAssetInformation> mAllCacheComponentReferenceAssetInfors = new Dictionary<UnityEngine.Object, GameObjectReferenceAssetInformation>();

        public static void ReleaseGameObjectReference(UnityEngine.GameObject targetObject)
        {
            if (targetObject == null )
            {
#if UNITY_EDITOR
                Debug.LogEditorError($"ReleaseComponentReference Fail,参数为 null ");
#endif
                return;
            }

            if (mAllCacheComponentReferenceAssetInfors.TryGetValue(targetObject, out var assetInfor) == false)
            {
                assetInfor = targetObject  .GetComponent<GameObjectReferenceAssetInformation>();
                if (assetInfor == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"当前组件 {targetObject} 的对象{targetObject.name} 没有引用其他加载的资源");
#endif
                    return;
                }

                mAllCacheComponentReferenceAssetInfors[targetObject] = assetInfor;
            }

            assetInfor.RemoveObjectReference();
        }
        
        public static void ReleaseComponentReference(Component targetComponent)
        {
            if (targetComponent == null)
            {
#if UNITY_EDITOR
                Debug.LogEditorError($"ReleaseComponentReference Fail,参数为 null ");
#endif
                return;
            }

            if (mAllCacheComponentReferenceAssetInfors.TryGetValue(targetComponent, out var assetInfor) == false)
            {
                assetInfor = targetComponent.gameObject.GetComponent<GameObjectReferenceAssetInformation>();
                if (assetInfor == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"当前组件 {targetComponent} 的对象{targetComponent.gameObject.name} 没有引用其他加载的资源");
#endif
                    return;
                }

                mAllCacheComponentReferenceAssetInfors[targetComponent] = assetInfor;
            }

            assetInfor.RemoveObjectComponentReference(targetComponent);
        }
        public static void ReleaseComponentReference(Component targetComponent, ReferenceAssetAndRecord referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
#if UNITY_EDITOR
                Debug.LogEditorError($"ReleaseComponentReference Fail,参数为 null ");
#endif
                return;
            }

            if (mAllCacheComponentReferenceAssetInfors.TryGetValue(targetComponent, out var assetInfor) == false)
            {
                assetInfor = targetComponent.gameObject.GetComponent<GameObjectReferenceAssetInformation>();
                if (assetInfor == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"当前组件 {targetComponent} 的对象{targetComponent.gameObject.name} 没有引用其他加载的资源");
#endif
                    return;
                }

                mAllCacheComponentReferenceAssetInfors[targetComponent] = assetInfor;
            }

            assetInfor.RemoveObjectComponentReference(targetComponent, referenceRecord);
        }
        
        
        /// <summary>/// 记录指定的组件的引用资源请求 /// </summary>
        /// <param name="isAuotAddReference">默认=true,标示对否在调用这个方法时候 组件已经引用了这个资源，如果不是自动关联资源的，请设置为false并在真正使用的时候添加引用</param>
        public static void AddObjectComponentReference(Component targetComponent, ReferenceAssetAndRecord referenceRecord, bool isAutoAttachReferenceAsset = true)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

            if (mAllCacheComponentReferenceAssetInfors.TryGetValue(targetComponent, out var assetInfor) == false)
            {
                assetInfor = targetComponent.gameObject. GetAddComponentEx <GameObjectReferenceAssetInformation>();
                if (assetInfor == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"当前组件 {targetComponent} 的对象{targetComponent.gameObject.name} 没有引用其他加载的资源");
#endif
                    return;
                }

                mAllCacheComponentReferenceAssetInfors[targetComponent] = assetInfor;
            }
            
            assetInfor.AddObjectComponentReference(targetComponent,referenceRecord,isAutoAttachReferenceAsset);
        }

        public static ReferenceAssetAndRecord GetComponentReference(Component targetComponent, GetCurComponentReferenceHandler referenceHandler,bool isForceCreateInstance, params object[] otherParameter)
        {
            if (targetComponent == null )
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return null;
            }

            if (mAllCacheComponentReferenceAssetInfors.TryGetValue(targetComponent, out var assetInfor) == false)
            {
                assetInfor = targetComponent.gameObject.GetComponent<GameObjectReferenceAssetInformation>();
                if (assetInfor == null)
                {
#if UNITY_EDITOR
                    Debug.LogEditorError($"当前组件 {targetComponent} 的对象{targetComponent.gameObject.name} 没有引用其他加载的资源");
#endif
                    return null;
                }
                mAllCacheComponentReferenceAssetInfors[targetComponent] = assetInfor;
            }

            return assetInfor.GetComponentReference(targetComponent,referenceHandler,isForceCreateInstance,otherParameter);
        }

    }
}
