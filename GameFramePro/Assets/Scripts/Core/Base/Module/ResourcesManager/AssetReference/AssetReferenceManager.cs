using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEngine.UI;

namespace GameFramePro.ResourcesEx
{
    /// <summary>
    /// 定义了如何从参数 component 组件的所有引用其他资源 allComponentReferences 中获取满足条件的第一个引用信息
    /// </summary>
    /// <param name="component"></param>
    /// <param name="allComponentReferences"></param>
    /// <returns></returns>
    public delegate ReferenceAssetAndRecord GetCurComponentReferenceHandler(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter);

    /// <summary>
    /// 管理整个项目中加载的的资源引用其他的资源的数据
    /// </summary>
    public static class AssetReferenceManager
    {
        //key =gameobject  Value={ key=component  value=引用的所有资源}
        private static Dictionary<Object, Dictionary<Component, List<ReferenceAssetAndRecord>>> s_AllObjectComponentReferenceRecord = new Dictionary<Object, Dictionary<Component, List<ReferenceAssetAndRecord>>>();
#if UNITY_EDITOR
        public static Dictionary<Object, Dictionary<Component, List<ReferenceAssetAndRecord>>> AllObjectComponentReferenceRecord { get { return s_AllObjectComponentReferenceRecord; } }
#endif

        public static ReferenceAssetAndRecord GetObjectComponentReference(Component targetComponent, GetCurComponentReferenceHandler referenceHandler, params object[] otherParameter)
        {
            if (targetComponent == null)
            {
                Debug.LogError("关联的组件参数为null");
                return null;
            }
            Dictionary<Component, List<ReferenceAssetAndRecord>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
#if UNITY_EDITOR
                Debug.LogInfor("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);
#endif
                return null;
            }
            List<ReferenceAssetAndRecord> componentReferences = null;
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


        public static void AddObjectComponentReference(Component targetComponent, ReferenceAssetAndRecord referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }
            Dictionary<Component, List<ReferenceAssetAndRecord>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
                Debug.LogInfor("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);

                gameObjectReferences = new Dictionary<Component, List<ReferenceAssetAndRecord>>();
                List<ReferenceAssetAndRecord> references = new List<ReferenceAssetAndRecord>();
                referenceRecord.AddReference();
                references.Add(referenceRecord);
                gameObjectReferences[targetComponent] = references;
                s_AllObjectComponentReferenceRecord[targetComponent.gameObject] = gameObjectReferences;
                return;
            }


            List<ReferenceAssetAndRecord> componentReferences = null;
            if (gameObjectReferences.TryGetValue(targetComponent, out componentReferences) == false)
            {
                componentReferences = new List<ReferenceAssetAndRecord>();
                referenceRecord.AddReference();
                componentReferences.Add(referenceRecord);
                gameObjectReferences[targetComponent] = componentReferences;
                s_AllObjectComponentReferenceRecord[targetComponent.gameObject] = gameObjectReferences;

                Debug.LogInfor("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
                return;
            }



            for (int dex = componentReferences.Count - 1; dex >= 0; dex--)
            {
                var reference = componentReferences[dex];
                if (reference.IsReferenceEnable == false)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference.CurLoadAssetRecord.AssetUrl == referenceRecord.CurLoadAssetRecord.AssetUrl &&
                    reference.ReferenceAsset.IsReferenceEqual(referenceRecord))
                {
                    reference.AddReference();
                    return;
                }
            }

            referenceRecord.AddReference();
            componentReferences.Add(referenceRecord);
        }

