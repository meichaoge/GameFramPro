using System;
using System.Collections;
using System.Collections.Generic;
using GameFramePro.ResourcesEx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// 当前实例对象引用的所有资源
    /// </summary>
    public class GameObjectReferenceAssetInformation : MonoBehaviour
    {
        #region 编辑器显示

# if UNITY_EDITOR
        [System.Serializable]
        /// <summary>/// 当个组件引用的资源/// </summary>
        private class ComponentReferenceAsset
        {
            public Component mTargetComponent;
            public List<BaseBeReferenceInformation> mReferenceAssetRecords = new List<BaseBeReferenceInformation>(3);
        }

        [SerializeField] private List<ComponentReferenceAsset> Debug_AllComponentsReferenceAssets = new List<ComponentReferenceAsset>(5);

        private void Update()
        {
            Debug_AllComponentsReferenceAssets.Clear();
            foreach (var componentsReferenceAsset in mAllComponentsReferenceAssets)
            {
                ComponentReferenceAsset componentReference = new ComponentReferenceAsset();
                componentReference.mTargetComponent = componentsReferenceAsset.Key;
                foreach (var reference in componentsReferenceAsset.Value)
                {
                    reference.UpdateDebugView();
                    componentReference.mReferenceAssetRecords.Add(reference);
                }

                Debug_AllComponentsReferenceAssets.Add(componentReference);
            }
        }

#endif

        #endregion


        #region 重写扩展

        /// <summary>/// 所有组件 引用的资源/// </summary>
        private readonly Dictionary<Component, List<BaseBeReferenceInformation>> mAllComponentsReferenceAssets = new Dictionary<Component, List<BaseBeReferenceInformation>>();

        #region 对外接口

        /// <summary>/// 获得指定的组件的引用资源信息/// </summary>
        public BaseBeReferenceInformation GetComponentReference(Component targetComponent, GetCurComponentReferenceHandler referenceHandler, bool isForceCreateInstance, params object[] otherParameter)
        {
            if (targetComponent == null)
            {
                Debug.LogError("关联的组件参数为null");
                return null;
            }

            if (isForceCreateInstance)
                return null;
            //    return referenceHandler != null ? referenceHandler(targetComponent, null, otherParameter) : null;


            if (mAllComponentsReferenceAssets.TryGetValue(targetComponent, out var componentReferences) == false)
            {
#if UNITY_EDITOR
                Debug.LogInfor("不包含对象 {0} 的 组价{1} 引用资源记录", gameObject.name, targetComponent);
#endif
                return null;
            }


            if (referenceHandler == null)
                if (componentReferences != null && componentReferences.Count > 0)
                    return componentReferences[0];
                else
                    return null;
            return referenceHandler(targetComponent, componentReferences, otherParameter);
        }

        /// <summary>/// 记录指定的组件的引用资源请求 /// </summary>
        /// <param name="isAuotAddReference">默认=true,标示对否在调用这个方法时候 组件已经引用了这个资源，如果不是自动关联资源的，请设置为false并在真正使用的时候添加引用</param>
        public void AddObjectComponentReference(Component targetComponent, BaseBeReferenceInformation referenceRecord, bool isAutoAddReference = true)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

            if (mAllComponentsReferenceAssets.TryGetValue(targetComponent, out var componentReferences) == false)
            {
                componentReferences = new List<BaseBeReferenceInformation>();
                if (isAutoAddReference)
                    referenceRecord.AddReference();
                componentReferences.Add(referenceRecord);
                mAllComponentsReferenceAssets[targetComponent] = componentReferences;
#if UNITY_EDITOR
                Debug.LogEditorInfor($"不包含对象 {gameObject.name} 的 组价{targetComponent} 引用资源记录");
#endif
                return;
            }


            for (int dex = componentReferences.Count - 1; dex >= 0; --dex)
            {
                var reference = componentReferences[dex];
                if (reference.IsReferenceAssetEnable == false)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference.IsReferenceEqual(referenceRecord))
                {
                    if (isAutoAddReference)
                        reference.AddReference();
                    return;
                }
            }

            if (isAutoAddReference)
                referenceRecord.AddReference();
            componentReferences.Add(referenceRecord);
        }

        /// <summary>/// 释放一个组件所有的资源引用/// </summary>
        public void RemoveObjectComponentReference(Component targetComponent, BaseBeReferenceInformation referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

            if (mAllComponentsReferenceAssets.TryGetValue(targetComponent, out var componentReferences) == false)
            {
#if UNITY_EDITOR
                Debug.LogInfor("不包含对象 {0} 的 组价{1} 引用资源记录", gameObject.name, targetComponent);
#endif
                return;
            }

            for (int dex = componentReferences.Count - 1; dex >= 0; --dex)
            {
                var reference = componentReferences[dex];

                if (reference.IsReferenceAssetEnable == false)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference == referenceRecord)
                    // if (reference.CurLoadAssetRecord.AssetUrl == referenceRecord.CurLoadAssetRecord.AssetUrl)
                {
                    reference.ReduceReference(); //先释放引用的资源
                    componentReferences.RemoveAt(dex);
                }
            }
        }

        /// <summary>/// 释放一个组件所有的资源引用/// </summary>
        public void RemoveObjectComponentReference(Component targetComponent)
        {
            if (targetComponent == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

            if (mAllComponentsReferenceAssets.TryGetValue(targetComponent, out var componentReferences) == false)
            {
                //  Debug.LogError("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
                return;
            }

            foreach (var reference in componentReferences)
                reference?.ReduceReference();

            componentReferences.Clear();
            mAllComponentsReferenceAssets.Remove(targetComponent);
        }

        /// <summary>/// 释放一个资源所有引用的资源/// </summary>
        public void RemoveObjectReference()
        {
            foreach (var componentReferences in mAllComponentsReferenceAssets)
            {
                foreach (var reference in componentReferences.Value)
                    reference?.ReduceReference();
                componentReferences.Value.Clear();
            }

            mAllComponentsReferenceAssets.Clear();
        }

        #endregion

        #endregion
    }
}
