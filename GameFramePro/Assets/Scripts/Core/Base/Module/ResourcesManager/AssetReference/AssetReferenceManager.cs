using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 定义了如何从参数 component 组件的所有引用其他资源 allComponentReferences 中获取满足条件的第一个引用信息
    /// </summary>
    /// <param name="component"></param>
    /// <param name="allComponentReferences"></param>
    /// <returns></returns>
    public delegate BaseAssetReference2 GetCurComponentReferenceHandler(Component component, List<BaseAssetReference2> allComponentReferences,params object[] otherParameter);

    /// <summary>
    /// 管理整个项目中加载的的资源引用其他的资源的数据
    /// </summary>
    public static class AssetReferenceManager
    {
        //key =gameobject  Value={ key=component  value=引用的所有资源}
        private static Dictionary<Object, Dictionary<Component, List<BaseAssetReference2>>> s_AllObjectComponentReferenceRecord = new Dictionary<Object, Dictionary<Component, List<BaseAssetReference2>>>();
#if UNITY_EDITOR
        public static Dictionary<Object, Dictionary<Component, List<BaseAssetReference2>>> AllObjectComponentReferenceRecord { get { return s_AllObjectComponentReferenceRecord; } }
#endif

        public static BaseAssetReference2 GetObjectComponentReference(Component targetComponent, GetCurComponentReferenceHandler referenceHandler, params object[] otherParameter)
        {
            if (targetComponent == null)
            {
                Debug.LogError("关联的组件参数为null");
                return null;
            }
            Dictionary<Component, List<BaseAssetReference2>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
#if UNITY_EDITOR
                Debug.LogInfor("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);
#endif
                return null;
            }
            List<BaseAssetReference2> componentReferences = null;
            if (gameObjectReferences.TryGetValue(targetComponent, out componentReferences) == false)
            {
#if UNITY_EDITOR
                Debug.LogInfor("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
#endif
                return null;
            }

            if (referenceHandler == null)
                if (componentReferences != null && componentReferences.Count > 0)
                    return componentReferences[0];
                else
                    return null;
            else
                return referenceHandler(targetComponent, componentReferences, otherParameter);
        }


        public static void AddObjectComponentReference(Component targetComponent, BaseAssetReference2 referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }
            //int gameObjectInstanceID = targetComponent.gameObject.GetInstanceID();
            //int componentInstanceID = targetComponent.GetInstanceID();
            Dictionary<Component, List<BaseAssetReference2>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
                Debug.LogInfor("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);
                gameObjectReferences = new Dictionary<Component, List<BaseAssetReference2>>();
                List<BaseAssetReference2> references = new List<BaseAssetReference2>();
                references.Add(referenceRecord);
                gameObjectReferences[targetComponent] = references;
                s_AllObjectComponentReferenceRecord[targetComponent.gameObject] = gameObjectReferences;
                return;
            }

           
            List<BaseAssetReference2> componentReferences = null;
            if (gameObjectReferences.TryGetValue(targetComponent, out componentReferences) == false)
            {
                componentReferences = new List<BaseAssetReference2>();
                componentReferences.Add(referenceRecord);
                gameObjectReferences[targetComponent] = componentReferences;
                s_AllObjectComponentReferenceRecord[targetComponent.gameObject] = gameObjectReferences;

                Debug.LogInfor("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
                return;
            }

            for (int dex = componentReferences.Count - 1; dex >= 0; dex--)
            {
                var reference = componentReferences[dex];

                if (reference == null|| reference.CurLoadAssetRecord==null)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference.CurLoadAssetRecord.AssetUrl == referenceRecord.CurLoadAssetRecord.AssetUrl)
                {
                    reference.CurLoadAssetRecord.AddReference();
                    return;
                }
            }
            componentReferences.Add(referenceRecord);

        }

        public static void RemoveObjectComponentReference(Component targetComponent, BaseAssetReference2 referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

         //   int gameObjectInstanceID = targetComponent.gameObject.GetInstanceID();
            Dictionary<Component, List<BaseAssetReference2>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
                Debug.LogError("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);
                return;
            }

            //int componentInstanceID = targetComponent.GetInstanceID();
            List<BaseAssetReference2> componentReferences = null;
            if (gameObjectReferences.TryGetValue(targetComponent, out componentReferences) == false)
            {
                Debug.LogError("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
                return;
            }

            for (int dex = componentReferences.Count - 1; dex >= 0; dex--)
            {
                var reference = componentReferences[dex];

                if (reference == null || reference.CurLoadAssetRecord == null)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference.CurLoadAssetRecord.AssetUrl == referenceRecord.CurLoadAssetRecord.AssetUrl)
                {
                    reference.CurLoadAssetRecord.ReduceReference();
                    if(reference.CurLoadAssetRecord.ReferenceCount<=0)
                        componentReferences.RemoveAt(dex);
                    return;
                }
            }

        }

    }
}