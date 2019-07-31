using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramePro.ResourcesEx;
using Object = UnityEngine.Object;

namespace GameFramePro
{
    /// <summary>
    /// 这里只有ResourcesManger 提供的对外访问接口和实现
    /// </summary>
    public class ResourcesManager : Single<ResourcesManager>
    {
        #region 对象的创建和销毁逻辑（实例化对象 (这里对内部的GameObject.Instantiate<T> 做了一层封装，主要是想后期能够监控对象的创建TODO)）

        // 实例化一个对象
        public static GameObject Instantiate(string goName)
        {
            GameObject go = new GameObject(goName);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        //标记为不会被销毁的对象
        public static bool MarkNotDestroyOnLoad(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError("MarkNotDestroyOnLoad Fail, Parameter is Null");
                return false;
            }

            GameObject.DontDestroyOnLoad(go);
            return true;
        }

        public static void Destroy(UnityEngine.Object obj)
        {
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.Destroy(obj);
        }

        public static void DestroyImmediate(UnityEngine.Object obj)
        {
            ResourcesTracker.UnRegisterTraceResources(obj);
            GameObject.DestroyImmediate(obj);
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original) where T : Object
        {
            T go = GameObject.Instantiate<T>(original, Vector3.zero, Quaternion.identity);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            T go = Instantiate<T>(original, parent, true);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
        {
            T go = Instantiate<T>(original, position, rotation, null);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            T go = GameObject.Instantiate<T>(original, position, rotation, parent);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }

        // 实例化一个对象
        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
        {
            if (original == null)
            {
                Debug.LogError("Instantiate Fail,参数预制体为null");
                return null;
            }

            T go = GameObject.Instantiate<T>(original, parent, worldPositionStays);
            ResourcesTracker.RegisterTraceResources(go, TraceResourcesStateEnum.Normal);
            return go;
        }


        #endregion


        #region 资源加载&释放接口
        /// <summary>
        /// 缓存中加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        private static BaseLoadAssetRecord LoadAssetFromCache(string assetPath)
        {
            BaseLoadAssetRecord asset = LocalResourcesManager.S_Instance.LoadAssetFromCache(assetPath);
            if (asset != null)
                return asset;
            else
                asset = AssetBundleManager.S_Instance.LoadSubAssetFromCache(assetPath);
            return asset;
        }

        /// <summary>
        /// 同步下载资源接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <returns></returns>
        public static BaseLoadAssetRecord LoadAssetSync(string assetPath)
        {
            BaseLoadAssetRecord record = null;

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

        /// <summary>
        /// 异步下载资源的接口
        /// </summary>
        /// <param name="assetpath"></param>
        /// <param name="loadCallback"></param>
        public static void LoadAssetAsync(string assetPath, System.Action<BaseLoadAssetRecord> loadCallback)
        {
            BaseLoadAssetRecord assetRecord = LoadAssetFromCache(assetPath);
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


        public static void UnLoadAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                return;
            }

#if UNITY_EDITOR
            Debug.LogInfor("卸载资源  " + asset.GetType() + "  " + asset.name);
#endif
            Resources.UnloadAsset(asset);


        }

        public static void UnLoadAssetBundle(AssetBundle asset, bool isUnloadAllLoadedObjects)
        {
            if (asset == null)
            {
                Debug.LogError("UnLoadAsset Fail!! 参数是null ");
                return;
            }
            asset.Unload(isUnloadAllLoadedObjects);
        }

        /// <summary>
        /// 卸载 Resources 资源
        /// </summary>
        /// <param name="assetRecord"></param>
        public static void UnLoadResourceAsset(BaseLoadAssetRecord assetRecord)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadResourceAsset Fail!! 参数是null ");
                return;
            }

            if (assetRecord.AssetLoadedType >= LoadedAssetTypeEnum.AssetBundle_UnKnown)
            {
                Debug.LogError("UnLoadResourceAsset Fail!! 当前资源不是Resource 资源 " + assetRecord.AssetUrl);
                return;
            }

            Debug.LogEditorInfor("释放Resources 资源 " + assetRecord.AssetUrl);
            (assetRecord.LoadUnityObjectAssetInfor as ResourceLoadUnityAssetInfor).UnLoadAsResourcesAsset();
        }
        /// <summary>
        /// 卸载 AssetBundle 资源加载
        /// </summary>
        /// <param name="assetRecord"></param>
        /// <param name="isUnloadAllLoadedObjects"></param>
        public static void UnLoadAssetBundle(BaseLoadAssetRecord assetRecord, bool isUnloadAllLoadedObjects)
        {
            if (assetRecord == null)
            {
                Debug.LogError("UnLoadAssetBundle Fail!! 参数是null ");
                return;
            }

            //if (assetRecord.AssetLoadedType <= LoadedAssetTypeEnum.LocalStore_UnKnown)
            //{
            //    Debug.LogError("UnLoadAssetBundle Fail!! 当前资源不是 AssetBundle 资源 " + assetRecord.AssetUrl);
            //    return;
            //}

            Debug.LogEditorInfor("释放 AssetBundle 资源 " + assetRecord.AssetUrl);
            (assetRecord.LoadUnityObjectAssetInfor as BundleLoadUnityAssetInfor).UnLoadAsAssetBundleAsset(isUnloadAllLoadedObjects);
        }

