﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.ResourcesEx.Reference
{
    /// <summary>
    /// 资源引用管理器
    /// </summary>
    public static class  ReferenceAssetManager 
    {
        //一个资源 Object 中的某个子资源id 到整个资源Object 的映射关系，key =子资源Instanceid  value=加载的整个资源Instanceid 
        private static readonly Dictionary<int, int> s_InstanceIdMapRecord = new Dictionary<int, int>(100);

        //所有被引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, ComponentReferenceAssetInfor> s_AllComponentReferenceAssetInfors = new Dictionary<int, ComponentReferenceAssetInfor>(100);

        //所有被弱引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, ILoadAssetRecord> s_AllWeakReferenceAssets = new Dictionary<int, ILoadAssetRecord>(100);

        /// <summary>
        /// 所有需要 被删除的整个资源  Instanceid 
        /// </summary>
        private static readonly HashSet<int> s_AllNeedRemoveInstanceIds = new HashSet<int>();


        /// <summary>
        /// 记录子资源与加载的资源的实例ID 关系
        /// </summary>
        /// <param name="subAssey"></param>
        /// <param name="loadAsset"></param>
        /// <returns>子资源被包含的加载记录中的资源id</returns>
        private static int RecordInstanceIdDependence(Object subAssey, Object loadAsset)
        {
            if (subAssey == null || loadAsset == null)
                return -1;
            int subInstanceID = subAssey.GetInstanceID();
            int loadAssetID = loadAsset.GetInstanceID();

            s_InstanceIdMapRecord[subInstanceID] = loadAssetID;
            return loadAssetID;
        }

        private static int GetInstanceIDFromRecord(Object subAssey)
        {
            if (subAssey == null)
                return -1;
            int subInstanceID = subAssey.GetInstanceID();
            if (s_InstanceIdMapRecord.TryGetValue(subInstanceID, out int InstanceID))
                return InstanceID;

            //#if UNITY_EDITOR
            //            Debug.LogEditorError($"没有记录的资源信息{subAssey.name}  缺失是否是通过新接口加载的资源"); //可能不是通过制定接口加载的资源
            //#endif
            return -1;
        }

        #region 强引用关系处理(组件引用某个资源)

        /// <summary>
        /// 增加强引用关系 (资源被关联到指定的组件上了)
        /// </summary>
        /// <param name="component"></param>
        /// <param name="directReferenceObject">直接引用的资源(可能是一个Object 中的某个组件资源)</param>
        /// <param name="loadAssetRecord"></param>
        private static void AddStrongReference(Component component, Object directReferenceObject, ILoadAssetRecord loadAssetRecord, int InstanceID) 
        {
            if (component == null || directReferenceObject == null || loadAssetRecord == null || loadAssetRecord.IsRecordEnable == false || InstanceID == -1)
                return;

#if UNITY_EDITOR
            AssetReferenceTag assetReferenceTag = component.gameObject.GetAddComponentEx<AssetReferenceTag>();
            assetReferenceTag.RecordReference(component,  directReferenceObject);
#endif

            if (s_AllComponentReferenceAssetInfors.TryGetValue(InstanceID, out var beferenceAsset) == false || beferenceAsset == null)
            {
                beferenceAsset = ComponentReferenceAssetInfor.GetBeferenceAsset();
                s_AllComponentReferenceAssetInfors[InstanceID] = beferenceAsset;
            }
            beferenceAsset.AddReference(component, directReferenceObject, loadAssetRecord);
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        /// <param name="component"></param>
        /// <param name="asset"></param>
        public static void ReduceStrongReference(Component component, Object asset) 
        {
            if (component == null || asset == null)
                return;

#if UNITY_EDITOR
            AssetReferenceTag assetReferenceTag = component.gameObject.GetAddComponentEx<AssetReferenceTag>();
            assetReferenceTag.RomoveReference(component,  asset);
#endif

            int InstanceID = GetInstanceIDFromRecord(asset);
            if (InstanceID == -1)
                return;
            if (s_AllComponentReferenceAssetInfors.TryGetValue(InstanceID, out var beferenceAsset))
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
        public static void AddWeakReference(Object directReferenceObject, ILoadAssetRecord loadAssetRecord)
        {
            if (directReferenceObject == null || loadAssetRecord == null || loadAssetRecord.IsRecordEnable == false)
                return;
            int InstanceID = RecordInstanceIdDependence(directReferenceObject, loadAssetRecord.GetLoadAsset());

            if (InstanceID == -1)
                return;
            if (s_AllWeakReferenceAssets.TryGetValue(InstanceID, out var beferenceAsset))
            {
                if (beferenceAsset != loadAssetRecord)
                {
                    Debug.LogError($"记录了弱引用关系，但是{beferenceAsset} 不对应{loadAssetRecord}");
                    return;
                }
            }
            s_AllWeakReferenceAssets[InstanceID] = loadAssetRecord;
        }

        public static void ReduceWeakReference(Object directReferenceObject)
        {
            if (directReferenceObject == null)
                return;
            int InstanceID = GetInstanceIDFromRecord(directReferenceObject);

            if (InstanceID == -1)
                return;
            s_AllWeakReferenceAssets.Remove(InstanceID);
        }

        /// <summary>
        /// 强关联一个资源(可能之前是强引用或者弱引用)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="directReferenceObject"></param>
        /// <param name="component"></param>
        public static bool StrongReferenceWithComponent(Object directReferenceObject,Component component) 
        {
            if (directReferenceObject == null || component == null)
                return false;
            int instanceID = GetInstanceIDFromRecord(directReferenceObject);
            if (instanceID != -1 && s_AllWeakReferenceAssets.TryGetValue(instanceID, out var weakReferenceAssetRecord))
            {
                AddStrongReference(component, directReferenceObject, weakReferenceAssetRecord, instanceID);
                return true;
            }

            Debug.LogError($"强引用资源失败{directReferenceObject}  {component},没有找到资源记录");
            return false;
        }


        #endregion

        #region 引用计数处理

        /// <summary>
        /// 去除参数对象的所有引用计数
        /// </summary>
        /// <param name="component"></param>
        public static void RemoveGameObjectReference(GameObject targetObject)
        {
            if (targetObject == null)
                return;

            foreach (var beReferenceAsset in s_AllComponentReferenceAssetInfors.Values)
            {
                if (beReferenceAsset == null) continue;
                beReferenceAsset.ReduceAllReference(targetObject.transform);
            }
        }

        /// <summary>
        /// 获取所有没有被引用的资源
        /// </summary>
        /// <returns></returns>
        internal static List<ComponentReferenceAssetInfor> GetAllNoReferenceAssetsForDelete()
        {
            List<ComponentReferenceAssetInfor> noReferenceAssets = new List<ComponentReferenceAssetInfor>((int)(s_AllComponentReferenceAssetInfors.Count * 0.5f));

            foreach (var beReferenceAssets in s_AllComponentReferenceAssetInfors.Values)
            {
                if (beReferenceAssets == null) continue;
                if (beReferenceAssets.mReferenceAssetStateUsage == ReferenceAssetStateUsage.Releasing)
                    continue; //已经被记录了不需要再记录

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
        internal static void DeleteAllNoReferenceAssets(List<ComponentReferenceAssetInfor> allNoReferenceAssets)
        {
            if (allNoReferenceAssets == null || allNoReferenceAssets.Count == 0)
                return;
            s_AllNeedRemoveInstanceIds.Clear();

            foreach (var noReferenceAsset in allNoReferenceAssets)
            {
                if (noReferenceAsset == null) continue;
                int instanceID = noReferenceAsset.mReferenceInstanceID;

                s_AllNeedRemoveInstanceIds.Add(instanceID);
                noReferenceAsset.NotifyBeDelete();
                s_AllComponentReferenceAssetInfors.Remove(instanceID);
            }

            if (s_AllNeedRemoveInstanceIds == null || s_AllNeedRemoveInstanceIds.Count == 0)
                return;

            AssetBundleManager.RemoveAllUnReferenceAssetBundleRecord( s_AllNeedRemoveInstanceIds);
            LocalResourcesManager.RemoveAllUnReferenceResourcesRecord( s_AllNeedRemoveInstanceIds);

            s_InstanceIdMapRecord.RemoveAll(item => s_AllNeedRemoveInstanceIds.Contains(item.Value));
            s_AllWeakReferenceAssets.RemoveAll(item => s_AllNeedRemoveInstanceIds.Contains(item.Key));
        }

        #endregion
    }
}