        public static void RemoveObjectComponentReference(Component targetComponent, ReferenceAssetAndRecord referenceRecord)
        {
            if (targetComponent == null || referenceRecord == null)
            {
                Debug.LogError("关联的组件参数为null" + (targetComponent == null));
                return;
            }

            Dictionary<Component, List<ReferenceAssetAndRecord>> gameObjectReferences = null;
            if (s_AllObjectComponentReferenceRecord.TryGetValue(targetComponent.gameObject, out gameObjectReferences) == false)
            {
                Debug.LogError("不包含对象 {0} 的引用资源记录", targetComponent.gameObject.name);
                return;
            }

            List<ReferenceAssetAndRecord> componentReferences = null;
            if (gameObjectReferences.TryGetValue(targetComponent, out componentReferences) == false)
            {
                Debug.LogError("不包含对象 {0} 的 组价{1} 引用资源记录", targetComponent.gameObject.name, targetComponent);
                return;
            }

            for (int dex = componentReferences.Count - 1; dex >= 0; dex--)
            {
                var reference = componentReferences[dex];

                if (reference.IsReferenceEnable == false)
                {
                    componentReferences.RemoveAt(dex);
                    continue;
                }

                if (reference.CurLoadAssetRecord.AssetUrl == referenceRecord.CurLoadAssetRecord.AssetUrl)
                {
                    reference.ReduceReference(); //先释放引用的资源
                    componentReferences.RemoveAt(dex);
                    return;
                }
            }

        }




        #region 从多个引用中获取各种类型的指定引用 接口
        //获取指定的Image 组件 Sprite 引用
        public static ReferenceAssetAndRecord GetSpriteAssetReference(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences.Count == 0) return null;
            Image targetImageComponent = component as Image;

            if (targetImageComponent == null || targetImageComponent.sprite == null)
                return null;

            foreach (var reference in allComponentReferences)
            {
                if (reference.ReferenceAsset == null) continue;
                if (reference.ReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.ReferenceAsset.ReferenceAssetType != typeof(Sprite))
                    continue;

                if (reference.ReferenceAsset.IsReferenceEqual(targetImageComponent.sprite))
                    return reference;
            }
            return null;
        }

        //获取指定的 AudioSource 组件 AudioClip 引用
        public static ReferenceAssetAndRecord GetSAudioClipAssetReference(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences.Count == 0) return null;
            AudioSource targetAudioSourcesComponent = component as AudioSource;

            if (targetAudioSourcesComponent == null || targetAudioSourcesComponent.clip == null)
                return null;

            foreach (var reference in allComponentReferences)
            {
                if (reference.ReferenceAsset == null) continue;
                if (reference.ReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.ReferenceAsset.ReferenceAssetType != typeof(AudioClip))
                    continue;

                if (reference.ReferenceAsset.IsReferenceEqual(targetAudioSourcesComponent.clip))
                    return reference;
            }
            return null;
        }

        //根据指定的参数中的对象名获取引用的对象
        public static ReferenceAssetAndRecord GetGameObjectFromAssetReference(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences.Count == 0) return null;
            Transform targetTransformComponent = component as Transform;

            if (targetTransformComponent == null || targetTransformComponent.childCount == 0)
                return null;
            if (otherParameter == null || otherParameter.Length == 0)
            {
                Debug.LogError("GetGameObjectFromAssetReference2 至少需要传一个额外的 AssetUrl 参数");
                return null;
            }

            string assetName = otherParameter[0].ToString().GetFileNameWithoutExtensionEx();
            if (string.IsNullOrEmpty(assetName))
            {
                Debug.LogError("GetGameObjectFromAssetReference2 至少需要传一个额外的 AssetUrl 参数,且能获取到正确的格式 {0}", otherParameter[0]);
                return null;
            }

            foreach (var reference in allComponentReferences)
            {
                if (reference.ReferenceAsset == null) continue;
                if (reference.ReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.ReferenceAsset.ReferenceAssetType != typeof(GameObject))
                    continue;

                if (reference.ReferenceAsset.IsNameEqual(assetName) == false) continue;

                ReferenceGameObjectAssetInfor gameObjectAssetInfor = reference.ReferenceAsset as ReferenceGameObjectAssetInfor;

                if (gameObjectAssetInfor == null)
                {
                    Debug.LogError("当前资源不能转换成 ReferenceGameObjectAssetInfor 类型 ");
                    continue;
                }

                if (gameObjectAssetInfor.IsChild(targetTransformComponent)           )
                    return reference;
            }
            return null;
        }
        #endregion










    }
}