using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.ResourcesEx.Reference
{
    /// <summary>
    /// 资源引用管理器
    /// </summary>
    public sealed class ReferenceAssetManager : Single<ReferenceAssetManager>
    {
        //一个资源 Object 中的某个子资源id 到整个资源Object 的映射关系，key =子资源Instanceid  value=加载的整个资源Instanceid 
        private static readonly Dictionary<int, int> mInstanceIdConnectDic = new Dictionary<int, int>();

        //所有被引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, BeferenceAsset> mAllBeReferenceAssets = new Dictionary<int, BeferenceAsset>();

        //所有被弱引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, ILoadAssetRecord> mAllWeakReferenceAssets = new Dictionary<int, ILoadAssetRecord>();



        /// <summary>
        /// 记录子资源与加载的资源的实例ID 关系
        /// </summary>
        /// <param name="subAssey"></param>
        /// <param name="loadAsset"></param>
        private int RecordInstanceID(Object subAssey, Object loadAsset)
        {
            if (subAssey == null || loadAsset == null)
                return -1;
            int subInstanceID = subAssey.GetInstanceID();
            int loadAssetID = loadAsset.GetInstanceID();

            mInstanceIdConnectDic[subInstanceID] = loadAssetID;
            return loadAssetID;
        }
        private int GetInstanceIDFromRecord(Object asset)
        {
            if (asset == null)
                return -1;
            int subInstanceID = asset.GetInstanceID();
            if (mInstanceIdConnectDic.TryGetValue(subInstanceID, out int InstanceID))
                return InstanceID;
            Debug.LogError($"没有记录的资源信息{asset.name}");  //可能不是通过制定接口加载的资源
            return -1;
        }

        #region 强引用关系处理(组件引用某个资源)

        /// <summary>
        /// 增加强引用关系
        /// </summary>
        /// <param name="component"></param>
        /// <param name="directReferenceObject">直接引用的资源(可能是一个Object 中的某个组件资源)</param>
        /// <param name="loadAssetRecord"></param>
        public void AddStrongReference<T>(Component component, Object directReferenceObject, ILoadAssetRecord loadAssetRecord) where T : UnityEngine.Object
        {
            if (component == null || directReferenceObject == null || loadAssetRecord == null || loadAssetRecord.IsRecordEnable == false)
                return;

            int InstanceID = RecordInstanceID(directReferenceObject, loadAssetRecord.GetLoadAsset());

            var assetReferenceTag = component.gameObject.GetAddComponentEx<AssetReferenceTag>();
            assetReferenceTag.RecordReference(component, typeof(T), directReferenceObject);

            if (InstanceID == -1)
                return;
            if (mAllBeReferenceAssets.TryGetValue(InstanceID, out var beferenceAsset))
            {
                if (beferenceAsset != null)
                {
                    beferenceAsset.AddReference(component, directReferenceObject);
                    return;
                }
            }
            beferenceAsset = BeferenceAsset.GetBeferenceAsset(loadAssetRecord, component);
            mAllBeReferenceAssets[InstanceID] = beferenceAsset;

        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        public void ReduceStrongReference<T>(Component component, Object asset) where T : UnityEngine.Object
        {
            if (component == null || asset == null)
                return;
            var assetReferenceTag = component.gameObject.GetAddComponentEx<AssetReferenceTag>();
            assetReferenceTag.RomoveReference(component, typeof(T), asset);

            int InstanceID = GetInstanceIDFromRecord(asset);
            if (InstanceID == -1)
                return;
            if (mAllBeReferenceAssets.TryGetValue(InstanceID, out var beferenceAsset))
            {
                if (beferenceAsset != null)
                {
                    beferenceAsset.ReduceReference(component, asset);
                    return;
                }
            }
        }

        #endregion

        #region  弱引用关系处理

        /// <summary>
        /// 增加弱引用关系 ，只记录资源的被引用的资源与加载记录关联关系（此时还没有关联到某个组件上）
        /// </summary>
        /// <param name="directReferenceObject">直接引用的资源(可能是一个Object 中的某个组件资源)</param>
        /// <param name="loadAssetRecord"></param>
        public void AddWeakReference<T>(Object directReferenceObject, ILoadAssetRecord loadAssetRecord) where T : UnityEngine.Object
        {
            if (directReferenceObject == null || loadAssetRecord == null || loadAssetRecord.IsRecordEnable == false)
                return;
            int InstanceID = RecordInstanceID(directReferenceObject, loadAssetRecord.GetLoadAsset());

            if (InstanceID == -1)
                return;
            if (mAllWeakReferenceAssets.TryGetValue(InstanceID, out var beferenceAsset))
            {
                if (beferenceAsset != null)
                    return;
            }
            beferenceAsset = loadAssetRecord;
            mAllWeakReferenceAssets[InstanceID] = beferenceAsset;
        }

        public void ReduceWeakReference<T>(Object directReferenceObject, ILoadAssetRecord loadAssetRecord) where T : UnityEngine.Object
        {
            if (directReferenceObject == null || loadAssetRecord == null || loadAssetRecord.IsRecordEnable == false)
                return;
            int InstanceID = RecordInstanceID(directReferenceObject, loadAssetRecord.GetLoadAsset());

            if (InstanceID == -1)
                return;
            mAllWeakReferenceAssets.Remove(InstanceID);
        }

        /// <summary>
        /// 强关联一个资源(可能之前是强引用或者弱引用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directReferenceObject"></param>
        /// <param name="component"></param>
        public void StrongReferenceWithComponent<T>(Object directReferenceObject,T component) where T : Component
        {
            if (directReferenceObject == null || component == null)
                return;
            int instanceID = GetInstanceIDFromRecord(directReferenceObject);
            if (instanceID == -1)
                return;
            if (mAllWeakReferenceAssets.TryGetValue(instanceID, out var weakReferenceAssetRecord))
            {
                AddStrongReference<T>(component, directReferenceObject, weakReferenceAssetRecord);
                return;
            }
            Debug.LogError($"强引用资源失败{directReferenceObject}  {component},没有找到资源记录");
        }

        #endregion

        #region 引用计数处理

        /// <summary>
        /// 减少引用计数
        /// </summary>
        /// <param name="component"></param>
        public void RemoveGameObjectReference(GameObject targetObject)
        {
            if (targetObject == null)
                return;
            foreach (var beReferenceAsset in mAllBeReferenceAssets.Values)
            {
                if (beReferenceAsset == null) continue;
                beReferenceAsset.ReduceAllReference(targetObject.transform);
            }


        }

        /// <summary>
        /// 获取所有没有被引用的资源
        /// </summary>
        /// <returns></returns>
        public List<BeferenceAsset> GetAllNoReferenceAssetsForDelete()
        {
            List<BeferenceAsset> noReferenceAssets = new List<BeferenceAsset>((int)(mAllBeReferenceAssets.Count * 0.5f));

            foreach (var beReferenceAssets in mAllBeReferenceAssets.Values)
            {
                if (beReferenceAssets == null) continue;
                beReferenceAssets.UpdateReferenceCount();
                if (beReferenceAssets.mReferenceAssetStateUsage == ReferenceAssetStateUsage.NoReference)
                {
                    beReferenceAssets.NotifyBeingReleasing();
                    noReferenceAssets.Add(beReferenceAssets);

                }
            }

            return noReferenceAssets;
        }

        /// <summary>
        /// 真正删除所有超时没有被使用的资源
        /// </summary>
        /// <param name=""></param>
        public void DeleteAllNoReferenceAssets(List<BeferenceAsset> allNoReferenceAssets)
        {
            foreach (var noReferenceAsset in allNoReferenceAssets)
            {
                if (noReferenceAsset == null) continue;
                noReferenceAsset.NotifyBeDelete();
                mAllBeReferenceAssets.Remove(noReferenceAsset.mReferenceInstanceID);
            }
        }

        #endregion

    }
}