using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace GameFramePro.ResourcesEx
{
    /// <summary>/// 对GameFramePro 命名空间中的类隐藏资源加载和卸载的实现/// </summary>
    public static class ResourcesUtility
    {
        #region 资源加载实现

        /// <summary>/// 同步下载资源接口/// </summary>
        public static LoadAssetAssetRecord LoadAssetSync(string assetPath)
        {
            LoadAssetAssetRecord record = null;

            if (AppSetting.S_IsLoadResourcesAssetPriority)
            {
                record = LocalResourcesManager.S_Instance.LoadAssetSync(assetPath);
                if (record == null)
                    record = AssetBundleManager.S_Instance.LoadAssetSync(assetPath);
            }
            else
            {
                record = AssetBundleManager.S_Instance.LoadAssetSync(assetPath);
                if (record == null)
                    record = LocalResourcesManager.S_Instance.LoadAssetSync(assetPath);
            }

            return record;
        }

        /// <summary>/// 异步下载资源的接口/// </summary>
        public static void LoadAssetAsync(string assetPath, System.Action<LoadAssetAssetRecord> loadCallback)
        {
            var assetRecord = ResourcesUtility.LoadAssetFromCache(assetPath);
            if (assetRecord != null)
            {
                if (loadCallback != null)
                    loadCallback(assetRecord);
                return;
            }

            if (AppSetting.S_IsLoadResourcesAssetPriority)
                LocalResourcesManager.S_Instance.ResourcesLoadAssetAsync(assetPath, loadCallback, null);
            else
                AssetBundleManager.S_Instance.LoadAssetAsync(assetPath, loadCallback);
        }


        /// <summary>/// 缓存中加载资源/// </summary>
        public static LoadAssetAssetRecord LoadAssetFromCache(string assetPath)
        {
            LoadAssetAssetRecord asset = LocalResourcesManager.S_Instance.LoadAssetFromCache(assetPath);
            if (asset != null)
                return asset;
            asset = AssetBundleManager.S_Instance.LoadSubAssetFromCache(assetPath);
            return asset;
        }

        #endregion

        #region 资源释放实现

        /// <summary>/// 卸载 AssetBundle 资源加载/// </summary>
        public static void UnLoadAssetBundle(LoadAssetBundleAssetRecord assetRecord, bool isUnloadAllLoadedObjects)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadAssetBundle Fail!! 参数是null ");
                return;
            }

            Debug.LogEditorInfor("释放 AssetBundle 资源 " + assetRecord.AssetUrl);
            assetRecord.LoadAssetBundleInformation.UnLoadAsAssetBundleAsset(isUnloadAllLoadedObjects);
        }

        //释放一个对象& 组件(根据类型判断) 上的所有引用
        public static void ReleaseComponentReference(Object targetObject)
        {
            if (targetObject == null) return;
            if (targetObject is Component)
                ReferenceAssetUtility.ReleaseComponentReference(targetObject as Component);
            else if (targetObject is GameObject)
                ReferenceAssetUtility.ReleaseGameObjectReference(targetObject as GameObject);
        }

        #endregion


        #region 各种类型的资源真实的加载逻辑

        #region Sprite 加载

        public static BaseBeReferenceInformation SetImageSpriteFromReference(Image targetImage, BaseBeReferenceInformation newReferenceInformation)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetImage, FindAssetReferenceUtility.GetSpriteAssetReference, false);

            if (curReference == newReferenceInformation)
            {
#if UNITY_EDITOR
                Debug.LogEditorInfor($"当前图片{targetImage} 与想要赋值的对象相同");
#endif
                return curReference;
            } //避免不必要的赋值

            if (curReference != null)
                ReferenceAssetUtility.ReleaseComponentReference(targetImage, curReference); //释放当前引用资源 以及删除对象引用记录

            if (newReferenceInformation == null || newReferenceInformation.IsReferenceAssetEnable == false)
            {
                targetImage.sprite = null;
                return null;
            }


            Sprite sp = newReferenceInformation.ParentReferenceAssetRecord.LoadBasicObjectAssetInfor.LoadSpriteFromSpriteRender();
            BaseBeReferenceInformation baseBeReferenceInformation = new BaseBeReferenceInformation(newReferenceInformation);
            ReferenceAssetUtility.AddObjectComponentReference(targetImage, baseBeReferenceInformation, true);

            targetImage.sprite = sp;
            return baseBeReferenceInformation;
        }

        /// <summary>/// 真实的赋值操作， 根据资源加载图片精灵  .isAutoAttachReferenceAsset=true标示默认关联指定的图片资源；isForceCreateInstance=false 则标示 优先引用已经存在的引用资源/// </summary>
        public static BaseBeReferenceInformation SetImageSpriteFromRecord(Image targetImage, LoadAssetAssetRecord assetRecord, bool isAutoAttachReferenceAsset = true, bool isForceCreateInstance = false)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetImage, FindAssetReferenceUtility.GetSpriteAssetReference, isForceCreateInstance);

            if (assetRecord == null || assetRecord.IsReferenceEnable == false)
            {
                if (isAutoAttachReferenceAsset)
                {
                    if (curReference != null)
                        ReferenceAssetUtility.ReleaseComponentReference(targetImage, curReference); //释放当前引用资源 以及删除对象引用记录

                    targetImage.sprite = null;
                }

                return null;
            }

            bool isAleadyReference = assetRecord.IsExitReference(curReference);
            if (isAleadyReference)
            {
                Debug.LogEditorInfor($"当前组件{targetImage} 想要引用的资源就是当前资源{assetRecord.AssetUrl}");
                return curReference;
            } //判断是否需要引用的资源就是当前资源 避免较少引用后增加引用的异常


            if (curReference != null)
            {
                if (isAutoAttachReferenceAsset)
                    ReferenceAssetUtility.ReleaseComponentReference(targetImage, curReference); //释放当前引用资源 以及删除对象引用记录
            }

            Sprite sp = assetRecord.LoadBasicObjectAssetInfor.LoadSpriteFromSpriteRender();
            BaseBeReferenceInformation baseBeReferenceInformation = new BaseBeReferenceInformation(sp, typeof(Sprite), assetRecord);
            ReferenceAssetUtility.AddObjectComponentReference(targetImage, baseBeReferenceInformation, isAutoAttachReferenceAsset);

            if (isAutoAttachReferenceAsset)
                targetImage.sprite = sp;
            return baseBeReferenceInformation;
        }

        #endregion

        #region GameObject 加载

        /// <summary>/// 内部操作  根据给定的路径 实例化一个对象在 指定的父节点上/// </summary>
        public static BaseBeReferenceGameObjectInformation InstantiateGameObjectFromRecord(Transform targetParent, LoadAssetAssetRecord assetRecord, bool isForceCreateInstance)
        {
            if (assetRecord == null || assetRecord.IsReferenceEnable == false)
            {
                Debug.LogError("InstantiateGameObjectFromRecord 失败 ，参数不可用");
                return null;
            }

            var curReference = ReferenceAssetUtility.GetComponentReference(targetParent, FindAssetReferenceUtility.GetGameObjectFromAssetReference, isForceCreateInstance, assetRecord.AssetUrl);

            if (curReference != null)
                return null;

            GameObject go = assetRecord.LoadBasicObjectAssetInfor.InstantiateInstance(targetParent);
            var referenceAssetInfor = new BaseBeReferenceGameObjectInformation(go, typeof(GameObject), assetRecord);

            ReferenceAssetUtility.AddObjectComponentReference(targetParent, referenceAssetInfor, true);

            return referenceAssetInfor;
        }

        #endregion


        #region Audio 加载

        public static BaseBeReferenceInformation SetAudioClipFromRecord(AudioSource targetAudioSources, LoadAssetAssetRecord assetRecord, bool isAutoAttachReferenceAsset, bool isForceCreateInstance = false)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetAudioSources, FindAssetReferenceUtility.GetSAudioClipAssetReference, isForceCreateInstance);

            if (assetRecord == null || assetRecord.IsReferenceEnable == false)
            {
                if (isAutoAttachReferenceAsset)
                {
                    if (curReference != null)
                        ReferenceAssetUtility.ReleaseComponentReference(targetAudioSources, curReference); //释放当前引用资源 以及删除对象引用记录

                    targetAudioSources.clip = null;
                }

                return null;
            }

            bool isAleadyReference = assetRecord.IsExitReference(curReference);
            if (isAleadyReference)
            {
#if UNITY_EDITOR
                Debug.LogEditorInfor($"当前组件{targetAudioSources} 想要引用的资源就是当前资源{assetRecord.AssetUrl}");
#endif
                return curReference;
            } //判断是否需要引用的资源就是当前资源 避免较少引用后增加引用的异常


            if (curReference != null)
            {
                if (isAutoAttachReferenceAsset)
                    ReferenceAssetUtility.ReleaseComponentReference(targetAudioSources, curReference);
            }


            var clip = assetRecord.LoadBasicObjectAssetInfor.LoadAudioClip();
            var referenceAssetInfor = new BaseBeReferenceInformation(clip, typeof(AudioClip), assetRecord);

            ReferenceAssetUtility.AddObjectComponentReference(targetAudioSources, referenceAssetInfor, isAutoAttachReferenceAsset);

            if (isAutoAttachReferenceAsset)
                targetAudioSources.clip = clip;

            return referenceAssetInfor;
        }


        public static BaseBeReferenceInformation SettAudioClipFromReference(AudioSource targetAudioSources, BaseBeReferenceInformation newReferenceInformation)
        {
            var curReference = ReferenceAssetUtility.GetComponentReference(targetAudioSources, FindAssetReferenceUtility.GetSAudioClipAssetReference, false);

            if (curReference == newReferenceInformation)
            {
#if UNITY_EDITOR
                Debug.LogEditorInfor($"当前音效{targetAudioSources} 与想要赋值的对象相同");
#endif
                return curReference;
            } //避免不必要的赋值

            if (curReference != null)
                ReferenceAssetUtility.ReleaseComponentReference(targetAudioSources, curReference); //释放当前引用资源 以及删除对象引用记录

            if (newReferenceInformation == null || newReferenceInformation.IsReferenceAssetEnable == false)
            {
                targetAudioSources.clip = null;
                return null;
            }

            var clip = newReferenceInformation.ParentReferenceAssetRecord.LoadBasicObjectAssetInfor.LoadAudioClip();
            var referenceAssetInfor = new BaseBeReferenceInformation(newReferenceInformation);

            ReferenceAssetUtility.AddObjectComponentReference(targetAudioSources, referenceAssetInfor, true);

            targetAudioSources.clip = clip;
            return referenceAssetInfor;
        }

        #endregion

        #endregion
    }
}