        #endregion

        #region 资源加载接口

        #region TextAsset/lua 文件的加载

        private static Dictionary<string, string> mAllCacheTextInfor = new Dictionary<string, string>();//缓存所有加载的文本资源
        /// <summary>
        /// 文本资源的加载(一般是配置文件或者lua 文件)
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string LoadTextAssettSync(string assetPath, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                    return resultStr;
            }
            BaseLoadAssetRecord assetRecord = LoadAssetSync(assetPath);
            if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor != null)
            {
                resultStr = assetRecord.LoadUnityObjectAssetInfor.LoadTextAssetContent();
                mAllCacheTextInfor[assetPath] = resultStr;
                return resultStr;
            }
            Debug.LogError("获取文本 资源失败 " + assetPath);
            return resultStr;

        }

        public static void LoadTextAssettAsync(string assetPath, System.Action<string> textAssetAction, bool isForceReload = false)
        {
            string resultStr = string.Empty;
            if (isForceReload == false)
            {
                if (mAllCacheTextInfor.TryGetValue(assetPath, out resultStr) && string.IsNullOrEmpty(resultStr) == false)
                {
                    if (textAssetAction != null) textAssetAction(resultStr);
                    return;
                }
            }
            LoadAssetAsync(assetPath, (assetRecord) =>
            {
                if (assetRecord != null && assetRecord.LoadUnityObjectAssetInfor != null)
                {
                    resultStr = assetRecord.LoadUnityObjectAssetInfor.LoadTextAssetContent();
                    mAllCacheTextInfor[assetPath] = resultStr;
                    if (textAssetAction != null) textAssetAction(resultStr);
                    return;
                }
                Debug.LogError("获取文本 资源失败 " + assetPath);
                if (textAssetAction != null) textAssetAction(null);
                return;
            });

        }
        #endregion

        #region Sprite 加载

        //根据路径加载图片资源 同步接口
        public static ReferenceAssetInfor SetImageSpriteByPathSync(Image targetImage, string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return SetImageSpriteFromRecordSync(targetImage, null);
            else
                return SetImageSpriteFromRecordSync(targetImage, LoadAssetSync(assetPath));
        }
        //根据资源加载图片精灵
        public static ReferenceAssetInfor SetImageSpriteFromRecordSync(Image targetImage, BaseLoadAssetRecord assetRecord)
        {
            ReferenceAssetAndRecord curReference = AssetReferenceManager.GetObjectComponentReference(targetImage, AssetReferenceManager.GetSpriteAssetReference);
            if (assetRecord == null)
            {
                if (curReference != null)
                    AssetReferenceManager.RemoveObjectComponentReference(targetImage, curReference);
                targetImage.sprite = null;
                return null;
            }

            if (curReference != null && curReference.CurLoadAssetRecord.isReferenceEqual(assetRecord))
                return null;
            Sprite sp = assetRecord.LoadUnityObjectAssetInfor.LoadSpriteFromSpriteRender();
            ReferenceAssetInfor referenceAssetInfor = new ReferenceAssetInfor(sp, typeof(Sprite));

            ReferenceAssetAndRecord newReference = new ReferenceAssetAndRecord(assetRecord, referenceAssetInfor);

            if (curReference != null)
                curReference.ModifyComponentReference<Image>(targetImage, newReference);
            else
                AssetReferenceManager.AddObjectComponentReference(targetImage, newReference);

            targetImage.sprite = sp;
            return referenceAssetInfor;
        }
        //异步设置Sprite 接口
        public static void SetImageSpriteByPathAsync(Image targetImage, string assetPath, Action<ReferenceAssetInfor> afterAttachSpriteAction)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                ReferenceAssetInfor assetInfor = SetImageSpriteFromRecordSync(targetImage, null);
                if (afterAttachSpriteAction != null)
                    afterAttachSpriteAction.Invoke(assetInfor);
            }
            else
            {
                LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    ReferenceAssetInfor assetInfor = SetImageSpriteFromRecordSync(targetImage, assetRecord);
                    if (afterAttachSpriteAction != null)
                        afterAttachSpriteAction.Invoke(assetInfor);
                });
            }
        }


        //fromImage 上的资源克隆到 toImage
        public static void CloneImageSprite(Image fromImage, Image toImage)
        {
            ReferenceAssetAndRecord fromsourceReference = AssetReferenceManager.GetObjectComponentReference(fromImage, AssetReferenceManager.GetSpriteAssetReference);
            ReferenceAssetAndRecord totargetReference = AssetReferenceManager.GetObjectComponentReference(toImage, AssetReferenceManager.GetSpriteAssetReference);

            if (fromsourceReference == null)
            {
                if (totargetReference != null)
                    AssetReferenceManager.RemoveObjectComponentReference(toImage, totargetReference);
                toImage.sprite = null;
            }
            else
            {
                if (totargetReference != null)
                    totargetReference.ModifyComponentReference(toImage, fromsourceReference);
                else
                    AssetReferenceManager.AddObjectComponentReference(toImage, fromsourceReference);

                toImage.sprite = fromsourceReference.CurLoadAssetRecord.LoadUnityObjectAssetInfor.LoadSpriteFromSpriteRender();
            }
        }


        #endregion

        #region GameObject加载

        public static ReferenceGameObjectAssetInfor InstantiateGameObjectByPathSync(Transform targetParent, string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
                return InstantiateGameObjectFromRecordSync(targetParent, null);
            else
                return InstantiateGameObjectFromRecordSync(targetParent, LoadAssetSync(assetPath));
        }

        public static void InstantiateGameObjectByPathAsync(Transform targetParent, string assetPath, Action<ReferenceGameObjectAssetInfor> afterInitialedInstanceAction = null)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                ReferenceGameObjectAssetInfor gameObjectAsset = InstantiateGameObjectFromRecordSync(targetParent, null);
                if (afterInitialedInstanceAction != null)
                    afterInitialedInstanceAction(gameObjectAsset);
            }
            else
            {
                LoadAssetAsync(assetPath, (assetRecord) =>
                {
                    ReferenceGameObjectAssetInfor gameObjectAsset = InstantiateGameObjectFromRecordSync(targetParent, assetRecord);
                    if (afterInitialedInstanceAction != null)
                        afterInitialedInstanceAction(gameObjectAsset);
                });
            }
        }

        public static ReferenceGameObjectAssetInfor InstantiateGameObjectFromRecordSync(Transform targetParent, BaseLoadAssetRecord assetRecord)
        {
            if (assetRecord == null)
                return null;

            ReferenceAssetAndRecord curReference = AssetReferenceManager.GetObjectComponentReference(targetParent, AssetReferenceManager.GetGameObjectFromAssetReference, assetRecord.AssetUrl);

            if (curReference != null)
                return null;

            GameObject go = assetRecord.LoadUnityObjectAssetInfor.InstantiateInstance(targetParent);
            ReferenceGameObjectAssetInfor referenceAssetInfor = new ReferenceGameObjectAssetInfor(go, typeof(GameObject));

            ReferenceAssetAndRecord newReference = new ReferenceAssetAndRecord(assetRecord, referenceAssetInfor);
            AssetReferenceManager.AddObjectComponentReference(targetParent, newReference);

            return referenceAssetInfor;
        }

        #endregion

        #region Audio 加载

        public static ReferenceAssetInfor GetAudioClipByPathSync(AudioSource targetAudioSources,string assetPath )
        {
            if (string.IsNullOrEmpty(assetPath))
                return GetAudioClipFromRecordSync(targetAudioSources, null);
            else
                return GetAudioClipFromRecordSync(targetAudioSources, LoadAssetSync(assetPath));
        }

        public static ReferenceAssetInfor GetAudioClipFromRecordSync(AudioSource targetAudioSources, BaseLoadAssetRecord assetRecord)
        {
            ReferenceAssetAndRecord curReference = AssetReferenceManager.GetObjectComponentReference(targetAudioSources, AssetReferenceManager.GetSAudioClipAssetReference);
            if (assetRecord == null)
            {
                if (curReference != null)
                    AssetReferenceManager.RemoveObjectComponentReference(targetAudioSources, curReference);
                return null;
            }

            if (curReference != null && curReference.CurLoadAssetRecord.isReferenceEqual(assetRecord))
                return null;
            AudioClip clip = assetRecord.LoadUnityObjectAssetInfor.LoadAudioClip();
            ReferenceAssetInfor referenceAssetInfor = new ReferenceAssetInfor(clip, typeof(AudioClip));
            ReferenceAssetAndRecord newReference = new ReferenceAssetAndRecord(assetRecord, referenceAssetInfor);

            if (curReference != null)
                curReference.ModifyComponentReference<AudioSource>(targetAudioSources, newReference);
            else
                AssetReferenceManager.AddObjectComponentReference(targetAudioSources, newReference);

         
            return referenceAssetInfor;
        }
        #endregion

        #endregion






    }
}