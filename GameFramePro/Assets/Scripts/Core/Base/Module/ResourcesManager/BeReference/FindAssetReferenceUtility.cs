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

    /// <summary>/// 从多个引用中获取各种类型的指定引用/// </summary>
    public static class FindAssetReferenceUtility
    {
        //获取指定的Image 组件 Sprite 引用
        public static ReferenceAssetAndRecord GetSpriteAssetReference(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences.Count == 0) return null;
            Image targetImageComponent = component as Image;

            if (targetImageComponent == null || targetImageComponent.sprite == null)
                return null;

            foreach (var reference in allComponentReferences)
            {
                if (reference.BeReferenceAsset == null) continue;
                if (reference.BeReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.BeReferenceAsset.ReferenceAssetType != typeof(Sprite))
                    continue;

                if (reference.BeReferenceAsset.IsReferenceEqual(targetImageComponent.sprite))
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
                if (reference.BeReferenceAsset == null) continue;
                if (reference.BeReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.BeReferenceAsset.ReferenceAssetType != typeof(AudioClip))
                    continue;

                if (reference.BeReferenceAsset.IsReferenceEqual(targetAudioSourcesComponent.clip))
                    return reference;
            }

            return null;
        }

        //根据指定的参数中的对象名获取引用的对象
        public static ReferenceAssetAndRecord GetGameObjectFromAssetReference(Component component, List<ReferenceAssetAndRecord> allComponentReferences, params object[] otherParameter)
        {
            if (allComponentReferences == null || allComponentReferences.Count == 0) return null;
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
                if (reference.BeReferenceAsset == null) continue;
                if (reference.BeReferenceAsset.IsReferenceAssetEnable == false) continue;
                if (reference.BeReferenceAsset.ReferenceAssetType != typeof(GameObject))
                    continue;

                if (reference.BeReferenceAsset.IsNameEqual(assetName) == false) continue;

                BaseBeReferenceGameObjectInformation gameObjectInformation = reference.BeReferenceAsset as BaseBeReferenceGameObjectInformation;

                if (gameObjectInformation == null)
                {
                    Debug.LogError("当前资源不能转换成 ReferenceGameObjectAssetInfor 类型 ");
                    continue;
                }

                if (gameObjectInformation.IsChild(targetTransformComponent))
                    return reference;
            }

            return null;
        }
    }
}
