using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFramePro.ResourcesEx.Reference
{
    /// <summary>
    /// 资源引用管理器
    /// </summary>
    public static class  ReferenceAssetManager 
    {
        private static float s_FullCheckTimeInterval { get { return 10; } } //全资源空引用检测时间间隔
        //一个资源 Object 中的某个子资源id 到整个资源Object 的映射关系，key =子资源Instanceid  value=加载的整个资源Instanceid 
        private static readonly Dictionary<int, int> s_InstanceIdMapRecord = new Dictionary<int, int>(100);

        //所有被引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, ComponentReferenceAssetInfor> s_AllComponentReferenceAssetInfors = new Dictionary<int, ComponentReferenceAssetInfor>(100);

        //所有被弱引用的资源信息  key=加载的整个资源Instanceid 
        private static readonly Dictionary<int, ILoadAssetRecord> s_AllWeakReferenceAssets = new Dictionary<int, ILoadAssetRecord>(100);

        #region 计时器删除无效资源
        static ReferenceAssetManager()
        {
            AsyncManager.InvokeRepeating(0, s_FullCheckTimeInterval, FullCheckUnReferenceAset);
        }

        //定时删除无效的资源
        private static void FullCheckUnReferenceAset()
        {
            Dictionary<int, ComponentReferenceAssetInfor> temp = new Dictionary<int, ComponentReferenceAssetInfor>(s_AllComponentReferenceAssetInfors);

            foreach (var beReferenceAssets in temp)
            {
                if (beReferenceAssets.Value == null)
                {
                    s_AllComponentReferenceAssetInfors.Remove(beReferenceAssets.Key);
                    continue;
                }
                beReferenceAssets.Value.UpdateReferenceCount();
            }
        }
        #endregion


        /// <summary>
        /// 记录子资源与加载的资源的实例ID 关系
        /// </summary>
        /// <param name="subAsset"></param>
        /// <param name="loadAsset"></param>
        /// <returns>子资源被包含的加载记录中的资源id</returns>
        private static int RecordInstanceIdDependence(Object subAsset, Object loadAsset)
        {
            if (subAsset == null || loadAsset == null)
                return -1;
            int subInstanceID = subAsset.GetInstanceID();
            int loadAssetID = loadAsset.GetInstanceID();

            if(s_InstanceIdMapRecord.TryGetValue(subInstanceID,out var recordID))
            {
                if(recordID!= loadAssetID)
                {
                    Debug.LogError($"异常 记录的值{recordID} 与新的值不一致{loadAssetID}");
                    s_InstanceIdMapRecord.Remove(subInstanceID);
                    s_InstanceIdMapRecord[subInstanceID] = loadAssetID;
                }
            }
            else
            {
                s_InstanceIdMapRecord[subInstanceID] = loadAssetID;
            }
          
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
            //2019/12/14 修改 增加 ReferenceAssetStateUsage 状态判断，避免在回收过程中被引用的错误
            if (s_AllComponentReferenceAssetInfors.TryGetValue(InstanceID, out var beferenceAsset) == false || beferenceAsset == null )
            {
                beferenceAsset = ComponentReferenceAssetInfor.GetComponentReferenceAssetInforInstance();
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
                else
                {
                    s_AllComponentReferenceAssetInfors.Remove(InstanceID);
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
                else
                    return;
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
            Dictionary<int, ComponentReferenceAssetInfor> temp = new Dictionary<int, ComponentReferenceAssetInfor>(s_AllComponentReferenceAssetInfors);

            foreach (var beReferenceAsset in temp)
            {
                if (beReferenceAsset.Value == null)
                {
                    s_AllComponentReferenceAssetInfors.Remove(beReferenceAsset.Key);

                    continue;
                }
                beReferenceAsset.Value.ReduceAllReference(targetObject.transform);
            }
        }


        /// <summary>
        /// 当引用计数为0 时候需要移除加载记录
        /// </summary>
        /// <param name="componentReference"></param>
        internal static void RemoveNoReferenceAssetRecord(ComponentReferenceAssetInfor componentReference)
        {
            if (componentReference == null) return;
            if (componentReference.mILoadAssetRecord.GetLoadAsset() == null) return;
            int instanceID = componentReference.mILoadAssetRecord.GetLoadAsset().GetInstanceID();

            s_AllComponentReferenceAssetInfors.Remove(instanceID);
            s_InstanceIdMapRecord.RemoveAll((item) => item.Value == instanceID);
        }


        #endregion
    }
}